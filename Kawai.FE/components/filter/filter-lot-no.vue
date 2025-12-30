<template>
  <table>
    <tr>
      <td :style="this.style">
        <input-multiselect
          v-model="tempValue"
          :options="list"
          :close-on-select="true"
          :clear-on-select="false"
          :preserve-search="true"
          open-direction="bottom"
          :placeholder="placeholder || `Search Lot No `"
          :searchable="true"
          label="LotNo"
          track-by="LotNo"
          trackBy="LotNo"
          :hide-selected="true"
          :internal-search="false"
          :loading="isLoading"
          @search-change="search"
          @open="open"
          :select="change"
          :class="cClass || ''"
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
    "warehouse",
    "area",
    "address",
    "item",
    "showOptionAll",
    "style",
  ],
  data: () => ({
    isLoading: false,
    isOpen: false,
    list: [],
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function () {
      return (this["class"] ?? "") + (this.errors ? "is-invalid" : "");
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
    warehouse: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    area: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    address: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    item: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
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
            `/stock/ddl-lot-no-search?keyword=${q || ""}&ids=${
              d || ""
            }&warehouse=${this.warehouse}&area=${this.area}&address=${
              this.address
            }&item=${this.item || ""}`
          )
          .then((p) => {
            this.list =
              (this.showOptionAll || false) &&
              (q || "") == "" &&
              p.data.Data.length > 0
                ? [{ LotNo: "ALL" }, ...p.data.Data]
                : p.data.Data;

            if (d && p.data.Data.length > 0) {
              this.tempValue = d == "ALL" ? "ALL" : p.data.Data[0]?.LotNo;
            }
          })
          .finally(() => (this.isLoading = false));

        clearTimeout(this.debounce);
      }, 200);
    },
  },
};
</script>

<style>
.input-wrapper {
  min-width: 10em;
  width: 100%;
}
</style>
