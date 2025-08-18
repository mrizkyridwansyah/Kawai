<template>
  <div id="app" :class="[
    'app',
    'app-header-fixed',
    'app-sidebar-fixed',
    // 'app-gradient-enabled',
    { 'app-sidebar-mobile-toggled': $sidebarToggle.isSidebarToggled.value },
    { 'app-sidebar-minified': $sidebarToggle.isSidebarMinified.value }
  ]">
    <nav-header />
    <nuxt-page :key="$route.fullPath" />
    <!-- <v-boot-loader v-if="user.isLoading" /> -->
  </div>
</template>
<script>
const { $sidebarToggle } = useNuxtApp()

export default {
  computed: {
    user: function () {
      return useProfile();
    }
  }
}
</script>