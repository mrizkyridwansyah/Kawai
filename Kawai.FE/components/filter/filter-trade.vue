<template>
  <div class="ddl-row">
    <!-- DDL -->
    <div class="ddl-col ddl-left" :style="ddlStyle">
      <input-multiselect
        v-model="tempValue"
        :options="list"
        :close-on-select="true"
        :clear-on-select="false"
        :preserve-search="true"
        open-direction="bottom"
        :placeholder="placeholder || ' '"
        :searchable="true"
        :label="displayLabel"
        track-by="Trade_Code"
        :hide-selected="true"
        :internal-search="false"
        :loading="isLoading"
        @search-change="search"
        @open="open"
        @select="change"
        :multiple="multiple === true"
        :disabled="disabled === true"
        class="ddl-multiselect"
      />

      <div class="invalid-feedback d-block" v-if="errors">
        {{ errors[0] }}
      </div>

      <small class="form-text text-muted" v-if="description">
        {{ description }}
      </small>
    </div>

    <!-- DESCRIPTION -->
    <div class="ddl-col ddl-right" :style="descStyle">
      <input
        type="text"
        disabled
        class="form-control w-100"
        :value="selectedItem?.Trade_Name || ''"
      />
    </div>
  </div>
</template>

<script>
export default {
  emits: ["update:modelValue"],
  props: {
    modelValue: null,
    tradeCls: [String, Array],
    placeholder: String,
    description: String,
    errors: Array,
    disabled: Boolean,
    multiple: Boolean,
    showOptionAll: Boolean,

    /* === WIDTH CONTROL === */
    ddlWidth: {
      type: String,
      default: "30%",
    },
    descWidth: {
      type: String,
      default: "70%",
    },
  },

  data() {
    return {
      isLoading: false,
      list: [],
      tempValue: null,
      debounce: null,
    };
  },

  computed: {
    selectedItem() {
      return this.list.find((x) => x.Trade_Code === this.tempValue) || null;
    },

    displayLabel() {
      return this.tempValue ? "Trade_Code" : "DDLDescription";
    },

    ddlStyle() {
      return {
        flex: "0 0 auto",
        width: this.ddlWidth,
      };
    },

    descStyle() {
      return {
        flex: "1 1 auto",
        width: this.descWidth,
      };
    },
  },

  watch: {
    modelValue(v) {
      if (!v) this.tempValue = null;
      this.load("", v);
    },

    tempValue(v) {
      if (!v) this.$emit("update:modelValue", null);
    },
  },

  mounted() {
    this.load("", this.modelValue);
  },

  methods: {
    change(v) {
      this.$emit("update:modelValue", v);
    },

    search(q) {
      this.load(q);
    },

    open() {
      this.load("", this.modelValue);
    },

    load(q = "", d = "") {
      this.isLoading = true;
      this.list = [];

      if (this.debounce) clearTimeout(this.debounce);

      const tradeFlags = Array.isArray(this.tradeCls)
        ? this.tradeCls.map((x) => `&tradecls=${x}`).join("")
        : this.tradeCls
        ? `&tradecls=${this.tradeCls}`
        : "";

      this.debounce = setTimeout(() => {
        this.$http
          .get(`/trade/ddlsearch?keyword=${q}${tradeFlags}&ids=${d || ""}`)
          .then((r) => {
            if (d && r.data.Data?.length) {
              this.tempValue = r.data.Data[0].Trade_Code;
            }

            this.list =
              this.showOptionAll && !q
                ? [
                    {
                      Trade_Code: "ALL",
                      Trade_Name: "ALL",
                      DDLDescription: "ALL",
                    },
                    ...r.data.Data,
                  ]
                : r.data.Data;
          })
          .finally(() => (this.isLoading = false));
      }, 200);
    },
  },
};
</script>

<style scoped>
.ddl-row {
  display: flex;
  gap: 12px;
  width: 100%;
  align-items: flex-start;
}

.ddl-col {
  min-width: 0;
}

.ddl-multiselect {
  width: 100% !important;
  min-width: 0 !important;
}
</style>
