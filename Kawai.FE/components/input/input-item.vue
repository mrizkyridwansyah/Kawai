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
          :placeholder="placeholder || ``"
          :searchable="true"
          :label="displayLabel"
          track-by="ItemCode"
          trackBy="ItemCode"
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

        <small class="form-text text-muted" v-if="description">{{
          description
        }}</small>
      </td>
      <td :style="this.styleDesc" style="padding-left: 5px">
        <input
          v-if="!this.descNewRow"
          type="text"
          disabled
          :value="tempValue ? fuckingDescription : ''"
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
    <tr v-if="this.descNewRow">
      <td colspan="2" style="padding-top: 2px">
        <input
          type="text"
          disabled
          :value="tempValue ? fuckingDescription : ''"
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
    "itemType",
    "brand",
    "itemCls",
    "styleCode",
    "styleDesc",
    "descNewRow",
  ],

  data: () => ({
    isLoading: false,
    isOpen: false,
    list: [],
    tempValue: null,
    fuckingDescription: "",
    debounce: null,
  }),
  computed: {
    cClass: function () {
      return (this["class"] ?? "") + (this.errors ? " is-invalid" : "");
    },
    displayLabel() {
      if (this.isOpen) return "DDLDescription";
      return this.tempValue ? "ItemCode" : "DDLDescription";
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
    itemCls: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    itemType: function (after) {
      this.tempValue = null;
      this.load("", this.modelValue);
    },
    brand: function (after) {
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

      const selected = this.list.find((x) => x.ItemCode === v);
      this.fuckingDescription = selected?.ItemName || "";

      this.$emit("update:modelValue", v);
    },
    search: function (q) {
      this.load(q, null);
    },
    open: function () {
      this.isOpen = true;
      this.load("", this.modelValue);
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
          .get(`/item/ddlsearch?keyword=${q || ""}&ids=${d || ""}`) //&itemGroup=01&brand=${this.brand || ''}&itemType=${this.itemType || ''}&itemCls=${this.itemCls || ''}`)
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.ItemCode;
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
