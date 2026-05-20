<template>
  <table>
    <tr>
      <td :style="this.styleCode">
      <input-multiselect
        v-model="tempValue"
        :options="list"
        :close-on-select="true"
        :clear-on-select="false"
        :preserve-search="true"
        open-direction="bottom"
        :placeholder="placeholder || ` `"
        :searchable="true"
        :label="displayLabel"
        track-by="AddressCode"
        trackBy="AddressCode"
        :hide-selected="true"
        :internal-search="false"
        :loading="isLoading"
        @search-change="search"
        @open="open"
        @close="close"
        :select="change"
        :class="cClass || 'input-wrapper'"
        :multiple="multiple !== undefined || false"
        :disabled="
          (disabled !== undefined || disabled === true) && disabled !== false
        "
        select-label=""
        deselect-label=""
      />
      <div class="invalid-feedback d-block" v-if="errors">
        {{ errors[0] }}
      </div>
      <small class="form-text text-muted" v-if="description">{{
        description
      }}</small>
    </td>
    <td :style="this.styleDesc" style="padding-left: 5px">
      <input
        type="text"
        disabled
        :value="selectedItem?.AddressName || ''"
        class="w-100 form-control"
      />
      </td>
    </tr>
  </table>
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
    , 'disabled', 'multiple', 'class', 'warehouse', 'area',
    'styleCode',
    'styleDesc',
    "showOptionAll",
    "includeTemp",
  ],
  data: () => ({
    isLoading: false,
    isOpen: false,
    list: [],
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function() {
      return (this['class'] ?? '') + (this.errors ? 'is-invalid' : '');
    },
     selectedItem: function () {
      if (this.tempValue === "ALL") {
        return {
          AddressCode: "ALL",
          AddressName: "ALL",
          DDLDescription: "ALL",
        };
      }

      return this.list.find((x) => x.AddressCode === this.tempValue) || null;
    },
    
    displayLabel() {
      if (this.isOpen) return "DDLDescription";
      return this.tempValue ? "AddressCode" : "DDLDescription";
    },
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
    open: function () {
      this.isOpen = true;
      this.load("", null);
    },
    close: function() {
      this.isOpen = false;
    },
    load: function(q = '', d = '') {
      this.list = [];
      this.isLoading = true;
      if(this.debounce != null)
        clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http.get(`/address/ddlsearch-privileges?keyword=${q || ''}&ids=${d || ''}&warehouse=${this.warehouse || 'ALL'}&area=${this.area || 'ALL'}${
              this.includeTemp ? "&includeTemp=true" : "&includeTemp=false"
            }`)
              .then((p) => {
            this.list =
              (this.showOptionAll || false) &&
              (q || "") == "" &&
              p.data.Data.length > 0
                ? [
                    {
                      AddressCode: "ALL",
          AddressName: "ALL",
                      DDLDescription: "ALL",
                    },
                    ...p.data.Data,
                  ]
                : p.data.Data;

            if (d && p.data.Data.length > 0) {
              this.tempValue = d == "ALL" ? "ALL" : p.data.Data[0]?.AreaCode;
            }
          })
          .finally(() => this.isLoading = false);

        clearTimeout(this.debounce);
      }, 200)
    }
  }
}
</script>

<style scoped>
.input-wrapper {
  min-width: 10em;
  width: 100%;
}

.row-wrapper {
  display: flex;
  gap: 1rem; /* jarak antar elemen */
  align-items: center; /* biar vertikalnya rapih */
}

.input-ddl {
  flex: 1; /* biar bagian kiri melebar */
}

.fucking-info {
  width: 65%; /* bebas mau diset berapa */
}
</style>
