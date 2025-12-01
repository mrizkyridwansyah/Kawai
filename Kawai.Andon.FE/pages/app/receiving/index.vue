<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="chart-column" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Receiving Andon (Temporary Area)
      </span>
    </div>
    <div class="panel-body">
      <div class="row">
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #8d56a9"
          >
            <span class="title-summary">Total Receipt (In Complete)</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.total)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #888"
          >
            <span class="title-summary">Pending QC</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.pending)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #49a0bc"
          >
            <span class="title-summary">Passed QC</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.passed)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #51ac83"
          >
            <span class="title-summary">NG QC</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.ng)
            }}</span>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12">
          <div class="panel panel-inverse">
            <div class="panel-heading ui-sortable-handle">
              <font-awesome-icon
                icon="pencil"
                class="text-primary icon-title"
              />
              <span style="font-size: 1.05em" class="ml-3">Total Receipt</span>
            </div>
            <div class="panel-body">
              <div class="v-table-wrapper">
                <table
                  class="table table-striped mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr>
                      <th class="text-center">No.</th>
                      <th class="text-center">Date</th>
                      <th class="text-center">Supplier</th>
                      <th class="text-center">DN</th>
                      <th class="text-center">Item</th>
                      <th class="text-center">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in list">
                      <td class="text-right">{{ i + 1 }}.</td>
                      <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                      <td>{{ item.SupplierName }}</td>
                      <td>{{ item.DNNumber }}</td>
                      <td>{{ item.ItemName }}</td>
                      <td
                        :class="{
                          'text-success': item.StatusReceipt === 'OK',
                          'text-danger': item.StatusReceipt === 'NG',
                          'text-warning': item.StatusReceipt === 'PENDING',
                          'text-primary': item.StatusReceipt === 'NEW',
                        }"
                      >
                        {{ item.StatusReceipt }}
                      </td>
                    </tr>
                  </tbody>
                </table>
                <v-data-empty class="mt-3" v-if="list.length == 0" />
              </div>
            </div>
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12">
          <div class="panel panel-inverse">
            <div class="panel-heading ui-sortable-handle">
              <font-awesome-icon
                class="text-warning icon-title"
                icon="hourglass-start"
              />
              <span style="font-size: 1.05em" class="ml-3">Pending QC</span>
            </div>
            <div class="panel-body">
              <table class="table table-striped mb-0 align-middle w-100">
                <thead>
                  <tr>
                    <th class="text-center">No.</th>
                    <th class="text-center">Date</th>
                    <th class="text-center">Supplier</th>
                    <th class="text-center">DN</th>
                    <th class="text-center">Item</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, i) in listPending">
                    <td class="text-right">{{ i + 1 }}.</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty class="mt-3" v-if="listPending.length == 0" />
            </div>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12">
          <div class="panel panel-inverse">
            <div class="panel-heading ui-sortable-handle">
              <font-awesome-icon icon="check" class="text-success icon-title" />
              <span style="font-size: 1.05em" class="ml-3">Passed QC</span>
            </div>
            <div class="panel-body">
              <table class="table table-striped mb-0 align-middle w-100">
                <thead>
                  <tr>
                    <th class="text-center">No.</th>
                    <th class="text-center">Date</th>
                    <th class="text-center">Supplier</th>
                    <th class="text-center">DN</th>
                    <th class="text-center">Item</th>
                    <th class="text-center">Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, i) in listPassed">
                    <td class="text-right">{{ i + 1 }}.</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td
                      :class="{
                        'text-success': item.StatusReceipt === 'OK',
                        'text-danger': item.StatusReceipt === 'NG',
                        'text-warning': item.StatusReceipt === 'PENDING',
                        'text-primary': item.StatusReceipt === 'NEW',
                      }"
                    >
                      {{ item.StatusReceipt }}
                    </td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty class="mt-3" v-if="listPassed.length == 0" />
            </div>
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12">
          <div class="panel panel-inverse">
            <div class="panel-heading ui-sortable-handle">
              <font-awesome-icon icon="x" class="text-danger icon-title" />
              <span style="font-size: 1.05em" class="ml-3">NG QC</span>
            </div>
            <div class="panel-body">
              <table class="table table-striped mb-0 align-middle w-100">
                <thead>
                  <tr>
                    <th class="text-center">No.</th>
                    <th class="text-center">Date</th>
                    <th class="text-center">Supplier</th>
                    <th class="text-center">DN</th>
                    <th class="text-center">Item</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, i) in listNG">
                    <td class="text-right">{{ i + 1 }}.</td>
                    <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty class="mt-3" v-if="listNG.length == 0" />
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
      if (this.isLoading) {
        console.log("masih loading bro!");
        return;
      }

      this.isLoading = true;

      this.ds
        .load()
        .then((dt) => {
          console.log(dt);
          this.list = dt.data.Data;
          this.listPending = this.list.filter(
            (p) => p.StatusReceipt === "NEW" || p.StatusReceipt === "PENDING"
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
</style>

<style>
.v-table-wrapper {
  overflow: auto;
  max-height: 400px;
  /* border: 1px solid #ddd; */
  position: relative;
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
  background: #f8f9fa;
}

/* Sticky Columns (kiri) */
.sticky-left {
  position: sticky;
  background: white !important;
  background-color: white;
  z-index: 10;
  /* left akan diset via JS */
}

/* Kalau sticky kiri di header, beri z-index lebih tinggi */
thead .sticky-left {
  z-index: 30;
}
</style>
