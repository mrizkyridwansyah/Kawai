<template>
  <div class="alert alert-danger" role="alert">
    <div class="d-flex">
      <div>
        <icon-alert-circle />
      </div>
      <div class="ms-3">
        <h6 class="text-gray-900 fw-bolder">{{type}}!</h6>
        <div class="text-gray-700">{{ message }}</div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['message', 'type']
}
</script>