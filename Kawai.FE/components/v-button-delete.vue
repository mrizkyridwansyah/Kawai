<template>
  <button
    class="btn btn-sm btn-blue btn-elevate"
    :class="cClass"
    @click="submit"
    :disabled="
      (disabled !== undefined && disabled !== false) || isLoading
    "
  >
    <div
      class="spinner-border spinner-border-sm text-light"
      role="status"
      v-if="isLoading"
    >
      <span class="visually-hidden">Loading...</span>
    </div>
    <font-awesome-icon v-else :icon="icon || 'save'" />
    <span class="ml-2">{{ label || "Submit" }}</span>
  </button>
</template>

<script>
export default {
  props: ["label", "submit", "icon", "cClass", "isLoading", "disabled"],
};
</script>
