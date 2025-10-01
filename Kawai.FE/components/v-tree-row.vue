<!-- components/v-tree-row.vue -->
<template>
  <tr ref="row" :class="{ 'summary-row': node.LotNo === 'SUMMARY' }">
    <td
      v-for="(col, idx) in columns"
      :key="col.dataField"
      :class="{
        'sticky-left': activeFrozenColumns.includes(idx),
      }"
      :style="{
        textAlign: col.align || 'left',
        backgroundColor: node.LotNo === 'SUMMARY' ? '#d4edda !important' : '',
        // color: node.LotNo === 'SUMMARY' ? 'white !important' : '',
        fontWeight: node.LotNo === 'SUMMARY' ? 'bold' : '',
        paddingLeft: idx === 0 ? `${level * 2}em` : '0',
        left:
          activeFrozenColumns.includes(idx) && leftOffsets[idx] !== undefined
            ? `${leftOffsets[idx]}px`
            : undefined,
        zIndex: activeFrozenColumns.includes(idx) ? 10 : null,
        cursor: hasChildren && idx === 0 ? 'pointer' : 'default',
        whiteSpace: 'nowrap',
      }"
      @click="idx === 0 && hasChildren ? toggle() : null"
    >
      <template v-if="idx === 0">
        <span
          v-if="hasChildren"
          :class="['toggle-button', expanded ? 'collapse' : 'expand']"
        >
          {{ expanded ? "-" : "+" }}
        </span>
        <!-- <span v-if="hasChildren" style="padding-right: 0.5em">
          {{ expanded ? "▼" : "▶" }}
        </span> -->
        {{ node[col.dataField] }}
      </template>
      <template v-else>
        <span style="padding-left: 0.5em">
          {{ node[col.dataField] }}
        </span>
      </template>
    </td>
  </tr>

  <template v-if="expanded && hasChildren">
    <v-tree-row
      v-for="(child, i) in node[childKey]"
      :key="i"
      :node="child"
      :level="level + 1"
      :columns="columns"
      :child-key="childKey"
      :group-by-fields="groupByFields"
      :frozen-column-left="frozenColumnLeft"
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
    frozenColumnLeft: { type: Number, default: 0 },
    startCollapseLevel: { type: Number },
  },
  data() {
    return {
      expanded: true,
      leftOffsets: [],
      activeFrozenColumns: [],
    };
  },
  computed: {
    hasChildren() {
      return (
        this.node[this.childKey] &&
        Array.isArray(this.node[this.childKey]) &&
        this.node[this.childKey].length > 0
      );
    },
  },
  mounted() {
    this.computeLeftOffsets();
  },
  methods: {
    toggle() {
      this.expanded = !this.expanded;
    },
    computeLeftOffsets() {
      const screenWidth = window.innerWidth;
      const panelWidth = this.$el.parentElement?.clientWidth || screenWidth;
      const threshold = 0.7;

      let total = 0;
      const newLeftOffsets = [];
      const frozenIndexes = [];
      let frozenWidth = 0;

      this.columns.forEach((col, index) => {
        const width = col.width ? parseInt(col.width) : 150;
        newLeftOffsets[index] = total;
        total += width;

        if (index < this.frozenColumnLeft) {
          frozenWidth += width;
          frozenIndexes.push(index);
        }
      });

      this.leftOffsets = newLeftOffsets;

      if (screenWidth > 768 && frozenWidth < panelWidth * threshold) {
        this.activeFrozenColumns = frozenIndexes;
      } else {
        this.activeFrozenColumns = [];
      }
    },
  },
};
</script>

<style scoped>
.sticky-left {
  position: sticky;
  background: white !important;
  background-color: white;
  z-index: 10;
}

.toggle-button {
  margin-left: 1em;
  display: inline-block;
  width: 14px;
  height: 14px;
  line-height: 12px;
  font-size: 10px;
  font-weight: bold;
  text-align: center;
  border: 1px solid;
  border-radius: 50%; /* full bulat */
  cursor: pointer;
  margin-right: 6px;
  user-select: none;
}

.toggle-button.expand {
  color: #007bff;
  border-color: #007bff;
  background-color: #e6f0ff;
}

.toggle-button.collapse {
  color: #dc3545;
  border-color: #dc3545;
  background-color: #ffe6e6;
}

.toggle-button:hover {
  opacity: 0.85;
}
</style>
