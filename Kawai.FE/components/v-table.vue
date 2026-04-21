<template>
  <div class="mt-4">
    <div class="panel panel-inverse">
      <!-- BEGIN panel-header -->
      <div
        class="panel-heading ui-sortable-handle"
        style="background-color: #333"
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
      <div
        class="panel-body"
        style="border: 1px solid #d1d5db; border-radius: 0 0 0.375rem 0.375rem"
      >
        <!-- BEGIN table-responsive -->
        <div ref="tableContainer">
          <div
            class="v-table-wrapper"
            :style="{
              maxHeight: formatHeight(maxHeight),
              height: formatHeight(defaultHeight),
            }"
          >
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
          <div v-if="ds">
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
        </div>
        <!-- END table-responsive -->
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
    defaultHeight: { type: Number, default: 100 },
    maxHeight: { type: Number, default: 100 },
    ds: { type: Object },
    dsData: { type: Object },
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
          const screenWidth = window.innerWidth;

          const table = this.$refs.tableContainer?.querySelector("table");
          if (!table) return;

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
    formatHeight: function (val) {
      if (typeof val === "number") return val + "px";
      return val; // misal '60vh', '100%', dll
    },
  },
};
</script>

<style>
.v-table-wrapper {
  overflow: auto;
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
