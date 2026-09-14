<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { addPlayerToLobby, appState, getLobbyByCode } from '../store/appState'

const router = useRouter()
const codeInput = ref('')
const playerName = ref('')
const joinError = ref('')
const nameError = ref('')
const inLobby = ref(false)
const joinedCode = ref('')

const selectedLobby = computed(() => appState.lobbies.find((item) => item.code === joinedCode.value))
const submitCode = async () => {
  const lobby = await getLobbyByCode(codeInput.value.trim())
  if (!lobby) {
    joinError.value = 'That code is not valid.'
    return
  }

  joinedCode.value = lobby.code
  joinError.value = ''
  inLobby.value = true
}

const addPlayer = async () => {
  const trimmedName = playerName.value.trim()

  if (!trimmedName) {
    nameError.value = 'Enter a player name.'
    return
  }

  const playerId = await addPlayerToLobby(joinedCode.value, trimmedName)
  if (!playerId) {
    nameError.value = 'Unable to join this lobby.'
    return
  }

  playerName.value = ''
  nameError.value = ''
  router.push({
    name: 'lobby',
    params: { code: joinedCode.value },
    query: { role: 'student', playerId: String(playerId) },
  })
}
</script>

<template>
  <div class="screen">
    <h1>Join Event</h1>
    <p class="subtitle">Enter a game code, then your name to join</p>

    <div class="card">
      <div v-if="!inLobby" class="code-entry">
        <input
          v-model.trim="codeInput"
          type="text"
          placeholder="Game code"
          maxlength="6"
          inputmode="numeric"
        />
        <button @click="submitCode">Enter Lobby</button>
        <p v-if="joinError" class="error">{{ joinError }}</p>
      </div>

      <div v-else class="lobby">
        <p class="lobby-title">{{ selectedLobby?.name || 'Lobby' }}</p>
        <p class="hint">Code: {{ joinedCode }} | Status: {{ selectedLobby?.status }}</p>
        <div class="add-player">
          <input v-model.trim="playerName" type="text" placeholder="Player name" />
          <button @click="addPlayer">Join</button>
        </div>
        <p v-if="nameError" class="error">{{ nameError }}</p>
      </div>
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
  width: min(520px, 94vw);
  border: 1px solid var(--brand-border);
  border-radius: 12px;
  background: var(--brand-surface);
  padding: 1.2rem;
}

.code-entry,
.lobby,
.add-player {
  display: grid;
  gap: 0.8rem;
}

input {
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  padding: 0.8rem;
  font-size: 1rem;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 0.8rem;
  font-weight: 700;
  cursor: pointer;
  background: var(--brand-secondary);
  color: var(--brand-surface);
}

.lobby-title {
  font-size: 1.2rem;
  font-weight: 800;
}

.hint {
  color: var(--brand-muted);
}

.error {
  color: var(--brand-danger);
  font-weight: 600;
}
</style>
