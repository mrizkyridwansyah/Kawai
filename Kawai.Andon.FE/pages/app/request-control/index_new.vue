```vue
<template>
  <div class="womin-dashboard">
    <!-- =========================================================
         HEADER
    ========================================================== -->
    <div class="dashboard-header">
      <div class="dashboard-title">
        <font-awesome-icon icon="file-lines" class="dashboard-title-icon" />

        <span>Womin Request Control</span>
      </div>

      <div class="dashboard-header-right">
        <div class="live-status">
          <span class="live-dot"></span>
          <span>LIVE</span>
        </div>

        <div class="header-separator"></div>

        <div class="dashboard-clock">
          {{ currentDateTime }}
        </div>

        <button type="button" class="refresh-button" @click="manualRefresh">
          <font-awesome-icon
            icon="arrows-rotate"
            :class="{ 'spin-icon': isLoading }"
          />
        </button>
      </div>
    </div>

    <!-- =========================================================
         FILTER AREA
    ========================================================== -->
    <div
      v-show="showFilter"
      id="containerfilter"
      class="dashboard-filter"
      @click.stop
    >
       <table style="width: 99%;">
      <tr>
        <td Style="width:2%"></td>
        <td Style="width:10%"> <label class="form-label mb-0 me-2" style="min-width: 40px">Line</label></td>
        <td Style="width:39%">     
            <filter-line-factory
              class="form-control w-100"
              company="11111"
              manufacture="ALL"
              v-model="filter.line"
              v-model:line-name="filter.lineName"
              :show-option-all="true"
              default-option-all="ALL"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
         </td>
        <td Style="width:1%"></td>
        <td Style="width:10%"><label class="form-label mb-0 me-2" style="min-width: 40px">Preparation Status</label></td>
        <td Style="width:39%">
           <filter-status
              class="form-control w-100"
              v-model="filter.status"
             :show-option-all="true"
              default-option-all="ALL"
              v-model:status-descs="filter.statusDescs"
              
            />
             </td>
         <td Style="width:8%"></td>
      </tr>
      <tr style="height:40px ;">
        <td Style="width:2%"></td>
        <td Style="width:10%"> <label class="form-label mb-0 me-2" style="min-width: 40px">Parts Group</label></td>
        <td Style="width:39%">     
           <filter-area
              class="form-control w-100"
              v-model="filter.area"
                 :show-option-all="true"
              default-option-all="ALL"
              v-model:area-name="filter.areaName"
              warehouse=""
            />
         </td>
        <td Style="width:1%"></td>
        <td Style="width:10%"></td>
        <td Style="width:39%">  <v-button-search :search="search" /></td>
         <td Style="width:8%"></td>
      </tr>
      <tr></tr>
      <tr></tr>
    </table>
    </div>

    <!-- =========================================================
         MAIN CONTENT
    ========================================================== -->
    <div class="dashboard-content" @click="handleDashboardClick">
      <!-- =======================================================
           SUMMARY CARDS
      ======================================================== -->
      <div class="summary-grid">
        <!-- TOTAL REQUEST -->
        <div class="summary-card summary-purple">
          <div class="summary-icon">
            <font-awesome-icon icon="file-lines" />
          </div>

          <div class="summary-information">
            <div class="summary-title">Total Request</div>

            <div class="summary-value">
              {{ formatNumber(summary.total) }}
            </div>
          </div>
        </div>

        <!-- WOMIN -->
        <div class="summary-card summary-gray">
          <div class="summary-icon">
            <font-awesome-icon icon="users" />
          </div>

          <div class="summary-information">
            <div class="summary-title">Womin</div>

            <div class="summary-value">
              {{ formatNumber(summary.womin) }}
            </div>
          </div>
        </div>

        <!-- PICKING PROGRESS -->
        <div class="summary-card summary-blue">
          <div class="summary-icon">
            <font-awesome-icon icon="cart-shopping" />
          </div>

          <div class="summary-information progress-information">
            <div class="summary-title">Picking Progress (Item)</div>

            <div class="summary-value progress-value">
              {{ formatNumber(summary.totalItem) }}

              
            </div>
          </div>
        </div>

        <!-- REMAINING -->
        <div class="summary-card summary-blue">
          <div class="summary-icon">
            <font-awesome-icon icon="box" />
          </div>

          <div class="summary-information">
            <div class="summary-title">Remaining Item</div>

            <div class="summary-value">
              {{ formatNumber(summary.remaining) }}
            </div>
          </div>
        </div>
      </div>

      <!-- =======================================================
           TABLE PANEL
      ======================================================== -->
      <div class="data-panel">
        <!-- TABLE TITLE -->
        <div class="data-panel-header">
          <div class="data-panel-title">
            <font-awesome-icon icon="hourglass-start" class="data-panel-icon" />

            <span>
              {{ tableTitle }}
            </span>
          </div>

          <div class="total-data">
            Total Data :
            <strong>
              {{ formatNumber(tableData.length) }}
            </strong>
          </div>
        </div>

        <!-- =====================================================
             TABLE
        ====================================================== -->
        <div class="table-container">
          <table class="womin-table">
            <thead>
              <tr>
                <th class="col-line">Line</th>

                <th class="col-proddate">Production Date</th>
                
                <th class="col-request">Request No</th>

                <th class="col-partgroup">Part Group</th>

                
                <th class="col-model">Model</th>

                <th class="col-ws">WS</th>

                <th class="col-status">Status</th>

                <th class="col-trolly">Trolly Number</th>

                <th class="col-position">Current Position</th>

                <th class="col-next">Next Location</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="(item, i) in pagedData" :key="getRowKey(item, i)">
                <!-- LINE -->
                <td class="text-center">
                  <a
                    href="javascript:void(0)"
                    class="table-link"
                    @click.stop="
                      viewWominByLine(item.LineCode, item.ProductionDate)
                    "
                  >
                    {{ item.Line }}
                  </a>
                </td>

                 <!-- ITEM -->
                <td>
                 {{ $func.formatDate(item.ProductionDate) }}
                </td>

                <!-- REQUEST -->
                <td>
                  <a
                    href="javascript:void(0)"
                    class="table-link request-link"
                    @click.stop="viewWomin(item.RequestNo, item.GroupingPart)"
                  >
                    {{ item.RequestNo }}
                  </a>
                </td>

                 <!-- ITEM -->
                <td>
                {{ item.GroupingPart || "-" }}
                </td>

              

                <!-- MODEL -->
                <td>
                  {{ item.Model || "-" }}
                </td>

                <!-- WS -->
                <td class="text-center">
                  {{ item.WorkStation || "-" }}
                </td>

                <!-- STATUS -->
                <td>
                  <span
                    class="status-badge"
                    :class="getStatusClass(item.PreparationStatus)"
                  >
                    {{ item.PreparationStatus || "-" }}
                  </span>
                </td>

                <!-- TROLLY -->
                <td>
                  {{ item.TrollyNumber || "-" }}
                </td>

                <!-- CURRENT POSITION -->
                <td>
                  {{ item.CurrentPosition || "-" }}
                </td>

                <!-- NEXT LOCATION -->
                <td>
                  {{ item.NextLocation || "-" }}
                </td>
              </tr>

              <!-- EMPTY -->
              <tr v-if="!ds.isLoading && pagedData.length === 0">
                <td colspan="10" class="empty-data">
                  <v-data-empty />
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- =====================================================
             FOOTER PAGINATION
        ====================================================== -->
        <div class="table-footer">
          <!-- LEFT -->
          <div class="footer-left">
            <span class="showing-label"> Show </span>

            <select v-model.number="pageSize" class="page-size-select">
              <option :value="9">9</option>

              <option :value="10">10</option>

              <option :value="15">15</option>

              <option :value="20">20</option>

              <option :value="25">25</option>

              <option :value="30">30</option>
            </select>

            <span class="showing-label"> Records per Page </span>
          </div>

          <!-- CENTER -->
          <div class="footer-center">
            <div class="next-page-label">Auto Page Transition</div>

            <div class="countdown">
              00:{{ String(pageCountdown).padStart(2, "0") }}
            </div>
          </div>

          <!-- RIGHT -->
          <div class="footer-right">
            <!-- PREVIOUS -->
            <button
              type="button"
              class="pagination-button"
              :disabled="totalPages <= 1"
              @click.stop="previousPage"
            >
              <font-awesome-icon icon="chevron-left" />
            </button>

            <!-- CURRENT PAGE -->
            <div class="current-page">
              {{ currentPage }}
            </div>

            <span class="page-total"> / {{ totalPages }} </span>

            <!-- NEXT -->
            <button
              type="button"
              class="pagination-button"
              :disabled="totalPages <= 1"
              @click.stop="nextPage"
            >
              <font-awesome-icon icon="chevron-right" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- =========================================================
         MODAL DETAIL WOMIN
    ========================================================== -->
    <v-modal
      title="Detail Status Womin"
      class="modal-lg"
      id="modal-list-detailwomin"
    >
      <shared-request-womin-list
        :refno="selectedRefNo"
        :groupclass="selectedGroupClass"
        :counter="counter"
      />
    </v-modal>

    <!-- =========================================================
         MODAL DETAIL WOMIN BY LINE
    ========================================================== -->
    <v-modal
      title="Detail Status Womin By Line"
      class="modal-lg"
      id="modal-list-detailwominbyline"
    >
      <shared-request-wominbyline-list
        :linecode="selectedLineCode"
        :scheduledate="selectedScheduleDate"
        :counter="counter"
      />
    </v-modal>
  </div>
</template>

 
<script>
const REFRESH_INTERVAL = 3000;
const PAGINATION_INTERVAL = 1000;
const PAGE_DURATION = 6;

export default {
  /* ===========================================================
     DATA
  ============================================================ */
  data() {
    return {
      /* ---------------------------------------------------------
         UI STATE
      --------------------------------------------------------- */
      selected: null,
      isLoading: false,
      showFilter: true,

      /* ---------------------------------------------------------
         INTERVALS
      --------------------------------------------------------- */
      intervalLoad: null,
      pageInterval: null,
      clockInterval: null,

      /* ---------------------------------------------------------
         CLOCK
      --------------------------------------------------------- */
      currentDateTime: "",

      /* ---------------------------------------------------------
         PAGINATION
      --------------------------------------------------------- */
      currentPage: 1,
      pageSize: 9,
      pageCountdown: PAGE_DURATION,

      /* ---------------------------------------------------------
         SELECTED DETAIL
      --------------------------------------------------------- */
      selectedRefNo: null,
      selectedGroupClass: null,
      selectedLineCode: null,
      selectedScheduleDate: null,
      counter: 0,

      /* ---------------------------------------------------------
         DATA
      --------------------------------------------------------- */
      list: [],

      /* ---------------------------------------------------------
         SUMMARY
      --------------------------------------------------------- */
      summary: {
        total: 0,
        totalItem: 0,
        totalItemTarget: 0,
        womin: 0,
        item: 0,
        remaining: 0,
        progressPercent: 0,
      },

      /* ---------------------------------------------------------
         FILTER
      --------------------------------------------------------- */
      filter: {
        area: null,
        areaName: "",
        status: "",
        line: null,
        lineName: "",
        statusDescs: "",
      },
    };
  },

  /* ===========================================================
     COMPUTED
  ============================================================ */
  computed: {
    /**
     * Womin request data source.
     */
    ds() {
      return useWominRequest();
    },

    /**
     * Normalize datasource menjadi Array.
     *
     * Support:
     * - Array
     * - Vue Ref
     * - { Data: [...] }
     */
    tableData() {
      return this.normalizeData(this.ds.data);
    },

    /**
     * Total halaman.
     */
    totalPages() {
      if (!this.tableData.length) {
        return 1;
      }

      return Math.max(
        1,
        Math.ceil(
          this.tableData.length / this.pageSize
        )
      );
    },

    /**
     * Data yang ditampilkan pada halaman aktif.
     */
    pagedData() {
      const start =
        (this.currentPage - 1) *
        this.pageSize;

      const end =
        start + this.pageSize;

      return this.tableData.slice(
        start,
        end
      );
    },

    /**
     * Judul tabel berdasarkan filter aktif.
     */
    tableTitle() {
      const line =
        this.filter.lineName || "ALL";

      const area =
        this.filter.areaName || "ALL";

      const status =
        this.filter.statusDescs || "ALL";

      return (
        `Remaining Item - Material Type ` +
        `(Line : ${line} | ` +
        `Parts Group : ${area} | ` +
        `Preparation Status : ${status})`
      );
    },
  },

  /* ===========================================================
     WATCHERS
  ============================================================ */
  watch: {
    /**
     * Page size berubah.
     */
    pageSize() {
      this.currentPage = 1;
      this.resetPageTimer();
    },

    /**
     * Jumlah halaman berubah karena data refresh.
     *
     * Contoh:
     * sebelumnya 10 halaman
     * sekarang hanya 5 halaman
     */
    totalPages(newTotalPages) {
      if (
        this.currentPage >
        newTotalPages
      ) {
        this.currentPage = 1;
      }
    },
  },

  /* ===========================================================
     LIFECYCLE
  ============================================================ */
  mounted() {
    this.startClock();

    this.$nextTick(() => {
      this.search();
    });
  },

  beforeUnmount() {
    this.cleanup();
  },

  /* ===========================================================
     METHODS
  ============================================================ */
  methods: {
    /* =========================================================
       DATA
    ========================================================== */

    /**
     * Normalize datasource menjadi Array.
     */
    normalizeData(source) {
      let data = source;

      // Vue Ref
      if (
        data &&
        typeof data === "object" &&
        "value" in data
      ) {
        data = data.value;
      }

      // Response object { Data: [] }
      if (
        data &&
        typeof data === "object" &&
        !Array.isArray(data) &&
        Array.isArray(data.Data)
      ) {
        data = data.Data;
      }

      return Array.isArray(data)
        ? data
        : [];
    },

    /* =========================================================
       SEARCH / API
    ========================================================== */

    /**
     * Load data dari API.
     *
     * isAuto hanya sebagai informasi
     * apakah request berasal dari auto refresh.
     */
    async search(isAuto = false) {
      // Jangan overlap request.
      if (this.isLoading) {
        return;
      }

      this.isLoading = true;

      try {
        const response =
          await this.ds.load(
            this.filter.line,
            this.filter.area,
            this.filter.status
          );

        const data =
          this.normalizeData(
            response?.Data
          );

        this.list = data;

        this.updateSummary(data);

        this.validateCurrentPage();

        /*
         * Jangan reset countdown di sini.
         *
         * Auto refresh 3 detik dan
         * auto pagination 6 detik harus
         * berjalan independen.
         */

        if (
          this.filter.area !== null
        ) {
          this.enterDashboardMode();

          this.startAutoRefresh();

          this.ensurePagination();
        }
      } catch (error) {
        console.error(
          "Error loading Womin data:",
          error
        );
      } finally {
        this.isLoading = false;
      }
    },

    /**
     * Update summary dashboard.
     */
    updateSummary(data) {
      const firstRow =
        data.length > 0
          ? data[0]
          : null;

      this.summary.total =  data.length;
      this.summary.totalItem = this.list.length > 0 ? this.list[0].PickingProgress : 0;
      this.summary.remaining = this.list.length > 0 ? this.list[0].Remaining : 0;
      this.summary.womin = this.list.length > 0 ? this.list[0].Womin : 0;

      
    },

    /**
     * Hitung progress percentage.
     */
    calculateProgress(
      current,
      target
    ) {
      if (!target || target <= 0) {
        return 0;
      }

      return Math.round(
        (current / target) * 100
      );
    },

    /**
     * Pastikan current page masih valid
     * setelah data berubah.
     */
    validateCurrentPage() {
      if (
        this.currentPage >
        this.totalPages
      ) {
        this.currentPage = 1;
      }
    },

    /* =========================================================
       AUTO REFRESH
    ========================================================== */

    /**
     * Start auto refresh API.
     *
     * Hanya boleh ada satu interval.
     */
    startAutoRefresh() {
      if (this.intervalLoad) {
        return;
      }

      this.intervalLoad =
        setInterval(() => {
          this.search(true);
        }, REFRESH_INTERVAL);
    },

    /**
     * Stop auto refresh API.
     */
    stopAutoRefresh() {
      if (!this.intervalLoad) {
        return;
      }

      clearInterval(
        this.intervalLoad
      );

      this.intervalLoad = null;
    },

    /* =========================================================
       AUTO PAGINATION
    ========================================================== */

    /**
     * Pastikan pagination aktif.
     *
     * Tidak membuat interval baru
     * jika sudah aktif.
     */
    ensurePagination() {
      if (this.pageInterval) {
        return;
      }

      this.startPagination();
    },

    /**
     * Start timer pagination.
     *
     * Timer berjalan setiap 1 detik.
     * Setelah 6 detik -> next page.
     */
    startPagination() {
      if (this.pageInterval) {
        return;
      }

      this.pageCountdown =
        PAGE_DURATION;

      this.pageInterval =
        setInterval(() => {
          this.processPagination();
        }, PAGINATION_INTERVAL);
    },

    /**
     * Logic countdown pagination.
     */
    processPagination() {
      // Filter terbuka.
      if (this.showFilter) {
        return;
      }

      // Hanya ada satu halaman.
      if (this.totalPages <= 1) {
        this.currentPage = 1;
        this.pageCountdown =
          PAGE_DURATION;
        return;
      }

      this.pageCountdown--;

      if (this.pageCountdown <= 0) {
        this.nextPage();
      }
    },

    /**
     * Stop pagination timer.
     */
    stopPagination() {
      if (!this.pageInterval) {
        return;
      }

      clearInterval(
        this.pageInterval
      );

      this.pageInterval = null;
    },

    /**
     * Reset countdown.
     */
    resetPageTimer() {
      this.pageCountdown =
        PAGE_DURATION;
    },

    /**
     * Next page.
     */
    nextPage() {
      if (this.totalPages <= 1) {
        this.currentPage = 1;
        this.resetPageTimer();
        return;
      }

      if (
        this.currentPage <
        this.totalPages
      ) {
        this.currentPage++;
      } else {
        this.currentPage = 1;
      }

      this.resetPageTimer();
    },

    /**
     * Previous page.
     */
    previousPage() {
      if (this.totalPages <= 1) {
        return;
      }

      if (this.currentPage > 1) {
        this.currentPage--;
      } else {
        this.currentPage =
          this.totalPages;
      }

      this.resetPageTimer();
    },

    /* =========================================================
       CLOCK
    ========================================================== */

    /**
     * Start clock.
     */
    startClock() {
      this.updateClock();

      this.clockInterval =
        setInterval(() => {
          this.updateClock();
        }, 1000);
    },

    /**
     * Update clock display.
     */
    updateClock() {
      const now = new Date();

      const day =
        String(
          now.getDate()
        ).padStart(2, "0");

      const monthNames = [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec",
      ];

      const month =
        monthNames[
          now.getMonth()
        ];

      const year =
        now.getFullYear();

      const hours =
        String(
          now.getHours()
        ).padStart(2, "0");

      const minutes =
        String(
          now.getMinutes()
        ).padStart(2, "0");

      const seconds =
        String(
          now.getSeconds()
        ).padStart(2, "0");

      this.currentDateTime =
        `${day} ${month} ${year} ` +
        `${hours}:${minutes}:${seconds}`;
    },

    /* =========================================================
       DASHBOARD MODE
    ========================================================== */

    /**
     * Masuk mode fullscreen dashboard.
     */
    enterDashboardMode() {
      if (!this.showFilter) {
        return;
      }

      this.showFilter = false;

      this.hideApplicationHeader();

      this.setFullscreenLayout();
    },

    /**
     * Hide application header.
     */
    hideApplicationHeader() {
      const header =
        document.getElementById(
          "header"
        );

      if (header) {
        header.style.display =
          "none";
      }
    },

    /**
     * Set fullscreen layout.
     */
    setFullscreenLayout() {
      const appContent =
        document.getElementById(
          "app"
        );

      if (appContent) {
        appContent.style.paddingTop =
          "0px";

        appContent.style.marginTop =
          "0px";
      }

      document.body.style.backgroundColor =
        "#ffffff";

      document.body.style.overflow =
        "hidden";
    },

    /**
     * Restore normal application layout.
     */
    restoreDashboard() {
      const header =
        document.getElementById(
          "header"
        );

      if (header) {
        header.style.display = "";
      }

      const appContent =
        document.getElementById(
          "app"
        );

      if (appContent) {
        appContent.style.paddingTop =
          "";

        appContent.style.marginTop =
          "";
      }

      document.body.style.backgroundColor =
        "";

      document.body.style.overflow =
        "";
    },

    /* =========================================================
       DASHBOARD CLICK
    ========================================================== */

    /**
     * Click area dashboard.
     *
     * Klik biasa -> buka filter.
     *
     * Klik button/link/input/select
     * tidak membuka filter.
     */
    handleDashboardClick(event) {
      if (this.showFilter) {
        return;
      }

      const target =
        event.target;

      if (
        target.closest(
          "button, a, input, select, textarea"
        )
      ) {
        return;
      }

      this.showFilter = true;

      this.stopAutoRefresh();
      this.stopPagination();

      this.restoreDashboard();
    },

    /* =========================================================
       FILTER
    ========================================================== */

    /**
     * Reset filter.
     */
    reset() {
      this.filter = {
        area: null,
        areaName: "",
        status: "",
        line: null,
        lineName: "",
        statusDescs: "",
      };

      this.currentPage = 1;

      this.resetPageTimer();

      this.stopAutoRefresh();
      this.stopPagination();

      this.restoreDashboard();

      this.search();
    },

    /* =========================================================
       MODAL
    ========================================================== */

    /**
     * Open detail Womin berdasarkan Request No.
     */
    viewWomin(
      refno,
      groupclass
    ) {
      this.selectedRefNo =
        refno;

      this.selectedGroupClass =
        groupclass;

      this.counter++;

      this.showModal(
        "modal-list-detailwomin"
      );
    },

    /**
     * Open detail Womin berdasarkan Line.
     */
    viewWominByLine(
      linecode,
      scheduledate
    ) {
      this.selectedLineCode =
        linecode;

      this.selectedScheduleDate =
        scheduledate;

      this.counter++;

      this.showModal(
        "modal-list-detailwominbyline"
      );
    },

    /**
     * Centralized modal handler.
     */
    showModal(modalId) {
      if (!this.$bvModal) {
        return;
      }

      this.$bvModal.show(
        modalId
      );
    },

    /* =========================================================
       TABLE
    ========================================================== */

    /**
     * Generate row key.
     */
    getRowKey(item, index) {
      return (
        item.RequestNo ||
        item.TrollyNumber ||
        item.LineCode ||
        `row-${index}`
      );
    },

    /**
     * Status CSS class.
     */
    getStatusClass(status) {
      if (!status) {
        return "status-default";
      }

      const value =
        String(status)
          .toUpperCase()
          .trim();

      const statusMap = [
        {
          condition:
            value === "NEW",
          className:
            "status-new",
        },
        {
          condition:
            value.includes(
              "PICKING ON PROGRESS"
            ),
          className:
            "status-progress",
        },
        {
          condition:
            value.includes(
              "COMPLETE PICKING IN AREA"
            ),
          className:
            "status-area-complete",
        },
        {
          condition:
            value === "COMPLETE" ||
            value.includes(
              "COMPLETED"
            ),
          className:
            "status-complete",
        },
        {
          condition:
            value.includes("WAIT"),
          className:
            "status-waiting",
        },
      ];

      const matched =
        statusMap.find(
          (item) =>
            item.condition
        );

      return (
        matched?.className ||
        "status-default"
      );
    },

    /* =========================================================
       UTILITY
    ========================================================== */

    /**
     * Format angka.
     */
    formatNumber(value) {
      if (
        value === null ||
        value === undefined ||
        value === ""
      ) {
        return "0";
      }

      const number =
        Number(value);

      if (Number.isNaN(number)) {
        return value;
      }

      return number.toLocaleString(
        "en-US"
      );
    },

    /* =========================================================
       CLEANUP
    ========================================================== */

    /**
     * Bersihkan semua interval
     * ketika component dihancurkan.
     */
    cleanup() {
      this.stopAutoRefresh();
      this.stopPagination();

      if (this.clockInterval) {
        clearInterval(
          this.clockInterval
        );

        this.clockInterval = null;
      }

      this.restoreDashboard();
    },
  },
};
</script>
   
<style scoped>
/* =============================================================
   GLOBAL DASHBOARD
============================================================= */

.womin-dashboard {
  position: relative;

  width: 100%;

  height: 100vh;

  min-height: 600px;

  background: #ffffff;

  overflow: hidden;

  font-family: Arial, Helvetica, sans-serif;

  color: #17202a;
}

/* =============================================================
   HEADER
============================================================= */

.dashboard-header {
  height: 65px;

  min-height: 65px;

  display: flex;

  align-items: center;

  justify-content: space-between;

  padding: 0 22px;

  background: #07111f;

  color: #ffffff;

  border-bottom: 1px solid #1f2937;
}

.dashboard-title {
  display: flex;

  align-items: center;

  gap: 15px;

  font-size: 23px;

  font-weight: 700;

  letter-spacing: 0.2px;
}

.dashboard-title-icon {
  font-size: 28px;

  color: #ffffff;
}

.dashboard-header-right {
  display: flex;

  align-items: center;

  gap: 20px;
}

.live-status {
  display: flex;

  align-items: center;

  gap: 8px;

  color: #00f28a;

  font-size: 17px;

  font-weight: 700;
}

.live-dot {
  width: 10px;

  height: 10px;

  border-radius: 50%;

  background: #00ef86;

  box-shadow: 0 0 8px rgba(0, 239, 134, 0.8);
}

.header-separator {
  width: 1px;

  height: 28px;

  background: rgba(255, 255, 255, 0.4);
}

.dashboard-clock {
  font-size: 17px;

  font-weight: 500;

  letter-spacing: 0.4px;
}

.refresh-button {
  border: 0;

  outline: none;

  background: transparent;

  color: #ffffff;

  font-size: 22px;

  cursor: pointer;

  padding: 4px 8px;
}

.refresh-button:hover {
  opacity: 0.8;
}

.spin-icon {
  animation: dashboard-spin 1s linear infinite;
}

@keyframes dashboard-spin {
  from {
    transform: rotate(0deg);
  }

  to {
    transform: rotate(360deg);
  }
}

/* =============================================================
   FILTER
============================================================= */

.dashboard-filter {
  background: #ffffff;

  border-bottom: 1px solid #d9dfe5;

  padding: 15px 20px;
}

.filter-row {
  display: flex;

  align-items: end;

  gap: 20px;

  width: 100%;
}

.filter-item {
  display: flex;

  align-items: center;

  gap: 10px;
}

.filter-item label {
  white-space: nowrap;

  font-weight: 600;

  margin: 0;
}

.filter-control {
  min-width: 220px;
}

.filter-line {
  flex: 1;
}

.filter-area-item {
  flex: 1;
}

.filter-status {
  flex: 1;
}

.filter-button {
  display: flex;

  align-items: center;

  justify-content: center;
}

/* =============================================================
   CONTENT
============================================================= */

.dashboard-content {
  height: calc(100vh - 80px);

  padding: 15px 18px;

  overflow: hidden;
}

/* =============================================================
   SUMMARY GRID
============================================================= */

.summary-grid {
  display: grid;

  grid-template-columns: repeat(4, 1fr);

  gap: 14px;

  width: 100%;
}

.summary-card {
  min-height: 100px;

  border-radius: 3px;

  display: flex;

  align-items: center;

  padding: 18px 20px;

  color: #ffffff;

  overflow: hidden;

  position: relative;

  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.15);
}

/* =============================================================
   SUMMARY COLORS
============================================================= */

.summary-purple {
  background: #7850aa;
}

.summary-gray {
  background: #777777;
}

.summary-blue {
  background: #18a6d7;
}

/* =============================================================
   SUMMARY ICON
============================================================= */

.summary-icon {
  width: 60px;

  height: 60px;

  min-width: 60px;

  border-radius: 18px;

  display: flex;

  align-items: center;

  justify-content: center;

  background: rgba(255, 255, 255, 0.13);

  font-size: 30px;

  margin-right: 25px;
}

.summary-information {
  flex: 1;

  min-width: 0;
}

.summary-title {
  font-size: 21px;

  font-weight: 700;

  text-align: center;

  white-space: nowrap;
}

.summary-value {
  font-size: 48px;

  line-height: 1.05;

  font-weight: 700;

  text-align: center;

  margin-top: 5px;
}

.progress-information {
  width: 100%;
}

.progress-value {
  font-size: 45px;
}

.progress-divider {
  margin: 0 3px;
}

/* =============================================================
   DATA PANEL
============================================================= */

.data-panel {
  margin-top: 14px;

  height: calc(100vh - 255px);

  min-height: 350px;

  display: flex;

  flex-direction: column;

  border-radius: 2px;

  overflow: hidden;

  background: #ffffff;
}

/* =============================================================
   DATA PANEL HEADER
============================================================= */

.data-panel-header {
  min-height: 48px;

  height: 48px;

  background: #07111f;

  color: #ffffff;

  display: flex;

  align-items: center;

  justify-content: space-between;

  padding: 0 18px;
}

.data-panel-title {
  display: flex;

  align-items: center;

  gap: 14px;

  font-size: 16px;

  font-weight: 700;

  white-space: nowrap;

  overflow: hidden;

  text-overflow: ellipsis;
}

.data-panel-icon {
  color: #ffb400;

  font-size: 21px;
}

.total-data {
  font-size: 16px;

  white-space: nowrap;
}

/* =============================================================
   TABLE CONTAINER
============================================================= */

.table-container {
  flex: 1;

  min-height: 0;

  overflow: hidden;

  padding: 12px 3px 0;
}

/* =============================================================
   TABLE
============================================================= */

.womin-table {
  width: 100%;

  table-layout: fixed;

  border-collapse: collapse;

  background: #ffffff;
}

.womin-table thead th {
  height: 43px;

  padding: 7px 10px;

  background: #b9e1ed;

  color: #15212b;

  border: 1px solid #d6e0e5;

  font-size: 16px;

  font-weight: 700;

  text-align: center;

  vertical-align: middle;
}

.womin-table tbody td {
  height: 43px;

  padding: 7px 12px;

  border: 1px solid #e0e6ea;

  font-size: 12px;

  white-space: nowrap;

  overflow: hidden;

  text-overflow: ellipsis;

  vertical-align: middle;
}

.womin-table tbody tr {
  background: #ffffff;
}

.womin-table tbody tr:nth-child(even) {
  background: #fafcfd;
}

.womin-table tbody tr:hover {
  background: #edf7fb;
}

/* =============================================================
   COLUMN WIDTH
============================================================= */

.col-line {
  width: 4.84%;
}

.col-proddate {
  width: 7.48%;
}

.col-request {
  width: 8.68%;
}

.col-partgroup {
  width: 4.68%;
}



.col-model {
  width: 21.29%;
}

.col-ws {
  width: 5.65%;
}

.col-status {
  width: 16.13%;
}

.col-trolly {
  width: 10.87%;
}

.col-position {
  width: 10.26%;
}

.col-next {
  width: 10.02%;
}

/* =============================================================
   LINKS
============================================================= */

.table-link {
  color: #0768df;

  font-weight: 500;

  text-decoration: none;
}

.table-link:hover {
  text-decoration: underline;
}

.request-link {
  font-weight: 600;
}

/* =============================================================
   STATUS BADGE
============================================================= */

.status-badge {
  display: inline-flex;

  align-items: center;

  justify-content: center;

  min-height: 27px;

  padding: 3px 14px;

  border-radius: 6px;

  font-size: 11px;

  font-weight: 600;

  white-space: nowrap;
}

/* NEW */

.status-new {
  background: #1769e0;

  color: #ffffff;
}

/* PICKING */

.status-progress {
  background: #f79400;

  color: #ffffff;
}

/* COMPLETE AREA */

.status-area-complete {
  background: #c9f59d;

  color: #075b42;
}

/* COMPLETE */

.status-complete {
  background: #d4e2fa;

  color: #075ac9;
}

/* WAITING */

.status-waiting {
  background: #eeeeee;

  color: #555555;
}

.status-default {
  background: #eeeeee;

  color: #444444;
}

/* =============================================================
   EMPTY
============================================================= */

.empty-data {
  height: 200px;

  text-align: center;
}

/* =============================================================
   FOOTER
============================================================= */

.table-footer {
  height: 66px;

  min-height: 66px;

  background: #07111f;

  color: #ffffff;

  display: grid;

  grid-template-columns:
    1fr
    1fr
    1fr;

  align-items: center;

  padding: 0 18px;
}

/* =============================================================
   FOOTER LEFT
============================================================= */

.footer-left {
  display: flex;

  align-items: center;

  gap: 8px;

  font-size: 15px;
}

.showing-label {
  white-space: nowrap;
}

.page-size-select {
  width: 96px;

  height: 40px;

  background: #172333;

  color: #ffffff;

  border: 1px solid #4a5664;

  border-radius: 5px;

  padding: 0 12px;

  font-size: 15px;

  outline: none;
}

/* =============================================================
   FOOTER CENTER
============================================================= */

.footer-center {
  text-align: center;
}

.next-page-label {
  font-size: 15px;

  line-height: 20px;
}

.countdown {
  color: #1587ff;

  font-size: 29px;

  line-height: 30px;

  font-weight: 700;
}

/* =============================================================
   FOOTER RIGHT
============================================================= */

.footer-right {
  display: flex;

  justify-content: flex-end;

  align-items: center;

  gap: 10px;
}

.pagination-button {
  width: 43px;

  height: 40px;

  border: 1px solid #4a5664;

  background: #172333;

  color: #ffffff;

  border-radius: 5px;

  font-size: 17px;

  cursor: pointer;
}

.pagination-button:hover:not(:disabled) {
  background: #25364b;
}

.pagination-button:disabled {
  opacity: 0.45;

  cursor: not-allowed;
}

.current-page {
  width: 48px;

  height: 40px;

  display: flex;

  align-items: center;

  justify-content: center;

  background: #1769e0;

  color: #ffffff;

  border-radius: 5px;

  font-size: 17px;

  font-weight: 700;
}

.page-total {
  font-size: 17px;

  font-weight: 600;
}

/* =============================================================
   LARGE SCREEN
============================================================= */

@media (min-width: 1600px) {
  .dashboard-header {
    height: 70px;
  }

  .dashboard-title {
    font-size: 25px;
  }

  .dashboard-clock {
    font-size: 19px;
  }

  .summary-card {
    min-height: 160px;
  }

  .summary-title {
    font-size: 22px;
  }

  .summary-value {
    font-size: 52px;
  }

  .womin-table thead th {
    height: 44px;

    font-size: 17px;
  }

  .womin-table tbody td {
    height: 44px;

    font-size: 16px;
  }

  .status-badge {
    font-size: 15px;
  }
}

/* =============================================================
   VERY LARGE SCREEN / 4K
============================================================= */

@media (min-width: 2200px) {
  .dashboard-header {
    height: 80px;
  }

  .dashboard-title {
    font-size: 30px;
  }

  .dashboard-title-icon {
    font-size: 34px;
  }

  .summary-card {
    min-height: 190px;
  }

  .summary-icon {
    width: 100px;

    height: 100px;

    min-width: 100px;

    font-size: 52px;
  }

  .summary-title {
    font-size: 25px;
  }

  .summary-value {
    font-size: 62px;
  }

  .data-panel-header {
    height: 56px;
  }

  .data-panel-title,
  .total-data {
    font-size: 19px;
  }

  .womin-table thead th {
    height: 50px;

    font-size: 19px;
  }

  .womin-table tbody td {
    height: 50px;

    font-size: 18px;
  }

  .status-badge {
    font-size: 17px;

    padding: 4px 16px;
  }

  .table-footer {
    height: 75px;
  }
}

/* =============================================================
   RESPONSIVE
============================================================= */

@media (max-width: 1200px) {
  .summary-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .dashboard-content {
    overflow-y: auto;
  }

  .data-panel {
    height: auto;

    min-height: 500px;
  }

  .filter-row {
    flex-wrap: wrap;
  }
}
</style>
