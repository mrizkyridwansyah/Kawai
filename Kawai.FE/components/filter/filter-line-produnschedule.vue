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
          track-by="LineCode"
          trackBy="LineCode"
          :hide-selected="true"
          :internal-search="false"
          :loading="isLoading"
          @search-change="search"
          @open="open"
          @close="close"
          :select="change"
          :class="cClass || 'input-wrapper'"
          :multiple="multiple !== undefined || false"
          :disabled="false"
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
        <div>
          <input
            type="text"
            disabled
            :value="selectedItem?.LineName || ''"
            class="w-100 form-control"
          />
        </div>
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
  emits: ["update:modelValue", "selected"],
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
    "styleCode",
    "styleDesc",
    "showOptionAll",
    "defaultOptionAll",
  ],
  data: () => ({
    isLoading: false,
    list: [],
    isOpen: false,
    tempValue: null,
    debounce: null,
  }),
  computed: {
    cClass: function () {
      return (this["class"] ?? "") + (this.errors ? "is-invalid" : "");
    },
    selectedItem: function () {
      return this.list.find((x) => x.LineCode === this.tempValue) || null;
    },
    displayLabel() {
      if (this.isOpen) return "DDLDescription";
      return this.tempValue ? "LineCode" : "DDLDescription";
    },
    
    // displayLabel() {
    //   return this.tempValue ? "LineCode" : "LineName";
    // },
  },
  watch: {
    modelValue: function (after, before) {
      if (!after) this.tempValue = null;

      this.load("", after);

      // const selected = localStorage.getItem("SelectedLine");
      // if (selected) {
      //   this.tempValue = selected;
      //   this.load("", selected);
      //   this.$emit("update:modelValue", selected);
        
      //   return;
      // }

      // if (!after) this.tempValue = null;

      // this.load("", after);
    },
    tempValue: function (after) {
      if (!after) this.$emit("update:modelValue", null);
    },

  },
  mounted: function () {
    this.load("", this.modelValue);

    // const selected = localStorage.getItem("SelectedLine");
    // if (selected) {
    //   this.tempValue = selected;
    //   this.load("", selected);
    //   this.$emit("update:modelValue", selected);
    // } else {
    //   this.load("", this.modelValue); // fallback
    // }
  },
  methods: {
    // change: function (v) {
    //   if (this.onSelect) this.onSelect(v);
    //   const selected = this.list.find((x) => x.LineCode === v);
    //   this.$emit("update:modelValue", v);
    //   this.$emit("selected", selected);
    // },
     change(v) {
      const selectedItem = this.list.find(x => x.LineCode === v);

      this.$emit("update:modelValue", v);
      this.$emit("selected", selectedItem);
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
            `/production-unschedule/ddl-line-search?keyword=${q || ""}&ids=${d || ""}`,
          )
          .then((p) => {
            if (d && p.data.Data.length > 0) {
              this.tempValue = p.data.Data[0]?.LineCode;
            }
            this.list = p.data.Data;
            console.log(p.data.Data);
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
