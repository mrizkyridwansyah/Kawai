<template>
  <div class="ddl-row">
    <!-- MULTISELECT -->
    <div class="ddl-col ddl-left" :style="leftStyle">
      <input-multiselect
        v-model="tempValue"
        :options="list"
        :loading="isLoading"
        :placeholder="placeholder || 'Search Cls'"
        :label="displayLabel"
        track-by="ClsCode"
        :multiple="multiple === true"
        :disabled="disabled === true"
        @search-change="search"
        @open="open"
        @select="change"
        class="ddl-multiselect"
      />

      <div class="invalid-feedback d-block" v-if="errors">
        {{ errors[0] }}
      </div>
    </div>

    <!-- DESCRIPTION -->
    <div class="ddl-col ddl-right" :style="rightStyle">
      <input
        type="text"
        disabled
        class="form-control w-100"
        :value="selectedItem?.Description || ''"
      />
    </div>
  </div>
</template>

<script>
export default {
  emits: ["update:modelValue"],
  props: {
    modelValue: null,
    typeData: String,
    placeholder: String,
    errors: Array,
    disabled: Boolean,
    multiple: Boolean,

    /* === WIDTH CONTROL === */
    ddlWidth: {
      type: String,
      default: "20%",
    },
    descWidth: {
      type: String,
      default: "80%",
    },
  },

  data() {
    return {
      list: [],
      tempValue: null,
      isLoading: false,
      debounce: null,
    };
  },

  computed: {
    selectedItem() {
      return this.list.find((x) => x.ClsCode === this.tempValue) || null;
    },

    displayLabel() {
      return this.tempValue ? "ClsCode" : "DDLDescription";
    },

    leftStyle() {
      return {
        flex: "0 0 auto",
        width: this.ddlWidth,
      };
    },

    rightStyle() {
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

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/cls/ddlsearch?keyword=${q}&typedata=${this.typeData}&ids=${
              d || ""
            }`
          )
          .then((r) => {
            if (d && r.data.Data.length) {
              this.tempValue = r.data.Data[0].ClsCode;
            }
            this.list = r.data.Data;
          })
          .finally(() => (this.isLoading = false));
      }, 250);
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

/* IMPORTANT */
.ddl-col {
  min-width: 0;
}

/* FORCE multiselect width */
.ddl-multiselect {
  width: 100% !important;
  min-width: 0 !important;
}
</style>
