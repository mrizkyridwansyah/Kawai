
<template>
  <div class="panel panel-inverse andon-screen">

    <!-- =====================================================
         HEADER
         ===================================================== -->
    <div class="panel-heading ui-sortable-handle andon-header">
      <font-awesome-icon
        icon="chart-column"
        class="andon-header-icon"
      />

      <span
        id="header-panel"
        class="ml-3 andon-title"
      >
        Receiving Andon (Temporary Area)
      </span>
    </div>


    <!-- =====================================================
         FILTER
         ===================================================== -->
    <div
      class="filter-area p-3"
      id="containerfilter"
      v-show="showFilter"
      @click.stop
    >

      <div class="row align-items-center mt-1">

        <!-- Supplier -->
        <div
          class="col-12 col-md-5 col-lg-5 d-flex align-items-center"
        >
          <label
            class="form-label mb-0 me-2"
            style="min-width: 40px"
          >
            Supplier
          </label>

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


        <!-- Search -->
        <div
          class="col-12 col-md-2 col-lg-auto d-flex align-items-center"
        >
          <v-button-search :search="search" />
        </div>

      </div>
    </div>


    <!-- =====================================================
         MAIN BODY
         ===================================================== -->
    <div class="panel-body andon-body">

      <!-- ===================================================
           SUMMARY
           =================================================== -->
      <div class="row summary-row">

        <!-- Total -->
        <div
          class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12"
        >
          <div
            class="p-2 header-summary-content"
            style="background-color: #8d56a9"
          >
            <span class="title-summary">
              Total Receipt (Unprocessed to storage)
            </span>

            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.total) }}
            </span>
          </div>
        </div>


        <!-- Pending -->
        <div
          class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12"
        >
          <div
            class="p-2 header-summary-content"
            style="background-color: #888"
          >
            <span class="title-summary">
              QC Inprogress
            </span>

            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.pending) }}
            </span>
          </div>
        </div>


        <!-- Passed -->
        <div
          class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12"
        >
          <div
            class="p-2 header-summary-content"
            style="background-color: #18b2e0"
          >
            <span class="title-summary">
              Passed QC
            </span>

            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.passed) }}
            </span>
          </div>
        </div>


        <!-- NG -->
        <div
          class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12"
        >
          <div
            class="p-2 header-summary-content"
            style="background-color: #e60808"
          >
            <span class="title-summary">
              NG QC
            </span>

            <span class="qty-summary">
              {{ $func.formatMoney(this.summary.ng) }}
            </span>
          </div>
        </div>

      </div>


      <!-- ===================================================
           ROW 1
           =================================================== -->
      <div class="row mt-3 dashboard-row">

        <!-- =================================================
             PENDING RECEIPT CHECK
             ================================================= -->
        <div
          class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 dashboard-col"
        >
          <div class="panel panel-inverse dashboard-panel">

            <div class="panel-heading ui-sortable-handle">

              <font-awesome-icon
                icon="file-pen"
                class="text-white icon-title"
              />

              <span class="ml-3 panel-title-responsive">
                Total Receipt (Unprocessed to storage)
              </span>

            </div>


            <div class="panel-body dashboard-panel-body">

              <div
                class="v-table-wrapper"
                @scroll="onScroll($event, 'main')"
              >

                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                >

                  <thead>
                    <tr>

                      <th>
                        Date
                      </th>

                      <th class="text-center">
                        Supplier
                      </th>

                      <th class="text-center">
                        DN
                      </th>

                      <th class="text-center">
                        Item
                      </th>

                      <th class="text-center">
                        Qty(Unit)
                      </th>

                      <th class="text-center">
                        Qty(Pack)
                      </th>

                      <th class="text-center">
                        Status
                      </th>

                    </tr>
                  </thead>


                  <tbody>

                    <tr
                      v-for="(item, i) in displayList"
                      :key="'main-' + i"
                    >

                      <td>
                        {{ $func.formatDate(item.ReceiptDate) }}
                      </td>

                      <td>
                        {{ item.SupplierName }}
                      </td>

                      <td>
                        {{ item.DNNumber }}
                      </td>

                      <td>
                        {{ item.ItemName }}
                      </td>

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

                          'text-danger':
                            item.StatusReceiptName === 'NG QC',

                          'text-warning':
                            item.StatusReceiptName === 'QC Inprogres',

                          'text-primary':
                            item.StatusReceiptName === 'New'
                        }"
                      >
                        {{ item.StatusReceiptName }}
                      </td>

                    </tr>

                  </tbody>

                </table>


                <v-data-empty
                  class="mt-3"
                  v-if="list.length === 0"
                />

              </div>

            </div>

          </div>
        </div>


        <!-- =================================================
             QC INPROGRESS
             ================================================= -->
        <div
          class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 dashboard-col"
        >

          <div class="panel panel-inverse dashboard-panel">

            <div class="panel-heading ui-sortable-handle">

              <font-awesome-icon
                class="text-warning icon-title"
                icon="hourglass-start"
              />

              <span class="ml-3 panel-title-responsive">
                QC Inprogress
              </span>

            </div>


            <div class="panel-body dashboard-panel-body">

              <div
                class="v-table-wrapper"
                @scroll="onScroll($event, 'pending')"
              >

                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                >

                  <thead>
                    <tr>

                      <th class="text-center">
                        Date
                      </th>

                      <th class="text-center">
                        Supplier
                      </th>

                      <th class="text-center">
                        DN
                      </th>

                      <th class="text-center">
                        Item
                      </th>

                      <th class="text-center">
                        Qty(Unit)
                      </th>

                      <th class="text-center">
                        Qty(Pack)
                      </th>

                    </tr>
                  </thead>


                  <tbody>

                    <tr
                      v-for="(item, i) in displayListPending"
                      :key="'pending-' + i"
                    >

                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B'
                        }"
                      >
                        {{ $func.formatDate(item.ReceiptDate) }}
                      </td>

                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B'
                        }"
                      >
                        {{ item.SupplierName }}
                      </td>

                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B'
                        }"
                      >
                        {{ item.DNNumber }}
                      </td>

                      <td
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B'
                        }"
                      >
                        {{ item.ItemName }}
                      </td>

                      <td
                        class="text-right"
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B'
                        }"
                      >
                        {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                      </td>

                      <td
                        class="text-right"
                        :class="{
                          'bg-danger': item.FlagGrid === 'C',
                          'bg-warning': item.FlagGrid === 'B'
                        }"
                      >
                        {{ $func.formatMoney(item.ReceiptQtyPack) }}
                      </td>

                    </tr>

                  </tbody>

                </table>


                <v-data-empty
                  class="mt-3"
                  v-if="listPending.length === 0"
                />

              </div>

            </div>

          </div>

        </div>

      </div>


      <!-- ===================================================
           ROW 2
           =================================================== -->
      <div class="row mt-3 dashboard-row">

        <!-- =================================================
             PASSED QC
             ================================================= -->
        <div
          class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 dashboard-col"
        >

          <div class="panel panel-inverse dashboard-panel">

            <div class="panel-heading ui-sortable-handle">

              <font-awesome-icon
                icon="check"
                class="text-success icon-title"
              />

              <span class="ml-3 panel-title-responsive">
                Passed QC
              </span>

            </div>


            <div class="panel-body dashboard-panel-body">

              <div
                class="v-table-wrapper"
                @scroll="onScroll($event, 'passed')"
              >

                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                >

                  <thead>
                    <tr>

                      <th class="text-center">
                        Date
                      </th>

                      <th class="text-center">
                        Supplier
                      </th>

                      <th class="text-center">
                        DN
                      </th>

                      <th class="text-center">
                        Item
                      </th>

                      <th class="text-center">
                        Qty(Unit)
                      </th>

                      <th class="text-center">
                        Qty(Pack)
                      </th>

                    </tr>
                  </thead>


                  <tbody>

                    <tr
                      v-for="(item, i) in displayListPassed"
                      :key="'passed-' + i"
                    >

                      <td>
                        {{ $func.formatDate(item.ReceiptDate) }}
                      </td>

                      <td>
                        {{ item.SupplierName }}
                      </td>

                      <td>
                        {{ item.DNNumber }}
                      </td>

                      <td>
                        {{ item.ItemName }}
                      </td>

                      <td class="text-right">
                        {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                      </td>

                      <td class="text-right">
                        {{ $func.formatMoney(item.ReceiptQtyPack) }}
                      </td>

                    </tr>

                  </tbody>

                </table>


                <v-data-empty
                  class="mt-3"
                  v-if="listPassed.length === 0"
                />

              </div>

            </div>

          </div>

        </div>


        <!-- =================================================
             NG QC
             ================================================= -->
        <div
          class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 dashboard-col"
        >

          <div class="panel panel-inverse dashboard-panel">

            <div class="panel-heading ui-sortable-handle">

              <font-awesome-icon
                icon="x"
                class="text-danger icon-title"
              />

              <span class="ml-3 panel-title-responsive">
                NG QC
              </span>

            </div>


            <div class="panel-body dashboard-panel-body">

              <div
                class="v-table-wrapper"
                @scroll="onScroll($event, 'ng')"
              >

                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                >

                  <thead>

                    <tr class="datatable-color">

                      <th class="text-center">
                        Date
                      </th>

                      <th class="text-center">
                        Supplier
                      </th>

                      <th class="text-center">
                        DN
                      </th>

                      <th class="text-center">
                        Item
                      </th>

                      <th class="text-center">
                        Qty(Unit)
                      </th>

                      <th class="text-center">
                        Qty(Pack)
                      </th>

                    </tr>

                  </thead>


                  <tbody>

                    <tr
                      v-for="(item, i) in displayListNG"
                      :key="'ng-' + i"
                    >

                      <td>
                        {{ $func.formatDate(item.ReceiptDate) }}
                      </td>

                      <td>
                        {{ item.SupplierName }}
                      </td>

                      <td>
                        {{ item.DNNumber }}
                      </td>

                      <td>
                        {{ item.ItemName }}
                      </td>

                      <td class="text-right">
                        {{ $func.formatMoney(item.ReceiptQtyUnit) }}
                      </td>

                      <td class="text-right">
                        {{ $func.formatMoney(item.ReceiptQtyPack) }}
                      </td>

                    </tr>

                  </tbody>

                </table>


                <v-data-empty
                  class="mt-3"
                  v-if="listNG.length === 0"
                />

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
      ng: 0
    },

    intervalLoad: null,

    showFilter: true,

    filter: {
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
      ng: 50
    }

  }),


  computed: {

    ds: function () {
      return useReceiptAndon();
    },


    listPending() {
      return this.list.filter(
        (p) => p.StatusReceipt === "PENDING"
      );
    },


    listPassed() {
      return this.list.filter(
        (p) => p.StatusReceipt === "OK"
      );
    },


    listNG() {
      return this.list.filter(
        (p) => p.StatusReceipt === "NG"
      );
    },


    displayList() {
      return this.list.slice(
        0,
        this.renderLimits.main
      );
    },


    displayListPending() {
      return this.listPending.slice(
        0,
        this.renderLimits.pending
      );
    },


    displayListPassed() {
      return this.listPassed.slice(
        0,
        this.renderLimits.passed
      );
    },


    displayListNG() {
      return this.listNG.slice(
        0,
        this.renderLimits.ng
      );
    }

  },


  mounted: async function () {

    document.addEventListener(
      "click",
      this.handleGlobalClick
    );

  },


  beforeUnmount: function () {

    this.stopInterval();

    document.removeEventListener(
      "click",
      this.handleGlobalClick
    );

  },


  methods: {

    // =====================================================
    // START INTERVAL
    // =====================================================

    startInterval: function () {

      this.stopInterval();

      this.intervalLoad = setInterval(() => {

        this.search(true);

      }, 3000);

    },


    // =====================================================
    // STOP INTERVAL
    // =====================================================

    stopInterval: function () {

      if (this.intervalLoad) {

        clearInterval(
          this.intervalLoad
        );

        this.intervalLoad = null;

      }

    },


    // =====================================================
    // GLOBAL CLICK
    // =====================================================

   handleGlobalClick: function (event) {

  if (!this.showFilter) {

    this.showFilter = true;

    this.stopInterval();

    // =========================================
    // TAMPILKAN HEADER TEMPLATE
    // =========================================

    const header =
      document.getElementById("header");

    if (header) {
      header.style.display = "";
    }

    // =========================================
    // RESTORE APP
    // =========================================

    const appContent =
      document.getElementById("app");

    if (appContent) {
      appContent.style.paddingTop = "";
      appContent.style.marginTop = "";
    }

    // =========================================
    // RESTORE BODY
    // =========================================

    document.body.style.backgroundColor = "";
    document.body.style.overflow = "";

    // =========================================
    // RESTORE HEADER PANEL
    // =========================================

    const headerPanel =
      document.getElementById("header-panel");

    if (headerPanel) {

      headerPanel.innerText =
        "Receiving Andon (Temporary Area)";

    }

  }

}, 


    // =====================================================
    // TABLE SCROLL
    // =====================================================

    onScroll: function (e, type) {

      const {
        scrollTop,
        scrollHeight,
        clientHeight
      } = e.target;


      if (
        scrollTop +
        clientHeight >=
        scrollHeight - 50
      ) {

        let maxLen = 0;


        if (type === "main") {

          maxLen = this.list.length;

        } else if (type === "pending") {

          maxLen =
            this.listPending.length;

        } else if (type === "passed") {

          maxLen =
            this.listPassed.length;

        } else if (type === "ng") {

          maxLen =
            this.listNG.length;

        }


        if (
          this.renderLimits[type] <
          maxLen
        ) {

          this.renderLimits[type] += 50;

        }

      }

    },


    // =====================================================
    // SEARCH / LOAD DATA
    // =====================================================

    search: function () {

      if (this.isLoading) {

        console.log(
          "masih loading bro!"
        );

        return;

      }


      this.isLoading = true;


      this.ds
        .loadbysupplier(
          this.filter.supplier
        )

        .then((dt) => {

          let rawData =
            dt.data.Data || [];


          // =============================================
          // SORTING
          // =============================================

          rawData.sort((a, b) => {

            let dateA =
              new Date(
                a.ReceiptDate
              ).getTime();


            let dateB =
              new Date(
                b.ReceiptDate
              ).getTime();


            // Date ASC
            if (dateA !== dateB) {

              return dateA - dateB;

            }


            // DN ASC
            let dnA =
              a.DNNumber || "";


            let dnB =
              b.DNNumber || "";


            return dnA.localeCompare(
              dnB
            );

          });


          // =============================================
          // FREEZE DATA
          // =============================================

          this.list =
            Object.freeze(rawData);


          // =============================================
          // RESET DISPLAY LIMIT
          // =============================================

          this.renderLimits = {
            main: 50,
            pending: 50,
            passed: 50,
            ng: 50
          };


          // =============================================
          // SUMMARY
          // =============================================

          this.summary.total =
            this.list.length;


          this.summary.pending =
            this.listPending.length;


          this.summary.passed =
            this.listPassed.length;


          this.summary.ng =
            this.listNG.length;

        })

        .catch((err) => {

          console.error(err);

        })

        .finally(() => {

          this.isLoading = false;

        });


      // =================================================
      // HEADER
      // =================================================

      if (
        this.filter.supplier != null
      ) {

        const headerPanel =
          document.getElementById(
            "header-panel"
          );


        if (headerPanel) {

          headerPanel.innerText =
            `Receiving Andon (Temporary Area) - Supplier : ${this.filter.SupplierName}`;

        }


        setTimeout(() => {

          this.showFilter = false;

  // =========================================
  // HIDE HEADER TEMPLATE
  // =========================================

  const header =
    document.getElementById("header");

  if (header) {
    header.style.display = "none";
  }

  // =========================================
  // FULLSCREEN APP
  // =========================================

  const appContent =
    document.getElementById("app");

  if (appContent) {

    appContent.style.paddingTop = "0px";

    appContent.style.marginTop = "0px";

  }

  
 

  // =========================================
  // START POLLING
  // =========================================

  this.startInterval();

}, 100);

      }

    }

  }

};
</script>


