<template>
  <div id="header" class="app-header">
    <!-- BEGIN navbar-header -->
    <div class="navbar-header">
      <span class="navbar-brand">
        <span class="navbar-logo"></span><b class="me-3px">TOS</b> ANDON
      </span>
    </div>
    <!-- END navbar-header -->

    <!-- BEGIN header-nav -->
    <div class="navbar-nav">
      <div class="navbar-item navbar-form">
        <v-search-menu placeholder="Search Menu..." />
      </div>
      <header-menu-user-info />
    </div>

    <!-- BEGIN header-nav -->
    <div class="navbar-nav" style="display: none">
      <header-menu-time />
    </div>
    <!-- END header-nav -->
  </div>
  <!-- NAVIGATION SLIDER -->
  <div class="app-header-menu mt-5" style="display: none">
    <div
      class="navbar-menu-wrapper"
      @mousedown="startDrag"
      @touchstart="startDrag"
      @mouseup="stopDrag"
      @touchend="stopDrag"
      @mouseleave="stopDrag"
      @mousemove="onDrag"
      @touchmove="onDrag"
    >
      <div
        class="navbar-menu-loop"
        :style="{
          transform: `translateX(${currentTranslate}px)`,
          animationPlayState: isDragging ? 'paused' : 'running',
          animationDirection: animationDirection,
        }"
        ref="menuLoop"
      >
        <div class="navbar-menu ml-3" v-for="i in 10" :key="'loop-' + i">
          <v-app-link
            v-for="menu in list"
            :key="menu.url + '-' + i"
            :to="menu.url"
            class="menu-item"
          >
            <div class="menu-icon">
              <font-awesome-icon :icon="menu.icon" />
            </div>
            <div class="menu-text">{{ menu.text }}</div>
          </v-app-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data: () => ({
    list: [
      { url: "/receiving", icon: "bell", text: "Receiving" },
      { url: "/request", icon: "bell", text: "Request Control" },
    ],
    isDragging: false,
    startX: 0,
    currentTranslate: 0,
    prevTranslate: 0,
    animationDirection: "normal", // normal = kiri ke kanan, reverse = kanan ke kiri
  }),
  methods: {
    startDrag(e) {
      this.isDragging = true;
      this.startX = e.type.includes("touch") ? e.touches[0].clientX : e.clientX;
      this.prevTranslate = this.currentTranslate;
    },
    stopDrag() {
      if (!this.isDragging) return;
      this.isDragging = false;
      // Jika terakhir drag ke kanan (posisi bertambah), ubah arah animasi jadi reverse
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
.backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1040;
}

.app-header-menu {
  width: 100%;
  background: white; // biru gradasi ke muda
  padding: 0.5em 0 0.5em 0;
  box-shadow: 0 2px 2px rgba(0, 0, 0, 0.1);
}

.navbar-menu-wrapper {
  width: 100%;
  height: 8em !important;
  overflow: hidden;
  position: relative;
  cursor: grab;
}

.navbar-nav {
  width: 100%;
  background: linear-gradient(to right, #007bff, #8ec5fc); // gradasi biru muda
  padding: 0.5rem 1rem; // bisa disesuaikan
}
.navbar-menu-loop {
  display: flex;
  width: max-content;
  animation: scroll-loop 100s linear infinite;
  animation-direction: normal;
}

.navbar-menu-loop:hover {
  animation-play-state: paused;
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

  background: #888; // linear-gradient(to bottom right, #888, #ddd); // abu2 ke abu2 terang
  // border-radius: 16px;
  color: white;
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
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .menu-text {
    // color: white; // agar tetap kontras di background abu2
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
</style>
