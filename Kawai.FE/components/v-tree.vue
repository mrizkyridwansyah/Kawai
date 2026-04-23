<template>
  <div
    class="mt-2 d-flex flex-column"
    :style="{
      height: formatHeight(topContentHeight),
    }"
  >
    <div class="tree-content-input">
      <div class="tree-scroll" ref="panelBody">
        <div ref="tableContainer" class="tree2-scroll">
          <div class="tree-inner">
            <table
              class="tree-table v-fixed-table table table-bordered table-striped mb-0 align-middle"
              ref="table"
            >
              <thead>
                <tr>
                  <th
                    v-for="col in columns"
                    :key="col.dataField"
                    :style="{
                      width: col.width || 'auto',
                      textAlign: col.align || 'left',
                    }"
                    style="white-space: nowrap"
                  >
                    {{ col.text }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <v-tree-row
                  v-for="row in treeData"
                  :panel-body-ref="$refs.panelBody"
                  :key="row[childKey]"
                  :node="row"
                  :level="0"
                  :columns="columns"
                  :child-key="childKey"
                  :frozen-column-left="frozenColumnLeft"
                />
              </tbody>
            </table>
            <v-loading-2 class="m-5 p-5" v-if="isLoading" />
            <div>
              <v-data-empty
                class="mt-3"
                v-if="
                  !isLoading &&
                  treeData.length == 0 &&
                  !isNetworkError &&
                  !isServerError
                "
              />
              <v-error-server class="mt-3" v-if="!isLoading && isServerError" />
              <v-error-network
                class="mt-3"
                v-if="!isLoading && isNetworkError"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    isLoading: { type: Boolean, default: false },
    isServerError: { type: Boolean, default: false },
    isNetworkError: { type: Boolean, default: false },
    treeData: { type: Array, required: true },
    columns: { type: Array, required: true },
    childKey: { type: String, required: true },
    frozenColumnLeft: { type: Number, default: 0 },
    topContentHeight: { type: Number, default: 170 },
  },
  watch: {
    treeData: function () {
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

      const rows = table.querySelectorAll("tr");

      // Hitung posisi kiri dari definisi kolom, bukan dari DOM
      const colPositions = [];
      let totalLeft = 0;
      this.columns.forEach((col, index) => {
        colPositions[index] = totalLeft;
        const width = col.width ? parseInt(col.width) : 150;
        totalLeft += width;
      });

      // Set sticky + left
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
    setFrozenColumns2: function (columnIndexes = []) {
      const table = this.$refs.tableContainer?.querySelector("table");
      if (!table) return;

      const headerRow = table.querySelector("thead tr");
      if (!headerRow) return;

      const colPositions = [];
      columnIndexes.forEach((colIndex) => {
        let leftOffset = 0;
        for (let i = 0; i < colIndex; i++) {
          leftOffset += headerRow.cells[i]?.offsetWidth || 0;
        }
        colPositions[colIndex] = leftOffset;
      });

      const rows = table.querySelectorAll("tr");
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
          const screenWidth = window.innerWidth;
          const frozenIndexes = Array.from(
            { length: this.frozenColumnLeft },
            (_, i) => i,
          );

          // Dapatkan total width kolom yang akan di-freeze
          let frozenWidth = 0;
          for (let i = 0; i < this.frozenColumnLeft; i++) {
            const col = this.columns[i];
            const colWidth = col && col.width ? parseInt(col.width) : 150; // default fallback
            frozenWidth += colWidth;
          }

          // Dapatkan lebar panel-body atau table container
          const panel = this.$el.querySelector(".panel-body");
          const panelWidth = panel ? panel.clientWidth : screenWidth;

          const threshold = 0.7; // 70%          // Jika layar lebar (> 768px), aktifkan sticky column
          if (screenWidth > 768 && frozenWidth < panelWidth * threshold) {
            this.setFrozenColumns(frozenIndexes);
          } else {
            // Kalau layar kecil, hapus sticky-left jika sebelumnya sudah diset
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
    waitForDOMThenFreeze2: function () {
      this.$nextTick(() => {
        setTimeout(() => {
          const frozenIndexes = Array.from(
            { length: this.frozenColumnLeft },
            (_, i) => i,
          );
          this.setFrozenColumns(frozenIndexes);
        }, 500); // lebih lama supaya semua row render
      });
    },
    formatHeight: function (val) {
      return `calc(100vh - ${val}px)`;
    },
  },
};
</script>

<style scoped>
.tree-table {
  width: 100%;
  border-collapse: collapse;
}

.v-table-wrapper {
  overflow: auto;
  max-height: 500px;
  position: relative;
}

/* .scroll-x-wrapper {
  overflow-x: auto;
  width: 100%;
} */

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

/* Sticky Header */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20;
  background: #8ec5fc;
  text-align: center !important;
}

/* Sticky Columns (left) */
.sticky-left {
  position: sticky;
  background: #8ec5fc !important;
  background-color: #8ec5fc;
  z-index: 10;
}

/* Sticky Columns in thead */
thead .sticky-left {
  z-index: 30;
}

.tree-content-input {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;

  border-radius: 0.5em;
  border: 1px solid #d1d5db;
  padding: 1em;
}

.tree-scroll {
  flex: 1;
  display: flex; /* 🔥 penting */
  flex-direction: column; /* 🔥 penting */
  min-height: 0; /* 🔥 WAJIB buat scroll */
  overflow: hidden; /* scroll pindah ke dalam */
}

.tree2-scroll {
  flex: 1; /* 🔥 bikin dia stretch */
  min-height: 0; /* 🔥 biar bisa scroll */
  overflow: auto; /* 🔥 scroll di sini */
}

.tree-inner {
  min-width: 100%;
  width: max-content; /* kalau mau horizontal scroll langsung muncul */
}
</style>
