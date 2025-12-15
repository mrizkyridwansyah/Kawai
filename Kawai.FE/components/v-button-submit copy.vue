<template>
  <div
    class="d-grid gap-2"
    :class="{
      'mt-4': nomargintop == undefined || !nomargintop,
    }"
  >
    <button
      class="btn btn-white rounded-pill"
      type="button"
      style="color: black"
      :style="{
        width: width,
      }"
      @click="close"
      :disabled="
        (disabled !== undefined && disabled !== false) || isLoading !== false
      "
    >
      <div
        class="spinner-border spinner-border-sm text-light"
        role="status"
        v-if="isLoading"
      >
        <span class="visually-hidden">Loading...</span>
      </div>
      {{ label || "Cancel" }}
    </button>
  </div>
</template>

<script>
export default {
  props: ["isLoading", "disabled", "label", "submit", "width", "nomargintop"],
  mounted: function () {},
};
</script>
