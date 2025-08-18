<template>
  <nuxt-link :to="link"
    :exact-active-class="exactActiveClass || ''"
    :active-class="activeClass || ''"
  >
    <slot />
  </nuxt-link>
</template>

<script>
export default {
  props: ['to', 'exact-active-class', 'active-class'],
  computed:{
    wsId: function() {
      return this.$route.params.id;
    },
    link: function() {
      return `/admin-console/${this.wsId}${this.to}`;
    },
  }
}
</script>