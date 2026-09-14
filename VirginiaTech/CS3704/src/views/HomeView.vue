<script setup lang="ts"></script>

<template>
  <div class="screen home-screen">
    <h1>Swiftician</h1>
    <p class="subtitle">Choose how you want to continue</p>

    <div class="actions">
      <RouterLink class="action-btn host" to="/console">Host / Console</RouterLink>
      <RouterLink class="action-btn join" to="/join">Join Event</RouterLink>
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

h1 {
  font-size: clamp(2rem, 5vw, 3.4rem);
  font-weight: 800;
}

.subtitle {
  color: var(--brand-muted);
}

.actions {
  margin-top: 1rem;
  display: grid;
  gap: 0.8rem;
}

.action-btn {
  color: var(--brand-text);
  text-decoration: none;
  font-weight: 700;
  border-radius: 10px;
  padding: 0.85rem 1rem;
}

.host {
  background: var(--brand-primary);
}

.join {
  background: var(--brand-primary);
}
</style>
