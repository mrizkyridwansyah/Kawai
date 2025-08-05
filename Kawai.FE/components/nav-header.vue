<template>
  <div id="header" class="app-header">
    <!-- BEGIN navbar-header -->
    <div class="navbar-header">
      <v-app-link class="navbar-brand" to="/"
        ><span class="navbar-logo"></span
        ><b class="me-3px">TOS</b> Admin</v-app-link
      >
      <button
        type="button"
        style="padding: 2em; cursor: pointer"
        class="navbar-mobile-toggler"
        data-toggle="app-sidebar-mobile"
        @click="handleToggle"
      >
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </button>
    </div>
    <!-- END navbar-header -->

    <!-- BEGIN header-nav -->
    <div class="navbar-nav">
      <div class="navbar-item navbar-form">
        <v-search-menu placeholder="Search Menu..."/>
      </div>
      <header-menu-user-info />
    </div>
    <!-- END header-nav -->
    <!-- Backdrop -->
    <div v-if="isSidebarOpen" class="backdrop" @click="handleToggle"></div>
  </div>
</template>

<script setup>
const { $sidebarToggle } = useNuxtApp();

function handleToggle() {
  $sidebarToggle.toggleSidebar();
}

const isSidebarOpen = $sidebarToggle.isSidebarToggled;

</script>

<style lang="scss">
.navbar-light {
  background-color: #fff !important;
  border-bottom: solid 1px rgba(0, 0, 0, 0.1);
}

.backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1040; // Pastikan lebih tinggi dari sidebar
}
</style>
