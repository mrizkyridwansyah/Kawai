<template>
  
  <button class="v-btn v-theme--PurpleTheme v-btn--density-default elevation-4 v-btn--size-default v-btn--variant-tonal" @click="() => $router.push(to)"  style="width: 30%; height: 4em">{{ label }}
  </button>
</template>
<script>
export default {
  props: ['to', 'label']
}
</script>