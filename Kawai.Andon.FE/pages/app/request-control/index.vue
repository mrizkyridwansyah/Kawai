<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="file-lines" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Womin Request Control
      </span>
    </div>
    <!--buatkan div untuk filter area-->
    <div class="filter-area p-3" id="containerfilter">
      <!--dropdown area-->
      <div class="row">
        <div class="row mt-1">
          <label
            class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
            >Area</label
          >

          <div class="col-sm-6">
            <filter-area
              class="form-control"
              v-model="filter.area"
              v-model:area-name="filter.areaName"
              warehouse=""
            ></filter-area>
          </div>
          <div
            style="margin-top: -5px"
            class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2"
          >
            <v-button-search class="ms-1" :search="search" />
          </div>
        </div>
      </div>
    </div>

    <div class="panel-body" id="panelbody">
      <div class="row">
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #8d56a9"
          >
            <span class="title-summary">Total Request </span>
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
            style="background-color: #18b2e0"
          >
            <span class="title-summary">Picking Progress (Item)</span>
            <br />
            <span class="qty-summary">{{ this.summary.totalItem }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #18b2e0"
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

              <span id="header-panel" style="font-size: 1.05em" class="ml-3"
                >Remaining Item - Material Type (Group)</span
              >
            </div>
            <div class="panel-body">
              <table
                class="table mb-0 align-middle w-100 v-fixed-table"
                ref="table"
              >
                <thead>
                  <tr>
                    <th class="text-center">Request No</th>
                    <th class="text-center">Production Date</th>
                    <th class="text-center">Line</th>
                    <th class="text-center">Item</th>
                    <th class="text-center">Model</th>
                    <th class="text-center">Work Station</th>
                    <th class="text-center">Preparation Status</th>
                    <th class="text-center">Trolly Number</th>
                    <th class="text-center">Current Position</th>
                    <th class="text-center">Next Location</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, i) in ds.data">
                    <td>{{ item.RequestNo }}</td>
                    <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                    <td>{{ item.Line }}</td>
                    <td>{{ item.PickingArea }}</td>
                    <td>{{ item.Model }}</td>
                    <td>{{ item.WorkStation }}</td>
                    <td>{{ item.PreparationStatus }}</td>
                    <td>{{ item.TrollyNumber }}</td>
                    <td>{{ item.CurrentPosition }}</td>
                    <td>{{ item.NextLocation }}</td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty class="mt-3" v-if="ds.data.Length == 0" />
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
    filter: {
      // warehouse: null,
      area: null,
      areaName: "",
    },
  }),
  computed: {
    ds: function () {
      return useWominRequest();
    },
  },
  watch: {
    // "filter.warehouse": function () {
    //   this.ds.data.Items = [];
    // },
    "filter.area": function () {
      this.filter.area;
    },
  },
  mounted: async function () {
    this.search();
    // const nuxtApp = useNuxtApp();
    // const signalr = await nuxtApp.$createSignalR("/notifapprovalhub");

    // if (!signalr) {
    //   console.warn("SignalR connection not found!");
    //   return;
    // }

    // // Register event handler
    // signalr.on("AndonReceivingNotification", (data) => {
    //   this.hasNewNotification = true;
    //   this.search();
    // });
  },
  methods: {
    search: function () {
      let filters = [
        {
          AreaCode: this.filter.area || "",
        },
      ];
      this.ds.load(this.filter.area);
      this.ds
        .load(this.filter.area)
        .then((dt) => {
          debugger;
          this.list = dt.Data;
          //total diambil dari countdata datalist
          this.summary.total = this.list.length;
          this.summary.totalItem =
            this.list.length > 0 ? this.list[0].PickingProgress : 0;
          this.summary.remaining =
            this.list.length > 0 ? this.list[0].Remaining : 0;

          //untuk womin summary semua workstasion yang sama aja
          this.summary.womin = this.list.length > 0 ? this.list[0].Womin : 0;
          //   index === self.findIndex((t) => (
          //     t.WorkStation === item.WorkStation
          //   ))
          // ).length;
        })
        .catch((err) => {
          console.error("Error loading data:", err);
        });

      if (this.filter.area != null) {
        //header-panel change text to "Remaining Item" dan nama area bukan codenya
        const headerPanel = document.getElementById("header-panel");
        if (headerPanel) {
          headerPanel.innerText = `Remaining Item - Material Type (${this.filter.areaName})`;
        }

        //hide filter id="containerfilter" after search
        const containerFilter = document.getElementById("containerfilter");
        const panelBody = document.getElementById("panelbody");
        if (containerFilter && panelBody) {

          /* HIDE TOP NAVBAR
          const header = document.getElementById("header");
          const appContent = document.getElementById("app");
          header.style.display = "none";
          appContent.style.paddingTop = "0";
          */

          containerFilter.style.display = "none";
          panelBody.style.marginTop = "0px";
        }

        //jika saya klik sembarang tombo maka filter muncul lagi
        document.addEventListener("click", (event) => {
          const isClickInside = containerFilter.contains(event.target);
          if (!isClickInside) {
            containerFilter.style.display = "block";
            panelBody.style.marginTop = "20px";
          }
        });
      }
    },
    reset: function () {
      this.filter.area = null;
      this.search();
    },

    //load: function () {
    //   this.ds.load().then((dt) => (this.listRemaining = dt.Data));
    // this.ds.loadSummary().then((dt) => {
    //   this.summary.total = dt.Data.Total;
    //   this.summary.womin = dt.Data.Womin;
    //   this.summary.item = dt.Data.Item;
    //   this.summary.remaining = dt.Data.Remaining;
    // });
    //},
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
  font-size: 1.4em;
}

.qty-summary {
  color: white;
  font-size: 3em;
  font-weight: bold;
}

.icon-title {
  font-size: 1.5em;
  font-weight: bolder;
}

/* Bikin table bisa scroll horizontal juga */
.v-fixed-table {
  width: max-content; /* agar scroll horizontal muncul */
  min-width: 100%;
  border: 1px solid gainsboro !important;
  /* border-collapse: separate; */
  /* border-spacing: 0; */
}

.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  border: 1px solid #dee2e6;
  background: #fff;
}

/* Sticky Header (atas) */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20; /* harus lebih tinggi dari sticky kiri */
  background: lightblue !important;
}

.center-filter {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
