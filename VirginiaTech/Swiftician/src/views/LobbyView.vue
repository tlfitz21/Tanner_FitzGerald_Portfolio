<!--Claude modified this file to hopefully let the buttons change color when they are pressed for feedback hueristic  -->
<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  appState,
  endLobby,
  getCurrentQuestion,
  getLobbyByCode,
  goToNextQuestion,
  refreshLobby,
  startLobby,
  submitStudentAnswer,
} from '../store/appState'
import ScoreBoard from '../components/ScoreBoard.vue'
import QuestionTimer from '../components/QuestionTimer.vue'
import { supabase } from '../supabase/client'

const route = useRoute()
const router = useRouter()
const answerInput = ref('')
const submitError = ref('')
const loading = ref(true)
const selectedAnswer = ref<string | null>(null)
let roomId = ''
let channel: ReturnType<typeof supabase.channel> | null = null

const lobbyCode = computed(() => String(route.params.code || ''))
const lobby = computed(() => appState.lobbies.find((item) => item.code === lobbyCode.value))
const role = computed(() => (route.query.role === 'admin' ? 'admin' : 'student'))
const playerId = computed(() => String(route.query.playerId || ''))
const question = computed(() => getCurrentQuestion(lobbyCode.value))

const QUESTION_TIME_LIMIT = 30

const isAdmin = computed(
  () =>
    role.value === 'admin' &&
    !!lobby.value &&
    appState.loggedInUserId &&
    appState.loggedInUserId === lobby.value.hostUserId,
)

const myAnswer = computed(() =>
  lobby.value?.answers
    .filter((item) => item.playerId === playerId.value)
    .at(-1),
)

const rankedPlayers = computed(() => [...(lobby.value?.players ?? [])].sort((a, b) => b.points - a.points))
const myRank = computed(() => rankedPlayers.value.findIndex((item) => item.id === playerId.value) + 1)
const me = computed(() => lobby.value?.players.find((item) => item.id === playerId.value))

onMounted(async () => {
  const loaded = await getLobbyByCode(lobbyCode.value)
  if (!loaded) {
    loading.value = false
    return
  }
  roomId = loaded.id
  await refreshLobby(roomId)
  channel = supabase
    .channel(`room-${roomId}`)
    .on('postgres_changes', { event: '*', schema: 'public', table: 'game_rooms', filter: `id=eq.${roomId}` }, async () => {
      await refreshLobby(roomId)
    })
    .on('postgres_changes', { event: '*', schema: 'public', table: 'participants', filter: `room_id=eq.${roomId}` }, async () => {
      await refreshLobby(roomId)
    })
    .on('postgres_changes', { event: '*', schema: 'public', table: 'answers', filter: `room_id=eq.${roomId}` }, async () => {
      await refreshLobby(roomId)
    })
    .subscribe()
  loading.value = false
})

onUnmounted(() => {
  if (channel) {
    supabase.removeChannel(channel)
  }
})

const handleTimeUp = () => {
  if (isAdmin.value && lobby.value) {
    goToNextQuestion(lobby.value.id)
  }
}

const submitAnswer = async (value?: string) => {
  const payload = value ?? answerInput.value
  selectedAnswer.value = payload
  setTimeout(() => { selectedAnswer.value = null }, 400)
  const error = await submitStudentAnswer(lobbyCode.value, playerId.value, payload)
  if (error) {
    submitError.value = error
    return
  }
  submitError.value = ''
  answerInput.value = ''
}

const goToResults = () => {
  router.push({
    name: 'results',
    params: { code: lobbyCode.value },
    query: {
      role: isAdmin.value ? 'admin' : 'student',
      ...(isAdmin.value ? {} : { playerId: playerId.value }),
    },
  })
}
</script>

<template>
  <div class="screen" v-if="!loading && lobby">
    <h1>{{ lobby.name }}</h1>
    <p class="subtitle">Code: {{ lobby.code }}</p>

    <div class="card">
      <p><strong>Status:</strong> {{ lobby.status }}</p>
      <p><strong>Players:</strong> {{ lobby.players.length }}</p>
      <p><strong>Question:</strong> {{ lobby.currentQuestionIndex + 1 }} / {{ lobby.questions.length }}</p>
    </div>

    <div class="card" v-if="question">
      <h2>Current Question</h2>
      <QuestionTimer
        v-if="lobby.status === 'live'"
        :duration="QUESTION_TIME_LIMIT"
        :active="lobby.status === 'live'"
        :key="lobby.currentQuestionIndex"
        @time-up="handleTimeUp"
      />
      <p class="prompt">{{ question.prompt }}</p>

      <div v-if="isAdmin" class="admin-panel">
        <p class="hint">Admin controls</p>
        <div class="actions">
          <button v-if="lobby.status === 'waiting'" @click="startLobby(lobby.id)">Start Lobby</button>
          <button v-if="lobby.status === 'live'" @click="goToNextQuestion(lobby.id)">Next Question</button>
          <button class="danger" @click="endLobby(lobby.id)">End Game</button>
          <button v-if="lobby.status === 'ended'" @click="goToResults">View Results</button>
        </div>
        <ScoreBoard :lobby="lobby" />
      </div>

      <div v-else class="student-panel">
        <p v-if="lobby.status === 'waiting'" class="hint">Waiting for teacher to start...</p>
        <p v-else-if="lobby.status === 'ended'" class="hint">Game ended by teacher.</p>
        <template v-else>
          <div v-if="question.answerType === 'multiple-choice'" class="options">
            <button v-for="option in question.options" :key="option" @click="submitAnswer(option)"
              :class="{ selected: selectedAnswer === option }">
              {{ option }}
            </button>
          </div>
          <div v-else class="free-response">
            <input v-model="answerInput" placeholder="Type your answer" />
            <button @click="submitAnswer()" :class="{ selected: selectedAnswer === answerInput }">Submit</button>
          </div>
          <p v-if="myAnswer" class="hint">Your answer: {{ myAnswer.value }}</p>
          <p class="hint">Points: {{ me?.points ?? 0 }} | Rank: {{ myRank || '-' }}</p>
          <p class="hint">Last answer points: {{ me?.lastAnswerPoints ?? 0 }}</p>
          <p v-if="submitError" class="error">{{ submitError }}</p>
        </template>
        <button v-if="lobby.status === 'ended'" @click="goToResults">View Results</button>
      </div>
    </div>
  </div>

  <div v-else-if="!loading" class="screen">
    <h1>Lobby not found</h1>
  </div>
</template>

<style scoped>
.screen {
  min-height: 100vh;
  display: grid;
  place-content: start center;
  gap: 1rem;
  padding: 2rem;
}

.subtitle,
.hint {
  color: var(--brand-muted);
}

.card {
  width: min(720px, 95vw);
  background: var(--brand-surface);
  border: 1px solid var(--brand-border);
  border-radius: 12px;
  padding: 1rem;
  display: grid;
  gap: 0.7rem;
}

.prompt {
  font-size: 1.2rem;
  font-weight: 700;
}

.actions,
.options,
.free-response {
  display: grid;
  gap: 0.6rem;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 0.75rem;
  font-weight: 700;
  background: var(--brand-primary);
  transition: background 0.15s ease, transform 0.1s ease;
}

button.selected {
  background: var(--brand-success, #22c55e);
  transform: scale(0.97);
}

.danger {
  background: var(--brand-danger);
}

input {
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  padding: 0.7rem;
}

.error {
  color: var(--brand-danger);
}
</style>