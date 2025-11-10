<template>
  <div class="mt-4">
    <div class="panel panel-inverse">
      <!-- BEGIN panel-body -->
      <div
        class="panel-body"
        style="border: 1px solid #d1d5db; border-radius: 0 0 0.375rem 0.375rem"
      >
        <!-- BEGIN table-responsive -->
        <div ref="tableContainer">
          <div class="v-table-wrapper">
            <slot name="table-content" />
          </div>
          <div v-if="ds">
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
              <v-data-empty
                class="mt-3"
                v-if="
                  !ds.isLoading &&
                  ds.data.Items.length == 0 &&
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
        <!-- END table-responsive -->
      </div>
      <!-- END panel-body -->
    </div>
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
          const screenWidth = window.innerWidth;

          const table = this.$refs.tableContainer?.querySelector("table");
          if (!table) return;

          const headerRow = table.querySelector("thead tr");
          if (!headerRow) return;

          const frozenIndexes = Array.from(
            { length: this.frozenColumnLeft },
            (_, i) => i
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

          if (screenWidth > 768 && frozenWidth < panelWidth * threshold) {
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
  /* border: 1px solid #ddd; */
  position: relative;
}

.v-table-wrapper thead th {
  background-color: #8ec5fc;
}

/* Bikin table bisa scroll horizontal juga */
.v-fixed-table {
  width: max-content; /* agar scroll horizontal muncul */
  min-width: 100%;
  /* border-collapse: separate; */
  /* border-spacing: 0; */
}

.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  /* border: 1px solid #dee2e6; */
  background: #fff;
}

/* Sticky Header (atas) */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20; /* harus lebih tinggi dari sticky kiri */
  background: #8ec5fc;
}

/* Sticky Columns (kiri) */
.sticky-left {
  position: sticky;
  /* background: #8ec5fc !important;
  background-color: #8ec5fc; */
  z-index: 10;
  /* left akan diset via JS */
}

/* Kalau sticky kiri di header, beri z-index lebih tinggi */
thead .sticky-left {
  z-index: 30;
}
</style>
