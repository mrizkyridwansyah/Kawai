<template>
  <div>
    <input type="text" v-model="test" @input="changePageTitle" >
    <strong>{{ test }}</strong>
  </div>
</template>

<script>
export default {
  props: {
    modelValue: String, // previously was `value: String`
    moe: String,
  },
  emits: ['update:modelValue', 'update:moe'],
  data: () => ({
    test: null,
  }),
  methods: {
    changePageTitle: function(title) {
      this.$emit('update:modelValue', title.target.value) // previously was `this.$emit('input', title)`
      this.$emit('update:moe', title.target.value) // previously was `this.$emit('input', title)`
    }
  }
}
</script>