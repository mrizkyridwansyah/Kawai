<template>
  <v-button-popup>
    <template #button-content>
      <!-- <v-icon icon="arrow-down-up" scale=".8rem" /> -->
      Sort
    </template>
    <div>
      <div class="d-flex pb-1 align-items-center" v-for="(item, i) in items || []" :key="i">
        <div>
          <b-form-checkbox v-model="item.selected" />
        </div>
        <div style="flex: 1;white-space: nowrap;margin-left: 7px; color: black;">
          {{ item.label }}
        </div>
        <div class="ps-3">
          <b-form-radio-group
            v-model="item.direction"
            :options="[
              { text: 'Ascending', value: 'asc' },
              { text: 'Descending', value: 'desc' },
            ]"
            buttons
            button-variant="outline-primary"
            size="xs"
          />
        </div>
        <div class="ms-2 me-2 d-flex">
          <button
            @click="() => slideUp(item, i)"
            :disabled="i == 0"
            class="me-1"
          >
            <v-icon name="chevron-up" :width="14" weight="3" style="color: #445575" />
          </button>
          <button
            @click="() => slideDown(item, i)"
            :disabled="i == items.length - 1"
          >
            <v-icon name="chevron-down" :width="14" weight="3" style="color: #445575" />
          </button>
        </div>
      </div>
    </div>
  </v-button-popup>
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