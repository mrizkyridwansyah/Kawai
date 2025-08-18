<template>
  <div :class="col ? ('form-group col-' + col) : 'form-group'">
    <label class="d-block">{{(label)}}</label>
    <div>
      <input
        type="password"
        class="form-control"
        :class="errors ? 'is-invalid' : null"
         v-model="tempValue" 
         @keyup="change"
         autocomplete="off"
      />
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>

<script>
export default {
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: ['modelValue', 'label', 'col', 'description', 'placeholder', 'onSelect', 'errors', 'value', 'multiline', 'rows'],
  data: () => ({
    tempValue: null,
  }),
  watch: {
    modelValue: function(after, before) {
      this.tempValue = structuredClone(after)
    }
  },
  mounted: function() {
    this.tempValue = structuredClone(this.modelValue);
  },
  methods: {
    change: function(e) {
      this.$emit("update:modelValue", e.target.value);
    },
  },
}
</script>