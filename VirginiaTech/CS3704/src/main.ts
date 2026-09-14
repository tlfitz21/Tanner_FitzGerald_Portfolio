import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { initializeAuthFromSession } from './store/appState'

await initializeAuthFromSession()

createApp(App).use(router).mount('#app')
