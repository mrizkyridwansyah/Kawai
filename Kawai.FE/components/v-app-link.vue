<template>
  <nuxt-link :to="link"
    :exact-active-class="exactActiveClass || ''"
    :active-class="activeClass || ''"
    @click="handleClick"
    >
    <slot />
  </nuxt-link>
</template>

<script>
export default {
  props: ['to', 'exact-active-class', 'active-class', 'disabled'],
  computed: {
    link: function () {
      return this.disabled ? null : `/app${this.to}`;
    },
    dsClosing: function () {
      return useMktNUPClosing();
    },
  },
  methods: {
    async handleClick(event) {
      // If Ctrl/Cmd key is pressed or middle mouse button, allow default behavior
      if (event.ctrlKey || event.metaKey || event.which === 2) {
        return;
      }
      
      // Prevent default only for normal clicks
      event.preventDefault();
      
      // Run your function before navigation
      await this.beforeRedirect();

      // Setelah fungsi selesai, baru lakukan navigasi
      if (!this.disabled) {
        this.$router.push(this.link);
      }
    },
    async beforeRedirect() {
      if (window.location.pathname != "/app/launching/closing/create-1")
        return;

      await this.dsClosing.processUnit(this.$route.query.id, { UnitStatus: "AVAILABLE" });
    }
  }
}
</script>