<template>
  <b-modal 
    :id="id"
    :title="title"
    :body-class="bodyClass || ''"
    :class="direction"
    hide-footer
    no-close-on-esc
  >
    <template #header>
      <h5 class="modal-title">{{title}}</h5>
      <button type="button" class="btn-close" 
        data-bs-dismiss="modal" 
        aria-label="Close"
        :data-bs-target="`#${id}`"
      >

      </button>
    </template>
    <v-loading class="mb-3" v-if="loading === true" style="margin-left: -24px;margin-right: -24px;"></v-loading>
    <slot />
  </b-modal>
  <!-- <div class="modal fade" :class="direction" :id="id" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header" style="border-bottom: solid 1px #eeeeee; padding: 1rem;">
          <h5 class="modal-title mt-1">{{title}}</h5>
          <button type="button" class="btn btn-secondary" @click="closeModal">
            X
          </button>
        </div>
        <div class="modal-body">
          <slot />
        </div>

        <slot name="footer" />
      </div>
    </div>
  </div> -->
</template>

<script>

export default {
  props: ['id', 'shown', 'hidden', 'title', 'direction', 'bodyClass', 'loading'],
  mounted: function() {
    const myModalEl = document.getElementById(this.id)
    myModalEl?.addEventListener('hidden.bs.modal',() => this.$emit('hidden'))
    myModalEl?.addEventListener('shown.bs.modal',() => this.$emit('shown'))
    
  },
  methods: {
    closeModal: function() {
      this.$bvModal.hide(this.id)
    }
  }
}
</script>

<style lang="scss">
.modal {
  &.left {
    transition: all 200ms linear;
    translate: -20vw;
    opacity: .5;
    .modal-dialog {
      width: 50vw!important;
      position: fixed;
      top: 0;
      bottom: 0;
      margin: 0;

      .modal-content {
        border-radius: unset !important;
        height: 100%;
        width: 50vw;
        min-width: 600px;
      }
    }
    &.show {
      translate: 0;
      opacity: 1;
    }
  }
  
  &.right {
    transition: all 200ms linear;
    translate: 70vw;
    opacity: .5;
    .modal-dialog {
      width: 50vw!important;
      position: fixed;
      // right: 50vw;
      top: 0;
      bottom: 0;
      margin: 0;

      .modal-content {
        border-radius: unset !important;
        height: 100%;
        width: 50vw;
        min-width: 600px;
      }
    }
    &.show {
      translate: 50vw;
      opacity: 1;
    }
  }
}
</style>