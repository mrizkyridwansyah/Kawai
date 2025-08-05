<template>
  <button class="btn btn-sm btn-primary btn-elevate" :class="cClass" @click="import">
    <font-awesome-icon :icon="icon || 'upload'" />
    <span class="ml-2">{{ label || "Import" }}</span>
  </button>
</template>
<script>
export default {
    props: ["label", "import", 'icon', 'cClass']
};
</script>
