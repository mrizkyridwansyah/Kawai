<template>
  <!-- =========================================================
       FILTER
       ========================================================= -->
  <div
    class="filter-area p-3"
    id="container_filter"
  >

    <div
      class="panel panel-inverse"
      style="border:none"
    >

      <div class="panel-heading ui-sortable-handle">

        <font-awesome-icon
          icon="file-lines"
          style="font-size:1.25em"
        />

        <span
          style="font-size:1.25em"
          class="ml-3"
        >
          Production
        </span>

      </div>


      <table
        width="100%"
        style="background-color:#ffffff"
      >

        <tr>

          <td style="width:2%"></td>

          <td style="width:12%">
            &nbsp;
          </td>

          <td style="width:36%"></td>

          <td style="width:2%"></td>

          <td style="width:12%"></td>

          <td style="width:36%"></td>

        </tr>


        <!-- LINE / DATE -->

        <tr>

          <td style="width:2%"></td>

          <td style="width:12%">
            <label class="form-label">
              Line
            </label>
          </td>

          <td style="width:36%">

            <filter-line
              class="form-control"
              v-model="filter.Line"
              v-model:line-name="filter.lineName"
            ></filter-line>

          </td>


          <td style="width:2%"></td>


          <td style="width:12%">

            <label class="form-label">
              Production Date
            </label>

          </td>


          <td style="width:36%">

            <input-date
              v-model="filter.ProductionDate"
              style-date="width:100px"
            />

          </td>

        </tr>


        <!-- MODEL / SEARCH -->

        <tr style="height:37px">

          <td style="width:2%"></td>

          <td style="width:12%">

            <label class="form-label">
              Model
            </label>

          </td>


          <td style="width:36%">

            <filter-modelcls
              class="form-control"
              v-model="filter.Model"
              v-model:model-descs="filter.modelDescs"
            ></filter-modelcls>

          </td>


          <td style="width:2%"></td>


          <td style="width:12%">

            <v-button-search
              class="ms-1"
              :search="search"
            />

          </td>


          <td style="width:36%"></td>

        </tr>


        <tr>

          <td style="width:2%"></td>

          <td style="width:12%">
            &nbsp;
          </td>

          <td style="width:36%"></td>

          <td style="width:2%"></td>

          <td style="width:12%"></td>

          <td style="width:36%"></td>

        </tr>

      </table>

    </div>

  </div>


  <!-- =========================================================
       ANDON
       ========================================================= -->

  <div
    class="andon-wrapper"
    id="container_andon"
    style="display:none"
  >


    <!-- =======================================================
         HEADER
         ======================================================= -->

    <div class="dashboard-header">


      <!-- BRAND -->

      <div class="brand-card">

        <div class="logo-box">

          <div class="logo-title">
            KAWAI
          </div>

          <div class="logo-subtitle">
            PIANO
          </div>

          <div class="logo-subtitle">
            MANUFACTURING
          </div>

        </div>


        <div class="andon-title">

          ANDON

          <span>
            PRODUCTION
          </span>

        </div>

      </div>


      <!-- LINE -->

      <div class="header-card line-card">

        <div class="header-label">
          PRODUCTION LINE
        </div>

        <div class="header-value cyan-text">

          {{ model.Production_Name }}
          

        </div>

      </div>


      <!-- DATE -->

      <div class="header-card">

        <div class="header-label">
          PRODUCTION DATE
        </div>

        <div class="header-value white-text">

        {{ formatDate(filter.ProductionDate) }}

        </div>

      </div>


      <!-- TIME -->

      <div class="header-card">

        <div class="header-label">
          CURRENT TIME
        </div>

        <div class="clock-time">

          {{ model.currentTime }}

        </div>

      </div>


      <!-- STATUS -->

      <div class="header-card">

        <div class="header-label">
          LINE STATUS
        </div>

        <div
          class="running-status"
          :class="
            model.LineStatus === 'RUNNING' ||
            model.LineStatus === 'Running'
              ? 'status-running'
              : 'status-stop'
          "
        >

          <span class="running-dot"></span>

          {{ model.LineStatus }}

        </div>

      </div>

    </div>


    <!-- =======================================================
         MAIN
         ======================================================= -->

    <div class="main-grid">


      <!-- =====================================================
           LEFT
           ===================================================== -->

      <div class="left-column">


        <!-- ===================================================
             PROGRESS
             =================================================== -->

        <div class="dashboard-panel progress-panel">

          


          <div class="progress-layout">


            <!-- CIRCLE -->

            <div class="progress-circle-wrap">

              <div class="progress-circle">

                <svg viewBox="0 0 120 120">

                  <circle
                    class="progress-bg"
                    cx="60"
                    cy="60"
                    r="50"
                  ></circle>


                  <circle
                    class="progress-ring"
                    cx="60"
                    cy="60"
                    r="50"
                    stroke-dasharray="314"
                    :stroke-dashoffset="
                      314 -
                      (
                        314 *
                        Number(
                          model.Production_Progress || 0
                        ) /
                        100
                      )
                    "
                  ></circle>

                </svg>


                <div class="progress-center">

                  <div class="progress-percent">

                     {{ model.Production_Progress }}%
 

                  </div>


                  <div class="progress-caption">
                    PROGRESS
                  </div>

                </div>

              </div>

            </div>


            <!-- INFO -->

            <div class="progress-info">


              <div class="info-box">

                <div class="info-label">
                  LOT / BATCH
                </div>

                <div class="info-value cyan-text">

                  {{ model.Production_Lot }}

                </div>

              </div>


              <div class="info-box">

                <div class="info-label">
                  SCHEDULE
                </div>

                <div class="info-value cyan-text">

                  {{ scheduleProgress }}

                </div>

              </div>


            </div>

          </div>

        </div>


        <!-- ===================================================
             SUMMARY
             =================================================== -->

        <div class="dashboard-panel summary-panel">


          <div class="summary-header">

            <div class="panel-title-large">

              PRODUCTION SUMMARY

            </div>


            <div class="live-update">

              LIVE UPDATE

            </div>

          </div>


          <div class="summary-grid">


            <!-- PLAN -->

            <div class="summary-box plan-box">

              <div class="summary-label">
                PRODUCTION PLAN
              </div>

              <div class="summary-value">

                {{ model.Production_Target }}

              </div>

              <div class="summary-unit">
                PCS
              </div>

            </div>


            <!-- ACTUAL -->

            <div class="summary-box actual-box">

              <div class="summary-label">
                ACTUAL RESULT
              </div>

              <div class="summary-value green-text">

                {{ model.Production_Actual }}

              </div>

              <div class="summary-unit">
                PCS
              </div>

            </div>


            <!-- CYCLE -->

            <div class="summary-box cycle-box">

              <div class="summary-label">
                CYCLE TIME
              </div>

              <div class="summary-value cyan-text">

                {{ model.Production_Cycle }}

              </div>

              <div class="summary-unit">
                MIN / PCS
              </div>

            </div>


            <!-- REMAINING -->

            <div class="summary-box remaining-box">

              <div class="summary-label">
                REMAINING
              </div>

              <div class="summary-value red-text">

                {{ model.Production_Remaining }}

              </div>

              <div class="summary-unit">
                PCS
              </div>

            </div>


          </div>

        </div>

      </div>


      <!-- =====================================================
           RIGHT
           ===================================================== -->

      <div class="dashboard-panel schedule-panel">


        <div class="schedule-header">

          <div class="schedule-title">

            PRODUCTION SCHEDULE

          </div>


          <div class="schedule-total">

            TOTAL:
            {{ model.TotalSchedule }}
            SCHEDULES

          </div>

        </div>


        <div class="schedule-scroll">

          <table class="schedule-table">

            <thead>

              <tr>

                <th>
                  MODEL
                </th>

                <th>
                  PLAN QTY
                </th>

                <th>
                  RESULT QTY
                </th>

                <th>
                  STATUS
                </th>

              </tr>

            </thead>


            <tbody>

              <tr
                v-for="(item, i) in scheduleData"
                :key="i"
                :class="rowClass(item)"
              >

                <td class="model-cell">

                  {{ item.Model }}

                </td>


                <td class="qty-cell">

                  {{ item.PlanQty }}

                </td>


                <td class="qty-cell result-cell">

                  {{ item.ResultQty }}

                </td>


                <td class="status-cell">


                  <span
                    v-if="
                      item.Production_Status ===
                      'Complete'
                    "
                    class="status-icon complete-icon"
                  >
                    ✓
                  </span>


                  <span
                    v-else-if="
                      item.Production_Status ===
                      'On Progress'
                    "
                    class="status-icon progress-icon"
                  >
                    ▶
                  </span>


                  <span
                    v-else
                    class="status-icon waiting-icon"
                  >
                    ⌛
                  </span>


                </td>

              </tr>

            </tbody>

          </table>

        </div>

      </div>

    </div>


    <!-- =======================================================
         FOOTER
         ======================================================= -->

    <div class="dashboard-footer">


      <div class="legend">


        <span>

          <i class="legend-dot green-dot"></i>

          Running / Complete

        </span>


        <span>

          <i class="legend-dot cyan-dot"></i>

          On Progress

        </span>


        <span>

          <i class="legend-dot yellow-dot"></i>

          Waiting

        </span>


        <span>

          <i class="legend-dot red-dot"></i>

          Remaining

        </span>


      </div>


      <div class="last-update">

        Last update:
        {{ model.currentTime }}

      </div>


    </div>

  </div>

