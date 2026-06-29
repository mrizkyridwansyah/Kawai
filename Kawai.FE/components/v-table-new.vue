<template>
  <div
    class="mt-2 d-flex flex-column"
    :style="{
      height: formatHeight(topContentHeight),
    }"
  >
    <!-- BEGIN panel-header -->
    <div
      class="ui-sortable-handle"
      style="
        background-color: #333;
        display: flex;
        padding: 1em;
        border-start-start-radius: 0.5em;
        border-start-end-radius: 0.5em;
      "
      v-if="useHeader"
    >
      <v-button-sort
        class="mr-1"
        v-if="filter"
        v-model="filter.sorts"
        :items="filter.sortItems"
      />

      <div class="input-group ml-2 mr-2">
        <input
          type="text"
          placeholder="Search..."
          class="form-control"
          style="padding: 4px 10px"
          v-if="filter"
          v-model="filter.keyword"
        />
      </div>

      <button
        v-if="exportExcel"
        class="btn btn-sm btn-green btn-elevate"
        :disabled="isExporting"
        @click="handleExport"
      >
        <span v-if="!isExporting">
          <font-awesome-icon icon="file-excel" style="font-size: 1.25em" />
        </span>
        <span v-else>
          <i class="fa fa-spinner fa-spin"></i>
        </span>
      </button>
    </div>
    <!-- END panel-header -->

    <!-- BEGIN panel-body -->
    <div class="table-content">
      <div ref="tableContainer" class="table-scroll">
        <div class="table-inner">
          <slot name="table-content" />

          <v-data-empty
            class="mt-3"
            v-if="
              !(ds && ds.isLoading) &&
              tableData &&
              tableData.Items &&
              tableData.Items.length == 0 &&
              !(ds && ds.isNetworkError) &&
              !(ds && ds.isServerError)
            "
          />
        </div>
      </div>
    </div>

    <!-- BEGIN pagination -->
    <div class="mt-auto paging-wrapper" v-if="ds || dsData">
      <v-loading-2 class="m-5 p-5" v-if="ds && ds.isLoading" />

      <div>
        <v-table-pagination-new
          v-if="
            usePaging &&
            !(ds && ds.isLoading) &&
            tableData &&
            tableData.Items &&
            tableData.Items.length > 0 &&
            !(ds && ds.isNetworkError) &&
            !(ds && ds.isServerError)
          "
          class="mt-3"
          :table="tableData"
          :page-change="handlePageChange"
          :length-change="handleLengthChange"
          @page-change="emitPageChange"
          @length-change="emitLengthChange"
        />

        <v-error-server
          class="mt-3"
          v-if="ds && !ds.isLoading && ds.isServerError"
          :refresh="dsLoad || ds.load"
        />

        <v-error-network
          class="mt-3"
          v-if="ds && !ds.isLoading && ds.isNetworkError"
          :refresh="dsLoad || ds.load"
        />
      </div>
    </div>
    <!-- END pagination -->
  </div>
</template>

