<template>
  <div>
    <input-multiselect
      v-model="tempValue"
      :options="list"
      :close-on-select="true"
      :clear-on-select="false"
      :preserve-search="true"
      open-direction="bottom"
      :placeholder="placeholder || `Search Area`"
      :searchable="true"
      label="AreaName"
      track-by="AreaCode"
      trackBy="AreaCode"
      :hide-selected="true"
      :internal-search="false"
      :loading="isLoading"
      @search-change="search"
      @open="open"
      :select="change"
      :class="cClass || 'input-wrapper'"
      :multiple="multiple !== undefined || false"
      :disabled="disabled !== undefined || false"
      select-label=""
      deselect-label=""
    />
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
  props: [
    'modelValue', 'type', 'label', 'col', 'description', 
    'placeholder', 'onSelect', 'errors'
    , 'disabled', 'multiple', 'class', 'warehouse', 'location'
  ],
  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function() {
      return (this['class'] ?? '') + (this.errors ? 'is-invalid' : '');
    }
  },
  watch: {
    modelValue: function(after, before) {
      if(!after)
        this.tempValue = null;
        
      this.load('', after);
    },
    warehouse: function(after) {
      this.tempValue = null;
      this.load('', this.modelValue);
    },
    location: function(after) {
      this.tempValue = null;
      this.load('', this.modelValue);
    },
    tempValue: function(after) {
      if(!after)
        this.$emit("update:modelValue", null);
    },
  },
  mounted: function() {
    this.load('', this.modelValue);
  },
  methods: {
    change: function(v) {
      if(this.onSelect)
        this.onSelect(v);

      this.$emit("update:modelValue", v);
    },
    search: function(q) {
      this.load(q, null);
    },
    open: function() {
      this.load('', null);
    },
    load: function(q = '', d = '') {
      this.list = [];
      this.isLoading = true;
      if(this.debounce != null)
        clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http.get(`/area/ddlsearch?keyword=${q || ''}&ids=${d || ''}&warehouse=${this.warehouse || 'ALL'}&location=${this.location || 'ALL'}`)
          .then(p => {
            if(d && p.data.length > 0) {
              this.tempValue = p.data[0]?.AreaCode;
            }
            this.list = p.data;
          })
          .finally(() => this.isLoading = false);
          
        clearTimeout(this.debounce);
      }, 200)
    }
  }
}
</script>

<style>
.input-wrapper {
  min-width: 10em;
  width: 100%;
}

</style>