</template>


<script>

export default {

  data: () => ({

    scheduleData: [],
    intervalLoad: null,
    globalClickHandler: null,
    isFullscreen: false,
    filter: {
      ProductionDate: new Date(),
      Line: null,
      Model: null,
      modelDescs: "",
      lineName: "",
    },


    model: {
      LineCode: "",
      LineName: "",
      ModelCls: "",
      ModelDescs: "",
      LineStatus: "",
      Production_Image: "",
      Production_Lot: "",
      Production_Name: "",
      Production_Color: "",
      Production_Progress: "",
      Production_Date: "",
      Production_Target: "",
      Production_Actual: "",
      Production_Cycle: "",
      Production_Remaining: "",
      TotalSchedule: "",
      progress: 20,
      currentDate: "",
      currentTime: "",
      timer: null,

    },

  }),


  computed: {
    ds() {return useProductionControl();},
    dsschedule() {return useProductionControl();},
    scheduleProgress() {

      const total =
        Number(
          this.model.TotalSchedule || 0
        );


      if (!total) {
        return "00 / 00";
      }


      const completed =
        (this.scheduleData || [])
          .filter(
            x =>
              x.Production_Status ===
              "Complete"
          )
          .length;


      return `${String(completed).padStart(2, "0")} / ${String(total).padStart(2, "0")}`;

    },

  },


  mounted() {

    this.updateClock();


    this.model.timer =
      setInterval(
        () => {

          this.updateClock();

        },
        1000
      );


    /*
     * =======================================================
     * CLICK HANDLER
     *
     * HANYA 1 KALI DIPASANG.
     *
     * Jangan dipasang di search().
     * =======================================================
     */

    this.globalClickHandler =
      event => {
        if (!this.isFullscreen) {
          return;
        }


        const filter =
          document.getElementById(
            "container_filter"
          );


        
        if (filter && filter.contains(event.target)
        ) {
          return;
        }
        this.exitFullscreen();

      };

    document.addEventListener(
      "click",
      this.globalClickHandler,
      true
    );

  },


  beforeUnmount() {

    clearInterval(
      this.model.timer
    );


    this.stopInterval();


    if (this.globalClickHandler) {

      document.removeEventListener(
        "click",
        this.globalClickHandler,
        true
      );


      this.globalClickHandler =
        null;

    }

  },


  methods: {


    formatDate(date) {
      if (!date) return "";

      const d = new Date(date);

      const months = [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
      ];

      return `${String(d.getDate()).padStart(2, "0")} ${
        months[d.getMonth()]
      } ${d.getFullYear()}`;
    },

    /* =======================================================
       START AUTO REFRESH
       ======================================================= */

    startInterval() {

      this.stopInterval();


      this.intervalLoad =
        setInterval(
          () => {


            if (
              this.isFullscreen
            ) {

              this.search(true);

            }

          },
          3000
        );

    },


    /* =======================================================
       STOP AUTO REFRESH
       ======================================================= */

    stopInterval() {

      if (this.intervalLoad) {

        clearInterval(
          this.intervalLoad
        );

        this.intervalLoad =
          null;

      }

    },


    /* =======================================================
       CLOCK
       ======================================================= */

    updateClock() {

      const now =
        new Date();


      this.model.currentTime =
        now.toLocaleTimeString(
          "en-GB"
        );


      this.model.currentDate =
        now.toLocaleDateString(
          "en-GB",
          {
            day: "2-digit",
            month: "short",
            year: "numeric",
          }
        );

    },


    /* =======================================================
       ROW CLASS
       ======================================================= */

    rowClass(item) {

      if (
        item.Production_Status === "Complete"
      ) {
        return "row-complete";
      }


      if (
        item.Production_Status === "On Progress"
      ) {
        return "row-progress";
      }


      return "row-waiting";

    },


    /* =======================================================
       ENTER FULLSCREEN
       ======================================================= */

    enterFullscreen() {

      const containerFilter =
        document.getElementById(
          "container_filter"
        );


      const containerAndon =
        document.getElementById(
          "container_andon"
        );


      const header =
        document.getElementById(
          "header"
        );


      const appContent =
        document.getElementById(
          "app"
        );


      /*
       * HEADER GLOBAL HIDE
       */

      if (header) {

        header.style.display =
          "none";

      }


      /*
       * BODY
       */

      document.body.style.backgroundColor =
        "black";

      document.body.style.overflow =
        "hidden";


      /*
       * APP
       */

      if (appContent) {

        appContent.style.paddingTop =
          "0px";

        appContent.style.marginTop =
          "0px";

      }


      /*
       * FILTER HIDE
       */

      if (containerFilter) {

        containerFilter.style.display =
          "none";

      }


      /*
       * ANDON SHOW
       */

      if (containerAndon) {

        containerAndon.style.display =
          "flex";

        /*
         * Ini yang membuat Andon
         * memenuhi layar F11.
         */

        containerAndon.style.height =
          "100vh";

        containerAndon.style.minHeight =
          "0";

      }


      /*
       * STATUS
       */

      this.isFullscreen =
        true;

    },


    /* =======================================================
       EXIT FULLSCREEN
       ======================================================= */

    exitFullscreen() {

      /*
       * STOP AUTO REFRESH
       */

      this.stopInterval();


      const containerFilter =
        document.getElementById(
          "container_filter"
        );


      const containerAndon =
        document.getElementById(
          "container_andon"
        );


      const header =
        document.getElementById(
          "header"
        );


      const appContent =
        document.getElementById(
          "app"
        );


      /*
       * FILTER SHOW
       */

      if (containerFilter) {

        containerFilter.style.display =
          "block";

      }


      /*
       * ANDON BENAR-BENAR HIDE
       *
       * Ini perbedaan penting dari
       * kode sebelumnya.
       */

      if (containerAndon) {

        containerAndon.style.display =
          "none";

        containerAndon.style.height =
          "";

        containerAndon.style.minHeight =
          "";

      }


      /*
       * HEADER SHOW
       */

      if (header) {

        header.style.display =
          "";

      }


      /*
       * BODY RESET
       */

      document.body.style.backgroundColor =
        "";

      document.body.style.overflow =
        "";


      /*
       * APP RESET
       */

      if (appContent) {

        appContent.style.paddingTop =
          "";

        appContent.style.marginTop =
          "";

      }


      /*
       * STATUS
       */

      this.isFullscreen =
        false;

    },


    /* =======================================================
       SEARCH
       ======================================================= */

    search(
      isAutoRefresh = false
    ) {


      /*
       * VALIDASI LINE
       */

      if (!this.filter.Line) {

        if (!isAutoRefresh) {

          toastDanger(
            "Please select Line!"
          );

        }

        return;

      }


      /*
       * VALIDASI MODEL
       */

      if (!this.filter.Model) {

        if (!isAutoRefresh) {

          toastDanger(
            "Please select Model!"
          );

        }

        return;

      }


      /*
       * =====================================================
       * HEADER DATA
       * =====================================================
       */

      this.ds
        .loadheaderinfo(
          this.filter.Line,
          this.filter.Model,
          this.filter.ProductionDate
        )

        .then(dt => {


          this.list =
            dt.Data || [];


          const x =
            this.list.length > 0
              ? this.list[0]
              : {};


          this.model.LineCode =
            x.LineCode || "";


          this.model.LineName =
            x.LineName || "";


          this.model.ModelCls =
            x.ModelCls || "";


          this.model.ModelDescs =
            x.ModelDescs || "";


          this.model.LineStatus =
            x.LineStatus || "";


          this.model.Production_Image =
            x.Production_Image || "";


          this.model.Production_Lot =
            x.Production_Lot || "";


          this.model.Production_Name =
            x.Production_Name || "";


          this.model.Production_Color =
            x.Production_Color || "";


          this.model.Production_Progress =
            x.Production_Progress || 0;


          this.model.Production_Target =
            x.Production_Target || 0;


          this.model.Production_Actual =
            x.Production_Actual || 0;


          this.model.Production_Cycle =
            x.Production_Cycle || 0;


          this.model.Production_Remaining =
            x.Production_Remaining || 0;


          this.model.ImageBase64 =
            x.ImageBase64 || "";

        })


        .catch(
          err =>
            console.error(
              "Error loading header data:",
              err
            )
        );


      /*
       * =====================================================
       * SCHEDULE
       * =====================================================
       */

      this.dsschedule
        .loadlistschedule(
          this.filter.Line,
          this.filter.Model,
          this.filter.ProductionDate
        )

        .then(dt => {


          this.scheduleData =
            dt.Data || [];


          this.model.TotalSchedule =
            this.scheduleData.length;

        })


        .catch(
          err =>
            console.error(
              "Error loading schedule data:",
              err
            )
        );


     
      


      /*
       * =====================================================
       * MASUK DASHBOARD
       * =====================================================
       */

      this.enterFullscreen();


      /*
       * =====================================================
       * START AUTO REFRESH
       *
       * Hanya kalau belum ada interval.
       * =====================================================
       */

      if (!this.intervalLoad) {

        setTimeout(
          () => {


            if (
              this.isFullscreen
            ) {

              this.startInterval();

            }

          },
          100
        );

      }

    },

  },

};

