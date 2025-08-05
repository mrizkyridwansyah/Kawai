<template>
  <div>
    <div class="input-group">
      <div
        class="input-group-prepend"
        v-if="prependText !== undefined && prependText !== null"
      >
        <div class="input-group-text">{{ prependText }}</div>
      </div>
      <input
        type="text"
        class="w-100 form-control"
        v-model="val"
        @keyup="change"
        :class="cClass"
        :disabled="disabled"
      />
      <div
        class="input-group-append"
        v-if="appendText !== undefined && appendText !== null"
      >
        <div class="input-group-text">{{ appendText }}</div>
      </div>
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
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
    "label",
    "col",
    "noGroup",
    "errors",
    "appendText",
    "prependText",
    "disabled",
    "isInt",
  ],
  data: () => ({
    val: 0,
  }),
  computed: {
    cClass: function () {
      return this.errors ? "is-invalid text-right" : "text-right";
    },
  },
  watch: {
    modelValue: function (after) {
      this.val = structuredClone(after.toFixed(9));
      this.val = after < 0 ? after * -1 : after;

      let result = this.formatIfExponential(this.val);
      this.val = this.$func.formatNumber(result);
    },
  },
  mounted: function () {
    if (!isNaN(this.modelValue)) {
      this.val = this.$func.formatNumber(this.modelValue);
    }
  },
  methods: {
    change: function (e) {
      var val = e.target.value.split(",").join("");

      if (isNaN(val)) val = this.modelValue;

      if (this.isInt) val = parseInt(val);

      this.val = this.$func.formatNumber(val);
      this.$emit("update:modelValue", +val || 0);
    },
    formatIfExponential: function (val) {
      const str = String(val);

      // Cek apakah bentuk eksponensial (ada huruf 'e' atau 'E')
      if (str.includes("e") || str.includes("E")) {
        console.log(str);
        return Number(val).toFixed(9); // konversi ke desimal string
      }

      return val;
    },
  },
};
</script>
