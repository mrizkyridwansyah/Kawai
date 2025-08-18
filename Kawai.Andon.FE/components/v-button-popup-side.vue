<template>
  <div class="filter-button">
    <div class="back-drop" v-if="showPopUp" @click="() => showPopUp = !showPopUp">
    </div>
    <b-button variant="secondary" @click="() => showPopUp = !showPopUp">
      <slot name="button-content">
        Filter
      </slot>
    </b-button>
    <div v-if="showPopUp" class="c-pop-up-side  slide-right" >
      <div class="c-pop-up-side-close" @click="() => showPopUp = !showPopUp">
        <v-icon name="x" :width="20" style="color: #555" />
      </div>
      <div class="c-pop-up-side-container">
        <slot />
      </div>
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

  .c-pop-up-side {
    position: fixed;
    top: 95px;
    left: 61px;
    bottom: 0;
    z-index: 2;
    background: #fff;
    // border-radius: 10px;
    padding: 20px;
    box-shadow: 1px 1px 10px rgba($color: #000000, $alpha: .1);
    opacity: 1;
    transition: opacity 500ms ease-in-out;
    min-width: 300px;
    .c-pop-up-side-container {
      overflow-y: auto;
      overflow-x: hidden;
      position: absolute;
      top: 0;
      bottom: 0;
      left: 0;
      right: 0;

      .accordion-button {
        padding:  12px 15px;
        font-size: 14px;
        font-weight: 500;
        border-radius: 0;

        &::after {
          width: 14px;
          height: 14px;
          background-size: 14px;
        }
      }

      &::-webkit-scrollbar {
        width: 7px;
        height: 7px;
      }
      
      /* Track */
      &::-webkit-scrollbar-track {
        background: #f1f1f1;
      }
      
      /* Handle */
      &::-webkit-scrollbar-thumb {
        background: rgba(0, 0, 0, 0.2);
        border-radius: 5px;
      }
      
      /* Handle on hover */
      &::-webkit-scrollbar-thumb:hover {
        background: rgba(0, 0, 0, 0.3);
      }
    }
  }

  .c-pop-up-side-close {
    padding: 7px;
    border-radius: 10px;
    position: absolute;
    right: -35px;
    top: 15px;
    // border: solid 1px rgba($color: #000000, $alpha: .1);
    box-shadow: 1px 1px 10px rgba($color: #000000, $alpha: .2);
    background: #fff;
    z-index: 2;
    &:hover{
      background-color: #eee;
      cursor: pointer;
    }
  }
}
.btn-group-xs>.btn, .btn-xs {
  padding: 0.15rem 0.25rem!important;
  font-size: .875rem!important;
  border-radius: 0.2rem!important;
}

.btn-xs {
  padding-left: 8px!important;
  padding-right: 8px!important;
  &:disabled{
    background-color: #c2d1df!important;
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
