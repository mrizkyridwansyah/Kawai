<template>
  <div v-if="noGroup !== undefined">
    <input
      type="file"
      class="form-control"
      :class="customErrors ? 'is-invalid' : null"
      @input="change"
      :placeholder="placeholder"
      :id="`input-text-${key}`"
      :disabled="disabled !== undefined"
      :accept="accept"
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
      <input
        type="file"
        class="form-control"
        :class="customErrors ? 'is-invalid' : null"
        @input="change"
        :placeholder="placeholder"
        :disabled="disabled !== undefined"
        :accept="accept"
        ref="v-input-file"
      />
    </div>
    <div class="invalid-feedback d-block" v-if="customErrors">
      {{ customErrors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>

<script>
export default {
  // key: uniqueId(),
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: ['modelValue', 'label', 'col', 'description', 
    'placeholder', 'onSelect', 'errors', 'accept', 
    'value', 'multiline', 'rows', 'no-group', 'disabled',
    'maxLength'
  ],
  data: () => ({
    tempValue: null,
    key: uniqueId(),
    invalidMessage: null,
  }),
  computed: {
    customErrors: function() {
      if(this.invalidMessage)
        return [...(this.errors ?? []), this.invalidMessage];

      return this.errors;
    }
  },
  watch: {
    modelValue: function(after, before) {
      if(after)
        this.tempValue = structuredClone(after)
      else {
        this.$refs['v-input-file'].value = null;
        this.tempValue = null;
      }
    },
    errors: function(after) {

    },
  },
  mounted: function() {
    this.tempValue = structuredClone(this.modelValue)
  },
  methods: {
    change: function(e) {
      this.invalidMessage = null; 
      if(e.target.files.length == 0)
        return;
        
      if(!isNaN(this.maxLength)) {
        if(e.target.files[0].size > this.maxLength)
          this.invalidMessage = `The file is too large. Allowed maximum size is ${this.$func.bytesToSize(this.maxLength)}`;
      }

      this.$emit("update:modelValue", e.target.files[0]);

      if(this['onSelect'])
        this['onSelect'](e.target.files[0]);
    },
  },
}
</script>