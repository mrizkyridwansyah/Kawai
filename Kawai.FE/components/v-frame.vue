<template>
  <div class="panel panel-inverse">
    <div
      class="panel-heading ui-sortable-handle"
      style="background-color: #002060 !important"
    >
      <font-awesome-icon
        v-if="icon"
        :icon="icon"
        style="font-size: larger"
        class="mr-3"
      />
      <span style="font-size: larger">{{ title }}</span>
    </div>
    <div class="panel-body">
        <slot name="frame-content" />
    </div>
  </div>
</template>

<script>
export default {
  props: ["title", "icon"],
};
</script>

<style scoped>
.elevated-tree-container {
  border: 1px solid #e0e0e0;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  border-radius: 8px;
  background-color: #fff;
}
.panel.panel-inverse {
  display: flex;
  flex-direction: column;
  height: max-content; /* full height of viewport */
  min-height: 85vh; /* full height of viewport */
  /* max-height: 85vh; */
  overflow: hidden; /* prevent full panel overflow */
}

.panel-body {
  flex: 1 1 auto;
  overflow-y: auto;
  padding: 16px;
}
</style>
