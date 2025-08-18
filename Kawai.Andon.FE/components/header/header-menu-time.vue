<template>
  <div
    class="text-center mt-3 mb-3 mr-5"
    style="font-size: 1.25em; font-weight: bolder"
  >
    <span class="d-md-inline fw-bold">{{
      $func.formatDate(this.time)
    }}</span>
    <br />
    <span class="d-md-inline fw-bold">{{
      $func.formatTimeSecond(this.time)
    }}</span>
  </div>
</template>

<script>
export default {
  data: () => ({
    time: new Date(),
  }),
  mounted: function () {
    setInterval(() => {
      this.time = new Date();
    }, 1000);
  },
};
</script>
