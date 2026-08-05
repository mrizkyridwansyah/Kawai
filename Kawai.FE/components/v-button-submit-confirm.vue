<template>
  <button
    class="btn btn-sm btn-primary btn-elevate"
    :class="cClass"
    @click="add"
  >
    <font-awesome-icon :icon="icon || 'check'" />
    <span class="ml-2">{{ label || "Confimation" }}</span>
  </button>
</template>

<script>
export default {
  props: ["label", "add", "icon", "cClass"],
};
</script>
