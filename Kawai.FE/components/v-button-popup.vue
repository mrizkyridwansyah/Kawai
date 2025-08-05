<template>
  <div class="filter-button">
    <div class="back-drop" v-if="showPopUp" @click="() => showPopUp = !showPopUp">
    </div>
    <button class="btn btn-sm btn-lime btn-elevate" @click="() => showPopUp = !showPopUp">
      <slot name="button-content" />
    </button>
    <div v-if="showPopUp" class="c-pop-up animate__animated animate__fadeInDown" style="z-index: 100;" >
      <div class="c-pop-up-close" @click="() => showPopUp = !showPopUp">
        <v-icon name="x" :width="20" style="color: #555" />
      </div>
      <slot />
    </div>
  </div>
</template>

<script>
export default {
  data: () => ({
    showPopUp: false,
  })
}
</script>

<style lang="scss">
.filter-button {
  display: inline;
  position: relative;
  .back-drop {
    background: rgba($color: #000000, $alpha: .2);
    position: fixed;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    z-index: 1;
    opacity: 1;
    transition: opacity 500ms ease-in-out;
  }

  .but {
    position: relative;
    z-index: 1;
  }

  .c-pop-up {
    position: absolute;
    top: 20px;
    z-index: 4;
    background: #fff;
    border-radius: 5px;
    // padding: 20px;
    padding: 20px 18px 10px 20px;
    box-shadow: 1px 1px 10px rgba($color: #000000, $alpha: .1);
    opacity: 1;
    transition: opacity 500ms ease-in-out;
  }

  .c-pop-up-close {
    padding: 2px;
    border-radius: 5px;
    position: absolute;
    right: -5px;
    top: -5px;
    // border: solid 1px rgba($color: #000000, $alpha: .1);
    box-shadow: 1px 1px 10px rgba($color: #000000, $alpha: .2);
    background: #fff;
    &:hover{
      background-color: #eee;
      cursor: pointer;
    }
  }
  .btn-group-xs>.btn, .btn-xs {
    padding: 0.15rem 0.25rem!important;
    font-size: .675rem!important;
    border-radius: 0.2rem!important;
  }

  .btn-xs {
    padding-left: 8px!important;
    padding-right: 8px!important;
    &:disabled{
      background-color: #c2d1df!important;
    }
  }
}
.btn-group {
  .form-check-inline {
    margin-right: 0;
  }
  // .btn-xs {
  //   padding-left: 10px!important;
  //   padding-right: 10px!important;
  //   &:disabled{
  //     background-color: #c2d1df!important;
  //   }
  // }
  .form-check-inline:first-child {
    .btn-xs {
      border-top-left-radius: 7px!important;
      border-bottom-left-radius: 7px!important;
      border-top-right-radius: 0!important;
      border-bottom-right-radius: 0!important;
    }
  }
  .form-check-inline:last-child {
    .btn-xs {
      border-top-right-radius: 7px!important;
      border-bottom-right-radius: 7px!important;
      border-top-left-radius: 0!important;
      border-bottom-left-radius: 0!important;
    }
  }
}

</style>
