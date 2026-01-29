<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="chart-column" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Receiving Andon (Temporary Area)
      </span>
    </div>

    <div class="panel-body">
      <!-- SUMMARY -->
      <div class="row">
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div class="p-2 header-summary-content bg-purple">
            <span class="title-summary">
              Total Receipt
            </span>
            <br />
            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.total) }}
            </span>
          </div>
        </div>

        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div class="p-2 header-summary-content bg-gray">
            <span class="title-summary">QC Inprogress</span>
            <br />
            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.pending) }}
            </span>
          </div>
        </div>

        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div class="p-2 header-summary-content bg-blue">
            <span class="title-summary">Passed QC</span>
            <br />
            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.passed) }}
            </span>
          </div>
        </div>

        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div class="p-2 header-summary-content bg-green">
            <span class="title-summary">NG QC</span>
            <br />
            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.ng) }}
            </span>
          </div>
        </div>
      </div>

      <!-- TABLES -->
      <div class="row mt-3">
        <!-- PENDING RECEIPT -->
        <div class="col-xl-6 col-lg-6 col-md-12 col-sm-12 col-12">
          <div class="panel panel-inverse equal-panel">
            <div class="panel-heading">
              <font-awesome-icon icon="file-pen" class="icon-title text-white" />
              <span class="ml-3">Pending Receipt Check</span>
            </div>

            <div class="v-table-wrapper">
              <table class="table mb-0 v-fixed-table">
                <thead>
                  <tr>
                    <th class="sticky-head text-center">Receipt</th>
                    <th>Date</th>
                    <th>Supplier</th>
                    <th>DN</th>
                    <th>Item</th>
                    <th>Qty(Unit)</th>
                    <th>Qty(Pack)</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in list" :key="item.ReceiptNo">
                    <td class="sticky-body text-center">{{ item.ReceiptNo }}</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyPack) }}
                    </td>
                    <td>{{ item.StatusReceiptName }}</td>
                  </tr>
                </tbody>
              </table>

              <v-data-empty v-if="list.length === 0" />
            </div>
          </div>
        </div>

        <!-- QC INPROGRESS -->
        <div class="col-xl-6 col-lg-6 col-md-12 col-sm-12 col-12">
          <div class="panel panel-inverse equal-panel">
            <div class="panel-heading">
              <font-awesome-icon
                icon="hourglass-start"
                class="icon-title text-warning"
              />
              <span class="ml-3">QC Inprogress</span>
            </div>

            <div class="v-table-wrapper">
              <table class="table mb-0 v-fixed-table">
                <thead>
                  <tr>
                    <th  class="sticky-head">Receipt</th>
                    <th>Date</th>
                    <th>Supplier</th>
                    <th>DN</th>
                    <th>Item</th>
                    <th>Qty(Unit)</th>
                    <th>Qty(Pack)</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in listPending" :key="item.ReceiptNo">
                    <td class="sticky-body">{{ item.ReceiptNo }}</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyPack) }}
                    </td>
                  </tr>
                </tbody>
              </table>

              <v-data-empty v-if="listPending.length === 0" />
            </div>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <!-- PASSED QC -->
        <div class="col-xl-6 col-lg-6 col-md-12 col-sm-12 col-12">
          <div class="panel panel-inverse equal-panel">
            <div class="panel-heading">
              <font-awesome-icon icon="check" class="icon-title text-success" />
              <span class="ml-3">Passed QC</span>
            </div>

            <div class="v-table-wrapper">
              <table class="table mb-0 v-fixed-table">
                <thead>
                  <tr>
                    <th class="sticky-head">Receipt</th>
                    <th>Date</th>
                    <th>Supplier</th>
                    <th>DN</th>
                    <th>Item</th>
                    <th>Qty(Unit)</th>
                    <th>Qty(Pack)</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in listPassed" :key="item.ReceiptNo">
                    <td class="sticky-body">{{ item.ReceiptNo }}</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyPack) }}
                    </td>
                  </tr>
                </tbody>
              </table>

              <v-data-empty v-if="listPassed.length === 0" />
            </div>
          </div>
        </div>

        <!-- NG QC -->
        <div class="col-xl-6 col-lg-6 col-md-12 col-sm-12 col-12">
          <div class="panel panel-inverse equal-panel">
            <div class="panel-heading">
              <font-awesome-icon icon="x" class="icon-title text-danger" />
              <span class="ml-3">NG QC</span>
            </div>

            <div class="v-table-wrapper">
              <table class="table mb-0 v-fixed-table">
                <thead>
                  <tr>
                    <th class="sticky-head">Receipt</th>
                    <th>Date</th>
                    <th>Supplier</th>
                    <th>DN</th>
                    <th>Item</th>
                    <th>Qty(Unit)</th>
                    <th>Qty(Pack)</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in listNG" :key="item.ReceiptNo">
                    <td class="sticky-body">{{ item.ReceiptNo }}</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.ReceiptQtyPack) }}
                    </td>
                  </tr>
                </tbody>
              </table>

              <v-data-empty v-if="listNG.length === 0" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data: () => ({
    summary: {
      total: 0,
      pending: 0,
      passed: 0,
      ng: 0,
    },
    isLoading: false,
    list: [],
    listPending: [],
    listPassed: [],
    listNG: [],
  }),
  computed: {
    ds: function () {
      return useReceiptAndon();
    },
  },
  mounted: async function () {
    setInterval(() => {
      this.load();
    }, 3000);
  },
  methods: {
    load: function () {
      if (this.isLoading) return;

      this.isLoading = true;

      this.ds
        .load()
        .then((dt) => {
          this.list = dt.data.Data;
          this.listPending = this.list.filter(
            (p) => p.StatusReceipt === "PENDING"
          );
          this.listPassed = this.list.filter((p) => p.StatusReceipt === "OK");
          this.listNG = this.list.filter((p) => p.StatusReceipt === "NG");

          this.summary.total = this.list.length;
          this.summary.pending = this.listPending.length;
          this.summary.passed = this.listPassed.length;
          this.summary.ng = this.listNG.length;
        })
        .finally(() => (this.isLoading = false));
    },
  },
};
</script>

