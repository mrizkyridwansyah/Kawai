<template>
  <div v-if="noGroup !== undefined">
    <input type="color" class="form-control form-control-color" 
      title="Choose your color"
      v-model="tempValue"
      @input="change"
    />
    <!-- <b-popover 
      v-if="errors" 
      :target="`input-text-${key}`" 
      triggers="hover" 
      placement="top"
      variant="danger"
    >
      <template #title><span class="text-danger">Invalid</span></template>
      {{ errors[0] }}
    </b-popover> -->
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
  <div v-else :class="col ? ('form-group col-' + col) : 'form-group'">
    <label class="d-block">{{(label)}}</label>
    <div>
      <input type="color" class="form-control form-control-color" 
        title="Choose your color"
        v-model="tempValue"
        @input="change"
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
    'modelValue', 'label', 'col', 
    'description', 'placeholder', 'onSelect', 
    'errors', 'value',
    'rows', 'no-group', 'disabled',
    'text-center',
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
        // const popover = new bootstrap.Popover(`#input-text-${this.key}`, {
        //   container: '.modal-body'
        // })
      }
    }
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