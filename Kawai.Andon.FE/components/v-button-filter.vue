<template>
  <v-button-popup-side>
    <template #button-content>
      <!-- <v-icon icon="arrow-down-up" scale=".8rem" /> -->
      Filter
    </template>
    <div>
      <div class="p-3" style="position: sticky;top: 0;background: #fff;z-index: 2;">
        <h5>Filters</h5>
      </div>
      <slot />
    </div>
  </v-button-popup-side>
</template>

<script>
export default {
  model: {
    prop: 'modelValue',
    event: "update",
  },
  props: ["modelValue", "items"],
  emits: ['update:modelValue'],
  computed: {
    data: function () {
      var obj = {};
      if (this.items?.length > 0) {
        this.items.filter((p) => p.selected === true).forEach((r) => (obj[r.value] = r.direction));
      }
      return obj;
    },
  },
  watch: {
    items: function (after) {
      if (JSON.stringify(after) == "{}" && JSON.stringify(this.data) != "{}")
        this.$emit("update:modelValue", this.data);
    },
    value: function (after) {
      if (JSON.stringify(after) == "{}" && JSON.stringify(this.data) != "{}")
        this.$emit("update:modelValue", this.data);
    },
    data: function (after) {
      this.$emit("update:modelValue", after);
    },
  },
  data: () => ({
    fields: [],
    model: {},
  }),
  methods: {
    slideUp: function (item, i) {
      this.items.splice(i, 1);
      this.items.splice(i - 1, 0, item);
    },
    slideDown: function (item, i) {
      this.items.splice(i, 1);
      this.items.splice(i + 1, 0, item);
    },
  },
};
</script>