<!-- =====================================================
     SCOPED STYLE
     ===================================================== -->

<style scoped>

.header-summary-content {

  min-height:
    clamp(
      60px,
      9vh,
      150px
    );

  text-align: center;

  display: flex;

  flex-direction: column;

  justify-content: center;

}


.title-summary {

  color: white;

  font-size:
    clamp(
      14px,
      1.1vw,
      30px
    );

  font-weight: 500;

}


.qty-summary {

  color: white;

  font-size:
    clamp(
      28px,
      3vw,
      70px
    );

  font-weight: bold;

}


.icon-title {

  font-size:
    clamp(
      18px,
      1.4vw,
      32px
    );

  font-weight: bolder;

}


.panel-title-responsive {

  font-size:
    clamp(
      14px,
      1.05vw,
      28px
    );

}


.andon-header-icon {

  font-size:
    clamp(
      18px,
      1.25vw,
      32px
    );

}


.andon-title {

  font-size:
    clamp(
      16px,
      1.3vw,
      32px
    );

}

</style>


<!-- =====================================================
     GLOBAL STYLE
     ===================================================== -->

<style>

html,
body {

  height: 100%;

  margin: 0;

  overflow: hidden !important;

}


/* =====================================================
   MAIN SCREEN
   ===================================================== */

