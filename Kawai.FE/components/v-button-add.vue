<template>
  <button
    class="btn btn-sm btn-primary btn-elevate"
    :class="cClass"
    @click="add"
  >
    <font-awesome-icon :icon="icon || 'add'" />
    <span class="ml-2">{{ label || "New" }}</span>
  </button>
</template>

<script>
export default {
  props: ["label", "add", "icon", "cClass"],
};
</script>