<style scoped>
.header-summary-content {
  min-height: 5em;
  text-align: center;
}

.title-summary {
  color: white;
  font-size: 1.5em;
}

.qty-summary {
  color: white;
  font-size: 3em;
}

.icon-title {
  font-size: 1.5em;
  font-weight: bolder;
}
/* ==============================
   WRAPPER (JANGAN ADA TRANSFORM)
============================== */
.panel,
.panel-body,
.v-table-wrapper {
  transform: none !important;
}

/* ==============================
   TABLE WRAPPER
============================== */
.v-table-wrapper {
  max-height: 240px;
  overflow-x: auto;
  overflow-y: auto;
  position: relative;
}

/* ==============================
   TABLE BASE
============================== */
.v-fixed-table {
  border-collapse: separate;      /* WAJIB untuk sticky */
  border-spacing: 0;
  table-layout: fixed;            /* WAJIB */
  min-width: 650px;              /* PAKSA SCROLL KANAN */
  width: max-content;
  font-size: 0.9em;
  background: #fff;
}

/* ==============================
   CELL BASE
============================== */
.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 12px;
  border: 1px solid #dee2e6;
  background-clip: padding-box;
}

/* ==============================
   STICKY HEADER
============================== */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  background: lightblue;
  z-index: 50;
}

/* ==============================
   STICKY RECEIPT HEADER
============================== */
.v-fixed-table thead .sticky-head {
  position: sticky;
  left: 0;
  min-width: 160px;
  max-width: 160px;
  background: lightblue;
  z-index: 100;                   /* PALING ATAS */
  box-shadow: 2px 0 0 #aaa;
}

/* ==============================
   STICKY RECEIPT BODY
============================== */
.v-fixed-table tbody .sticky-body {
  position: sticky;
  left: 0;
  min-width: 160px;
  max-width: 160px;
  background: #fff;
  z-index: 20;
  box-shadow: 2px 0 0 #aaa;
}

/* ==============================
   OPTIONAL: ROW HOVER
============================== */
.v-fixed-table tbody tr:hover td {
  background: #f7fbff;
}

/* ==============================
   OPTIONAL: NG HIGHLIGHT
============================== */
.tr-ng td {
  background: #ffecec;
}

</style>