.andon-screen {

  height: 100vh;

  width: 100%;

  margin: 0;

  display: flex;

  flex-direction: column;

  overflow: hidden;

}


/* =====================================================
   HEADER
   ===================================================== */

.andon-screen > .panel-heading {

  flex: 0 0 auto;

  min-height:
    clamp(
      40px,
      4vh,
      75px
    );

  display: flex;

  align-items: center;

}


/* =====================================================
   FILTER
   ===================================================== */

.andon-screen > .filter-area {

  flex: 0 0 auto;

}


/* =====================================================
   MAIN BODY
   ===================================================== */

.andon-screen > .panel-body {

  flex: 1 1 auto;
  min-height: 0;
  overflow: hidden;

}


.andon-body {

  height: 100%;
  min-height: 0;
  display: flex;
  flex-direction: column;
  overflow: hidden;

}


/* =====================================================
   SUMMARY
   ===================================================== */

.summary-row {

  flex: 0 0 auto;

}


/* =====================================================
   DASHBOARD ROW
   ===================================================== */

.dashboard-row {

  flex: 1 1 0;

  min-height: 0;

}


.andon-body > .dashboard-row {

  margin-top:
    clamp(
      4px,
      0.5vh,
      12px
    ) !important;

}


/* =====================================================
   DASHBOARD COLUMN
   ===================================================== */

