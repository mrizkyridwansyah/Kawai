<template>
  <div v-if="noGroup !== undefined">
    <div :class="errors ? 'is-invalid' : null">
      <Multiselect
        v-model="tempValue"
        :options="options"
        :label="textField || 'Name'"
        :trackBy="valueField || 'Id'"
        :valueProp="valueField"
        :placeholder="placeholder || ``"
        @search="search"
        @search-change="searchChange"
        @open="open"
        @input="change"
        :searchable="true"
        :hide-selected="true"
        :internal-search="false"
        :loading="loading"
        :close-on-select="true"
        :clear-on-select="false"
        :preserve-search="true"
        :resolve-on-load="false"
        :filter-results="false"
      />
      <div class="invalid-feedback d-block" v-if="errors">
        {{ errors[0] }}
      </div>
      <small class="form-text text-muted" v-if="description">{{
        description
      }}</small>
    </div>
  </div>
  <div v-else :class="col ? 'form-group col-' + col : 'form-group'">
    <label class="d-block">{{ label }}</label>
    <div :class="errors ? 'is-invalid' : null">
      <Multiselect
        v-model="tempValue"
        :options="options"
        :label="textField || 'Name'"
        :trackBy="valueField || 'Id'"
        :valueProp="valueField"
        :placeholder="placeholder || ``"
        @search="search"
        @search-change="searchChange"
        @open="open"
        @input="change"
        :searchable="true"
        :hide-selected="true"
        :internal-search="false"
        :loading="loading"
        :close-on-select="true"
        :clear-on-select="false"
        :preserve-search="true"
        :resolve-on-load="false"
        :filter-results="false"
      />
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{
      description
    }}</small>
  </div>
</template>

<script>
import Multiselect from "@vueform/multiselect";

export default {
  model: {
    prop: "modelValue",
    event: "update",
  },
  emits: ["update:modelValue"],
  props: [
    "modelValue",
    "options",
    "loading",
    "label",
    "text-field",
    "value-field",
    "searchable",
    "errors",
    "col",
    "noGroup",
    "description",
    "placeholder",
  ],
  components: { Multiselect },
  data: () => ({
    tempValue: null,
  }),
  watch: {
    modelValue: function (after, before) {
      if (after === 0) this.tempValue = 0;
      else
        this.tempValue =
          after == null ? null : JSON.parse(JSON.stringify(after));
    },
  },
  mounted: function () {
    if (this.modelValue === 0) this.tempValue = 0;
    else
      this.tempValue =
        this.modelValue == null
          ? null
          : JSON.parse(JSON.stringify(this.modelValue));
  },
  methods: {
    change: function (v) {
      this.$emit("update:modelValue", v);

      if (this.$attrs["onInput"]) this.$attrs["onInput"](v);
    },
    search: function (v) {
      if (this.$attrs["onSearch"]) this.$attrs["onSearch"](v);
    },
    searchChange: function (v) {
      if (this.$attrs["onSearchChange"]) this.$attrs["onSearchChange"](v);
    },
    open: function (v) {
      if (this.$attrs["onOpen"]) this.$attrs["onOpen"]();
    },
  },
};
</script>

<style>
.multiselect-search {
  height: 80%;
  top: 5%;
}
</style>
