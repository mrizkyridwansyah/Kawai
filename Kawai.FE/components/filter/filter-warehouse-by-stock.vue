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
          :value="selectedItem?.WarehouseName || ''"
          class="w-100 form-control"
        />
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
    "itemCode",
    "statusReceipt",
    "statusHoldNg",
    "showOptionAll",
    "styleCode",
    "styleDesc",
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
          WarehouseCode: "ALL",
          WarehouseName: "ALL",
          DDLDescription: "ALL",
        };
      }

      return this.list.find((x) => x.WarehouseCode === this.tempValue) || null;
    },
    displayLabel: function () {
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
    factoryCode: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    itemCode: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    statusReceipt: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    statusHoldNg: function (after) {
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
      this.isOpen = true;
      this.load("", null);
    },
    close: function () {
      this.isOpen = false;
    },
    load: function (q = "", d = "") {
      this.list = [];
      this.isLoading = true;
      if (this.debounce != null) clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/warehouse/ddl-warehouse-search-by-stock?keyword=${q || ""}&ids=${
              d || ""
            }&item=${this.itemCode}&factoryCode=${this.factoryCode || ""}&statusReceipt=${this.statusReceipt || ""}&statusHoldNG=${this.statusHoldNg || ""}`,
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.WarehouseCode;
            }

            this.list =
              (this.showOptionAll || false) &&
              (q || "") == "" &&
              p.data.Data.length > 0
                ? [
                    {
                      WarehouseCode: "ALL",
                      WarehouseName: "ALL",
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
