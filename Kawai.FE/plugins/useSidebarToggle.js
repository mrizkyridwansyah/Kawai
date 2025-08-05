// plugins/sidebarToggle.client.js
import { ref } from 'vue'

export default defineNuxtPlugin((nuxtApp) => {
  const isSidebarToggled = ref(false)
  const isSidebarMinified = ref(false)

  const toggleSidebar = () => {
    isSidebarToggled.value = !isSidebarToggled.value;
  }

  const toggleSidebarMinified = () => {
    isSidebarMinified.value = !isSidebarMinified.value;
  }

  // Dapatkan router instance
  const router = useRouter()

  // Setiap route berubah, reset sidebar toggle ke false
  router.afterEach(() => {
    isSidebarToggled.value = false
    isSidebarMinified.value = false
  })

  nuxtApp.provide('sidebarToggle', {
    isSidebarToggled,
    isSidebarMinified,
    toggleSidebar,
    toggleSidebarMinified
  })
})
