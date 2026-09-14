import { reactive } from 'vue'
import { multiplicationTablePool } from '../data/multiplicationTables'
import { supabase } from '../supabase/client'

export type AnswerType = 'multiple-choice' | 'free-response'
export type TemplateDifficulty = 'easy' | 'medium' | 'hard' | 'everything-goes'
export type LobbyStatus = 'waiting' | 'live' | 'ended'

export type Question = {
  id: string
  prompt: string
  answerType: AnswerType
  correctAnswer: string
  options: string[]
}

export type Player = {
  id: string
  name: string
  points: number
  lastAnswerPoints: number
}

export type StudentAnswer = {
  id: number
  roomId: string
  questionId: string
  playerId: string
  playerName: string
  value: string
  isCorrect: boolean
  pointsAwarded: number
  timeTakenMs: number
  submittedAt: string
}

export type Lobby = {
  id: string
  hostUserId: string
  hostEmail: string
  code: string
  name: string
  status: LobbyStatus
  currentQuestionIndex: number
  questionStartedAt: string | null
  players: Player[]
  questions: Question[]
  answers: StudentAnswer[]
}

type AppState = {
  loggedInUserEmail: string
  loggedInUserId: string
  lobbies: Lobby[]
}

export const appState = reactive<AppState>({
  loggedInUserEmail: '',
  loggedInUserId: '',
  lobbies: [],
})

const randomIntInclusive = (min: number, max: number): number => {
  return Math.floor(Math.random() * (max - min + 1)) + min
}

const generateCode = (): string => {
  let code = ''
  do {
    code = randomIntInclusive(100000, 999999).toString()
  } while (appState.lobbies.some((lobby) => lobby.code === code))
  return code
}

const createMultipleChoiceOptions = (correctAnswer: number): string[] => {
  const options = new Set<number>([correctAnswer])
  while (options.size < 4) {
    const offset = randomIntInclusive(-8, 8)
    const candidate = Math.max(2, correctAnswer + offset)
    options.add(candidate)
  }
  return Array.from(options)
    .sort(() => Math.random() - 0.5)
    .map((option) => option.toString())
}

const templateMatchers: Record<TemplateDifficulty, (left: number, right: number) => boolean> = {
  easy: (left, right) => left <= 5 && right <= 5,
  medium: (left, right) => left >= 4 && left <= 9 && right >= 4 && right <= 9,
  // Hard now means "at least one high factor" to keep enough unique problems.
  hard: (left, right) => left >= 10 || right >= 10,
  'everything-goes': () => true,
}

const generateTemplateQuestions = (difficulty: TemplateDifficulty, count = 10): Question[] => {
  const isEligible = templateMatchers[difficulty]
  const uniquePool = new Map<string, (typeof multiplicationTablePool)[number]>()

  for (const item of multiplicationTablePool) {
    if (!isEligible(item.left, item.right)) continue
    // Avoid mirrored repeats like 6x7 and 7x6 in the same generated set.
    const a = Math.min(item.left, item.right)
    const b = Math.max(item.left, item.right)
    const key = `${a}x${b}`
    if (!uniquePool.has(key)) {
      uniquePool.set(key, { left: a, right: b, product: a * b })
    }
  }

  const shuffled = Array.from(uniquePool.values()).sort(() => Math.random() - 0.5)
  const picked = shuffled.slice(0, Math.min(count, shuffled.length))

  return picked.map((item, index) => {
    const correct = item.product
    return {
      id: `q-${Date.now()}-${index}-${randomIntInclusive(1, 999)}`,
      prompt: `${item.left} x ${item.right} = ?`,
      answerType: 'multiple-choice',
      correctAnswer: correct.toString(),
      options: createMultipleChoiceOptions(correct),
    }
  })
}
const calculatePoints = (isCorrect: boolean, timeTakenMs: number): number => {
  if (!isCorrect) return 0
  const seconds = Math.max(0, timeTakenMs / 1000)
  if (seconds <= 10) return 1000
  if (seconds >= 45) return 500
  const scale = (seconds - 10) / 35
  return Math.round(1000 - scale * 500)
}

