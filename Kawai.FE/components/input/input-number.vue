<template>
  <div v-if="noGroup !== undefined">
    <div>
      <input type="number" v-model="val" @input="change" @click="onClick" 
        :id="`input-number-${key}`"
        :class="inputClass"
        data-bs-toggle="tooltip" 
        data-bs-placement="top"
        data-bs-custom-class="danger-tooltip"
        :data-bs-title="errors ? errors[0] : null"
        :disabled="disabled"
        min="0"
      />
    </div>
    <!-- <b-popover 
      v-if="errors" 
      :target="`input-number-${key}`" 
      triggers="hover" 
      placement="top"
      variant="danger"
      class="danger"
    >
      <template #title><span class="text-danger">Invalid</span></template>
      {{ errors[0] }}
    </b-popover> -->
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
  <div v-else :class="col ? 'form-group col-' + col : 'form-group'">
    <label class="d-block">{{ label }}</label>
    <div>
      <input type="number" v-model="val" @input="change" @click="onClick" 
        :disabled="disabled"
        :class="inputClass" min="0"
      />
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>
<script>
import bootstrap from 'bootstrap/dist/js/bootstrap.bundle';
export default {
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: [
    'modelValue', "label", 'description', 
    "col", "errors", 'max', 'min', 'noGroup', 'textCenter',
    'disabled', 'style'
  ],
  computed: {
    inputClass: function() {
      var c = 'w-100 form-control';
      if(this.errors)
        c = c.concat('is-invalid', ' ')

      if(this.textCenter !== undefined && this.textCenter !== false) {
        c = c.concat('text-center', ' ')
      }
      
      return c;
    },
  },
  watch: {
    modelValue: function (after) {
      if(!isNaN(after))
        this.val = parseFloat(after);
      else
        this.val = 0;
      // this.val = structuredClone(after);
      // this.val = this.$func.formatNumber(this.val);
    },
    errors: function(after) {
      if(JSON.stringify(after) != '{}') {
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
        const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))
      }
    },
  },
  data: () => ({
    val: '',
    key: uniqueId(),
  }),
  mounted: function () {
    this.val = this.modelValue;
    // if (!isNaN(this.modelValue)) {
    //   this.val = this.$func.formatNumber(this.modelValue);
    // }
  },
  methods: {
    onClick: function(e) {
      if(e.target.value == 0)
        e.target.value = '';
    },
    change: function (e) {
      if(!isNaN(this.max) || !isNaN(this.min)) {
        if(+e.target.value > +this.max && !isNaN(this.max)) {
          this.val = this.modelValue;
          return;
        }
        if(+e.target.value < +this.min && !isNaN(this.min)) {
          this.val = this.modelValue;
          return;
        }
        this.$emit("update:modelValue", +e.target.value);
      }
      else {
        this.$emit("update:modelValue", +e.target.value);
      }
    },
  },
};
</script>