</script>


<style scoped>

/* =========================================================
   GLOBAL BOX SIZING
   ========================================================= */

* {

  box-sizing: border-box;

  font-family:
    Arial,
    Helvetica,
    sans-serif;

}


/* =========================================================
   ANDON WRAPPER
   ========================================================= */

.andon-wrapper {

  /*
   * TIDAK menggunakan position:fixed.
   */

  width: 100%;

  height: 100vh;

  min-height: 0;

  overflow: hidden;

  background: #050b0f;

  color: #fff;

  padding: 10px;

  display: flex;

  flex-direction: column;

  gap: 6px;

}


/* =========================================================
   HEADER
   ========================================================= */

.dashboard-header {

  flex: 0 0 69px;

  height: 69px;

  width: 100%;

  min-width: 0;

  min-height: 0;

  display: grid;

  /*
   * Pakai fr supaya total tidak overflow.
   */

  grid-template-columns:

    minmax(0, 23fr)

    minmax(0, 31fr)

    minmax(0, 15fr)

    minmax(0, 14fr)

    minmax(0, 17fr);

  gap: 8px;

}


/* =========================================================
   CARD
   ========================================================= */

.brand-card,
.header-card,
.dashboard-panel,
.dashboard-footer {

  border:
    1px solid #273b47;

  background:
    #050b0f;

}


