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
      label="ReceiptNo"
      track-by="Id"
      trackBy="Id"
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
    "factoryCode",
    "supplierCode",
    "status",
    "sourceMenu",
    "periodFrom",
    "periodUntil",
    "showOptionAll",
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
    modelValue: function (after, before) {
      if (!after) this.tempValue = null;

      this.load("", after);
    },
    tempValue: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    factoryCode: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    supplierCode: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    typeDate: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    periodFrom: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    periodUntil: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    status: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },
    sourceMenu: function (after) {
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
          .get(
            `/receipt/ddlsearch?keyword=${q || ""}&ids=${d || ""}&factory=${this.factoryCode}&supplier=${this.supplierCode || ""}&status=${
              this.status || ""
            }&sourceMenu=${this.sourceMenu || ""}${
              this.periodFrom
                ? "&periodFrom=" +
                  this.$func.asUtcStringDateOnly(new Date(this.periodFrom))
                : ""
            }${this.periodUntil ? "&periodUntil=" + this.$func.asUtcStringDateOnly(new Date(this.periodUntil)) : ""}`,
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.Id;
            }

            if (d === "ALL" || d == "0") {
              this.tempValue = "ALL";

              this.list = [
                {
                  Id: "ALL",
                  ReceiptNo: "ALL"
                },
                ...p.data.Data || [],
              ];

              return;
            }

            this.list =
              this.showOptionAll && !q && p.data.Data.length > 0
                ? [
                    {
                      Id: "ALL",
                      ReceiptNo: "ALL",
                    },
                    ...p.data.Data,
                  ]
                : p.data.Data;

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
