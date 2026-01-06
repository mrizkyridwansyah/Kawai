<template>
  <div>
    <input-multiselect
      v-model="tempValue"
      :options="list"
      :close-on-select="true"
      :clear-on-select="false"
      :preserve-search="true"
      open-direction="bottom"
      :placeholder="placeholder || 'Select Year'"
      :searchable="true"
      label="Year"
      track-by="Year"
      trackBy="Year"
      :hide-selected="true"
      :internal-search="true"
      :loading="isLoading"
      @search-change="search"
      @open="open"
      @select="change"
      :class="cClass || 'input-wrapper'"
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
    "placeholder",
    "description",
    "errors",
    "disabled",
    "multiple",
    "class",
  ],

  data: () => ({
    isLoading: false,
    list: [],
    tempValue: null,
  }),

  computed: {
    cClass() {
      return (this["class"] ?? "") + (this.errors ? " is-invalid" : "");
    },
  },

  watch: {
    modelValue(after) {
      if (!after) this.tempValue = null;
      this.load("", after);
    },

    tempValue(after) {
      if (!after) this.$emit("update:modelValue", null);
    },
  },

  mounted() {
    this.load("", this.modelValue);
  },

  methods: {
    change(v) {
      this.$emit("update:modelValue", v);
    },

    search() {
      // internal-search true, tidak perlu filter manual
    },

    open() {
      this.load("", this.modelValue);
    },

    load(q = "", d = "") {
      this.isLoading = true;
      this.list = [];

      const startYear = 2022;
      const endYear = new Date().getFullYear() + 5;

      const data = [];
      for (let i = startYear; i <= endYear; i++) {
        data.push({
          Year: i.toString(),
        });
      }

      setTimeout(() => {
        this.list = data;

        if (d) {
          this.tempValue = d;
        }

        this.isLoading = false;
      }, 100);
    },
  },
};
</script>

<style>
.input-wrapper {
  max-width: 100px;
  width: 100%;
}
</style>
