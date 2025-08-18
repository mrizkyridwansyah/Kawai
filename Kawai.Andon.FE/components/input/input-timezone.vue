<template>
  <div :class="col ? ('form-group col-' + col) : 'form-group'">
    <label class="d-block">{{(label || 'Time Zone')}}</label>
    <div>
      <input-multiselect
        v-model="tempValue"
        :options="list"
        :close-on-select="true"
        :clear-on-select="false"
        :preserve-search="true"
        open-direction="bottom"
        :placeholder="placeholder || 'Search Timezone'"
        :searchable="true"
        label="Name"
        track-by="Id"
        trackBy="Id"
        :hide-selected="true"
        :internal-search="false"
        :loading="isLoading"
        @search-change="load"
        @select="change"
        :class="cClass"
        :multiple="multiple !== undefined || false"
        select-label=""
        deselect-label=""
      />
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
  emits: ['update:modelValue'],
  props: ['modelValue', 'label', 'col', 'description', 'placeholder', 'onSelect', 'errors', 'multiple'],
  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function() {
      return (this.errors ? 'is-invalid' : '');
    }
  },
  watch: {
    modelValue: function(after, before) {
      this.load(null, after || '');
    },
    tempValue: function(after) {
      if(!after) {
        this.$emit("update:modelValue", after);
        return;
      }
      if(this.multiple === undefined) {
        this.$emit("update:modelValue", after);
      }
      else
        this.$emit("update:modelValue", after?.filter(p => p !== null).map(p => p?.Id));
    },
  },
  mounted: function() {
    if(this.modelValue)
      this.tempValue = objCopy(this.modelValue);

    this.load();
  },
  methods: {
    change: function(v) {
      if(this.onSelect)
        this.onSelect(v);
    },
    load: function(q = '', d = null) {
      var ids = [];
      if(Array.isArray(d)) {
        ids = d.map(p => '&ids=' + p);
      }

      if(this.debounce != null)
        clearTimeout(this.debounce);

      this.isLoading = true;
      this.debounce = setTimeout(() => {
        this.$http.get(`/common/timezone-search?q=${q || ''}${ids.length == 0 ? (d ? '&ids=' + d : '') : ids.join('')}`)
          .then(p => {
            this.list = p.data.Data;
            if(d != '' && d != null && p.data.Data.length > 0) {
              if(this.multiple !== undefined) {
                this.tempValue = p.data.Data.map(p => p.Id);
              }
              else {
                this.tempValue = p.data.Data[0].Id;
                this.$emit("update:modelValue", this.tempValue);
              }
            }
          })
          .finally(() => this.isLoading = false);
          
        clearTimeout(this.debounce);
      }, 200)
    },
  }
}
</script>