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
    <!-- <header-change-password /> -->
    <nuxt-page :key="$route.fullPath" />
    <v-boot-loader v-if="user.isLoading" />
    <v-modal id="change-password" title="Change Password" size="400">
      <chg-pwd />
    </v-modal>
  </div>
</template>
<script>
import ChgPwd from './chg-pwd'
const { $sidebarToggle } = useNuxtApp()

export default {
  components: { ChgPwd },
  computed: {
    user: function () {
      return useProfile();
    }
  }
}
</script>