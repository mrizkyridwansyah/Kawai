<template>
  <div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true">
    <div class="toast-header">
      <svg class="bd-placeholder-img rounded me-2" width="20" height="20" xmlns="http://www.w3.org/2000/svg" aria-hidden="true" preserveAspectRatio="xMidYMid slice" focusable="false">
        <rect width="100%" height="100%" :fill="color"></rect>
      </svg>
      <strong class="me-auto">
        <span v-if="variant == 'success'">Success</span>
        <span v-if="variant == 'danger'">Error</span>
        <span v-if="variant == 'warning'">Warning</span>
      </strong>
      <!-- <small class="text-muted">just now</small> -->
      <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body">
      <!-- <v-icon 
        name="check-circle"
        color="#00e396"
        width="40"
      /> -->
      <span>{{message}}</span>
    </div>
  </div>
</template>

<script>
export default {
  data: () => ({
    message: 'test',
    variant: 'default'
  }),
  computed: {
    color: function() {
      if(this.variant.toLowerCase() == 'success')
        return '#00e396';

      if(this.variant.toLowerCase() == 'danger')
        return '#f44336';

      if(this.variant.toLowerCase() == 'warning')
        return 'yellow';

      return '#fff';
    }
  }
}
</script>

<style>
.toast-container{
  z-index: 1500;
}
</style>