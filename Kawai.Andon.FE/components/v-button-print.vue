<template>
  <button
    class="btn btn-sm btn-green btn-elevate"
    :class="cClass"
    @click="print"
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
    <font-awesome-icon v-else :icon="icon || 'qrcode'" />
    <span class="ml-2">{{ label || "Print" }}</span>
  </button>
</template>

<script>
export default {
  props: ["label", "print", "icon", "cClass", "isLoading", "disabled"],
};
</script>
