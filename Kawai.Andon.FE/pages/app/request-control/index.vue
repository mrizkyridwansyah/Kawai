<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="file-lines" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Womin Request Control
      </span>
    </div>
    <div class="panel-body">
      <div class="row">
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #8d56a9"
          >
            <span class="title-summary">Total Request</span>
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
            <span class="title-summary">Womin</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.womin)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #49a0bc"
          >
            <span class="title-summary">Item</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.item)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #49a0bc"
          >
            <span class="title-summary">Remaining Item</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.remaining)
            }}</span>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
          <div class="panel panel-inverse">
            <div class="panel-heading ui-sortable-handle">
              <font-awesome-icon
                icon="hourglass-start"
                class="text-warning icon-title"
              />
              <span style="font-size: 1.05em" class="ml-3">Remaining Item</span>
            </div>
            <div class="panel-body">
              <table class="table table-striped mb-0 align-middle w-100">
                <thead>
                  <tr>
                    <th class="text-center">Warehouse</th>
                    <th class="text-center">PIC</th>
                    <th class="text-center">Request No</th>
                    <th class="text-center">Request Date</th>
                    <th class="text-center">Production Date</th>
                    <th class="text-center">Womin</th>
                    <th class="text-center">To Line</th>
                    <th class="text-center">Item</th>
                    <th class="text-center">Qty</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, i) in listRemaining">
                    <td>{{ item.WarehouseName }}.</td>
                    <td>{{ item.PIC }}.</td>
                    <td>{{ item.RequestNo }}.</td>
                    <td>{{ $func.formatDate(item.RequestDate) }}</td>
                    <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                    <td>{{ item.Womin }}</td>
                    <td>{{ item.ToLineName }}</td>
                    <td>{{ item.ItemName }}</td>
                    <td>{{ $func.formatMoney(item.Qty) }}</td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty
                class="mt-3"
                v-if="
                  !ds.isLoading &&
                  listRemaining.length == 0 &&
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
      womin: 0,
      item: 0,
      remaining: 0,
    },
    listRemaining: [],
  }),
  computed: {
    ds: function () {
      return useWominRequest();
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
      this.ds.load().then((dt) => (this.listRemaining = dt.Data.Items));
      this.ds.loadSummary().then((dt) => {
        this.summary.total = dt.Data.Total;
        this.summary.womin = dt.Data.Womin;
        this.summary.item = dt.Data.Item;
        this.summary.remaining = dt.Data.Remaining;
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
