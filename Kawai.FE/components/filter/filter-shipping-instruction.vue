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
    lastRequestKey: null,
  }),
  computed: {
    cClass: function () {
      return (this["class"] ?? "") + (this.errors ? "is-invalid" : "");
    },
  },
  watch: {
    modelValue: function (after, before) {
      if (after === before) return;
      this.tempValue = after || null;
    },
    tempValue: function (after) {
      if (!after && this.modelValue) this.$emit("update:modelValue", null);
    },
    customer: function () {
      this.tempValue = "ALL";
      this.lastRequestKey = null;
      this.$emit("update:modelValue", "ALL");
      this.load("", null);
    },
    dateFrom: function () {
      this.tempValue = "ALL";
      this.lastRequestKey = null;
      this.$emit("update:modelValue", "ALL");
      this.load("", null);
    },
    dateTo: function () {
      this.tempValue = "ALL";
      this.lastRequestKey = null;
      this.$emit("update:modelValue", "ALL");
      this.load("", null);
    },
  },
  mounted: function () {
    this.tempValue = this.modelValue || null;
    this.load("", null);
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
      this.load("", null);
    },
    load: function (q = "", d = "") {
      if (!this.dateFrom || !this.dateTo) {
        this.isLoading = false;
        return;
      }

      const dateFrom = this.$func.asUtcStringDateOnly(new Date(this.dateFrom));
      const dateTo = this.$func.asUtcStringDateOnly(new Date(this.dateTo));
      const requestKey = `${q || ""}|${d || ""}|${this.customer || "ALL"}|${dateFrom}|${dateTo}`;
      if (requestKey === this.lastRequestKey) return;

      this.lastRequestKey = requestKey;
      this.isLoading = true;
      if (this.debounce != null) clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/shipping-instruction/ddlsearch?keyword=${q || ""}&custCode=${
              this.customer || "ALL"
            }&dateFrom=${dateFrom}&dateTo=${dateTo}&ids=${d || ""}`,
          )
          .then((p) => {
            const rows = p?.data?.Data || [];
            if (d && rows.length > 0) {
              this.tempValue = rows[0]?.SI_No || null;
            }
            this.list = rows;
          })
          .catch(() => {
            this.lastRequestKey = null;
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
