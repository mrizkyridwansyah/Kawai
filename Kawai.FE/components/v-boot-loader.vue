<template>
  <div class="v-boot-loader">
    <div class="row">
      <div class="col-lg-12 text-center mb-3">
        <img 
          src="/panasonic-login.png" alt="" 
          style="width: 320px" 
        />
      </div>
      <div class="col-lg-12 text-center mt-5">
        <v-loading-2 />
        <div>Loading please wait...</div>
      </div>
    </div>
  </div>
</template>

<style lang="scss">
.v-boot-loader {
  position: fixed;
  display: flex;
  bottom: 0;
  top: 0;
  left: 0;
  right: 0;
  background: rgba($color: #fff, $alpha: 1);
  z-index: 9999;
  justify-content: center;
  align-items: center;
}
</style>
