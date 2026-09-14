<!-- AI-GENERATED FILE — Claude (Anthropic), PM4 milestone -->
<!-- Prompt: "Create a leaderboard component that computes player scores from lobby data" -->
<script setup lang="ts">
import { computed } from 'vue'
import type { Lobby } from '../store/appState'

const props = defineProps<{ lobby: Lobby }>()

type PlayerScore = {
  playerId: string
  name: string
  points: number
}

const scores = computed<PlayerScore[]>(() => {
  const lobby = props.lobby
  if (!lobby) return []
  return [...lobby.players]
    .map((player) => ({
      playerId: player.id,
      name: player.name,
      points: player.points,
    }))
    .sort((a, b) => b.points - a.points)
})
</script>

<template>
  <div class="scoreboard">
    <h3>Leaderboard</h3>
    <p v-if="!scores.length" class="hint">No players yet.</p>
    <ol v-else class="ranking">
      <li
        v-for="(entry, index) in scores"
        :key="entry.playerId"
        :class="{ gold: index === 0, silver: index === 1, bronze: index === 2 }"
      >
        <span class="rank">{{ index + 1 }}</span>
        <span class="player-name">{{ entry.name }}</span>
        <span class="player-score">{{ entry.points }} pts</span>
      </li>
    </ol>
  </div>
</template>

<style scoped>
.scoreboard {
  display: grid;
  gap: 0.6rem;
}

.ranking {
  list-style: none;
  padding: 0;
  display: grid;
  gap: 0.4rem;
}

.ranking li {
  display: flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0.6rem 0.8rem;
  border-radius: 8px;
  background: var(--brand-surface-soft);
}

.gold {
  background: #fff8e1 !important;
  border: 1px solid #ffd54f;
}

.silver {
  background: #f5f5f5 !important;
  border: 1px solid #bdbdbd;
}

.bronze {
  background: #fff3e0 !important;
  border: 1px solid #ffb74d;
}

.rank {
  font-weight: 900;
  font-size: 1.1rem;
  width: 1.6rem;
  text-align: center;
}

.player-name {
  flex: 1;
  font-weight: 600;
}

.player-score {
  font-weight: 800;
  color: var(--brand-primary);
}

.hint {
  color: var(--brand-muted);
}
</style>
