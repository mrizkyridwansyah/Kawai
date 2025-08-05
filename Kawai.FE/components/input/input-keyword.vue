<template>
  <div>
    <input-multiselect 
    v-model="tempValue" 
    :options="list" 
    :close-on-select="true" :clear-on-select="false"
      :preserve-search="true" open-direction="bottom" :placeholder="placeholder || `Search Keyword Key`"
      :searchable="true" label="Name" track-by="Id" trackBy="Id" :hide-selected="true" :internal-search="false"
      :loading="isLoading" @search-change="search" @open="open" :select="change" :class="cClass"
      :multiple="multiple !== undefined || false" :disabled="disabled !== undefined || false" select-label=""
      deselect-label="" />
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{ description }}</small>
  </div>
</template>

<script>
export default {
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: [
    'modelValue', 'type', 'label', 'col', 'description',
    'placeholder', 'onSelect', 'errors'
    , 'disabled', 'multiple', 'class', 'lists'
  ],
  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function () {
      return (this['class'] ?? '') + (this.errors ? 'is-invalid' : '');
    }
  },
  watch: {
    modelValue: function (after, before) {
      if (!after)
        this.tempValue = null;

      this.load('', after);
    },
    tempValue: function (after) {
      if (!after)
        this.$emit("update:modelValue", null);
    },
  },
  mounted: function () {
    // this.load('', this.modelValue);
  },
  methods: {
    change: function (v) {
      if (this.onSelect)
        this.onSelect(v);

      this.$emit("update:modelValue", v);
    },
    search: function (q) {
      this.load(q, null);
    },
    open: function () {
      this.load('', null);
    },
    load: function (q = '', d = '') {
      this.isLoading = true;
      this.list = this.lists.filter(o => o.Name.toLowerCase().includes(q.toLowerCase()));

      if (d)
        this.list = this.lists.filter(o => o.Id == d);

      if (d && this.list.length > 0) {
        this.tempValue = this.list[0]?.Id;
      }  
      
      this.isLoading = false
    }
  }
}
</script>