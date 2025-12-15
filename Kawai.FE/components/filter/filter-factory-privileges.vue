<template>
  <div class="row">
    <div class="col-xl-4 col-lg-4 col-md-6 col-sm-6">
      <input-multiselect
        v-model="tempValue"
        :options="list"
        :close-on-select="true"
        :clear-on-select="false"
        :preserve-search="true"
        open-direction="bottom"
        :placeholder="placeholder || `Search Factory`"
        :searchable="true"
        :label="displayLabel"
        track-by="CompanyCode"
        trackBy="CompanyCode"
        :hide-selected="true"
        :internal-search="false"
        :loading="isLoading"
        @search-change="search"
        @open="open"
        :select="change"
        :class="cClass || 'input-wrapper'"
        :multiple="multiple !== undefined || false"
        :disabled="true"
        select-label=""
        deselect-label=""
      />
      <div class="invalid-feedback d-block" v-if="errors">
        {{ errors[0] }}
      </div>
      <small class="form-text text-muted" v-if="description">{{
        description
      }}</small>
    </div>
    <div class="col-xl-8 col-lg-8 col-md-6 col-sm-6">
      <input
        type="text"
        disabled
        :value="selectedItem?.CompanyName || ''"
        class="w-100 form-control"
      />
    </div>
  </div>
</template>

<script>
export default {
  model: {
    prop: "modelValue",
    event: "update",
  },
  emits: ["update:modelValue"],
  props: [
    "modelValue",
    "type",
    "label",
    "col",
    "description",
    "placeholder",
    "onSelect",
    "errors",
    "disabled",
    "multiple",
    "class",
  ],
  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function () {
      return (this["class"] ?? "") + (this.errors ? "is-invalid" : "");
    },
    selectedItem: function () {
      return this.list.find((x) => x.CompanyCode === this.tempValue) || null;
    },
    displayLabel() {
      return this.tempValue ? "CompanyCode" : "DDLDescription";
    },
  },
  watch: {
    modelValue: function (after, before) {
      const selected = localStorage.getItem("SelectedFactory");
      if (selected) {
        this.tempValue = selected;
        this.load("", selected);
        this.$emit("update:modelValue", selected);
        return;
      }

      if (!after) this.tempValue = null;

      this.load("", after);
    },
    tempValue: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
  },
  mounted: function () {
    const selected = localStorage.getItem("SelectedFactory");
    if (selected) {
      this.tempValue = selected;
      this.load("", selected);
      this.$emit("update:modelValue", selected);
    } else {
      this.load("", this.modelValue); // fallback
    }
  },
  methods: {
    change: function (v) {
      if (this.onSelect) this.onSelect(v);

      this.$emit("update:modelValue", v);
    },
    search: function (q) {
      this.load(q, null);
    },
    open: function () {
      this.load("", this.modelValue);
    },
    // refresh: function () {
    //   this.load('', this.modelValue);
    // },
    load: function (q = "", d = "") {
      this.list = [];
      this.isLoading = true;
      if (this.debounce != null) clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/factory/ddlsearch-privileges?keyword=${q || ""}&ids=${d || ""}`
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.CompanyCode;
            }
            this.list = p.data.Data;
          })
          .finally(() => (this.isLoading = false));

        clearTimeout(this.debounce);
      }, 200);
    },
  },
};
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
