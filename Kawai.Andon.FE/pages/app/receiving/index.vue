<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="chart-column" style="font-size: 1.25em" />
       
      <span  id="header-panel"  style="font-size: 1.25em" class="ml-3">
        Receiving Andon (Temporary Area)
      </span>
    </div>

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
            >Supplier</label
          >
          <div class="flex-grow-1">
            <filter-supplier
              class="form-control w-100"
              v-model="filter.supplier"
                 :show-option-all="true"
              default-option-all="ALL"
              v-model:supplier-name="filter.SupplierName"
              
            />
            
          </div>
        </div>

        

        <!-- Search Button -->
        <div class="col-12 col-md-2 col-lg-auto d-flex align-items-center">
          <v-button-search :search="search" />
        </div>
      </div>
    </div>

    <div class="panel-body">
      <div class="row">
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #8d56a9"
          >
            <span class="title-summary"
              >Total Receipt (Unprocessed to storage)</span
            >
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
            <span class="title-summary">QC Inprogress</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.pending)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #18b2e0"
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
            style="background-color: #e60808"
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
                icon="file-pen"
                class="text-white icon-title"
              />
              <span style="font-size: 1.05em" class="ml-3"
                >Pending Receipt Check</span
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
                      <!-- <th class="text-center">No</th> -->
                      <th style="width: 10px !important">Date</th>
                      <th class="text-center">Supplier</th>
                      <th class="text-center">DN</th>
                      <th class="text-center">Item</th>
                      <th class="text-center">Qty(Unit)</th>
                      <th class="text-center">Qty(Pack)</th>
                      <th class="text-center">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in displayList">
                      <!-- <td class="text-right">{{ i + 1 }}.</td>-->
                      <!-- <td class="text-center">{{ item.ReceiptNo }}</td> -->
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
                      <td
                        :class="{
                          'text-success':
                            item.StatusReceiptName === 'Passed QC',
                          'text-danger': item.StatusReceiptName === 'NG QC',
                          'text-warning':
                            item.StatusReceiptName === 'QC Inprogres',
                          'text-primary': item.StatusReceiptName === 'New',
                        }"
                      >
                        {{ item.StatusReceiptName }}
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
              <span style="font-size: 1.05em" class="ml-3">QC Inprogress</span>
            </div>
            <div class="panel-body">
              <div
                class="v-table-wrapper"
                @scroll="onScroll($event, 'pending')"
              >
                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr>
                      <!-- <th class="text-center">No</th> -->
                      <th class="text-center" style="width: fit-content">
                        Date
                      </th>
                      <th class="text-center">Supplier</th>
                      <th class="text-center">DN</th>
                      <th class="text-center">Item</th>
                      <th class="text-center">Qty(Unit)</th>
                      <th class="text-center">Qty(Pack)</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in displayListPending">
                      <!-- <td class="text-center">{{ item.ReceiptNo }}</td> -->
                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B',
                        }"
                      >
                        {{ $func.formatDate(item.ReceiptDate) }}
                      </td>
                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B',
                        }"
                      >
                        {{ item.SupplierName }}
                      </td>
                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B',
                        }"
                      >
                        {{ item.DNNumber }}
                      </td>
                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B',
                        }"
                      >
                        {{ item.ItemName }}
                      </td>
                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B',
                        }"
                        class="text-right"
                      >
                        {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                      </td>
                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B',
                        }"
                        class="text-right"
                      >
                        {{ $func.formatMoney(item.ReceiptQtyPack) }}
                      </td>
                    </tr>
                  </tbody>
                </table>
                <v-data-empty class="mt-3" v-if="listPending.length == 0" />
              </div>
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
              <div class="v-table-wrapper" @scroll="onScroll($event, 'passed')">
                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr>
                      <!-- <th class="text-center">No</th> -->
                      <th class="text-center">Date</th>
                      <th class="text-center">Supplier</th>
                      <th class="text-center">DN</th>
                      <th class="text-center">Item</th>
                      <th class="text-center">Qty(Unit)</th>
                      <th class="text-center">Qty(Pack)</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in displayListPassed">
                      <!-- <td class="text-center">{{ item.ReceiptNo }}</td> -->
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
                <v-data-empty class="mt-3" v-if="listPassed.length == 0" />
              </div>
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
              <div class="v-table-wrapper" @scroll="onScroll($event, 'ng')">
                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr class="datatable-color">
                      <!-- <th class="text-center">No</th> -->
                      <th class="text-center">Date</th>
                      <th class="text-center">Supplier</th>
                      <th class="text-center">DN</th>
                      <th class="text-center">Item</th>
                      <th class="text-center">Qty(Unit)</th>
                      <th class="text-center">Qty(Pack)</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, i) in displayListNG">
                      <!-- <td class="text-center">{{ item.ReceiptNo }}</td> -->
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
                <v-data-empty class="mt-3" v-if="listNG.length == 0" />
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
    summary: {
      total: 0,
      pending: 0,
      passed: 0,
      ng: 0,
    },
     intervalLoad: null,
     showFilter: true,
    filter:{
      supplier: "",
       SupplierName: ""
    },
    isLoading: false,
    list: [],
    pollingId: null,
    renderLimits: {
      main: 50,
      pending: 50,
      passed: 50,
      ng: 50,
    },
  }),
  computed: {
    ds: function () {
      return useReceiptAndon();
    },
    listPending() {
      return this.list.filter((p) => p.StatusReceipt === "PENDING");
    },
    listPassed() {
      return this.list.filter((p) => p.StatusReceipt === "OK");
    },
    listNG() {
      return this.list.filter((p) => p.StatusReceipt === "NG");
    },
    displayList() {
      return this.list.slice(0, this.renderLimits.main);
    },
    displayListPending() {
      return this.listPending.slice(0, this.renderLimits.pending);
    },
    displayListPassed() {
      return this.listPassed.slice(0, this.renderLimits.passed);
    },
    displayListNG() {
      return this.listNG.slice(0, this.renderLimits.ng);
    },
  },
  mounted: async function () {
     document.addEventListener("click", this.handleGlobalClick);
    // Eksekusi tarikan data pertama kali saat halaman dibuka
     
  },
 
  beforeUnmount: function () {
    // Wajib: bersihkan interval dan listener jika pindah halaman (mencegah memory leak)
     this.stopInterval();
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
    handleGlobalClick: function (event) {
      // Jika filter sedang disembunyikan (interval nyala), dan user klik sembarang tempat,
      // maka munculkan filter dan matikan interval
      if (!this.showFilter) {
        this.showFilter = true;
         this.stopInterval();
        
          // Kembalikan teks judul panel seperti semula
        const headerPanel = document.getElementById("header-panel");
        if (headerPanel) {
          headerPanel.innerText = "Receiving Andon (Temporary Area)";
        }

      }
    },

    onScroll: function (e, type) {
      const { scrollTop, scrollHeight, clientHeight } = e.target;
      // Jika scroll sudah mendekati bawah (sisa 50px jarak dari bawah)
      if (scrollTop + clientHeight >= scrollHeight - 50) {
        // Cek batasan max item dari masing-masing array
        let maxLen = 0;
        if (type === "main") maxLen = this.list.length;
        else if (type === "pending") maxLen = this.listPending.length;
        else if (type === "passed") maxLen = this.listPassed.length;
        else if (type === "ng") maxLen = this.listNG.length;

        if (this.renderLimits[type] < maxLen) {
          this.renderLimits[type] += 50;
        }
      }
    },

   search: function () {
      if (this.isLoading) {
        console.log("masih loading bro!");
        return;
      }
         debugger;
         this.isLoading = true;

         this.ds
        .loadbysupplier(this.filter.supplier)
        .then((dt) => {
          let rawData = dt.data.Data || [];

          // Paksa pengurutan (sorting) di Frontend agar posisinya stabil setiap ditarik
          rawData.sort((a, b) => {
            let dateA = new Date(a.ReceiptDate).getTime();
            let dateB = new Date(b.ReceiptDate).getTime();
            
            // Urutkan berdasarkan Tanggal (Terlama di atas / ASC)
            if (dateA !== dateB) return dateA - dateB;
            
            // Jika tanggal sama persis, urutkan berdasarkan DNNumber ASC agar posisinya terkunci mati
            let dnA = a.DNNumber || "";
            let dnB = b.DNNumber || "";
            return dnA.localeCompare(dnB);
          });

          // Freeze array agar tidak dibuatkan reactivity proxy yang berat
          this.list = Object.freeze(rawData);

          // Hitung summary langsung dari computed properties
          this.summary.total = this.list.length;
          this.summary.pending = this.listPending.length;
          this.summary.passed = this.listPassed.length;
          this.summary.ng = this.listNG.length;
        })
        .catch((err) => console.error(err))
        .finally(() => (this.isLoading = false));
      if (this.filter.supplier != null) {

        // Ubah teks judul panel sesuai area yang difilter
          const headerPanel = document.getElementById("header-panel");
          if (headerPanel) {
            headerPanel.innerText = `Receiving Andon (Temporary Area) - Supplier : ${this.filter.SupplierName} `;
          }

        // Gunakan setTimeout kecil untuk mencegah bentrok dengan handleGlobalClick
        setTimeout(() => {
          this.showFilter = false; // Sembunyikan area filter
           this.startInterval(); // Nyalakan interval tiap 3 detik
        }, 100);
      }
    },

    // load: function () {
    //   if (this.isLoading) {
    //     console.log("masih loading bro!");
    //     return;
    //   }

    //   this.isLoading = true;

    //   this.ds
    //     .load()
    //     .then((dt) => {
    //       let rawData = dt.data.Data || [];

    //       // Paksa pengurutan (sorting) di Frontend agar posisinya stabil setiap ditarik
    //       rawData.sort((a, b) => {
    //         let dateA = new Date(a.ReceiptDate).getTime();
    //         let dateB = new Date(b.ReceiptDate).getTime();
            
    //         // Urutkan berdasarkan Tanggal (Terlama di atas / ASC)
    //         if (dateA !== dateB) return dateA - dateB;
            
    //         // Jika tanggal sama persis, urutkan berdasarkan DNNumber ASC agar posisinya terkunci mati
    //         let dnA = a.DNNumber || "";
    //         let dnB = b.DNNumber || "";
    //         return dnA.localeCompare(dnB);
    //       });

    //       // Freeze array agar tidak dibuatkan reactivity proxy yang berat
    //       this.list = Object.freeze(rawData);

    //       // Hitung summary langsung dari computed properties
    //       this.summary.total = this.list.length;
    //       this.summary.pending = this.listPending.length;
    //       this.summary.passed = this.listPassed.length;
    //       this.summary.ng = this.listNG.length;
    //     })
    //     .catch((err) => console.error(err))
    //     .finally(() => {
    //       // $nextTick memastikan DOM sudah SELESAI dirender sepenuhnya
    //       this.$nextTick(() => {
    //         this.isLoading = false;

    //         // Jadwalkan tarikan data berikutnya setelah rendering DOM tuntas
    //         this.pollingId = setTimeout(() => {
    //           this.load();
    //         }, 3000);
    //       });
    //     });
    // },
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
  border: 1px solid gainsboro !important;
  /* border-collapse: separate; */
  /* border-spacing: 0; */
  font-size: 0.9em;
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
