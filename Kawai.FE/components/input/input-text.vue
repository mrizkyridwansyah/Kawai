<template>
  <div>
    <div>
      <input
        v-if="multiline === undefined"
        type="text"
        :class="inputClass"
        v-model="tempValue" 
        @keyup="change"
        :placeholder="placeholder"
        :disabled="(disabled !== undefined || disabled === true) && disabled !== false"
        :ref="`input-text-${key}`"
        :maxlength="maxlength || 100000"
        v-maska 
        :data-maska="mask"
        class="w-100 form-control"
      />
      <textarea
        v-else
        :class="inputClass"
        :rows="rows || 3"
        v-model="tempValue" 
        @keyup="change"
        :placeholder="placeholder"
        :disabled="(disabled !== undefined || disabled === true) && disabled !== false"
        :ref="`input-text-${key}`"
        v-maska 
        :data-maska="mask"
        class="w-100 form-control"
      />
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>

<script>
import { vMaska } from 'maska';
import bootstrap from 'bootstrap/dist/js/bootstrap.bundle';
export default {
  directives: { maska: vMaska },
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: [
    'modelValue', 'label', 'col', 
    'description', 'placeholder', 'onSelect', 
    'errors', 'value', 'multiline', 
    'rows', 'no-group', 'disabled',
    'text-center', 'mask', 'maxlength'
  ],
  data: () => ({
    tempValue: null,
    key: uniqueId(),
  }),
  computed: {
    inputClass: function() {
      var c = '';
      if(this.errors)
        c = c.concat('is-invalid', ' ')

      if(this.textCenter !== undefined && this.textCenter !== false) {
        c = c.concat('text-center', ' ')
      }
      
      return c;
    }
  },
  watch: {
    modelValue: function(after, before) {
      if(after === 0 )
        this.tempValue = 0;
      else
        this.tempValue = after == null ? null : structuredClone(after)
    },
    errors: function(after) {
      if(JSON.stringify(after) != '{}') {
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
        // const popover = new bootstrap.Popover(`#input-text-${this.key}`, {
        //   container: '.modal-body'
        // })
      }
    },
  },
  mounted: function() {
    if(this.modelValue === 0 )
      this.tempValue = 0;
    else
      this.tempValue = structuredClone(this.modelValue)
  },
  methods: {
    change: function(e) {
      this.$emit("update:modelValue", e.target.value);
    },
  },
}
</script>


<style>

.input-text-custom {
  height: 80%;
}
</style>