const finalizeCurrentQuestionScores = async (lobby: Lobby): Promise<void> => {
  const question = lobby.questions[lobby.currentQuestionIndex]
  if (!question) return

  const { data: answers } = await supabase
    .from('answers')
    .select('*')
    .eq('room_id', lobby.id)
    .eq('question_id', question.id)

  if (!answers?.length) return

  for (const answer of answers) {
    const pointsAwarded = calculatePoints(answer.is_correct, answer.time_taken_ms ?? 45000)
    const player = lobby.players.find((item) => item.id === answer.participant_id)
    const currentPoints = player?.points ?? 0

    await supabase.from('answers').update({ points_awarded: pointsAwarded }).eq('id', answer.id)
    await supabase
      .from('participants')
      .update({
        points: currentPoints + pointsAwarded,
        last_answer_points: pointsAwarded,
      })
      .eq('id', answer.participant_id)
  }
}

const mapLobby = (row: any): Lobby => ({
  id: row.id,
  hostUserId: row.host_user_id,
  hostEmail: row.host_email ?? '',
  code: row.code,
  name: row.name,
  status: row.status,
  currentQuestionIndex: row.current_question_index,
  questionStartedAt: row.question_started_at,
  players: [],
  questions: (row.questions as Question[]) ?? [],
  answers: [],
})

const hydrateLobby = async (lobby: Lobby): Promise<void> => {
  const [{ data: players }, { data: answers }] = await Promise.all([
    supabase.from('participants').select('*').eq('room_id', lobby.id).order('joined_at', { ascending: true }),
    supabase.from('answers').select('*').eq('room_id', lobby.id).order('submitted_at', { ascending: true }),
  ])

  lobby.players =
    players?.map((item: any) => ({
      id: item.id,
      name: item.display_name,
      points: item.points ?? 0,
      lastAnswerPoints: item.last_answer_points ?? 0,
    })) ?? []

  lobby.answers =
    answers?.map((item: any) => ({
      id: item.id,
      roomId: item.room_id,
      questionId: item.question_id,
      playerId: item.participant_id,
      playerName: item.participant_name,
      value: item.answer_value,
      isCorrect: item.is_correct,
      pointsAwarded: item.points_awarded,
      timeTakenMs: item.time_taken_ms,
      submittedAt: item.submitted_at,
    })) ?? []
}

export const createAccount = async (email: string, password: string): Promise<string | null> => {
  const { error } = await supabase.auth.signUp({ email: email.trim(), password: password.trim() })
  return error?.message ?? null
}

export const login = async (email: string, password: string): Promise<string | null> => {
  const { data, error } = await supabase.auth.signInWithPassword({
    email: email.trim(),
    password: password.trim(),
  })
  if (error) return error.message
  appState.loggedInUserEmail = data.user.email ?? ''
  appState.loggedInUserId = data.user.id
  await refreshLobbies()
  return null
}

export const logout = async (): Promise<void> => {
  await supabase.auth.signOut()
  appState.loggedInUserEmail = ''
  appState.loggedInUserId = ''
  appState.lobbies = []
}

export const initializeAuthFromSession = async (): Promise<void> => {
  const { data } = await supabase.auth.getSession()
  const user = data.session?.user
  if (!user) return
  appState.loggedInUserEmail = user.email ?? ''
  appState.loggedInUserId = user.id
  await refreshLobbies()
}

export const createLobby = async (
  lobbyName: string,
  questions: Question[],
): Promise<Lobby> => {
  const code = generateCode()
  const { data, error } = await supabase
    .from('game_rooms')
    .insert({
      host_user_id: appState.loggedInUserId,
      host_email: appState.loggedInUserEmail,
      code,
      name: lobbyName.trim() || 'Untitled Lobby',
      status: 'waiting',
      current_question_index: 0,
      question_started_at: null,
      questions,
    })
    .select('*')
    .single()
  if (error) throw error
  const lobby = mapLobby(data)
  appState.lobbies.unshift(lobby)
  return lobby
}

export const createMultiplicationTemplateQuestions = (
  difficulty: TemplateDifficulty,
  count = 10,
): Question[] => {
  return generateTemplateQuestions(difficulty, count)
}

export const addPlayerToLobby = async (lobbyCode: string, playerName: string): Promise<string | null> => {
  const lobby = await getLobbyByCode(lobbyCode)
  if (!lobby) return null
  const normalizedName = playerName.trim()
  if (!normalizedName) return null
  const { data, error } = await supabase
    .from('participants')
    .insert({ room_id: lobby.id, display_name: normalizedName, points: 0, last_answer_points: 0 })
    .select('*')
    .single()
  if (error) return null
  await refreshLobby(lobby.id)
  return data.id
}

