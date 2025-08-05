<!-- components/v-tree-row.vue -->
<template>
  <tr>
    <td :style="{ paddingLeft: `${level * 2}em` }">
      <span
        v-if="hasChildren"
        @click="toggle"
        style="cursor: pointer; padding-left: 1em"
      >
        {{ expanded ? "▼" : "▶" }}
      </span>
      <span style="padding-left: 1em">
        {{ node[columns[0].dataField] }}
      </span>
    </td>
    <td
      v-for="(col, idx) in columns.slice(1)"
      :key="col.dataField"
      :style="{ textAlign: col.align || 'left' }"
    >
      {{ node[col.dataField] }}
    </td>
  </tr>
  <template v-if="expanded">
    <v-tree-row
      v-for="child in node.children"
      :key="child[childKey]"
      :node="child"
      :level="level + 1"
      :columns="columns"
      :child-key="childKey"
    />
  </template>
</template>

<script>
export default {
  name: "VTreeRow",
  props: {
    node: { type: Object, required: true },
    level: { type: Number, required: true },
    columns: { type: Array, required: true },
    childKey: { type: String, required: true },
    groupByFields: { type: Array, required: true },
  },
  data() {
    return {
      expanded: true,
    };
  },
  computed: {
    hasChildren() {
      return this.node.children && this.node.children.length > 0;
    },
  },
  methods: {
    toggle() {
      this.expanded = !this.expanded;
    },
  },
};
</script>
