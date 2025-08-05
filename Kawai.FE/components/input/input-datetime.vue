<template>
  <div v-if="noGroup !== undefined">
    <div class="input-group">
      <date-picker
        class="form-control"
        :class="errors ? 'is-invalid' : null"
        v-model="tempValue"
        value-zone="utc"
        zone="utc"
        format="dd/MM/yyyy"
        :input-id="key"
        :disabled="disabled"
        v-model:open-pop-up="isOpen"
      />
      <span
        class="input-group-text btn btn-primary"
        @click="() => (this.disabled ? null : open())"
      >
        <v-icon name="calendar" width="16" />
      </span>
    </div>
    <!-- <v-date-picker v-model="tempValue" mode="date" /> -->
    <!-- <b-popover 
      v-if="errors" 
      :target="`input-text-${key}`" 
      triggers="hover" 
      placement="top"
      variant="danger"
    >
      <template #title><span class="text-danger">Invalid</span></template>
{{ errors[0] }}
</b-popover> -->
    <small class="form-text text-muted" v-if="description">{{
      description
    }}</small>
  </div>
  <div v-else :class="col ? 'form-group col-' + col : 'form-group'">
    <label class="d-block">{{ label }}</label>
    <div class="input-group">
      <date-picker
        class="form-control"
        :class="errors ? 'is-invalid' : null"
        v-model="tempValue"
        value-zone="utc"
        zone="utc"
        format="dd/MM/yyyy"
        :input-id="key"
        v-model:open-pop-up="isOpen"
        placeholder="dd/MM/yyyy"
        :disabled="disabled"
      />
      <span
        class="input-group-text btn btn-primary"
        @click="() => (this.disabled ? null : open())"
      >
        <v-icon name="calendar" width="16" />
      </span>
    </div>
    <!-- {{ tempValue }} {{ new Date(tempValue).getTime() }} {{ new Date(tempValue) }} -->
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{
      description
    }}</small>
  </div>
</template>

<script>
import moment from "moment";
import DatePicker from "./datetime/v-datetime.vue";
export default {
  // key: uniqueId(),
  model: {
    prop: "modelValue",
    event: "update",
  },
  emits: ["update:modelValue"],
  props: [
    "modelValue",
    "label",
    "col",
    "description",
    "placeholder",
    "onSelect",
    "errors",
    "value",
    "multiline",
    "rows",
    "no-group",
    "disabled",
    "style",
  ],
  components: { DatePicker },
  data: () => ({
    tempValue: null,
    key: uniqueId(),
    isOpen: false,
  }),
  watch: {
    modelValue: function (after, before) {
      if (after === null) {
        this.tempValue = null;
        return;
      }

      if (JSON.stringify(after) == JSON.stringify(before)) return;

      var d = new Date(after);
      this.tempValue = this.$func.asUtcString(d);
    },
    tempValue: function (after, before) {
      if (after === null) return;

      if (JSON.stringify(after) == JSON.stringify(before)) return;

      var d = new Date(after);
      this.$emit("update:modelValue", this.$func.asUtcString(d));
    },
  },
  mounted: function () {
    if (this.modelValue) {
      var d = new Date(this.modelValue);
      this.tempValue = moment(this.$func.asUtcString(d)).toISOString();
    }
  },
  methods: {
    change: function (e) {
      this.$emit("update:modelValue", e.target.value);
    },
    open: function () {
      this.isOpen = true;
      // var c = document.getElementById(this.key)
      // c.blur();
    },
  },
};
</script>
