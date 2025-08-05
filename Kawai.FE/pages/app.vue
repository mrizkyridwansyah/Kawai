<template>
  <side-nav />
  <div id="content" class="app-content">
    <nuxt-page />
  </div>
</template>

<script setup>
definePageMeta({
  layout: "app",
  middleware: 'auth',
});
</script>

<style>
.nav-tabs>.nav-item {
  margin-left: 10px;
}

.slide-left {
  -webkit-animation-duration: 400ms;
  animation-duration: 400ms;
  -webkit-animation-name: slideLeft;
  animation-name: slideLeft;
}

@-webkit-keyframes slideLeft {
  from {
    opacity: 0;
    transform: translate(100px, 0);
  }

  50% {
    opacity: 1;
    transform: translate(0, 0);
  }
}

@keyframes slideLeft {
  from {
    opacity: 0;
    transform: translate(100px, 0);
  }

  50% {
    opacity: 1;
    transform: translate(0, 0);
  }
}

.btn-elevate {
  color: white;
  padding: 6px 14px;
  border: none;
  border-radius: 4px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2); /* efek bayangan */
  transition: box-shadow 0.3s ease, transform 0.2s ease;
  display: inline-flex;
  align-items: center;
  cursor: pointer;
}

.btn-elevate:hover {
  box-shadow: 0 8px 12px rgba(0, 0, 0, 0.3);
  transform: translateY(-2px); /* angkat tombol */
}

.btn-elevate:active {
  box-shadow: 0 3px 5px rgba(0, 0, 0, 0.2);
  transform: translateY(0);
}
</style>