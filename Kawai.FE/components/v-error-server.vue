<template>
  <div class="text-center">
    <img src="/500.png" style="width: 25%;" />
    <div>
      <span style="font-size: 1.2rem;" class="mt-3">Internal Server Error</span>
      <div>
        <span>There's some problem with our server. Kindly, try refreshing the page or comeback later.</span>
      </div>
      <div>
        <span>Apologies for the inconvenience.</span>
      </div>
    </div>
    <div class="mt-3">
      <button class="btn btn-primary btn-lg rounded-pill btn-sm" @click="reload">
        <span class="mx-4">Refresh</span>
      </button>
    </div>
  </div>
</template>

<script>
export default {
  props: ['text', 'refresh'],
  methods: {
    reload: function() {
      if(this['refresh']) {
        this['refresh']()
      }
      else {
        location.reload();
      }
    },
  }
}
</script>