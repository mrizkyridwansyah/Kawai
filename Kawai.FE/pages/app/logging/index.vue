<template>
  <div style="width: 100%;text-align: center; margin-top: 150px;">
    <span style="font-size: 30px;opacity: .5;">Logging</span>
  </div>
</template>

<script>
export default {
  mounted: function () {
    this.$router.push("logging/data-log");    
  }
}
</script>