export const startLobby = async (lobbyId: string): Promise<void> => {
  await supabase
    .from('game_rooms')
    .update({ status: 'live', current_question_index: 0, question_started_at: new Date().toISOString() })
    .eq('id', lobbyId)
}

export const goToNextQuestion = async (lobbyId: string): Promise<void> => {
  const lobby = appState.lobbies.find((item) => item.id === lobbyId)
  if (!lobby) return
  await finalizeCurrentQuestionScores(lobby)
  const nextIndex = lobby.currentQuestionIndex + 1
  const hasMore = nextIndex < lobby.questions.length
  await supabase
    .from('game_rooms')
    .update({
      status: hasMore ? 'live' : 'ended',
      current_question_index: hasMore ? nextIndex : lobby.currentQuestionIndex,
      question_started_at: hasMore ? new Date().toISOString() : null,
    })
    .eq('id', lobbyId)
}

export const endLobby = async (lobbyId: string): Promise<void> => {
  const lobby = appState.lobbies.find((item) => item.id === lobbyId)
  if (lobby?.status === 'live') {
    await finalizeCurrentQuestionScores(lobby)
  }
  await supabase.from('game_rooms').update({ status: 'ended', question_started_at: null }).eq('id', lobbyId)
}

export const submitStudentAnswer = async (
  lobbyCode: string,
  playerId: string,
  answerValue: string,
): Promise<string | null> => {
  const lobby = await getLobbyByCode(lobbyCode)
  if (!lobby || lobby.status !== 'live') return 'Lobby is not active.'
  const question = lobby.questions[lobby.currentQuestionIndex]
  if (!question) return 'Question not found.'
  const player = lobby.players.find((item) => item.id === playerId)
  if (!player) return 'Player not found.'
  const normalizedAnswer = answerValue.trim()
  if (!normalizedAnswer) return 'Answer cannot be empty.'
  const isCorrect = normalizedAnswer === question.correctAnswer
  const timeTakenMs = lobby.questionStartedAt
    ? Math.max(0, Date.now() - new Date(lobby.questionStartedAt).getTime())
    : 45000
  await supabase.from('answers').upsert({
    room_id: lobby.id,
    question_id: question.id,
    participant_id: player.id,
    participant_name: player.name,
    answer_value: normalizedAnswer,
    is_correct: isCorrect,
    points_awarded: 0,
    time_taken_ms: timeTakenMs,
    submitted_at: new Date().toISOString(),
  }, { onConflict: 'room_id,question_id,participant_id' })

  return null
}

export const refreshLobbies = async (): Promise<void> => {
  if (!appState.loggedInUserId) return
  const { data } = await supabase
    .from('game_rooms')
    .select('*')
    .eq('host_user_id', appState.loggedInUserId)
    .order('created_at', { ascending: false })
  appState.lobbies = (data ?? []).map(mapLobby)
  await Promise.all(appState.lobbies.map((lobby) => hydrateLobby(lobby)))
}

export const refreshLobby = async (lobbyId: string): Promise<void> => {
  const { data } = await supabase.from('game_rooms').select('*').eq('id', lobbyId).single()
  if (!data) return
  const mapped = mapLobby(data)
  await hydrateLobby(mapped)
  const idx = appState.lobbies.findIndex((item) => item.id === lobbyId)
  if (idx >= 0) {
    appState.lobbies[idx] = mapped
  } else {
    appState.lobbies.unshift(mapped)
  }
}

export const getCurrentQuestion = (lobbyCode: string): Question | undefined => {
  const lobby = appState.lobbies.find((item) => item.code === lobbyCode)
  return lobby?.questions[lobby.currentQuestionIndex]
}

export const getAnswersForCurrentQuestion = (lobbyCode: string): StudentAnswer[] => {
  const lobby = appState.lobbies.find((item) => item.code === lobbyCode)
  if (!lobby) return []
  const currentQuestion = lobby.questions[lobby.currentQuestionIndex]
  if (!currentQuestion) return []
  return lobby.answers.filter((item) => item.questionId === currentQuestion.id)
}

export const getLobbyByCode = async (code: string): Promise<Lobby | undefined> => {
  const existing = appState.lobbies.find((lobby) => lobby.code === code)
  if (existing) return existing
  const { data } = await supabase.from('game_rooms').select('*').eq('code', code).single()
  if (!data) return undefined
  const lobby = mapLobby(data)
  await hydrateLobby(lobby)
  appState.lobbies.unshift(lobby)
  return lobby
}
