<template>
  <div v-if="noGroup !== undefined && noGroup !== false">
    <input class="form-check-input" type="checkbox" 
      v-model="tempValue"
      @input="change"
      :disabled="disabled"
      :checked="checked"
    >
  </div>
  <div v-else :class="col ? ('form-group col-' + col) : 'form-group'">
    <div>
      <div class="form-check">
        <input class="form-check-input" type="checkbox" :id="`defaultCheck-${label}`"
          v-model="tempValue"
          @input="change"
          :disabled="disabled"
          :checked="checked"
        >
        <label class="form-check-label" :for="`defaultCheck-${label}`">
          {{label}}
        </label>
      </div>
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
  </div>
</template>

<script>
export default {
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue', 'onChecked'],
  props: [
    'modelValue', 'value', 'options', 'loading', 'noGroup',
    'label', 'trackBy', 'searchable', 'disabled', 'checked',
    'col', 'errors'
  ],
  data: () => ({
    tempValue: null,
  }),
  watch: {
    modelValue: function(after, before) {
      if(after === 0 )
        this.tempValue = 0;
      else
        this.tempValue = after == null ? null : JSON.parse(JSON.stringify(after))
    }
  },
  mounted: function() {
    if(this.modelValue === 0 )
      this.tempValue = 0;
    else
      this.tempValue = this.modelValue == null ? null : JSON.parse(JSON.stringify(this.modelValue))
  },
  methods: {
    change: function(v) {
      this.$emit("update:modelValue", v.target.checked);
    },
    search: function(v) {
      if(this.$attrs['onSearch'])
        this.$attrs['onSearch'](v);
    },
    searchChange: function(v) {
      if(this.$attrs['onSearchChange'])
        this.$attrs['onSearchChange'](v);
    },
    open: function(v) {
      if(this.$attrs['onOpen'])
        this.$attrs['onOpen']();
    },
  },
}
</script>
