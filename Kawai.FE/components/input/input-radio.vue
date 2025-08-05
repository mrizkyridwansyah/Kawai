<template>
  <div :class="col ? ('col-' + col) : ''">
    <div class="form-group">
      <label class="d-block">{{label}}</label>
      <div role="radiogroup" class="w-100"
        :class="errors ? 'is-invalid' : null"
      >
        <label class="my-2" 
          v-for="(item, i) in (options || [])"
          :class="tempValue == item[valueField  || 'Id'] ? 'active' : null"
        >
          <input 
            class="form-check-input" 
            type="radio" 
            :name="`input_radio_${key}`" 
            :id="`input_radio_${key}`" 
            :value="item[valueField  || 'Id']"
            v-model="tempValue"
            :disabled="disabled"
            @click="change(item[valueField  || 'Id'])"
          >
          <span class="px-2">{{ item[textField  || 'Name'] }}</span>
        </label>
      </div>
      <div class="invalid-feedback d-block" v-if="errors">
        {{ errors[0] }}
      </div>
    </div>
  </div>
</template>

<script>
import { uniqueId } from 'lodash';

export default {
  emits: ['update:modelValue'],
  props: [
    'modelValue', 'label', 'col', 'description', 'placeholder', 'onSelect', 'errors', 'options',
    'value-field', 'text-field', 'buttons', 'stacked', 
    'disabled'
  ],
  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
    key: uniqueId(),
  }),
  watch: {
    modelValue: function(after, before) {
      if(!after)
        return;

      this.tempValue = objCopy(after);
    },
    tempValue: function(after) {
      if(!after) {
        this.$emit("update:modelValue", null);
        return;
      }
      this.$emit('update:modelValue', after)
    }
  },
  mounted: function() {
    if(this.modelValue)
      this.tempValue = objCopy(this.modelValue);
  },
  methods: {
    change: function(v) {
      if(this.onSelect)
        this.onSelect(v);//this.valueField ? this.options.filter(p => p[this.valueField] == v)[0] : this.options.filter(p => p['Id'] == v)[0]);
        
      this.$emit("update:modelValue", v);
    },
  }
}
</script>