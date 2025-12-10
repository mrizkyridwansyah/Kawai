<template>
  <div class="btn-group-nowrap d-inline-flex">
    <button
      class="btn btn-primary btn-elevate btn-search"
      v-if="!featureId || profile.allowed(featureId)"
      @click="search"
      :disabled="disabled"
    >
      <font-awesome-icon icon="search" />
      <span class="ml-2">Search</span>
    </button>

    <button
      class="btn btn-danger btn-elevate txt-light ms-1 btn-search"
      @click="reset"
      :disabled="disabled"
    >
      <font-awesome-icon icon="rotate-left" />
      <span class="ml-2">Clear</span>
    </button>
  </div>
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
<style scoped>
.btn-group-nowrap {
  white-space: nowrap;
}

</style>
