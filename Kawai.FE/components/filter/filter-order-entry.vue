<template>
  <div>
    <input-multiselect
      v-model="tempValue"
      :options="list"
      :close-on-select="true"
      :clear-on-select="false"
      :preserve-search="true"
      open-direction="bottom"
      :placeholder="placeholder || ` `"
      :searchable="true"
      label="DDLDescription"
      track-by="PO_No"
      trackBy="PO_No"
      :hide-selected="true"
      :internal-search="false"
      :loading="isLoading"
      @search-change="search"
      @open="open"
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
    "placeholder",
    "description",
    "onSelect",
    "errors",
    "disabled",
    "multiple",
    "class",
    "customer",
    "dateFrom",
    "dateTo",
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
  },
  watch: {
    modelValue: function (after) {
      if (!after) this.tempValue = null;
      this.load("", after);
    },
    tempValue: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    customer: function () {
      const selected = this.list.find((x) => x.PO_No === this.tempValue);
      const keepCurrent =
        selected &&
        selected.Cust_Code &&
        (selected.Cust_Code === this.customer ||
          (selected.Cust_Code === "All" && this.customer === "ALL"));

      if (!keepCurrent) {
        this.tempValue = null;
        this.$emit("update:modelValue", null);
      }
      this.load("", null);
    },
    dateFrom: function () {
      this.tempValue = null;
      this.$emit("update:modelValue", null);
      this.load("", null);
    },
    dateTo: function () {
      this.tempValue = null;
      this.$emit("update:modelValue", null);
      this.load("", null);
    },
  },
  mounted: function () {
    this.load("", this.modelValue);
  },
  methods: {
    change: function (v) {
      const poNo =
        typeof v === "object" && v !== null
          ? (v.PO_No || v.PONo || "").toString().trim()
          : (v || "").toString().trim();

      const directSelected =
        typeof v === "object" && v !== null
          ? v
          : null;

      const selected =
        (directSelected && directSelected.PO_No
          ? directSelected
          : null) ||
        this.list.find(
          (x) => x.PO_No === poNo && String(x.SI_NO || x.SI_No || "").trim()
        ) ||
        this.list.find((x) => x.PO_No === poNo) ||
        null;

      if (this.onSelect) this.onSelect(selected);
      this.$emit("update:modelValue", poNo || null);
    },
    search: function (q) {
      this.load(q, null);
    },
    open: function () {
      this.load("", this.modelValue);
    },
    load: function (q = "", d = "") {
      this.list = [];

      if (!this.dateFrom || !this.dateTo) {
        this.isLoading = false;
        return;
      }

      this.isLoading = true;
      if (this.debounce != null) clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/order-entry/ddlsearch?keyword=${q || ""}&custCode=${
              this.customer || "ALL"
            }&dateFrom=${this.$func.asUtcStringDateOnly(
              new Date(this.dateFrom),
            )}&dateTo=${this.$func.asUtcStringDateOnly(
              new Date(this.dateTo),
            )}&ids=${d || ""}`,
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.PO_No;
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
</style>
