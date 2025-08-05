<template>
  <div v-if="noGroup !== undefined">
    <div>
      <input type="text" v-model="val" @input="change" :id="`input-phone-${key}`" :class="inputClass"
        data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="danger-tooltip"
        :data-bs-title="errors ? errors[0] : null" :disabled="disabled" />
    </div>
    <small class="form-text text-muted" v-if="description">{{ description }}</small>
  </div>
  <div v-else :class="col ? 'form-group col-' + col : 'form-group'">
    <label class="d-block">{{ label }}</label>
    <div>
      <input type="text" v-model="val" @input="change" :disabled="disabled" :class="inputClass" />
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{ description }}</small>
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
    inputClass: function () {
      var c = 'w-100 form-control';
      if (this.errors)
        c = c.concat('is-invalid', ' ')

      if (this.textCenter !== undefined && this.textCenter !== false) {
        c = c.concat('text-center', ' ')
      }

      return c;
    },
  },
  watch: {
    modelValue: function (after) {
      this.val = after;
    },
    errors: function (after) {
      if (JSON.stringify(after) != '{}') {
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
      if(e.target.value == undefined || e.target.value == null)
        e.target.value = '';
    },
    change: function (e) {
      var phonePattern = /^\d+$/;
      if(e.target.value == "" || e.target.value == undefined || e.target.value == null) {
        this.$emit("update:modelValue", e.target.value);
      } else {
        if (phonePattern.test(e.target.value)) {
          this.$emit("update:modelValue", e.target.value);
        } else {
          // Jika tidak valid, kembalikan nilai input ke nilai sebelumnya
          e.target.value = this.modelValue;
        }
      }
    },
  },
};
</script>
