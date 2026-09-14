<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { appState, getLobbyByCode, refreshLobby } from '../store/appState'

const route = useRoute()
const router = useRouter()
const error = ref('')
const loading = ref(true)

const lobbyCode = computed(() => String(route.params.code || ''))
const lobby = computed(() => appState.lobbies.find((item) => item.code === lobbyCode.value))

const groupedAnswers = computed(() => {
  if (!lobby.value) return []
  return lobby.value.questions.map((question) => ({
    question,
    answers: lobby.value!.answers.filter((answer) => answer.questionId === question.id),
  }))
})

onMounted(async () => {
  const targetLobby = await getLobbyByCode(lobbyCode.value)
  if (!targetLobby) {
    error.value = 'Lobby not found.'
    loading.value = false
    return
  }
  if (targetLobby.hostUserId !== appState.loggedInUserId) {
    error.value = 'Only the teacher can view submitted answers.'
    loading.value = false
    return
  }
  await refreshLobby(targetLobby.id)
  loading.value = false
})
</script>

<template>
  <div class="screen">
    <h1>Answer Review</h1>
    <p v-if="lobby" class="subtitle">{{ lobby.name }} ({{ lobby.code }})</p>
    <p v-if="error" class="error">{{ error }}</p>
    <p v-else-if="loading" class="subtitle">Loading answers...</p>

    <div v-else class="card">
      <div v-for="entry in groupedAnswers" :key="entry.question.id" class="question-block">
        <h3>{{ entry.question.prompt }}</h3>
        <p class="hint">Correct answer: {{ entry.question.correctAnswer }}</p>
        <p v-if="!entry.answers.length" class="hint">No student answers yet.</p>
        <ul v-else>
          <li v-for="answer in entry.answers" :key="answer.id">
            <strong>{{ answer.playerName }}</strong>
            - {{ answer.value }}
            ({{ answer.pointsAwarded }} pts)
          </li>
        </ul>
      </div>
    </div>

    <button @click="router.push({ name: 'console' })">Back to Console</button>
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
  width: min(820px, 95vw);
  background: var(--brand-surface);
  border: 1px solid var(--brand-border);
  border-radius: 12px;
  padding: 1rem;
  display: grid;
  gap: 1rem;
}

.question-block {
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  padding: 0.8rem;
}

ul {
  list-style: none;
  padding: 0;
  display: grid;
  gap: 0.4rem;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 0.8rem;
  background: var(--brand-primary);
  font-weight: 700;
}

.error {
  color: var(--brand-danger);
}
</style>
