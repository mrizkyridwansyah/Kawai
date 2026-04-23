<template>
  <div
    class="mt-2 d-flex flex-column"
    :style="{
      height: formatHeight(topContentHeight),
    }"
  >
    <div class="table-content-input">
      <div ref="tableContainer" class="table-scroll">
        <div class="table-inner">
          <slot name="table-content" />

          <v-data-empty
            class="mt-3"
            v-if="
              ds &&
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
            !ds.isLoading &&
            ds.data.Items.length > 0 &&
            !ds.isNetworkError &&
            !ds.isServerError
          "
          class="mt-3"
          :table="ds.data"
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
  props: {
    keywordKeys: Array,
    dataItems: {
      type: Array,
      default: () => [],
    },
    frozenColumnLeft: { type: Number, default: 0 },
    topContentHeight: { type: Number, default: 170 },
    ds: { type: Object },
    dsPage: { type: Function },
    dsLength: { type: Function },
    dsLoad: { type: Function },
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
    setFrozenColumns: function (columnIndexes = []) {
      const table = this.$refs.tableContainer?.querySelector("table");
      if (!table) return;

      const headerRow = table.querySelector("thead tr");
      if (!headerRow) return;

      const rows = table.querySelectorAll("tr");

      // Hitung posisi kiri berdasarkan boundingClientRect
      const colPositions = [];
      let totalLeft = 0;
      for (let i = 0; i < headerRow.cells.length; i++) {
        const cell = headerRow.cells[i];
        const width = cell?.getBoundingClientRect().width || 100; // fallback lebih aman
        colPositions[i] = totalLeft;
        totalLeft += width;
      }

      // Terapkan sticky-left
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
        const tryFreeze = () => {
          const table = this.$refs.tableContainer?.querySelector("table");
          if (!table) return;

          const container = this.$refs.tableContainer;
          const prevWidth = table.style.width;
          const prevMinWidth = table.style.minWidth;

          // 🔥 1. paksa reflow width dulu
          table.style.width = "max-content";
          table.style.minWidth = "max-content";

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

          // Hitung total width kolom frozen
          let frozenWidth = 0;
          for (let i = 0; i < this.frozenColumnLeft; i++) {
            const th = headerRow.cells[i];
            frozenWidth += th?.getBoundingClientRect().width || 100;
          }

          const panel = this.$el.querySelector(".panel-body");
          const panelWidth = panel ? panel.clientWidth : window.innerWidth;

          const threshold = 0.7;
          if (window.innerWidth > 768 && frozenWidth < panelWidth * threshold) {
            this.clearFrozenColumns(); // reset dulu
            this.setFrozenColumns(frozenIndexes);
          } else {
            this.clearFrozenColumns();
          }
        };

        // pakai requestAnimationFrame biar benar-benar nunggu render
        requestAnimationFrame(() => {
          tryFreeze();
        });
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
  z-index: 10; /* IMPORTANT */
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
.table-content-input {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;

  border-radius: 0.5em;
  border: 1px solid #d1d5db;
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
