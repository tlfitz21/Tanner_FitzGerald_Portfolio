<!-- AI-GENERATED FILE — Claude (Anthropic), PM4 milestone -->
<!-- Prompt: "Create a countdown timer component for active quiz questions" -->
<script setup lang="ts">
import { ref, watch, onUnmounted } from 'vue'

const props = defineProps<{
  duration: number
  active: boolean
}>()

const emit = defineEmits<{
  (e: 'timeUp'): void
}>()

const remaining = ref(props.duration)
let intervalId: ReturnType<typeof setInterval> | null = null

const clearTimer = () => {
  if (intervalId !== null) {
    clearInterval(intervalId)
    intervalId = null
  }
}

const startTimer = () => {
  clearTimer()
  remaining.value = props.duration
  intervalId = setInterval(() => {
    remaining.value -= 1
    if (remaining.value <= 0) {
      clearTimer()
      emit('timeUp')
    }
  }, 1000)
}

watch(
  () => props.active,
  (isActive) => {
    if (isActive) {
      startTimer()
    } else {
      clearTimer()
    }
  },
  { immediate: true },
)

onUnmounted(clearTimer)

const percentage = ref(100)
watch(remaining, (val) => {
  percentage.value = Math.max(0, (val / props.duration) * 100)
})
</script>

<template>
  <div class="timer" :class="{ urgent: remaining <= 5 }">
    <div class="timer-bar">
      <div class="timer-fill" :style="{ width: percentage + '%' }" />
    </div>
    <span class="timer-text">{{ remaining }}s</span>
  </div>
</template>

<style scoped>
.timer {
  display: flex;
  align-items: center;
  gap: 0.6rem;
}

.timer-bar {
  flex: 1;
  height: 8px;
  border-radius: 4px;
  background: var(--brand-surface-soft);
  overflow: hidden;
}

.timer-fill {
  height: 100%;
  border-radius: 4px;
  background: var(--brand-primary);
  transition: width 1s linear;
}

.urgent .timer-fill {
  background: var(--brand-danger);
}

.timer-text {
  font-weight: 800;
  font-size: 1rem;
  min-width: 2.5rem;
  text-align: right;
}

.urgent .timer-text {
  color: var(--brand-danger);
}
</style>
