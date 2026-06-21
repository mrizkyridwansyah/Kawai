<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="file-lines" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Womin Request Control
      </span>
    </div>
    <!--buatkan div untuk filter area-->
    <div
      class="filter-area p-3"
      id="containerfilter"
      v-show="showFilter"
      @click.stop
    >
      <!--dropdown area-->
      <!--dropdown area-->
      <div class="row align-items-center mt-1">
        <!-- Line Filter -->
        <div class="col-12 col-md-5 col-lg-5 d-flex align-items-center">
          <label class="form-label mb-0 me-2" style="min-width: 40px"
            >Line</label
          >
          <div class="flex-grow-1">
            <filter-line-factory
              class="form-control w-100"
              company="ALL"
              manufacture="ALL"
              v-model="filter.line"
              :show-option-all="true"
              default-option-all="ALL"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </div>
        </div>

        <!-- Area Filter -->
        <div class="col-12 col-md-5 col-lg-5 d-flex align-items-center">
          <label class="form-label mb-0 me-2" style="min-width: 40px"
            >Area</label
          >
          <div class="flex-grow-1">
            <filter-area
              class="form-control w-100"
              v-model="filter.area"
              v-model:area-name="filter.areaName"
              warehouse=""
            />
          </div>
        </div>

        <!-- Search Button -->
        <div class="col-12 col-md-2 col-lg-auto d-flex align-items-center">
          <v-button-search :search="search" />
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
              <div class="table-responsive">
                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr>
                      <th class="text-center">Location</th>
                      <th class="text-center">Request No & Item</th>
                      <th class="text-center">Production Date</th>
                      <th class="text-center">Model</th>
                      <th class="text-center">Preparation Status</th>
                      <th class="text-center">Trolly Number</th>
                      <th class="text-center">Current Position</th>
                      <th class="text-center">Next Location</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in paginatedData">
                      <td>{{ item.Line }}<br />{{ item.WorkStation }}</td>
                      <td>{{ item.RequestNo }}<br />{{ item.PickingArea }}</td>
                      <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                      <td>{{ item.Model }}</td>
                      <td>
                        <span class="badge-danger">{{
                          item.PreparationStatus
                        }}</span>
                      </td>
                      <td>{{ item.TrollyNumber }}</td>
                      <td>{{ item.CurrentPosition }}</td>
                      <td>{{ item.NextLocation }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <v-data-empty class="mt-3" v-if="ds.data.Length == 0" />

              <!-- Pagination Footer -->
              <div
                v-if="totalPages > 0"
                class="d-flex justify-content-between align-items-center mt-3 p-2 px-3"
                style="
                  background: #15171c;
                  border-radius: 4px;
                  border: 1px solid #2d323f;
                "
              >
                <div class="fw-bold text-white d-flex align-items-center">
                  <span class="me-3"
                    >PAGE {{ currentPage }} / {{ totalPages }}</span
                  >
                  <span
                    class="badge bg-dark text-light border border-secondary me-3"
                    style="font-size: 0.85em; font-weight: normal"
                  >
                    Showing {{ startItem }} - {{ endItem }} of
                    {{ totalItems }} items
                  </span>
                  <div
                    class="d-flex align-items-center"
                    style="font-size: 0.85em; font-weight: normal"
                  >
                    <span class="text-white me-2">Show:</span>
                    <select
                      v-model.number="pageSize"
                      class="form-select form-select-sm bg-dark text-white border-secondary"
                      style="
                        width: auto;
                        padding: 2px 24px 2px 8px;
                        font-size: 1em;
                      "
                    >
                      <option :value="5">5</option>
                      <option :value="8">8</option>
                      <option :value="10">10</option>
                      <option :value="25">25</option>
                      <option :value="50">50</option>
                    </select>
                  </div>
                </div>
                <div
                  v-if="totalPages > 1"
                  class="d-flex flex-column align-items-end"
                  style="width: 200px"
                >
                  <div
                    class="text-white fw-bold mb-1"
                    style="font-size: 0.85em"
                  >
                    AUTO-NEXT IN {{ timeLeft }}s
                  </div>
                  <div
                    class="progress w-100"
                    style="height: 5px; background-color: #555"
                  >
                    <div
                      class="progress-bar bg-light"
                      role="progressbar"
                      :style="{ width: (timeLeft / 10) * 100 + '%' }"
                    ></div>
                  </div>
                </div>
              </div>
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
    isLoading: false,
    intervalLoad: null,
    showFilter: true,
    currentPage: 1,
    pageSize: 8,
    timeLeft: 10,
    progressInterval: null,
    summary: {
      total: 0,
      totalItem: 0,
      womin: 0,
      item: 0,
      remaining: 0,
    },
    listRemaining: [],
    filter: {
      // warehouse: null,
      area: null,
      areaName: "",
      line: null,
      lineName: "",
    },
  }),
  computed: {
    ds: function () {
      return useWominRequest();
    },
    totalPages: function () {
      const size = parseInt(this.pageSize, 10) || 8;
      return this.ds.data && Array.isArray(this.ds.data)
        ? Math.ceil(this.ds.data.length / size)
        : 0;
    },
    paginatedData: function () {
      if (!this.ds.data || !Array.isArray(this.ds.data)) return [];
      const size = parseInt(this.pageSize, 10) || 8;
      const start = (this.currentPage - 1) * size;
      const end = start + size;
      return this.ds.data.slice(start, end);
    },
    totalItems: function () {
      return this.ds.data && Array.isArray(this.ds.data)
        ? this.ds.data.length
        : 0;
    },
    startItem: function () {
      const size = parseInt(this.pageSize, 10) || 8;
      return this.totalItems === 0 ? 0 : (this.currentPage - 1) * size + 1;
    },
    endItem: function () {
      const size = parseInt(this.pageSize, 10) || 8;
      return Math.min(this.currentPage * size, this.totalItems);
    },
  },
  watch: {
    pageSize: function () {
      this.currentPage = 1;
      this.timeLeft = 10;
    },
    "ds.data": {
      handler: function () {
        if (this.totalPages > 0 && this.currentPage > this.totalPages) {
          this.currentPage = this.totalPages;
        }
      },
      deep: true,
    },
    "filter.area": function () {
      this.filter.area;
    },
  },
  mounted: async function () {
    document.addEventListener("click", this.handleGlobalClick);
  },
  beforeUnmount: function () {
    // Wajib: bersihkan interval dan listener jika pindah halaman (mencegah memory leak)
    this.stopInterval();
    this.stopProgress();
    document.removeEventListener("click", this.handleGlobalClick);
  },
  methods: {
    startInterval: function () {
      this.stopInterval(); // Pastikan tidak ada interval ganda
      this.intervalLoad = setInterval(() => {
        this.search(true); // true = pencarian dipicu otomatis oleh interval
      }, 3000);
    },
    stopInterval: function () {
      if (this.intervalLoad) {
        clearInterval(this.intervalLoad);
        this.intervalLoad = null;
      }
    },
    nextPage: function () {
      if (this.totalPages <= 1) return;
      this.currentPage++;
      if (this.currentPage > this.totalPages) {
        this.currentPage = 1;
      }
      this.timeLeft = 10;
    },
    startProgress: function () {
      this.stopProgress();
      this.timeLeft = 10;
      this.progressInterval = setInterval(() => {
        if (!this.showFilter && this.totalPages > 1) {
          this.timeLeft--;
          if (this.timeLeft <= 0) {
            this.nextPage();
          }
        }
      }, 1000);
    },
    stopProgress: function () {
      if (this.progressInterval) {
        clearInterval(this.progressInterval);
        this.progressInterval = null;
      }
    },
    handleGlobalClick: function (event) {
      // Jika filter sedang disembunyikan (interval nyala), dan user klik sembarang tempat,
      // maka munculkan filter dan matikan interval
      if (!this.showFilter) {
        this.showFilter = true;
        this.stopInterval();
        this.stopProgress();

        // Kembalikan teks judul panel seperti semula
        const headerPanel = document.getElementById("header-panel");
        if (headerPanel) {
          headerPanel.innerText = "Remaining Item - Material Type (Group)";
        }
      }
    },
    search: function (isAuto = false) {
      const isAutoRefresh = isAuto === true;

      if (this.isLoading) {
        console.log("masih loading bro!");
        return;
      }

      this.isLoading = true;

      this.ds
        .load(this.filter.line, this.filter.area)
        .then((dt) => {
          this.list = dt.Data;
          //total diambil dari countdata datalist
          this.summary.total = this.list.length;
          this.summary.totalItem =
            this.list.length > 0 ? this.list[0].PickingProgress : 0;
          this.summary.remaining =
            this.list.length > 0 ? this.list[0].Remaining : 0;
          //untuk womin summary semua workstasion yang sama aja
          this.summary.womin = this.list.length > 0 ? this.list[0].Womin : 0;
        })
        .catch((err) => {
          console.error("Error loading data:", err);
        })
        .finally(() => (this.isLoading = false));

      if (this.filter.area != null && !isAutoRefresh) {
        // Sembunyikan filter langsung saat tombol search diklik
        this.showFilter = false;
        this.startInterval(); // Nyalakan interval tiap 3 detik
        this.startProgress(); // Nyalakan interval progres halaman

        // Ubah teks judul panel sesuai area yang difilter
        const headerPanel = document.getElementById("header-panel");
        if (headerPanel) {
          headerPanel.innerText = `Remaining Item - Material Type (${this.filter.areaName})`;
        }
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
