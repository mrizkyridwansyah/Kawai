<template>
  <div class="mt-4">
    <div class="panel panel-inverse">
      <!-- BEGIN panel-header -->
      <div
        class="panel-heading ui-sortable-handle"
        style="background-color: #333"
      >
        <v-button-sort
          class="mr-1"
          v-model="this.filter.sorts"
          :items="this.filter.sortItems"
        />
        <div class="input-group ml-2 mr-2">
          <input
            type="text"
            placeholder="Search..."
            class="form-control"
            style="padding: 4px 10px"
            v-model="filter.keyword"
          />
        </div>

        <button
          v-if="this.exportExcel"
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
      <div
        class="panel-body"
        style="border: 1px solid #d1d5db; border-radius: 0 0 0.375rem 0.375rem"
      >
        <div ref="tableContainer">
          <div class="v-table-wrapper">
            <slot name="table-content" />
          </div>

          <!-- Loading only -->
          <v-loading-2 class="m-5 p-5" v-if="ds && ds.isLoading" />
          <div>
            <v-data-empty
              class="mt-3"
              v-if="
                !ds.isLoading &&
                (dsData || ds.data).Items.length == 0 &&
                !ds.isNetworkError &&
                !ds.isServerError
              "
            />
            <v-error-server
              class="mt-3"
              v-if="!ds.isLoading && ds.isServerError"
              :refresh="dsLoad || ds.load"
            />
            <v-error-network
              class="mt-3"
              v-if="!ds.isLoading && ds.isNetworkError"
              :refresh="dsLoad || ds.load"
            />
          </div>
        </div>
      </div>
      <!-- END panel-body -->
    </div>
  </div>
</template>

<script>
export default {
  data: () => ({
    isExporting: false,
  }),
  props: {
    filter: Object,
    keywordKeys: Array,
    exportExcel: Boolean,
    exportExcelAction: Function,
    dataItems: {
      type: Array,
      default: () => [],
    },
    frozenColumnLeft: { type: Number, default: 0 },
    ds: { type: Object },
    dsData: { type: Object },
  },
  watch: {
    dataItems: function () {
      this.waitForDOMThenFreeze();
      window.addEventListener("resize", this.waitForDOMThenFreeze);
    },
  },
  mounted: function () {
    this.waitForDOMThenFreeze();
  },
  beforeUnmount: function () {
    window.removeEventListener("resize", this.waitForDOMThenFreeze);
  },
  methods: {
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
        totalLeft += headerRow.cells[i]?.offsetWidth || 150;
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

          const headerRow = table.querySelector("thead tr");
          if (!headerRow) return;

          const frozenIndexes = Array.from(
            { length: this.frozenColumnLeft },
            (_, i) => i
          );

          let frozenWidth = 0;
          for (let i = 0; i < this.frozenColumnLeft; i++) {
            const th = headerRow.cells[i];
            frozenWidth += th?.offsetWidth || 150;
          }

          const panel = this.$el.querySelector(".panel-body");
          const panelWidth = panel ? panel.clientWidth : window.innerWidth;

          const threshold = 0.7;

          if (window.innerWidth > 768 && frozenWidth < panelWidth * threshold) {
            this.setFrozenColumns(frozenIndexes);
          } else {
            this.clearFrozenColumns();
          }
        }, 500);
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
  },
};
</script>

<style>
.v-table-wrapper {
  overflow: auto;
  max-height: 500px;
  position: relative;
}

.v-table-wrapper thead th {
  background-color: #8ec5fc;
}

.v-fixed-table {
  width: max-content;
  min-width: 100%;
}

.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  background: #fff;
}

.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20;
  background: #8ec5fc;
}

.sticky-left {
  position: sticky;
  z-index: 10;
}

thead .sticky-left {
  z-index: 30;
}
</style>
