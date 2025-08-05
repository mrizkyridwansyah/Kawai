<template>
  <tr ref="row">
    <td
      v-for="(col, idx) in columns"
      :key="col.dataField"
      :class="{ 'sticky-left': activeFrozenColumns.includes(idx) }"
      :style="{
        textAlign: col.align || 'left',
        paddingLeft: '0', //idx === groupingColIndex ? `${level * 2}em` : '0',
        left:
          activeFrozenColumns.includes(idx) && leftOffsets[idx] !== undefined
            ? `${leftOffsets[idx]}px`
            : undefined,
        zIndex: activeFrozenColumns.includes(idx) ? 10 : null,
        cursor:
          (col.isRender && col.showAtLevel.includes(level)) ||
          (idx === groupingColIndex && hasExpandableChildren)
            ? 'pointer'
            : 'default',
      }"
      style="white-space: nowrap"
      @click="
        col.isRender && col.showAtLevel.includes(level)
          ? col.action && col.action(node)
          : idx === groupingColIndex && hasExpandableChildren
          ? toggle()
          : null
      "
    >
      <template v-if="col.isRender && col.showAtLevel.includes(level)">
        <a href="#" @click.stop.prevent="col.action && col.action(node)">
          {{ col.text }}
        </a>
      </template>
      <template v-else-if="idx === groupingColIndex">
        <span v-if="hasExpandableChildren" style="padding-right: 0.5em">
          {{ expanded ? "▼" : "▶" }}
        </span>
        {{ node[col.dataField] }}
      </template>
      <template v-else>
        {{ node[col.dataField] }}
      </template>
    </td>
  </tr>

  <!-- hanya render anak jika masih di bawah level maksimal -->
  <template
    v-if="expanded && hasExpandableChildren && level + 1 < groupByFields.length"
  >
    <v-tree-row-group
      v-for="(child, i) in node[childKey]"
      :panel-body-ref="panelBodyRef"
      :key="child[childKey]"
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
  props: {
    panelBodyRef: { type: HTMLElement, default: null },
    node: { type: Object, required: true },
    level: { type: Number, required: true },
    columns: { type: Array, required: true },
    childKey: { type: String, required: true },
    groupByFields: { type: Array, required: true },
    frozenColumnLeft: { type: Number, default: 0 },
  },
  data() {
    return {
      expanded: true,
      leftOffsets: [],
      activeFrozenColumns: [],
    };
  },
  computed: {
    groupingColField: function () {
      return this.groupByFields[this.level]?.[0] || null;
    },
    groupingColIndex: function () {
      return this.columns.findIndex(
        (col) => col.dataField === this.groupingColField
      );
    },
    hasExpandableChildren: function () {
      return (
        this.level + 1 < this.groupByFields.length &&
        this.node[this.childKey] &&
        Array.isArray(this.node[this.childKey]) &&
        this.node[this.childKey].length > 0
      );
    },
  },
  mounted: function () {
    this.computeLeftOffsets();
  },
  methods: {
    toggle: function () {
      this.expanded = !this.expanded;
      // this.$nextTick(() => {
      //   setTimeout(() => {
      //     this.computeLeftOffsets();
      //   }, 3000);
      // });
    },
    computeLeftOffsets() {
      const screenWidth = window.innerWidth;
      const panelWidth = this.panelBodyRef?.clientWidth || window.innerWidth;
      const threshold = 0.7;

      let total = 0;
      const newLeftOffsets = [];
      const frozenIndexes = [];
      let frozenWidth = 0;

      this.columns.forEach((col, index) => {
        newLeftOffsets[index] = total;
        const width = col.width ? parseInt(col.width) : 150;
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
    computeLeftOffsets2() {
      // this.$nextTick(() => {
      let total = 0;
      this.columns.forEach((col, index) => {
        this.leftOffsets[index] = total;
        const width = col.width ? parseInt(col.width) : 150; // fallback default
        total += width;
      });
      // });
    },
  },
};
</script>

<style scoped>
/* Sticky Columns (left) */
.sticky-left {
  position: sticky;
  background: white !important;
  background-color: white;
  z-index: 10;
}
</style>
