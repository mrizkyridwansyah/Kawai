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
                  <tr v-for="(item, i) in list">
                    <td class="text-right">{{ i + 1 }}.</td>
                    <td>{{ $func.formatDate(item.Date) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td
                      :class="{
                        'text-success': item.Status === 'PASSED',
                        'text-danger': item.Status === 'FAILED',
                        'text-warning': item.Status === 'PENDING',
                      }"
                    >
                      {{ item.Status }}
                    </td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty
                class="mt-3"
                v-if="
                  !ds.isLoading &&
                  list.length == 0 &&
                  !ds.isNetworkError &&
                  !ds.isServerError
                "
              />
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
                    <td>{{ $func.formatDate(item.Date) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty
                class="mt-3"
                v-if="
                  !ds.isLoading &&
                  list.length == 0 &&
                  !ds.isNetworkError &&
                  !ds.isServerError
                "
              />
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
                    <td>{{ $func.formatDate(item.Date) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td
                      :class="{
                        'text-success': item.Status === 'PASSED',
                        'text-danger': item.Status === 'FAILED',
                        'text-warning': item.Status === 'PENDING',
                      }"
                    >
                      {{ item.Status }}
                    </td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty
                class="mt-3"
                v-if="
                  !ds.isLoading &&
                  list.length == 0 &&
                  !ds.isNetworkError &&
                  !ds.isServerError
                "
              />
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
                    <td>{{ $func.formatDate(item.Date) }}</td>
                    <td>{{ item.SupplierName }}</td>
                    <td>{{ item.DNNumber }}</td>
                    <td>{{ item.ItemName }}</td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty
                class="mt-3"
                v-if="
                  !ds.isLoading &&
                  list.length == 0 &&
                  !ds.isNetworkError &&
                  !ds.isServerError
                "
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
  data: () => ({
    summary: {
      total: 0,
      pending: 0,
      passed: 0,
      ng: 0,
    },
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
    this.load();

    const nuxtApp = useNuxtApp();
    const signalr = await nuxtApp.$createSignalR("/notifapprovalhub");

    if (!signalr) {
      console.warn("SignalR connection not found!");
      return;
    }

    // Register event handler
    signalr.on("AndonReceivingNotification", (data) => {
      this.hasNewNotification = true;
      this.load();
    });
  },
  methods: {
    load: function () {
      this.ds.load().then((dt) => {
        this.list = dt.Data.Items;
        this.listPending = dt.Data.Items.filter((p) => p.Status === "PENDING");
        this.listPassed = dt.Data.Items.filter((p) => p.Status === "PASSED");
        this.listNG = dt.Data.Items.filter((p) => p.Status === "NG");

        this.summary.total = this.list.length;
        this.summary.pending = this.listPending.length;
        this.summary.passed = this.listPassed.length;
        this.summary.ng = this.listNG.length;
      });
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
