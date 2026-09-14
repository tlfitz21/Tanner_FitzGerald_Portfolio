import { createRouter, createWebHistory } from 'vue-router'
import ConsoleView from '../views/ConsoleView.vue'
import HomeView from '../views/HomeView.vue'
import JoinView from '../views/JoinView.vue'
import LobbyView from '../views/LobbyView.vue'
import LoginView from '../views/LoginView.vue'
import GameResultsView from '../views/GameResultsView.vue'
import AnswerReviewView from '../views/AnswerReviewView.vue'
import { appState } from '../store/appState'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/join', name: 'join', component: JoinView },
    { path: '/lobby/:code', name: 'lobby', component: LobbyView },
    { path: '/login', name: 'login', component: LoginView },
    { path: '/results/:code', name: 'results', component: GameResultsView },
    { path: '/answers/:code', name: 'answers', component: AnswerReviewView, meta: { requiresAuth: true } },
    {
      path: '/console',
      name: 'console',
      component: ConsoleView,
      meta: { requiresAuth: true },
    },
  ],
})

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !appState.loggedInUserId) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  return true
})

export default router
