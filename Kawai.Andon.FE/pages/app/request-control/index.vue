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
              company="11111"
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
            >Parts Group</label
          >
          <div class="flex-grow-1">
            <filter-area
              class="form-control w-100"
              v-model="filter.area"
                 :show-option-all="true"
              default-option-all="ALL"
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
              <div class="v-table-wrapper" @scroll="onScroll($event, 'main')">
                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr>
                      <th class="text-center">Line</th>
                      <th class="text-center">Production Date</th>
                      <th class="text-center">Request No</th>
                      <th class="text-center">Parts Group</th>
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
                      <td>
                         <a
                          href="javascript:void(0)"
                          @click="viewWominByLine(item.LineCode,item.ProductionDate)"
                          >  {{ item.Line }}</a>
                         </td>
                      <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                       <td>
                          <a
                          href="javascript:void(0)"
                          @click="viewWomin(item.RequestNo,item.GroupingPart)"
                          > {{item.RequestNo }}</a>
                       </td>
                      <td>{{ item.GroupingPart }}</td>
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
                <v-data-empty class="mt-3" v-if="!ds.isLoading && ds.data.Length == 0" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
   <v-modal title="Detail Status Womin" class="modal-lg" id="modal-list-detailwomin">
    <shared-request-womin-list
      :refno="this.selectedRefNo"
      :groupclass="this.selectedGroupClass"
      :counter="this.counter"
    />
   </v-modal>

   <v-modal title="Detail Status Womin By Line" class="modal-lg" id="modal-list-detailwominbyline">
    <shared-request-wominbyline-list
      :linecode="this.selectedLineCode"
      :scheduledate="this.selectedScheduleDate"
      :counter="this.counter"
    />
   </v-modal>
</template>

<script>
export default {
  data: () => ({
    isLoading: false,
    intervalLoad: null,
    showFilter: true,
    selectedRefNo: null,
    selectedGroupClass: null,
    selectedLineCode: null,
    selectedScheduleDate: null,
    list: [],
     counter: 0,
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

     renderLimits: {
      main: 50 
      
    },
  }),
  computed: {
    ds: function () {
      return useWominRequest();
    },
  },
  watch: {
 
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
    document.removeEventListener("click", this.handleGlobalClick);
  },
  methods: {
        onScroll: function (e, type) {
      const { scrollTop, scrollHeight, clientHeight } = e.target;
      // Jika scroll sudah mendekati bawah (sisa 50px jarak dari bawah)
      if (scrollTop + clientHeight >= scrollHeight - 50) {
        // Cek batasan max item dari masing-masing array
        let maxLen = 0;
        if (type === "main") maxLen = this.ds.data.length;
        
        else if (type === "ng") maxLen = this.ds.data.length;

        if (this.renderLimits[type] < maxLen) {
          this.renderLimits[type] += 50;
        }
      }
    },
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
    handleGlobalClick: function (event) {
      // Jika filter sedang disembunyikan (interval nyala), dan user klik sembarang tempat,
      // maka munculkan filter dan matikan interval
      if (!this.showFilter) {
        this.showFilter = true;
        this.stopInterval();

        // Kembalikan teks judul panel seperti semula
        const headerPanel = document.getElementById("header-panel");
        if (headerPanel) {
          headerPanel.innerText = "Remaining Item - Material Type (Group)";
        }
      }
    },
    search: function () {
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

      if (this.filter.area != null) {
        // Gunakan setTimeout kecil untuk mencegah bentrok dengan handleGlobalClick
        setTimeout(() => {
          this.showFilter = false; // Sembunyikan area filter
          this.startInterval(); // Nyalakan interval tiap 3 detik

          // Ubah teks judul panel sesuai area yang difilter
          const headerPanel = document.getElementById("header-panel");
          if (headerPanel) {
            headerPanel.innerText = `Remaining Item - Material Type (${this.filter.areaName})`;
          }
        }, 100);
      }
    },
    reset: function () {
      this.filter.area = null;
      this.search();
    },

    viewWomin: function (refno , groupclass) {
      debugger;
      this.selectedRefNo = refno;
      this.selectedGroupClass = groupclass;
      this.counter++;
      this.$bvModal.show("modal-list-detailwomin");
    },

     viewWominByLine: function (linecode , scheduledate) {
      debugger;
      this.selectedLineCode = linecode;
      this.selectedScheduleDate = scheduledate;
      this.counter++;
      this.$bvModal.show("modal-list-detailwominbyline");
    },

 
  },
};
</script>

<style scoped>
.header-summary-content {
  min-height: 5em;
  text-align: center;
}

.v-table-wrapper {
  overflow: auto;
  max-height: 500px;
  /* border: 1px solid #ddd; */
  position: relative;
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
