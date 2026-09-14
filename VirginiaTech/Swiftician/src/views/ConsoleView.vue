<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  appState,
  createLobby,
  createMultiplicationTemplateQuestions,
  endLobby as endLobbyInStore,
  refreshLobbies,
  goToNextQuestion,
  startLobby as startLobbyInStore,
  type Lobby,
  type Question,
} from '../store/appState'

type BuildMode = 'custom' | 'multiplication-template'
type AnswerType = 'multiple-choice' | 'free-response'
type Difficulty = 'easy' | 'medium' | 'hard' | 'everything-goes'

const lobbyName = ref('')
const router = useRouter()
const buildMode = ref<BuildMode>('custom')
const templateDifficulty = ref<Difficulty>('easy')
const customPrompt = ref('')
const customAnswerType = ref<AnswerType>('multiple-choice')
const customCorrectAnswer = ref('')
const customOptionsRaw = ref('')
const customQuestions = ref<Question[]>([])
const selectedLobbyId = ref<string | null>(null)
const formError = ref('')

const myLobbies = computed(() =>
  appState.lobbies.filter((lobby) => lobby.hostUserId === appState.loggedInUserId),
)

const selectedLobby = computed(() =>
  myLobbies.value.find((lobby) => lobby.id === selectedLobbyId.value),
)

onMounted(async () => {
  await refreshLobbies()
})

const addCustomQuestion = () => {
  if (!customPrompt.value.trim() || !customCorrectAnswer.value.trim()) {
    formError.value = 'Custom question needs a prompt and a correct answer.'
    return
  }

  const options =
    customAnswerType.value === 'multiple-choice'
      ? customOptionsRaw.value
          .split(',')
          .map((item) => item.trim())
          .filter(Boolean)
      : []

  const fullOptions =
    customAnswerType.value === 'multiple-choice'
      ? Array.from(new Set([...options, customCorrectAnswer.value.trim()])).slice(0, 4)
      : []

  if (customAnswerType.value === 'multiple-choice' && fullOptions.length < 2) {
    formError.value = 'Multiple choice requires at least two options.'
    return
  }

  customQuestions.value.push({
    id: `q-${Date.now()}-${customQuestions.value.length}`,
    prompt: customPrompt.value.trim(),
    answerType: customAnswerType.value,
    correctAnswer: customCorrectAnswer.value.trim(),
    options: fullOptions,
  })

  customPrompt.value = ''
  customCorrectAnswer.value = ''
  customOptionsRaw.value = ''
  formError.value = ''
}

const createLobbyFromForm = async () => {
  if (!appState.loggedInUserId) {
    formError.value = 'Login is required.'
    return
  }

  let questions: Question[] = []

  if (buildMode.value === 'custom') {
    if (!customQuestions.value.length) {
      formError.value = 'Add at least one custom question.'
      return
    }
    questions = customQuestions.value
  } else {
    questions = createMultiplicationTemplateQuestions(templateDifficulty.value, 10)
  }

  try {
    const lobby = await createLobby(lobbyName.value, questions)
    selectedLobbyId.value = lobby.id
    lobbyName.value = ''
    customQuestions.value = []
    formError.value = ''
  } catch (error: any) {
    const code = error?.code ?? ''
    formError.value =
      code === 'PGRST205'
        ? "Supabase tables are not set up yet. Run the SQL from 'supabase-schema.sql' in your Supabase SQL editor."
        : error?.message || 'Unable to create lobby.'
  }
}

const openLobby = (lobby: Lobby) => {
  selectedLobbyId.value = lobby.id
}

const startLobby = async (lobby: Lobby) => {
  await startLobbyInStore(lobby.id)
}

const endLobby = async (lobby: Lobby) => {
  await endLobbyInStore(lobby.id)
}

const nextLobbyQuestion = async (lobby: Lobby) => {
  await goToNextQuestion(lobby.id)
}

const goToAdminView = (lobby: Lobby) => {
  router.push({
    name: 'lobby',
    params: { code: lobby.code },
    query: { role: 'admin' },
  })
}

const goToAnswerReview = (lobby: Lobby) => {
  router.push({ name: 'answers', params: { code: lobby.code } })
}
</script>