/* =========================================================
   BRAND
   ========================================================= */

.brand-card {

  display: flex;

  align-items: center;

  padding: 6px;

  min-width: 0;

}


.logo-box {

  width: 62px;

  height: 40px;

  border:
    1px solid #4b5c66;

  display: flex;

  flex-direction: column;

  justify-content: center;

  align-items: center;

  flex-shrink: 0;

}


.logo-title {

  font-size: 8px;

  font-weight: 800;

  line-height: 1;

}


.logo-subtitle {

  font-size: 6px;

  line-height: 1.3;

}


.andon-title {

  margin-left: 10px;

  font-size: 16px;

  font-weight: 800;

  white-space: nowrap;

}


.andon-title span {

  color: #00d5ff;

}


/* =========================================================
   HEADER CARD
   ========================================================= */

.header-card {

  min-width: 0;

  padding:
    6px 9px;

  display: flex;

  flex-direction: column;

  justify-content: center;

  overflow: hidden;

}


.header-label {

  font-size: 10px;

  color: #91a6b4;

  font-weight: 800;

  letter-spacing: .6px;

}


.header-value {

  font-size: 16px;

  font-weight: 700;

  line-height: 1.1;

  margin-top: 4px;

  white-space: nowrap;

  overflow: hidden;

  text-overflow: ellipsis;

}


