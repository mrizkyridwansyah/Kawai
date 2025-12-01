<template>
  <button
    class="btn btn-sm btn-primary btn-elevate"
    v-if="!featureId || profile.allowed(featureId)"
    @click="search"
    :disabled="disabled"
  >
    <font-awesome-icon icon="search" />
    <span class="ml-2">Search</span>
  </button>
  <button
    class="btn btn-sm btn-danger btn-elevate txt-light ms-1"
    @click="reset"
    :disabled="disabled"
  >
    <font-awesome-icon icon="rotate-left" />
    <span class="ml-2">Reset</span>
    
  </button>
</template>

<script>
export default {
  props: ["search", "reset", "disabled"],
  computed: {
    profile: function () {
      return useProfile();
    },
  },
};
</script>
