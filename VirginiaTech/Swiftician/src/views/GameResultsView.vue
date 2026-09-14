<!-- AI-GENERATED FILE — Claude (Anthropic), PM4 milestone -->
<!-- Prompt: "Create end-of-game results view with final leaderboard" -->
<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { appState, getLobbyByCode } from '../store/appState'
import ScoreBoard from '../components/ScoreBoard.vue'

const route = useRoute()
const router = useRouter()
const loading = ref(true)

const lobbyCode = computed(() => String(route.params.code || ''))
const lobby = computed(() => appState.lobbies.find((item) => item.code === lobbyCode.value))
const role = computed(() => (route.query.role === 'admin' ? 'admin' : 'student'))
const playerId = computed(() => String(route.query.playerId || ''))
const isAdmin = computed(
  () =>
    role.value === 'admin' &&
    !!lobby.value &&
    appState.loggedInUserId === lobby.value.hostUserId,
)

const totalQuestions = computed(() => lobby.value?.questions.length ?? 0)
const answersForView = computed(() => {
  if (!lobby.value) return []
  if (isAdmin.value) return lobby.value.answers
  return lobby.value.answers.filter((answer) => answer.playerId === playerId.value)
})

const answersByQuestion = computed(() => {
  if (!lobby.value) return []
  return lobby.value.questions.map((question) => ({
    question,
    answers: answersForView.value.filter((answer) => answer.questionId === question.id),
  }))
})

const goHome = () => {
  router.push({ name: 'home' })
}

onMounted(async () => {
  await getLobbyByCode(lobbyCode.value)
  loading.value = false
})
</script>

<template>
  <div class="screen" v-if="!loading && lobby">
    <h1>Game Over!</h1>
    <p class="subtitle">{{ lobby.name }}</p>
    <p class="hint">{{ totalQuestions }} questions played</p>
    <p class="hint" v-if="isAdmin">Teacher view: all student answers</p>
    <p class="hint" v-else>Student view: your submitted answers</p>

    <div class="card">
      <ScoreBoard :lobby="lobby" />
    </div>

    <div class="card answers">
      <h2>Answer Review</h2>
      <div v-for="entry in answersByQuestion" :key="entry.question.id" class="question-block">
        <p class="question">{{ entry.question.prompt }}</p>
        <p class="hint">Correct answer: {{ entry.question.correctAnswer }}</p>
        <p v-if="!entry.answers.length" class="hint">No answers recorded.</p>
        <ul v-else>
          <li v-for="answer in entry.answers" :key="answer.id">
            <template v-if="isAdmin">
              <strong>{{ answer.playerName }}:</strong> {{ answer.value }} ({{ answer.pointsAwarded }} pts)
            </template>
            <template v-else>
              Your answer: {{ answer.value }} ({{ answer.pointsAwarded }} pts)
            </template>
          </li>
        </ul>
      </div>
    </div>

    <button @click="goHome">Back to Home</button>
  </div>

  <div v-else-if="!loading" class="screen">
    <h1>Results not found</h1>
    <button @click="goHome">Back to Home</button>
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
  font-size: 1.2rem;
  font-weight: 700;
}

.hint {
  color: var(--brand-muted);
}

.card {
  width: min(760px, 95vw);
  background: var(--brand-surface);
  border: 1px solid var(--brand-border);
  border-radius: 12px;
  padding: 1.2rem;
}

.answers {
  text-align: left;
}

.question-block {
  padding: 0.8rem;
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  margin-bottom: 0.7rem;
}

.question {
  font-weight: 700;
}

ul {
  list-style: none;
  margin: 0.4rem 0 0;
  padding: 0;
  display: grid;
  gap: 0.4rem;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 0.85rem 1.6rem;
  font-weight: 700;
  background: var(--brand-primary);
  color: var(--brand-text);
  cursor: pointer;
}
</style>
