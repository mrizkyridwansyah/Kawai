<template>
  <div class="d-flex mt-3 align-items-end">
    <div class="flex-fill">
      <h5 v-if="type == 'SUB'">{{title}}</h5>
      <h3 v-else>{{title}}</h3>
    </div>
    <div>
      <v-submit-button-group 
        :submit="submit"
        :disabled="disabled || loading"
        :loading="loading"
        :btn-back="btnBack"
      />
    </div>
  </div>
  <hr>
</template>
<script>
export default {
  props: ['title', 'submit', 'loading', 'disabled', 'type', 'btnBack']
}
</script>