.line-card .header-value {

  font-size: 20px;

}


.clock-time {

  font-size: 18px;

  font-weight: 800;

  line-height: 1;

}


.running-status {

  font-size: 20px;

  font-weight: 800;

  display: flex;

  align-items: center;

  gap: 7px;

  margin-top: 4px;

}


.running-dot {

  width: 10px;

  height: 10px;

  border-radius: 50%;

  background: #35f05a;

  box-shadow:
    0 0 9px #35f05a;

}


.status-running {

  color: #35f05a;

}


.status-stop {

  color: #ff3150;

}


/* =========================================================
   MAIN GRID
   ========================================================= */

.main-grid {

  flex: 1 1 0;

  min-width: 0;

  min-height: 0;

  display: grid;

  /*
   * 48fr + 52fr = 100%
   * gap tidak menyebabkan overflow.
   */

  grid-template-columns:

    minmax(0, 48fr)

    minmax(0, 52fr);

  gap: 8px;

  overflow: hidden;

}


/* =========================================================
   LEFT COLUMN
   ========================================================= */

.left-column {

  min-width: 0;

  min-height: 0;

  height: 100%;

  display: grid;

  /*
   * 43fr + 57fr = 100%
   */

  grid-template-rows:

    minmax(0, 43fr)

    minmax(0, 57fr);

  gap: 8px;

  overflow: hidden;

}