.dashboard-col {

  height: 100%;

  min-height: 0;

  display: flex;

}


/* =====================================================
   INNER PANEL
   ===================================================== */
.dashboard-panel {
  width: 100%;
  height: 100%;
  margin-bottom: 0;

  display: flex;
  flex-direction: column;

  min-height: 0;
  overflow: visible;
}

.dashboard-panel > .panel-heading {
  flex: 0 0 auto;
  min-height: clamp(35px, 4vh, 65px);

  display: flex;
  align-items: center;
}

.dashboard-panel-body {
  flex: 1 1 auto;
  min-height: 0;

  display: flex;

  /* Jangan hidden karena scrollbar tabel bisa terpotong */
  overflow: visible;
}

/* =====================================================
   TABLE WRAPPER
   ===================================================== */

.v-table-wrapper {

  position: relative;
  width: 100%;
  height: 100%;
  min-height: 0;

  overflow-x: auto;
  overflow-y: auto;

  /* Supaya scrollbar horizontal tidak tertutup */
  padding-bottom: 8px;

}


/* =====================================================
   TABLE
   ===================================================== */

.v-fixed-table {

  width: max-content;

  min-width: 100%;

  border:
    1px solid gainsboro !important;

  font-size:
    clamp(
      12px,
      0.85vw,
      24px
    );

}


.v-fixed-table th,
.v-fixed-table td {

  white-space: nowrap;

  padding:
    clamp(
      5px,
      0.4vw,
      14px
    )
    clamp(
      8px,
      0.7vw,
      20px
    );

  border:
    1px solid #dee2e6;

  background: #fff;

}


/* =====================================================
   STICKY HEADER
   ===================================================== */

.v-fixed-table thead th {

  position: sticky;

  top: 0;

  z-index: 20;

  background:
    lightblue !important;

}


/* =====================================================
   STICKY LEFT
   ===================================================== */

.sticky-left {

  position: sticky;

  background:
    white !important;

  background-color:
    white;

  z-index: 10;

}


thead .sticky-left {

  z-index: 30;

}


/* =====================================================
   MOBILE
   ===================================================== */

@media (max-width: 767px) {

  html,
  body {

    overflow: auto !important;

  }


  .andon-screen {

    height: auto;

    min-height: 100vh;

    overflow: visible;

  }


  .andon-screen > .panel-body {

    overflow: visible;

  }


  .andon-body {

    overflow: visible;

  }


  .dashboard-row {

    flex: none;

  }


  .dashboard-col {

    height: 400px;

    margin-bottom: 10px;

  }

}

</style>
 
