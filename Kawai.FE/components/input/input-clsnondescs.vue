<template>
  <table>
    <tr>
      <td :style="styleCode">
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
          track-by="ClsCode"
          :hide-selected="true"
          :internal-search="false"
          :loading="isLoading"
          @search-change="search"
          @open="open"
          :select="change"
          :class="cClass || ''"
          :multiple="multiple !== undefined || false"
          :disabled="disabled !== undefined || false"
          select-label=""
          deselect-label=""
        />

        <div class="invalid-feedback d-block" v-if="errors">
          {{ errors[0] }}
        </div>

        <small class="form-text text-muted" v-if="description">
          {{ description }}
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
    "typeData",
    "label",
    "col",
    "description",
    "placeholder",
    "onSelect",
    "errors",
    "disabled",
    "multiple",
    "class",
    "styleCode",
    "styleDesc",
     
  ],

  data() {
    return {
      isLoading: false,
      list: [],
      tempValue: null,
      debounce: null,
    };
  },

  computed: {
    cClass() {
      return (this["class"] ?? "") + (this.errors ? " is-invalid" : "");
    },

    displayLabel() {
      return this.tempValue ? "ClsCode" : "DDLDescription";
    },
  },

  watch: {
    modelValue(after) {
      if (!after) {
        this.tempValue = null;
        return;
      }

      // sync value dari parent
      this.tempValue = after;

      // load list agar label muncul
      this.load("", after);
    },

    tempValue(after) {
      if (!after) {
        this.$emit("update:modelValue", null);
      }
    },

    line: function(after) {
      this.tempValue = null;
      this.load('', this.modelValue);
    },
    workstation: function(after) {
      this.tempValue = null;
      this.load('', this.modelValue);
    },
  },

  mounted() {
    if (this.modelValue) {
      this.tempValue = this.modelValue;
    }

    this.load("", this.modelValue);
  },

  methods: {
    change(v) {
      if (this.onSelect) this.onSelect(v);

      this.$emit("update:modelValue", v);
    },

    search(q) {
      this.load(q, null);
    },

    open() {
      this.load("", null);
    },

    load(q = "", d = "") {
      this.list = [];
      this.isLoading = true;

      if (this.debounce) clearTimeout(this.debounce);

      this.debounce = setTimeout(() => {
        this.$http
          .get(
            `/cls/ddlsearch?keyword=${q || ""}&typedata=${this.typeData}&ids=${
              d || ""
            }`, )
          .then((p) => {
            const data = p.data.Data || [];

            if (d && data.length > 0) {
              this.tempValue = data[0].ClsCode;
            }

            this.list = data;
          })
          .finally(() => {
            this.isLoading = false;
          });

        clearTimeout(this.debounce);
      }, 200);
    },
  },
};
</script>

<style>
.input-wrapper {
  max-width: 12em !important;
  width: 100%;
}
</style>