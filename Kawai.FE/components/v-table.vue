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
    <div class="table-content">
      <div ref="tableContainer" class="table-scroll">
        <div class="table-inner">
          <slot name="table-content" />

          <v-data-empty
            class="mt-3"
            v-if="
              !ds.isLoading &&
              (dsData || ds.data).Items.length == 0 &&
              !ds.isNetworkError &&
              !ds.isServerError
            "
          />
        </div>
      </div>
    </div>

    <!-- BEGIN pagination (NOW STICKS TO BOTTOM) -->
    <div class="mt-auto paging-wrapper" v-if="ds">
      <v-loading-2 class="m-5 p-5" v-if="ds.isLoading" />

      <div>
        <v-table-pagination
          v-if="
            usePaging &&
            !ds.isLoading &&
            (dsData || ds.data).Items.length > 0 &&
            !ds.isNetworkError &&
            !ds.isServerError
          "
          class="mt-3"
          :table="dsData || ds.data"
          :page-change="dsPage || ds.setPage"
          :length-change="dsLength || ds.setLength"
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
    <!-- END pagination -->
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
    handleExport: async function () {
      this.isExporting = true;
      try {
        console.log(this.isExporting, "try");
        await this.exportExcelAction();
      } catch (e) {
        console.error("Export error:", e);
      } finally {
        console.log(this.isExporting, "finally");
        this.isExporting = false;
      }
    },
    setFrozenColumns: function (columnIndexes = []) {
      const table = this.$refs.tableContainer?.querySelector("table");
      if (!table) return;

      const headerRow = table.querySelector("thead tr");
      if (!headerRow) return;

      const rows = table.querySelectorAll("tr");

      // 🔍 Hitung posisi kiri berdasarkan DOM
      const colPositions = [];
      let totalLeft = 0;
      for (let i = 0; i < headerRow.cells.length; i++) {
        colPositions[i] = totalLeft;
        const cell = headerRow.cells[i];
        totalLeft += cell?.offsetWidth || 150; // fallback kalau width belum kebaca
      }

      // 🔧 Terapkan sticky-left dan style ke sel-sel yang ditentukan
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
          console.log(window.innerWidth, "asd")
          const table = this.$refs.tableContainer?.querySelector("table");
          if (!table) return;

          const container = this.$refs.tableContainer;
          const prevWidth = table.style.width;
          const prevMinWidth = table.style.minWidth;
          // 🔥 1. paksa reflow width dulu
          table.style.width = "max-content";
          table.style.minWidth = "max-content";

          // 🔥 2. tunggu browser update layout
          requestAnimationFrame(() => {
            const tableWidth = table.offsetWidth;
            const containerWidth = container.offsetWidth;

            // 🔥 cek apakah lebih kecil dari container
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

            // 🔥 3. baru hitung width (sudah stabil)
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

            const frozenIndexesFinal = frozenIndexes;

            if (
              window.innerWidth > 768 &&
              frozenWidth < panelWidth * threshold
            ) {
              this.setFrozenColumns(frozenIndexesFinal);
            } else {
              console.log(window.innerWidth);
              this.clearFrozenColumns();
            }
          });
        }, 200);
      });
    },
    waitForDOMThenFreeze2: function () {
      this.$nextTick(() => {
        setTimeout(() => {
          const screenWidth = window.innerWidth;

          const table = this.$refs.tableContainer?.querySelector("table");
          if (!table) return;

          table.style.width = "max-content";
          table.style.minWidth = "max-content";

          const headerRow = table.querySelector("thead tr");
          if (!headerRow) return;

          const frozenIndexes = Array.from(
            { length: this.frozenColumnLeft },
            (_, i) => i,
          );

          // ✅ Ambil width dari th langsung
          let frozenWidth = 0;
          for (let i = 0; i < this.frozenColumnLeft; i++) {
            const th = headerRow.cells[i];
            if (th) {
              frozenWidth += th.offsetWidth || 0;
            } else {
              frozenWidth += 150; // fallback default
            }
          }

          // ✅ Dapatkan lebar panel-body
          const panel = this.$el.querySelector(".panel-body");
          const panelWidth = panel ? panel.clientWidth : screenWidth;

          const threshold = 0.7;

          console.log(
            screenWidth > 768 && frozenWidth < panelWidth * threshold,
          );
          if (screenWidth > 768 && frozenWidth < panelWidth * threshold) {
            this.setFrozenColumns(frozenIndexes);
          } else {
            console.log(screenWidth);
            this.clearFrozenColumns();
          }
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
/* 🔥 ONLY ONE SCROLL */
.table-scroll {
  flex: 1;
  min-height: 0;
  overflow: auto;
  position: relative;
}

/* TABLE WRAPPER (NO OVERFLOW!!) */
.table-inner {
  /* width: max-content; */
  min-width: 100%;
}

/* TABLE */
.v-fixed-table {
  width: max-content;
  min-width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

/* HEADER STICKY (FIXED) */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20; /* IMPORTANT */
  background: #8ec5fc;

  /* optional but helps stability */
  box-shadow: 0 2px 2px rgba(0, 0, 0, 0.08);
}

/* CELL STYLE */
.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  background: #fff;
}

/* FROZEN COLUMN */
.sticky-left {
  position: sticky;
  z-index: 50;
}

thead .sticky-left {
  z-index: 1000;
}

/* PANEL BODY */
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

/* PAGINATION */
.paging-wrapper {
  border-left: 1px solid #d1d5db;
  border-right: 1px solid #d1d5db;
  border-bottom: 1px solid #d1d5db;
  border-radius: 0 0 0.375rem 0.375rem;

  padding: 0 1em;
}

</style>
