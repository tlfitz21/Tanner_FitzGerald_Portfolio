<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { createAccount, login } from '../store/appState'

const route = useRoute()
const router = useRouter()

const username = ref('')
const password = ref('')
const error = ref('')
const notice = ref('')
const isCreateMode = ref(false)

const redirectPath = computed(() => {
  const redirect = route.query.redirect
  return typeof redirect === 'string' ? redirect : '/console'
})

const submitAuth = async () => {
  error.value = ''
  notice.value = ''

  if (isCreateMode.value) {
    const createError = await createAccount(username.value, password.value)
    if (createError) {
      error.value = createError
      return
    }

    notice.value = 'Check your email for the confirmation link, then sign in.'
    isCreateMode.value = false
    return
  }

  const loginError = await login(username.value, password.value)
  if (loginError) {
    error.value =
      loginError.toLowerCase().includes('email not confirmed')
        ? 'Check your email for the confirmation link.'
        : loginError
    return
  }

  error.value = ''
  router.push(redirectPath.value)
}
</script>

<template>
  <div class="screen">
    <h1>{{ isCreateMode ? 'Create Account' : 'Login' }}</h1>
    <p class="subtitle">Teacher sign in for hosting and review</p>

    <form class="card" @submit.prevent="submitAuth">
      <label>
        Email
        <input v-model.trim="username" type="email" placeholder="teacher@school.com" />
      </label>

      <label>
        Password
        <input v-model.trim="password" type="password" placeholder="Password" />
      </label>

      <button type="submit">{{ isCreateMode ? 'Create Account' : 'Continue to Console' }}</button>
      <button type="button" class="secondary" @click="isCreateMode = !isCreateMode">
        {{
          isCreateMode
            ? 'Already have an account? Login'
            : "Don't have an account? Create one"
        }}
      </button>
      <p v-if="notice" class="notice">{{ notice }}</p>
      <p v-if="error" class="error">{{ error }}</p>
    </form>
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
  width: min(420px, 90vw);
  display: grid;
  gap: 0.9rem;
  background: var(--brand-surface);
  border-radius: 12px;
  padding: 1.1rem;
  border: 1px solid var(--brand-border);
  text-align: left;
}

label {
  display: grid;
  gap: 0.3rem;
  font-size: 0.9rem;
  font-weight: 600;
}

input {
  border: 1px solid var(--brand-border);
  border-radius: 8px;
  padding: 0.7rem;
  font-size: 1rem;
}

button {
  border: 0;
  border-radius: 8px;
  padding: 0.8rem;
  font-weight: 700;
  background: var(--brand-primary);
  cursor: pointer;
}

.secondary {
  background: var(--brand-surface-soft);
}

.notice {
  color: var(--brand-secondary);
  font-weight: 600;
}

.error {
  color: var(--brand-danger);
  font-weight: 600;
}
</style>
