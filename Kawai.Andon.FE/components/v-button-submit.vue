<template>
  <div class="d-grid gap-2 mt-4">
    <button class="btn btn-primary rounded-pill" type="button" style="color: white;"
      :style="{
        width: width
      }"
      @click="submit"
      :disabled="(disabled !== undefined && disabled !== false) || isLoading !== false">
      <div class="spinner-border spinner-border-sm text-light" role="status" v-if="isLoading">
        <span class="visually-hidden">Loading...</span>
      </div>
      {{ label || 'Submit' }}
    </button>
  </div>
</template>

<script>
export default {
  props: ['isLoading', 'disabled', 'label', 'submit', 'width'],
  mounted: function() {
    
  }
}
</script>