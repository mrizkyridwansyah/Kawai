<template>
  <div id="header" class="app-header">
    <!-- BEGIN navbar-header -->
    <div class="navbar-header">
      <span class="navbar-brand">
        <span class="navbar-logo"></span
        ><b class="me-3px" style="color: white">ANDON</b>
        <span style="color: white">WMS</span>
      </span>
    </div>
    <!-- END navbar-header -->

    <!-- BEGIN header-nav -->
    <div class="navbar-nav" style="border-top: inherit">
      <div class="navbar-item navbar-form">
        <v-search-menu placeholder="Search Menu..." />
      </div>
      <button class="btn-hamburger" @click="toggleSidebar">
        <font-awesome-icon icon="bars" />
      </button>
      <!-- <header-menu-user-info /> -->
    </div>

    <!-- Optional time (hidden) -->
    <div class="navbar-nav" style="display: none">
      <header-menu-time />
    </div>
  </div>
  <!-- Sidebar Menu (slide dari kanan) -->
  <div class="sidebar" :class="{ open: isSidebarOpen }">
    <div class="sidebar-header">
      <span>Menu</span>
    </div>
    <div class="sidebar-body">
      <v-app-link
        v-for="menu in list"
        :key="'sidebar-' + menu.url"
        :to="menu.url"
        class="sidebar-menu-item"
        @click="toggleSidebar"
      >
        <font-awesome-icon :icon="menu.icon" />
        <span>{{ menu.text }}</span>
      </v-app-link>
    </div>
  </div>

  <!-- Backdrop -->
  <div v-if="isSidebarOpen" class="backdrop" @click="toggleSidebar"></div>
</template>

<script>
export default {
  data: () => ({
    list: [
      { url: "/receiving", icon: "chart-column", text: "Receiving Andon" },
      { url: "/request-control", icon: "file-lines", text: "Womin Request Control" },
       { url: "/production-control", icon: "file-lines", text: "Production" },
    ],
    isDragging: false,
    startX: 0,
    currentTranslate: 0,
    prevTranslate: 0,
    animationDirection: "normal",
    isSidebarOpen: false,
  }),
  methods: {
    toggleSidebar() {
      this.isSidebarOpen = !this.isSidebarOpen;
    },
    startDrag(e) {
      this.isDragging = true;
      this.startX = e.type.includes("touch") ? e.touches[0].clientX : e.clientX;
      this.prevTranslate = this.currentTranslate;
    },
    stopDrag() {
      if (!this.isDragging) return;
      this.isDragging = false;
      if (this.currentTranslate > this.prevTranslate) {
        this.animationDirection = "reverse";
      } else {
        this.animationDirection = "normal";
      }
    },
    onDrag(e) {
      if (!this.isDragging) return;
      const currentX = e.type.includes("touch")
        ? e.touches[0].clientX
        : e.clientX;
      const diff = currentX - this.startX;
      this.currentTranslate = this.prevTranslate + diff;
    },
  },
};
</script>

<style lang="scss">
.app-header {
  background: linear-gradient(to right, #007bff, #8ec5fc);
  padding: 0.5rem 1rem;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.navbar-header {
  display: flex;
  align-items: center;
}

.navbar-brand {
  font-size: 20px;
  font-weight: bold;
}

.navbar-nav {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.btn-hamburger {
  background: transparent;
  border: none;
  color: white;
  font-size: 20px;
  cursor: pointer;

  &:hover {
    color: #ddd;
  }
}

.app-header-menu {
  width: 100%;
  background: white;
  padding: 0.5em 0;
  box-shadow: 0 2px 2px rgba(0, 0, 0, 0.1);
}

.navbar-menu-wrapper {
  width: 100%;
  height: 8em !important;
  overflow: hidden;
  position: relative;
  cursor: grab;
}

.navbar-menu-loop {
  display: flex;
  width: max-content;
  animation: scroll-loop 100s linear infinite;
  animation-direction: normal;

  &:hover {
    animation-play-state: paused;
  }
}

.navbar-menu {
  display: flex;
  gap: 1em;
}

.menu-item {
  width: 10em;
  height: 7em;
  flex: 0 0 auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  background: #888;
  color: black;
  font-weight: bold;
  text-decoration: none;
  font-size: 14px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
  transition: all 0.3s ease;
  padding: 1em;
  cursor: grab;

  &:active {
    cursor: grabbing;
  }

  &:hover {
    transform: scale(1.05);
    background: linear-gradient(to bottom right, #e0e0e0, #fafafa);
    color: #333;
  }

  .menu-icon {
    font-size: 28px;
    margin-bottom: 6px;
  }

  .menu-text {
    font-weight: 600;
    font-size: 13px;
  }
}

@media (min-width: 768px) {
  .menu-item {
    min-width: 100px;
    height: 100px;
    font-size: 16px;

    .menu-icon {
      font-size: 32px;
    }

    .menu-text {
      font-size: 15px;
    }
  }
}

@keyframes scroll-loop {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(-50%);
  }
}

.sidebar {
  position: fixed;
  top: 0;
  right: -300px;
  width: 260px;
  height: 100vh;
  background-color: #fff;
  box-shadow: -2px 0 10px rgba(0, 0, 0, 0.2);
  z-index: 1050;
  transition: right 0.3s ease-in-out;
  display: flex;
  flex-direction: column;
}

.sidebar.open {
  right: 0;
}

.sidebar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1em;
  background: #f5f5f5;
  font-weight: bold;
  border-bottom: 1px solid #ddd;
}

.sidebar-body {
  padding: 1em;
  flex: 1;
  overflow-y: auto;
}

.sidebar-menu-item {
  display: flex;
  align-items: center;
  gap: 0.75em;
  padding: 0.75em 1em;
  text-decoration: none;
  color: #333;
  border-radius: 6px;
  transition: background 0.2s;

  &:hover {
    background-color: #f0f0f0;
  }

  svg {
    font-size: 18px;
  }
}

.backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1040;
}
</style>
