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
        :placeholder="placeholder || `Search Trade`"
        :searchable="true"
        :label="displayLabel"
        track-by="Trade_Code"
        trackBy="Trade_Code"
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
    </div>
    <div class="col-xl-8 col-lg-8 col-md-6 col-sm-6">
      <input
        type="text"
        disabled
        :value="selectedItem?.Trade_Name || ''"
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
    "tradeCls",
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
    "showOptionAll",
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
    selectedItem: function () {
      if (this.tempValue === "ALL") {
        return {
          Trade_Code: "ALL",
          Trade_Name: "ALL",
          DDLDescription: "ALL",
        };
      }

      return this.list.find((x) => x.Trade_Code === this.tempValue) || null;
    },
    displayLabel() {
      if (this.isOpen) return "DDLDescription";
      return this.tempValue ? "Trade_Code" : "DDLDescription";
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
      this.isOpen = true;
      this.load("", this.modelValue);
    },
    close: function() {
      this.isOpen = false;
    },
    load: function (q = "", d = "") {
      this.list = [];
      this.isLoading = true;

      var tradeFlags = Array.isArray(this.tradeCls)
        ? this.tradeCls.map((p) => "&tradecls=" + p)
        : [];

      if (this.debounce != null) clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/trade/ddlsearch?keyword=${q || ""}${
              tradeFlags.length == 0
                ? this.tradeCls
                  ? "&tradecls=" + this.tradeCls
                  : ""
                : tradeFlags.join("")
            }&ids=${d || ""}`
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.Trade_Code;
            }

            this.list =
              (this.showOptionAll || false) &&
              (q || "") == "" &&
              p.data.Data.length > 0
                ? [
                    {
                      Trade_Code: "ALL",
                      Trade_Name: "ALL",
                      DDLDescription: "ALL",
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