/* =========================================================
   PANEL
   ========================================================= */

.dashboard-panel {

  min-width: 0;

  min-height: 0;

  overflow: hidden;

}


/* =========================================================
   PANEL TITLE
   ========================================================= */

.panel-title-large {

  font-size: 20px;

  font-weight: 800;

  white-space: nowrap;

}


/* =========================================================
   PROGRESS
   ========================================================= */

.progress-panel {

  display: flex;

  flex-direction: column;

}


.progress-panel >
.panel-title-large {

  height: 48px;

  flex: 0 0 48px;

  display: flex;

  align-items: center;

  padding:
    0 12px;

  border-bottom:
    1px solid #273b47;

}


.progress-layout {

  flex: 1;

  min-height: 0;

  display: grid;

  grid-template-columns:
    minmax(0, 50fr)
    minmax(0, 50fr);

  align-items: center;

  padding: 8px;

}


.progress-circle-wrap {

  height: 100%;

  display: flex;

  align-items: center;

  justify-content: center;

}


.progress-circle {

  position: relative;

  width:
    min(250px, 28vh);

  height:
    min(250px, 28vh);

  max-width: 100%;

  max-height: 100%;

}


.progress-circle svg {

  width: 100%;

  height: 100%;

  transform:
    rotate(-90deg);

}


