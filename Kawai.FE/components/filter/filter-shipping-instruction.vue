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
          label="DDLDescription"
          track-by="SI_No"
          trackBy="SI_No"
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
        <small class="form-text text-muted" v-if="description">
          {{description}}
        </small>
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
    "styleCode",
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
      this.tempValue = null;
      this.$emit("update:modelValue", null);
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
      const selected = this.list.find((x) => x.SI_No === v) || null;
      if (this.onSelect) this.onSelect(v, selected);
      this.$emit("update:modelValue", v);
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
            `/shipping-instruction/ddlsearch?keyword=${q || ""}&custCode=${
              this.customer || "ALL"
            }&dateFrom=${this.$func.asUtcStringDateOnly(
              new Date(this.dateFrom),
            )}&dateTo=${this.$func.asUtcStringDateOnly(
              new Date(this.dateTo),
            )}&ids=${d || ""}`,
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.SI_No;
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
