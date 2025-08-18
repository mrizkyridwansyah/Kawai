<template>
  <span :class="sty"
    style="font-weight: 600;"
  >
    {{ text }}
  </span>
</template>
<script>
export default {
  props: ['text'],
  computed: {
    sty: function() {
      if(this.text == 'PENDING')
        return 'text-primary';
      if(this.text == 'APPROVED')
        return 'text-success';

      return '';
    },
  }
}
</script>