<template>
  <div class="screen">
    <h1>Host Console</h1>
    <p class="subtitle">Create and manage your lobbies</p>

    <div class="card">
      <label>
        Lobby name
        <input v-model.trim="lobbyName" type="text" placeholder="Weekly quiz night" />
      </label>

      <label>
        Game type
        <select v-model="buildMode">
          <option value="custom">Custom questions</option>
          <option value="multiplication-template">Multiplication template</option>
        </select>
      </label>

      <div v-if="buildMode === 'custom'" class="custom-builder">
        <h3>Custom Questions</h3>
        <label>
          Prompt
          <input v-model.trim="customPrompt" type="text" placeholder="What is Vue used for?" />
        </label>
        <label>
          Answer type
          <select v-model="customAnswerType">
            <option value="multiple-choice">Multiple choice</option>
            <option value="free-response">Free response</option>
          </select>
        </label>
        <label>
          Correct answer
          <input v-model.trim="customCorrectAnswer" type="text" placeholder="Building UIs" />
        </label>
        <label v-if="customAnswerType === 'multiple-choice'">
          Options (comma separated)
          <input v-model.trim="customOptionsRaw" type="text" placeholder="A,B,C,D" />
        </label>
        <button @click="addCustomQuestion">Add Question</button>
        <p class="hint">Questions added: {{ customQuestions.length }}</p>
      </div>

      <div v-else class="template-builder">
        <h3>Multiplication Tables Template</h3>
        <label>
          Difficulty
          <select v-model="templateDifficulty">
            <option value="easy">Easy (2-5)</option>
            <option value="medium">Medium (6-9)</option>
            <option value="hard">Hard (10-12)</option>
            <option value="everything-goes">Everything goes (2-12)</option>
          </select>
        </label>
        <p class="hint">This generates 10 random questions from the multiplication file.</p>
      </div>

      <button @click="createLobbyFromForm">Create Lobby</button>
      <p v-if="formError" class="error">{{ formError }}</p>
    </div>

    <div class="card">
      <h2>Your Lobbies</h2>
      <p v-if="!myLobbies.length" class="hint">No lobbies yet.</p>
      <ul v-else class="lobby-list">
        <li v-for="lobby in myLobbies" :key="lobby.id">
          <div>
            <p class="row-main">{{ lobby.name }} ({{ lobby.code }})</p>
            <p class="row-sub">Status: {{ lobby.status }} | Players: {{ lobby.players.length }}</p>
          </div>
          <button @click="openLobby(lobby)">Open Summary</button>
          <button @click="goToAdminView(lobby)">Enter as Admin</button>
          <button @click="goToAnswerReview(lobby)">Review Answers</button>
        </li>
      </ul>
    </div>

    <div v-if="selectedLobby" class="card">
      <h2>Lobby Detail: {{ selectedLobby.name }}</h2>
      <p class="code">Code: {{ selectedLobby.code }}</p>
      <p class="hint">Questions: {{ selectedLobby.questions.length }}</p>
      <p class="hint">Players: {{ selectedLobby.players.length }}</p>
      <div class="actions">
        <button class="start" @click="startLobby(selectedLobby)">Start Game</button>
        <button class="start" @click="nextLobbyQuestion(selectedLobby)">Next Question</button>
        <button class="end" @click="endLobby(selectedLobby)">End Game</button>
      </div>
      <ul class="players">
        <li v-for="player in selectedLobby.players" :key="player.id">{{ player.name }}</li>
      </ul>
    </div>
  </div>
</template>

<style scoped>
.screen {
  min-height: 100vh;
  display: grid;
  place-content: center;
  text-align: center;
  gap: 1rem;
  padding: 2rem;
}

.subtitle {
  color: var(--brand-muted);
}

.card {
  width: min(640px, 94vw);
  display: grid;
  gap: 1rem;
  background: var(--brand-surface);
  border: 1px solid var(--brand-border);
  border-radius: 12px;
  padding: 1.2rem;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 0.85rem;
  font-size: 1rem;
  font-weight: 800;
  background: var(--brand-primary);
  cursor: pointer;
}

.actions {
  display: grid;
  gap: 0.6rem;
}

.start {
  background: var(--brand-primary);
}

.end {
  background: var(--brand-danger);
}

.custom-builder,
.template-builder {
  display: grid;
  gap: 0.6rem;
  border: 1px solid var(--brand-border);
  padding: 0.8rem;
  border-radius: 10px;
}

label {
  display: grid;
  gap: 0.3rem;
  text-align: left;
  font-weight: 600;
}

input,
select {
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  padding: 0.7rem;
}

.code {
  font-size: 1.2rem;
  font-weight: 900;
  letter-spacing: 0.08em;
}

.hint {
  color: var(--brand-muted);
}

.error {
  color: var(--brand-danger);
  font-weight: 700;
}

.lobby-list,
.players {
  list-style: none;
  padding: 0;
  display: grid;
  gap: 0.6rem;
}

.lobby-list li {
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  padding: 0.6rem;
  display: grid;
  gap: 0.6rem;
}

.row-main {
  font-weight: 700;
}

.row-sub {
  color: var(--brand-muted);
}
</style>
