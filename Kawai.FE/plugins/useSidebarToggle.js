// plugins/useSidebarToggle.js
import { ref, watch, onMounted } from 'vue';

export default defineNuxtPlugin((nuxtApp) => {
  // State sidebar
  const isSidebarToggled = ref(false);   // overlay toggle (mobile)
  const isSidebarMinified = ref(false);  // minify toggle

  // Load persisted state dari localStorage
  onMounted(() => {
    const saved = localStorage.getItem('sidebarMinified');
    if (saved !== null) isSidebarMinified.value = saved === 'true';
  });

  // Simpan state minified ke localStorage setiap berubah
  watch(isSidebarMinified, (val) => {
    localStorage.setItem('sidebarMinified', val);
  });

  // Toggle overlay (untuk mobile)
  const toggleSidebar = () => {
    isSidebarToggled.value = !isSidebarToggled.value;
  };

  // Toggle minified sidebar
  const toggleSidebarMinified = () => {
    isSidebarMinified.value = !isSidebarMinified.value;
  };

  // Reset overlay (mobile) setelah navigasi, tapi jangan reset minified
  const router = nuxtApp.$router;
  router.afterEach(() => {
    isSidebarToggled.value = false;
  });

  // Provide state & methods ke app
  nuxtApp.provide('sidebarToggle', {
    isSidebarToggled,
    isSidebarMinified,
    toggleSidebar,
    toggleSidebarMinified
  });
});