.progress-circle circle {

  fill: none;

  stroke-width: 10;

}


.progress-bg {

  stroke: #172b39;

}


.progress-ring {

  stroke: #35ed5a;

  stroke-linecap: butt;

}


.progress-center {

  position: absolute;

  inset: 0;

  display: flex;

  flex-direction: column;

  align-items: center;

  justify-content: center;

}


.progress-percent {

  font-size: 50px;

  font-weight: 900;

  line-height: 1;

}


.progress-percent span {

  font-size: 27px;

}


.progress-caption {

  font-size: 14px;

  font-weight: 800;

  letter-spacing: 1px;

  margin-top: 5px;

}


.progress-info {

  height: 80%;

  display: flex;

  flex-direction: column;

  justify-content: center;

  gap: 10px;

  padding-right: 8px;

}


.info-box {

  flex: 1;

  border:
    1px solid #273b47;

  border-left:
    4px solid #35ed5a;

  background: #09131a;

  display: flex;

  flex-direction: column;

  align-items: center;

  justify-content: center;

}


.info-label {

  font-size: 13px;

  color: #9bb0bd;

  font-weight: 800;

  letter-spacing: 1px;

}


.info-value {

  font-size: 40px;

  font-weight: 800;

  line-height: 1;

  margin-top: 5px;

}


/* =========================================================
   SUMMARY
   ========================================================= */

.summary-panel {

  display: flex;

  flex-direction: column;

}


.summary-header {

  height: 48px;

  flex: 0 0 48px;

  display: flex;

  align-items: center;

  justify-content: space-between;

  padding:
    0 12px;

  border-bottom:
    1px solid #273b47;

}


.live-update {

  color: #00d5ff;

  font-size: 11px;

  font-weight: 800;

  letter-spacing: 1px;

}


.summary-grid {

  flex: 1;

  min-height: 0;

  display: grid;

  grid-template-columns:
    1fr 1fr;

  grid-template-rows:
    1fr 1fr;

  gap: 7px;

  padding: 7px;

}


.summary-box {

  border:
    1px solid #30414c;

  background: #09131a;

  display: flex;

  flex-direction: column;

  align-items: center;

  justify-content: center;

}


.summary-label {

  font-size: 13px;

  color: #9bb0bd;

  font-weight: 800;

  letter-spacing: 1px;

}


.summary-value {

  font-size: 54px;

  font-weight: 800;

  line-height: 1;

  margin-top: 5px;

}


.summary-unit {

  font-size: 13px;

  font-weight: 800;

  margin-top: 4px;

}


.plan-box {

  border-top:
    3px solid #d8d8d8;

}


.actual-box {

  border-top:
    3px solid #35ed5a;

}


.cycle-box {

  border-top:
    3px solid #00d5ff;

}


.remaining-box {

  border-top:
    3px solid #ff3150;

}


/* =========================================================
   SCHEDULE
   ========================================================= */

.schedule-panel {

  height: 100%;

  min-width: 0;

  min-height: 0;

  display: flex;

  flex-direction: column;

  overflow: hidden;

}


.schedule-header {

  height: 48px;

  flex: 0 0 48px;

  display: flex;

  align-items: center;

  justify-content: space-between;

  padding:
    0 12px;

  border-bottom:
    1px solid #273b47;

}


.schedule-title {

  font-size: 24px;

  font-weight: 800;

}


.schedule-total {

  color: #00d5ff;

  font-size: 12px;

  font-weight: 800;

  letter-spacing: .7px;

}


.schedule-scroll {

  flex: 1;

  min-height: 0;

  overflow-y: auto;

  overflow-x: hidden;

}


.schedule-scroll::-webkit-scrollbar {

  width: 7px;

}


