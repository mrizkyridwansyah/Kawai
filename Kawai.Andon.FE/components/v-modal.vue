<template>
  <b-modal 
    :id="id"
    :title="title"
    :size="size"
    :body-class="bodyClass || ''"
    class="zoom"
    top
    hide-footer
    no-close-on-esc
    no-close-on-backdrop
    :scrollable="scrollable !== undefined && scrollable !== false"
    v-model="isShow"
  >
    <template #header>
      <h5 class="modal-title">{{title}}</h5>
      <button type="button" class="btn-close" 
        data-bs-dismiss="modal" 
        aria-label="Close"
        :data-bs-target="`#${id}`"
        :disabled="disableClose"
      >
      </button>
    </template>
    <v-loading class="mb-3" v-if="loading === true" style="margin-left: -24px;margin-right: -24px;"></v-loading>
    <v-alert-invalid-form  
      v-if="errors && JSON.stringify(errors) != '{}'"
      type="Invalid"
      message="Please fill the field correctly"
    />
    <v-alert-invalid-form  
      v-if="error && JSON.stringify(error) != '{}'"
      :type="error.Status"
      :message="error.Message || 'Please fill the field correctly'"
    />
    <slot v-if="isShow"/>
  </b-modal>  
</template>

<script>

export default {
  props: ['id', 'title', 'errors', 'error', 'loading', 'size', 'bodyClass', 'disableClose', 'scrollable'],
  emits: ['shown', 'hidden', 'hide', 'show'],
  data: () => ({
    isShow: false,
  }),
  mounted: function() {
    const myModalEl = document.getElementById(this.id)
    var _this = this;
    myModalEl?.addEventListener('show.bs.modal',() => {
      _this.isShow = true
      this.$emit('show')
    })
    myModalEl?.addEventListener('hide.bs.modal',() => {
      _this.isShow = true
      this.$emit('hide')
    })
    myModalEl?.addEventListener('hidden.bs.modal',() => this.$emit('hidden'))
    myModalEl?.addEventListener('shown.bs.modal',() => this.$emit('shown'))
  },
  methods: {
  }
}
</script>