<script>
export default {
  data: () => ({
    isExporting: false,
  }),
  emits: ["page-change", "length-change"],
  props: {
    filter: Object,
    keywordKeys: Array,
    exportExcel: Boolean,
    usePaging: {
      type: Boolean,
      default: true,
    },
    useHeader: {
      type: Boolean,
      default: true,
    },
    exportExcelAction: Function,
    dataItems: {
      type: Array,
      default: () => [],
    },
    frozenColumnLeft: { type: Number, default: 0 },
    topContentHeight: { type: Number, default: 170 },
    ds: { type: Object },
    dsData: { type: Object },
    dsPage: { type: Function },
    dsLength: { type: Function },
    dsLoad: { type: Function },
  },
  computed: {
    tableData: function () {
      return this.dsData || (this.ds ? this.ds.data : { Items: [], Page: 1, Length: 10, Filtered: 0, Total: 0 });
    },
  },
  watch: {
    dataItems: function () {
      this.waitForDOMThenFreeze();
      window.addEventListener("resize", this._onResize);
    },
  },
  mounted: function () {
    this._onResize = this.waitForDOMThenFreeze.bind(this);
    window.addEventListener("resize", this._onResize);

    this.waitForDOMThenFreeze();
  },
  beforeUnmount: function () {
    window.removeEventListener("resize", this._onResize);
  },
  methods: {
    handlePageChange: function (page) {
      if (this.dsPage) {
        this.dsPage(page);
      } else if (this.ds && this.ds.setPage) {
        this.ds.setPage(page);
      }
      this.$emit("page-change", page);
    },
    handleLengthChange: function (length) {
      if (this.dsLength) {
        this.dsLength(length);
      } else if (this.ds && this.ds.setLength) {
        this.ds.setLength(length);
      }
      this.$emit("length-change", length);
    },
    emitPageChange: function (page) {
      this.$emit("page-change", page);
    },
    emitLengthChange: function (length) {
      this.$emit("length-change", length);
    },
    handleExport: async function () {
      this.isExporting = true;
      try {
        await this.exportExcelAction();
      } catch (e) {
        console.error("Export error:", e);
      } finally {
        this.isExporting = false;
      }
    },
    setFrozenColumns: function (columnIndexes = []) {
      const table = this.$refs.tableContainer?.querySelector("table");
      if (!table) return;

      const headerRow = table.querySelector("thead tr");
      if (!headerRow) return;

      const rows = table.querySelectorAll("tr");

      const colPositions = [];
      let totalLeft = 0;
      for (let i = 0; i < headerRow.cells.length; i++) {
        colPositions[i] = totalLeft;
        const cell = headerRow.cells[i];
        totalLeft += cell?.offsetWidth || 150;
      }

      rows.forEach((row) => {
        columnIndexes.forEach((colIndex) => {
          const cell = row.cells[colIndex];
          if (cell) {
            cell.classList.add("sticky-left");
            cell.style.left = `${colPositions[colIndex]}px`;
            cell.style.zIndex = row.parentNode.tagName === "THEAD" ? 30 : 10;
          }
        });
      });
    },
    waitForDOMThenFreeze: function () {
      this.$nextTick(() => {
        setTimeout(() => {
          const table = this.$refs.tableContainer?.querySelector("table");
          if (!table) return;

          const container = this.$refs.tableContainer;
          const prevWidth = table.style.width;
          const prevMinWidth = table.style.minWidth;

          table.style.width = "max-content";
          table.style.minWidth = "max-content";

          requestAnimationFrame(() => {
            const tableWidth = table.offsetWidth;
            const containerWidth = container.offsetWidth;

            if (tableWidth < containerWidth) {
              table.style.width = prevWidth || "100%";
              table.style.minWidth = prevMinWidth || "100%";
            }

            const headerRow = table.querySelector("thead tr");
            if (!headerRow) return;

            const frozenIndexes = Array.from(
              { length: this.frozenColumnLeft },
              (_, i) => i,
            );

            let frozenWidth = 0;
            const colPositions = [];
            let totalLeft = 0;

            for (let i = 0; i < this.frozenColumnLeft; i++) {
              const th = headerRow.cells[i];
              const width = th?.offsetWidth || 150;

              colPositions[i] = totalLeft;
              totalLeft += width;
              frozenWidth += width;
            }

            const panel = this.$el.querySelector(".panel-body");
            const panelWidth = panel ? panel.clientWidth : window.innerWidth;

            const threshold = 0.7;

            if (
              window.innerWidth > 768 &&
              frozenWidth < panelWidth * threshold
            ) {
              this.setFrozenColumns(frozenIndexes);
            } else {
              this.clearFrozenColumns();
            }
          });
        }, 200);
      });
    },
    clearFrozenColumns: function () {
      const table = this.$refs.tableContainer?.querySelector("table");
      if (!table) return;

      const rows = table.querySelectorAll("tr");
      rows.forEach((row) => {
        Array.from(row.cells).forEach((cell) => {
          cell.classList.remove("sticky-left");
          cell.style.left = null;
          cell.style.zIndex = null;
        });
      });
    },
    formatHeight: function (val) {
      return `calc(100vh - ${val}px)`;
    },
  },
};
</script>

<style>
.table-scroll {
  flex: 1;
  min-height: 0;
  overflow: auto;
  position: relative;
}

.table-inner {
  min-width: 100%;
}

.v-fixed-table {
  width: max-content;
  min-width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20;
  background: #8ec5fc;
  box-shadow: 0 2px 2px rgba(0, 0, 0, 0.08);
}

.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  background: #fff;
}

.sticky-left {
  position: sticky;
  z-index: 50;
}

thead .sticky-left {
  z-index: 1000;
}

.table-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;

  border-top: 1px solid #d1d5db;
  border-left: 1px solid #d1d5db;
  border-right: 1px solid #d1d5db;
  padding: 1em;
}

.paging-wrapper {
  border-left: 1px solid #d1d5db;
  border-right: 1px solid #d1d5db;
  border-bottom: 1px solid #d1d5db;
  border-radius: 0 0 0.375rem 0.375rem;

  padding: 0 1em;
}
</style>
