<template>
  <div class="text-center">
    <img src="/not-found.png" style="width: 30%;">
    <!-- <h4 class="mt-3">No results found</h4> -->
    <div>
      <span style="font-size: .8rem;" class="mt-3">{{ text || 'No results found' }}</span>
    </div>
  </div>
</template>

<script>
export default {
  props: ['text']
}
</script>