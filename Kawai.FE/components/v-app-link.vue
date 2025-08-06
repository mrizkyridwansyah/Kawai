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
  },
}
</script>