.schedule-scroll::-webkit-scrollbar-track {

  background: #080d11;

}


.schedule-scroll::-webkit-scrollbar-thumb {

  background: #30434f;

  border-radius: 4px;

}


/* =========================================================
   TABLE
   ========================================================= */

.schedule-table {

  width: 100%;

  border-collapse: collapse;

  table-layout: fixed;

}


.schedule-table th {

  height: 35px;

  background: #263844;

  color: #fff;

  font-size: 13px;

  font-weight: 800;

}


.schedule-table td {

  height: 87px;

  border-bottom:
    1px solid #26343d;

  text-align: center;

  padding: 2px;

}


.schedule-table th:nth-child(1),
.schedule-table td:nth-child(1) {

  width: 38%;

  text-align: left;

  padding-left: 12px;

}


.schedule-table th:nth-child(2),
.schedule-table td:nth-child(2) {

  width: 22%;

}


.schedule-table th:nth-child(3),
.schedule-table td:nth-child(3) {

  width: 22%;

}


.schedule-table th:nth-child(4),
.schedule-table td:nth-child(4) {

  width: 18%;

}


/* =========================================================
   TABLE DATA
   ========================================================= */

.model-cell {

  color: #00d5ff;

  font-size: 48px;

  font-weight: 800;

  white-space: nowrap;

  overflow: hidden;

  text-overflow: ellipsis;

}


.qty-cell {

  color: #fff;

  font-size: 46px;

  font-weight: 800;

}


.result-cell {

  font-weight: 800;

}


.row-complete
.result-cell {

  color: #35ed5a;

}


.row-progress {

  background: #09212b;

  box-shadow:
    inset 4px 0 #00d5ff;

}


.row-progress
.result-cell {

  color: #00d5ff;

}


.row-waiting
.result-cell {

  color: #ffc21c;

}


.status-cell {

  font-size: 42px;

}


.complete-icon {

  color: #35ed5a;

  font-weight: 900;

}


.progress-icon {

  color: #00d5ff;

}


.waiting-icon {

  color: #ffc21c;

}


/* =========================================================
   FOOTER
   ========================================================= */

.dashboard-footer {

  height: 31px;

  min-height: 31px;

  flex: 0 0 31px;

  display: flex;

  align-items: center;

  justify-content: space-between;
margin-bottom: 10px;
  padding:
    0 10px;

}


.legend {

  display: flex;
 background-color: transparent;
  align-items: center;

  gap: 18px;
  border: none;
  font-size: 10px;

}


.legend span {

  display: flex;

  align-items: center;

  gap: 4px;

}


.legend-dot {

  width: 7px;

  height: 7px;

  border-radius: 50%;

  display: inline-block;

}


.green-dot {

  background: #35ed5a;

}


.cyan-dot {

  background: #00d5ff;

}


.yellow-dot {

  background: #ffc21c;

}


.red-dot {

  background: #ff3150;

}


.last-update {

  font-size: 10px;

  color: #a6b3ba;

}


/* =========================================================
   COLORS
   ========================================================= */

.cyan-text {

  color: #00d5ff !important;

}


.green-text {

  color: #35ed5a !important;

}


.yellow-text {

  color: #ffc21c !important;

}


.white-text {

  color: #fff !important;

}


.red-text {

  color: #ff3150 !important;

}


/* =========================================================
   RESPONSIVE
   ========================================================= */

@media (max-width:1100px) {


  .andon-title {

    font-size: 16px;

  }


  .line-card .header-value {

    font-size: 15px;

  }


  .header-value {

    font-size: 14px;

  }


  .clock-time {

    font-size: 18px;

  }


  .running-status {

    font-size: 16px;

  }


  .schedule-title {

    font-size: 19px;

  }


  .model-cell {

    font-size: 35px;

  }


  .qty-cell {

    font-size: 34px;

  }


  .summary-value {

    font-size: 42px;

  }


  .progress-percent {

    font-size: 38px;

  }

}

</style>