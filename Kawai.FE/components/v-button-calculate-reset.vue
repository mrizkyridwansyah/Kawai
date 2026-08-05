<template>
  <div class="btn-group-nowrap d-inline-flex">
    <button
      class="btn btn-primary btn-elevate btn-search"
      @click="search"
      :disabled="disabled"
    >
      <font-awesome-icon icon="cogs" />
      <span class="ml-2">Calculate BOM</span>
    </button>
    <span style="padding:5px;font-weight: bold;color:gray;" >Kebutuhan material = BOM Qty × Input Qty.</span>
    <span style="margin-left: 10px;"></span>
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
