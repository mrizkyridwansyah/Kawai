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
          track-by="WarehouseCode"
          trackBy="WarehouseCode"
          :hide-selected="true"
          :internal-search="false"
          :loading="isLoading"
          @search-change="search"
          @open="open"
          :select="change"
          :class="cClass"
          :multiple="multiple !== undefined || false"
          :disabled="
            (disabled !== undefined || disabled === true) && disabled !== false
          "
          select-label=""
          deselect-label=""
        />
        <small class="form-text text-muted" v-if="description">{{
          description
        }}</small>
      </td>
      <td :style="this.styleDesc" style="padding-left: 5px">
        <input
          type="text"
          disabled
          :value="selectedItem?.WarehouseName || ''"
          class="w-100 form-control"
        />
      </td>
    </tr>
    <tr>
      <td colspan="2">
        <div class="invalid-feedback d-block" v-if="errors">
          {{ errors[0] }}
        </div>
      </td>
    </tr>
  </table>
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
    "typeData",
    "label",
    "col",
    "description",
    "placeholder",
    "onSelect",
    "errors",
    "disabled",
    "multiple",
    "class",
    "styleCode",
    "styleDesc",
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
      return this.list.find((x) => x.WarehouseCode === this.tempValue) || null;
    },
    displayLabel() {
      if (this.isOpen) return "DDLDescription";
      return this.tempValue ? "WarehouseCode" : "DDLDescription";
    },
  },
  watch: {
    modelValue: function (after, before) {
      if (!after) this.tempValue = null;

      this.load("", after);
    },
    tempValue: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
  },
  mounted: function () {
    this.load("", this.modelValue);
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
      this.load("", null);
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
          .get(`/item/warehouse-ddlsearch?keyword=${q || ""}&ids=${d || ""}`)
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.WarehouseCode;
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
.fucking-info {
  width: 65%; /* bebas mau diset berapa */
}
</style>
