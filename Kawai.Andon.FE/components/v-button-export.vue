<template>
  <button class="btn btn-sm btn-primary btn-elevate" :class="cClass" @click="export">
    <font-awesome-icon :icon="icon || 'download'" />
    <span class="ml-2">{{ label || "Export" }}</span>
  </button>
</template>
<script>
export default {
    props: ["label", "export", 'icon', 'cClass']
};
</script>
