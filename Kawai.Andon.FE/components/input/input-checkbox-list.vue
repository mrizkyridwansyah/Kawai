<template>
  <div :class="col ? ('col-' + col) : ''">
    <div class="form-group">
      <label class="d-block">{{ label }}</label>
      <div role="radiogroup" class="w-100" :class="errors ? 'is-invalid' : null">
        <div class="form-check" v-for="(item, i) in (options || [])"
          :class="tempValue == item[valueField || 'value'] ? 'active' : null">
          <input class="form-check-input" type="checkbox" :id="`defaultCheck${i}`" @click="item.event" :checked="checkedAll">
          <label class="form-check-label" :for="`defaultCheck${i}`">
            {{ item[textField || 'text'] }}
          </label>
        </div>
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
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: [
    'modelValue', 'label', 'col', 'description', 'placeholder', 'onSelect', 'errors', 'options',
    'value-field', 'text-field', 'buttons', 'stacked',
    'disabled', 'checkedAll'
  ],
  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
    key: uniqueId(),
  }),
  watch: {
    modelValue: function (after, before) {
      if (!after)
        return;

      this.tempValue = objCopy(after);
    },
    tempValue: function (after) {
      this.$emit('update:modelValue', after)
    }
  },
  mounted: function () {
    if (this.modelValue)
      this.tempValue = objCopy(this.modelValue);
  },
  methods: {
    change: function (v) {
      if (this.onSelect)
        this.onSelect(this.valueField ? this.options.filter(p => p[this.valueField] == v)[0] : this.options.filter(p => p['Id'] == v)[0]);

      this.$emit("change:modelValue", v);
    },
  }
}
</script>