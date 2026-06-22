<template>
  <div class="filter-area p-3" id="container_filter">
      <!--dropdown area-->
     <div class="panel panel-inverse" style="border:none">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="file-lines" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Production
      </span> 
    </div>

    <table width="100%" style="background-color: #ffffff;" >
            
      <tr>
          <td style="width:2%;"></td>
          <td style="width:12%;"> &nbsp;</td>
          <td style="width:36%;">  </td>
          <td style="width:2%;"></td>
          <td style="width:12%;"></td>
          <td style="width:36%;"></td>
           </tr> 
        <tr>
          <td style="width:2%;"></td>
          <td style="width:12%;"><label class="form-label">Line</label></td>
          <td style="width:36%;"> 
           <filter-line
              class="form-control"
              v-model="filter.Line"
              v-model:line-name="filter.lineName"
              
            ></filter-line>
             </td>
          <td style="width:2%;"></td>
          <td style="width:12%;"><label class="form-label">Production Date</label></td>
          <td style="width:36%;"><input-date
                    v-model="filter.ProductionDate"
                    style-date="width: 100px"
                  /></td>
         
        </tr>
          <tr style="height: 37px;">
          <td style="width:2%;"></td>
          <td style="width:12%;"><label class="form-label">Model</label></td>
          <td style="width:36%;">    
              <filter-modelcls
              class="form-control"
              v-model="filter.Model"
              v-model:model-descs="filter.modelDescs"
              
            ></filter-modelcls> </td>
          <td style="width:2%;"></td>
          <td style="width:12%;">   <v-button-search class="ms-1" :search="search" /> </td>
          <td style="width:36%;"> </td>
         
        </tr>
        <tr>
          <td style="width:2%;"></td>
          <td style="width:12%;"> &nbsp;</td>
          <td style="width:36%;">  </td>
          <td style="width:2%;"></td>
          <td style="width:12%;"></td>
          <td style="width:36%;"></td>
           </tr> 
         
      </table>
    </div> 
        
 
    </div>
  <div class="container-fluid andon-wrapper"id="container_andon">

    <!-- ================= HEADER ================= -->
    <div class="row g-custom header-row">

      <div class="col-12">

        <div class="top-header">

          <!-- LEFT -->
          <div class="header-left">

            <div class="logo-box">
              <div class="logo-title">KAWAI PIANO</div>
              <div class="logo-subtitle">MANUFACTURING</div>
            </div>

            <div class="andon-title">
              ANDON PRODUCTION
            </div>

          </div>

          <!-- RIGHT -->
          <div class="header-right">

            <div class="header-card">
               
              <div class="header-value cyan-text">
                {{ model.LineName }}
              </div>
            </div>

            <div class="header-card">
              
              <div class="header-value cyan-text">
                {{ model.ModelDescs }}
              </div>
            </div>

            <div class="header-card">

              <div class="clock-time">
                {{ model.currentTime }}
              </div>

              <div class="clock-date">
                {{ model.currentDate }}
              </div>

            </div>

            <div class="header-card">

              <div class="header-label" style="color: #7dff71;" >
                LINE STATUS
              </div>

              <div class="running-status">
                <span class="running-dot"></span>
                {{ model.LineStatus }}
              </div>

            </div>

          </div>

        </div>

      </div>

    </div>

    <!-- ================= TOP CONTENT ================= -->
    <div class="row g-custom top-content-row">

      <!-- LEFT -->
      <div class="col-4">

        <div class="panel">

          <div class="panel-header">

            <div class="panel-number">1</div>

            <div class="panel-title">
              PRODUCTION RUNNING
            </div>

          </div>

          <div class="panel-body running-body">

            <div class="running-grid">

              <!-- LEFT -->
              <div class="running-image-area">

                <div class="section-title">
                  PRODUCTION ITEM
                </div>

                <div class="image-wrapper">

              <img
              class="product-image"
              :src="imageSource"
            />

                </div>

                <div class="lot-wrapper">

                  <div class="small-label">
                    LOT / BATCH
                  </div>

                  <div class="lot-number">
                   {{ model.Production_Lot }}
                  </div>

                </div>

              </div>

              <!-- RIGHT -->
              <div class="running-detail-area">

                <div class="section-title">
                  DESCRIPTION
                </div>

                <div class="product-name">
                 {{ model.Production_Name }}
                </div>

                <div class="divider"></div>

                <div class="small-label">
                  COLOR
                </div>

                <div class="product-color">
                  {{ model.Production_Color }}
                </div>

              </div>

            </div>

          </div>

        </div>

      </div>

       <!-- CENTER -->
      <div class="col-4 ml-0">

        <div class="panel">

          <div class="panel-header">

            <div class="panel-number">2</div>

            <div class="panel-title">
              PRODUCTION PROGRESS
            </div>

          </div>

          <div class="panel-body running-body">

            <div class="running-grid2">

              <!-- LEFT -->
              <div class="running-image-area">

             <table width="100%" style="height: 100%;">
              <tr>
                <td>   <!-- CIRCLE -->
              
                <div class="progress-circle">

                  <svg viewBox="0 0 120 120">

                    <circle
                      class="bg"
                      cx="60"
                      cy="60"
                      r="50"
                    />

                    <circle
                      class="progress"
                      cx="60"
                      cy="60"
                      r="50"
                      stroke-dasharray="314"
                      :stroke-dashoffset="314 - (314 *   model.Production_Progress  / 100)"
                    />

                  </svg>

                  <div class="progress-content">

                    <div class="progress-percent">
                      {{ model.Production_Progress }}%
                    </div>

                    <div class="progress-caption">
                      Progress
                    </div>

                  </div>

                </div>
              </td>
                <td>   
               <!-- TARGET -->
              <div class="target-area">

                <div class="target-box">

                  <div class="target-item">

                    <div class="target-label">
                      TARGET
                    </div>

                    <div class="target-value">
                      {{ model.Production_Target }}
                      <span>PCS</span>
                    </div>

                  </div>

                  <div class="target-divider"></div>

                  <div class="target-item">

                    <div class="target-label">
                      ACTUAL
                    </div>

                    <div class="target-value green-text">
                      {{ model.Production_Actual }}
                      <span>PCS</span>
                    </div>

                  </div>

                </div>

              </div></td>
              </tr>
              <tr>
                <td colspan="2">
                           <!-- PROGRESS BAR -->
            <div class="progress-bottom">

              <div class="progress-bar-wrapper">

                <div
                  class="progress-fill"
                  :style="{ width: model.Production_Progress + '%' }"
                ></div>

                <div
                  class="progress-marker"
                  :style="{ left: model.Production_Progress + '%' }"
                ></div>

              </div>

              <div class="progress-scale">
                <span>0%</span>
                <span>25%</span>
                <span>50%</span>
                <span>75%</span>
                <span>100%</span>
              </div>

            </div>
                </td>
                 
              </tr>
             </table>

              </div>

              <!-- RIGHT -->
              <div class="running-detail-area">

                  <div class="cycle-area">

                <div class="cycle-title">
                  CYCLE TIME
                </div>

                <div class="cycle-value">
                  {{ model.Production_Cycle }}
                </div>

                <div class="cycle-unit">
                  Menit / PCS
                </div>
                <div class="divider"></div>
                <div class="remaining-title">
                  REMAINING
                </div>

                <div class="remaining-value">
                  {{ model.Production_Remaining }}
                </div>

                <div class="remaining-unit">
                  PCS
                </div>

              </div>

            

              </div>

            </div>

          </div>

        </div>

      </div>


      <!-- RIGHT -->
      <div class="col-4 ml-0">

        <div class="panel">

          <div class="panel-header">

            <div class="panel-number">3</div>

            <div class="panel-title">
              LIST SCHEDULE
            </div>

            <div class="panel-right-title">
              Total : {{ model.TotalSchedule }} Schedule
            </div>

          </div>

          <div class="panel-body p-0">
<div class="schedule-scroll">
            <table class="schedule-table">

              <thead>
                <tr>
                  <th>SCHEDULE DATE</th>
                  <th>MODEL</th>
                  <th>PLAN QTY</th>
                  <th>RESULT QTY</th>
                  <th>STATUS</th>
                </tr>
              </thead>

              <tbody>

                <tr v-for="(item, i) in scheduleData" :key="i">
                    <td
                      :class="{
                        'white-text': item.Production_Status === 'Complete',
                        'cyan-text': item.Production_Status === 'On Progress',
                        'yellow-text': item.Production_Status === 'Waiting'
                      }"
                    >{{ item.ScheduleDate }}</td>
                                        <td
                      :class="{
                        'white-text': item.Production_Status === 'Complete',
                        'cyan-text': item.Production_Status === 'On Progress',
                        'yellow-text': item.Production_Status === 'Waiting'
                      }"
                    >{{ item.Model }}</td>
                                        <td
                      :class="{
                        'white-text': item.Production_Status === 'Complete',
                        'cyan-text': item.Production_Status === 'On Progress',
                        'yellow-text': item.Production_Status === 'Waiting'
                      }"
                    >{{ item.PlanQty }}</td>
                                        <td
                      :class="{
                        'white-text': item.Production_Status === 'Complete',
                        'cyan-text': item.Production_Status === 'On Progress',
                        'yellow-text': item.Production_Status === 'Waiting'
                      }"
                    >{{ item.ResultQty }}</td>
                    <td
                    :class="{
                      'white-text': item.Production_Status === 'Complete',
                      'cyan-text': item.Production_Status === 'On Progress',
                      'yellow-text': item.Production_Status === 'Waiting'
                    }"
                  >{{ item.Production_Status }}</td>
                    
                  </tr>
 

              </tbody>

            </table>
</div>
          </div>

        </div>

      </div>

    </div>

    <!-- ================= BOTTOM CONTENT ================= -->
    <div class="row g-custom bottom-row">

      <div class="col-12">

        <div class="panel">

          <!-- HEADER -->
          <div class="panel-header">

            <div class="panel-number">4</div>

            <div class="panel-title">
              MATERIAL READY - TROLLEY ( BUFFER AREA )
            </div>

            <div class="panel-right-title">
              TOTAL :  {{ model.TotalTrolley }} TROLLEY
            </div>

          </div>

          <!-- BODY -->
          <div class="panel-body trolley-body">

            <div class="trolley-grid">

              <!-- WORKSTATION -->
              <div
              class="trolley-column"
              v-for="station in trolleyGroup"
              :key="station.WorkStationCode"
            >

                <!-- TITLE -->
                <div class="workstation-title">
                   {{ station.WorkStationName }}
                </div>

                <!-- ITEM -->
               <div
                class="trolley-box"
                v-for="item in station.Items"
                :key="item.Trolley_No"
              >

              <div class="trolley-code cyan-text">
                {{ item.Trolley_No }}
              </div>

              <div class="trolley-buffer">
                {{ item.AddressCode }}
              </div>

              <div class="trolley-qty green-text">
                {{ item.Qty }}
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
    scheduleData: [],
    trolleyData: [],
     intervalLoad: null,
     filter: {
     
      ProductionDate: new Date(),
      Line: null,
      Model: null, 
      modelDescs: "", 
      lineName: "", 
    },
    model: {
      LineCode	: "",
      LineName	: "",
      ModelCls	: "",
      ModelDescs	: "",
      LineStatus	: "",
      Production_Image	: "",
      Production_Lot	: "",
      Production_Name	: "",
      Production_Color	: "",
      Production_Progress	: "",
      Production_Target	: "",
      Production_Actual	: "",
      Production_Cycle	: "",
      Production_Remaining: "",
      TotalSchedule: "",
      TotalTrolley: "",
      ImageBase64:null,

      progress: 20,
      currentDate: "",
      currentTime: "",
      timer: null,
      
    },
  }),
 computed: {
    ds: function () {
      return useProductionControl();
    },
    dsschedule: function () {
      return useProductionControl();
    },
    dstrolley: function () {
      return useProductionControl();
    },
     trolleyGroup() {
    const groups = {};

    (this.trolleyData || []).forEach(item => {

      const key = item.WorkStationCode;

      if (!groups[key]) {
        groups[key] = {
          WorkStationCode: item.WorkStationCode,
          WorkStationName: item.WorkStationName,
          Items: []
        };
      }

      groups[key].Items.push(item);
    });

    const result = Object.values(groups);

    // cari jumlah item terbanyak
    const maxRows = Math.max(
      ...result.map(x => x.Items.length),
      0
    );

    // isi kekurangan dengan blank
    result.forEach(station => {

      while (station.Items.length < maxRows) {

        station.Items.push({
          IsBlank: true,
          Trolley_No: "",
          AddressCode: "",
          Qty: ""
        });

      }

    });

    return result;
  },
   imageSource() {

    if (!this.model.ImageBase64)
      return "/images/no-image.png";

    // jika sudah lengkap data:image...
    if (this.model.ImageBase64.startsWith("data:image"))
      return this.model.ImageBase64;

    // jika hanya base64 murni
    return `data:image/jpeg;base64,${this.model.ImageBase64}`;
  }
  },

 
  mounted: async function () {
    
    this.updateClock();
    this.timer = setInterval(() => {
      this.updateClock();
    }, 1000);
  },

  beforeUnmount() {
    clearInterval(this.timer);
    this.stopInterval();
    
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
    updateClock() {
      const now = new Date();

      this.model.currentTime = now.toLocaleTimeString("en-GB");

      this.model.currentDate = now.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric",
      });
    },

     search: function () {
       
       
       if (!this.filter.Line) {
        toastDanger("Please select Line!");
        return;
        }

        if (!this.filter.Model) {
        toastDanger("Please select Model!");
        return;
        }
        this.ds
        .loadheaderinfo(this.filter.Line,this.filter.Model,this.filter.ProductionDate)
        .then((dt) => {
          this.list = dt.Data;
          this.model.LineCode = this.list.length > 0 ? this.list[0].LineCode : "";
          this.model.LineName	 = this.list.length > 0 ? this.list[0].LineName : "";
          this.model.ModelCls = this.list.length > 0 ? this.list[0].ModelCls : "";
          this.model.ModelDescs = this.list.length > 0 ? this.list[0].ModelDescs : "";
          this.model.LineStatus = this.list.length > 0 ? this.list[0].LineStatus : "";
          this.model.Production_Image = this.list.length > 0 ? this.list[0].Production_Image : "";
          this.model.Production_Lot = this.list.length > 0 ? this.list[0].Production_Lot : "";
          this.model.Production_Name = this.list.length > 0 ? this.list[0].Production_Name : "";
          this.model.Production_Color = this.list.length > 0 ? this.list[0].Production_Color : "";
          this.model.Production_Progress = this.list.length > 0 ? this.list[0].Production_Progress : "";
          this.model.Production_Target = this.list.length > 0 ? this.list[0].Production_Target : "";
          this.model.Production_Actual = this.list.length > 0 ? this.list[0].Production_Actual : "";
          this.model.Production_Cycle = this.list.length > 0 ? this.list[0].Production_Cycle : "";
          this.model.Production_Remaining  = this.list.length > 0 ? this.list[0].Production_Remaining : "";
          this.model.ImageBase64  = this.list.length > 0 ? this.list[0].ImageBase64 : "";
         
        })
        .catch((err) => {
          console.error("Error loading data:", err);
        })
        .finally(() => (this.isLoading = false));

      this.dsschedule
        .loadlistschedule(this.filter.Line,this.filter.Model,this.filter.ProductionDate)
        .then((dt) => {
        this.scheduleData = dt.Data || [];
         this.model.TotalSchedule = this.scheduleData.length;

          
        })
        .catch((err) => {
          console.error("Error loading data:", err);
        })
        .finally(() => (this.isLoading = false));

      this.dstrolley
        .loadlisttrolley(this.filter.Line,this.filter.Model,this.filter.ProductionDate)
        .then((dt) => {
           this.trolleyData = dt.Data || [];
            this.model.TotalTrolley = this.trolleyData.length;

         
         })
        .catch((err) => {
          console.error("Error loading data:", err);
        })
        .finally(() => (this.isLoading = false));


        
         //hide filter id="containerfilter" after search
        const containerFilter = document.getElementById("container_filter");
        const containerAndon = document.getElementById("container_andon");
          
         
          const header = document.getElementById("header");
          const appContent = document.getElementById("app");
          header.style.display = "none";
          document.body.style.backgroundColor = "black";
          appContent.style.paddingTop = "0px";
          appContent.style.marginTop = "0px";
          containerFilter.style.display = "none";
          containerAndon.style.display = "block";
          document.body.style.overflow = "hidden";
        
            //jika saya klik sembarang tombo maka filter muncul lagi
          document.addEventListener("click", (event) => {
          const isClickInside = containerFilter.contains(event.target);
          if (!isClickInside) {
             this.stopInterval();
             containerFilter.style.display = "block";
             header.style.display = "";
             document.body.style.backgroundColor = "";
             document.body.style.overflow = "";
             appContent.style.paddingTop = "";
             appContent.style.marginTop = "";

          }
        });

     if (this.filter.Line != null) {
        // Gunakan setTimeout kecil untuk mencegah bentrok dengan handleGlobalClick
        setTimeout(() => {
          this.startInterval(); // Nyalakan interval tiap 3 detik
           console.log("masih loading bro!");
        }, 100);
      }
      
       
    },

  },
};
</script>
 
<style scoped>

 
#__nuxt,
#__layout {
  width: 100%;
  height: 100%;
  margin: 0;
  overflow: hidden;
  background: #000;
}

* {
  box-sizing: border-box;
  font-family: Arial, Helvetica, sans-serif;
}

/* =========================================================
MAIN LAYOUT
========================================================= */

.andon-wrapper {
  width: 100%;
  height: 100%;

  overflow: hidden;

  background: #000;
  color: #fff;

  padding: 0.18vw;

  display: flex;
  flex-direction: column;
}

/* =========================================================
BOOTSTRAP GAP
========================================================= */

.g-custom {
  --bs-gutter-x: 0.18vw;
  --bs-gutter-y: 0.18vw;
}

.row {
  margin: 0 !important;
}

[class*="col-"] {
  padding: 0 !important;
}

/* =========================================================
HEADER
========================================================= */

.header-row {
  flex: 0 0 9.5vh;
 height: 100%;
  width: 100%;
}

.top-header {
  width: 100%;
  height: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 0.8vw;
}

.logo-box {
  width: 7vw;
  height: 8vh;

  border: 0.06vw solid #5d5d5d;

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.logo-title {
  font-size: clamp(0.6rem, 0.9vw, 1.5rem);
  line-height: 1;
  font-weight: 700;
}

.logo-subtitle {
 font-size: clamp(0.2rem, 0.6vw, 1.1rem);
  line-height: 1;
}

.andon-title {
  font-size: clamp(1rem, 1.8vw, 3rem);
  font-weight: 700;
  letter-spacing: 0.03vw;
}

.header-right {
  display: flex;
  gap: 0.25vw;
}

.header-card {
  min-width: 13vw;
  height: 7vh;

  border: 0.06vw solid #5d5d5d;

  padding: 0.35vw 0.55vw;

  display: flex;
  flex-direction: column;
  justify-content: center;

  background: #050505;
}

.header-label {
  color: #8c8c8c;
  font-size: clamp(0.35rem, 0.52vw, 0.8rem);
}

.header-value {
  font-size: clamp(0.9rem, 1.25vw, 2rem);
  font-weight: 700;
}

.clock-time {
  font-size: clamp(0.95rem, 1.35vw, 2.2rem);
  line-height: 1;
  font-weight: 700;
}

.clock-date {
  font-size: clamp(0.4rem, 0.6vw, 0.9rem);
  margin-top: 0.2vh;
  color: #b9b9b9;
}

.running-status {
  display: flex;
  align-items: center;
  gap: 0.35vw;

  font-size: clamp(0.9rem, 1.25vw, 2rem);
  font-weight: 700;

  color: #7dff71;
}

.running-dot {
  width: 0.55vw;
  height: 0.55vw;

  border-radius: 50%;
  background: #7dff71;
}

/* =========================================================
TOP CONTENT
========================================================= */

.top-content-row {
  flex: 0 0 35vh;

  width: 100%;
 height: 100%;
  margin-top: 0.2vh;
  margin-bottom: 0.6vh;

  flex-shrink: 0;
}

/* =========================================================
BOTTOM CONTENT
========================================================= */

.bottom-row {
  flex: 1;

  min-height: 0;

  width: 100%;
  height: 100%;

  margin-top: 0.2vh;

  flex-shrink: 0;
}

/* =========================================================
PANEL
========================================================= */

.panel {
  width: 100%;
  height: 100%;

  min-height: 0;

  border: 0.06vw solid #666;
  background: #000;

  display: flex;
  flex-direction: column;
}

.panel-header {
  flex: 0 0 4.2vh;

  border-bottom: 0.06vw solid #666;

  display: flex;
  align-items: center;
  gap: 0.45vw;

  padding: 0 0.5vw;
}

.panel-number {
  width: 1.1vw;
  height: 1.1vw;

  border-radius: 50%;

  display: flex;
  align-items: center;
  justify-content: center;

  background: #74d84e;
  color: #ffffff;

  font-size: clamp(0.35rem, 0.55vw, 0.8rem);
  font-weight: 700;
}

.panel-title {
  font-size: clamp(0.7rem, 0.95vw, 1.5rem);
  font-weight: 700;
}

.panel-right-title {
  margin-left: auto;

  font-size: clamp(0.55rem, 0.8vw, 1.2rem);
  font-weight: 700;

  color: #00d0ff;
}

.panel-body {
  flex: 1;
  min-height: 0;
  overflow: hidden;
}

 

/* GANTI CSS .filter-wrapper lama dengan ini */

.filter-wrapper1 {
  display: grid;
  grid-template-columns: repeat(2, minmax(320px, 1fr));
  grid-auto-flow: column; /* isi atas ke bawah dulu */
  gap: 4px 20px;
  width: 100%;
  align-items: center;
}

/* jumlah baris otomatis sesuai jumlah item */

.filter-wrapper1:has(.filter-item:nth-child(10)) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(9)):not(
    :has(.filter-item:nth-child(10))
  ) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(8)):not(
    :has(.filter-item:nth-child(9))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(7)):not(
    :has(.filter-item:nth-child(8))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(6)):not(
    :has(.filter-item:nth-child(7))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(5)):not(
    :has(.filter-item:nth-child(6))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(4)):not(
    :has(.filter-item:nth-child(5))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(3)):not(
    :has(.filter-item:nth-child(4))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper1:has(.filter-item:nth-child(2)):not(
    :has(.filter-item:nth-child(3))
  ) {
  grid-template-rows: repeat(1, auto);
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
  min-height: 32px;
  width: 100%;
}

.filter-item label {
  width: 70px;
  min-width: 70px;
  white-space: nowrap;
}

/* =========================================================
RUNNING
========================================================= */

.running-body {
  padding: 0;
}

.running-grid {
  display: grid;
  grid-template-columns: 50% 50%;

  width: 100%;
  height: 100%;
}

.running-grid2 {
  display: grid;
  grid-template-columns: 70% 36%;

  width: 100%;
  height: 100%;
}

.running-image-area {
  border-right: 0.06vw solid #444;

  padding: 0.7vw;
}

.running-detail-area {
  padding: 0.7vw;
}

.section-title,
.small-label {
  font-size: clamp(0.38rem, 0.58vw, 0.9rem);
  color: #a8a8a8;
}

.image-wrapper {
  margin-top: 0.7vh;
}

.product-image {
  width: 100%;
  aspect-ratio: 1 / 1;

  object-fit: cover;

  border: 0.06vw solid #111;
}

.product-name {
  margin-top: 1.5vh;

  font-size: clamp(0.9rem, 1.3vw, 2rem);
  font-weight: 500;
}

.divider {
  width: 100%;
  height: 0.06vw;

  background: #333;

  margin: 3vh 0;
}

.product-color {
  margin-top: 1vh;

  font-size: clamp(0.8rem, 1vw, 1.5rem);
}

.lot-wrapper {
  margin-top: 2vh;
}

.lot-number {
  margin-top: 0.5vh;

  font-size: clamp(0.8rem, 1vw, 1.5rem);
  font-weight: 700;

  color: #00d0ff;
}

/* =========================================================
PROGRESS
========================================================= */

.progress-panel-body {
  padding: 0.7vw;

  display: flex;
  flex-direction: column;

  justify-content: space-between;

  min-height: 0;
}

.progress-main-grid {
   display: grid;
  grid-template-columns: 50% 50%;

  width: 100%;
  height: 100%;
}

.progress-circle-box {
  background: #07101b;
}

.progress-circle {
  width: 100%;
  aspect-ratio: 1 / 1;

  position: relative;
}

.progress-circle svg {
  width: 100%;
  height: 100%;

  transform: rotate(-90deg);
}

.progress-circle circle {
  fill: none;
  stroke-width: 0.7vw;
}

.progress-circle .bg {
  stroke: #203040;
}

.progress-circle .progress {
  stroke: #35d04f;
  stroke-linecap: round;
}

.progress-content {
  position: absolute;
  inset: 0;

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.progress-percent {
  font-size: clamp(1rem, 1.8vw, 2.8rem);
  font-weight: 700;
}

.progress-caption {
   font-size: clamp(0.8rem, 1vw, 1.5rem);
}

.target-box {
  height: 100%;

  border: 0.06vw solid #444;
  border-radius: 1vw;

  overflow: hidden;
}

.target-item {
  padding: 0.6vw;
}

.target-divider {
  width: 100%;
  height: 0.06vw;

  background: #444;
}

.target-label {
  font-size: clamp(0.4rem, 0.55vw, 0.8rem);
  color: #a8a8a8;
}

.target-value {
  display: flex;
  align-items: baseline;
  gap: 0.3vw;

  font-size: clamp(1.5rem, 2.8vw, 4rem);
  font-weight: 700;

  line-height: 1;
}

.target-value span {
  font-size: clamp(0.55rem, 0.9vw, 1.2rem);
  font-weight: 400;
}

.cycle-area {
  padding-left: 0.5vw;
}

.cycle-title,
.remaining-title {
  color: #00d0ff;

  font-size: clamp(0.5rem, 0.8vw, 1.2rem);
  font-weight: 700;
}

.cycle-value {
  margin-top: 0.5vh;

  font-size: clamp(1.2rem, 2vw, 3rem);
  font-weight: 700;
}

.cycle-unit {
  margin-top: 0.5vh;
  margin-bottom: 3vh;

  font-size: clamp(0.45rem, 0.65vw, 0.9rem);
}

.remaining-value {
  margin-top: 1vh;

  font-size: clamp(2rem, 3.2vw, 5rem);
  font-weight: 700;

  line-height: 1;

  color: red;
}

.remaining-unit {
  font-size: clamp(0.5rem, 0.8vw, 1rem);
}

.progress-bottom {
  margin-top: 1vh;
}

.progress-bar-wrapper {
  width: 100%;
  height: 1.1vh;

  background: #1f2c3b;

  position: relative;
}

.progress-fill {
  height: 100%;
  background: #35d04f;
}

.progress-marker {
  position: absolute;
  top: -0.35vh;

  width: 0.12vw;
  height: 1.8vh;

  background: #7dff71;
}

.progress-scale {
  margin-top: 0.5vh;

  display: flex;
  justify-content: space-between;

  font-size: clamp(0.35rem, 0.5vw, 0.8rem);

  color: #999;
}

/* =========================================================
TABLE
========================================================= */

 
.schedule-scroll {
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
   max-height: calc(48vh - 2vh);
}

/* Header tetap terlihat saat scroll */
.schedule-table thead th {
  position: sticky;
  top: 0;
  z-index: 10;
  background: #4e4e4e;
}

/* scrollbar */
.schedule-scroll::-webkit-scrollbar {
  width: 8px;
}

.schedule-scroll::-webkit-scrollbar-track {
  background: #111;
}

.schedule-scroll::-webkit-scrollbar-thumb {
  background: #555;
  border-radius: 4px;
}

.schedule-scroll::-webkit-scrollbar-thumb:hover {
  background: #777;
}

.schedule-table {
  width: 100%;
  
  border-collapse: collapse;
}

.schedule-table th,
.schedule-table td {
  border: 0.06vw solid #666;

  text-align: center;

  padding: 0.45vw 0.2vw;

  font-size: clamp(0.7rem, 0.9vw, 1.3rem);
}

.schedule-table th {
  background: #4e4e4e;
  color: #d7d7d7;
}

/* =========================================================
BOTTOM TROLLEY
========================================================= */

.trolley-body {
  padding: 0.25vw;

  overflow: hidden;

  height: 100%;
}

.trolley-grid {
  width: 100%;
  height: 100%;

  display: grid;

  grid-template-columns: repeat(12, 1fr);

  gap: 0.15vw;

  align-items: stretch;
}

.trolley-column {
  display: flex;
  flex-direction: column;

  gap: 0.12vw;

  min-height: 0;
}

.workstation-title {
  flex: 0 0 2.6vh;

  border: 0.06vw solid #555;
  background: #1d1d1d;

  display: flex;
  justify-content: center;
  align-items: center;

    font-size: clamp(0.5rem, 0.8vw, 1.2rem);
  font-weight: 600;

  color: #cfcfcf;
}

.trolley-box {
  flex: 1;

  min-height: 0;

  border: 0.06vw solid #555;

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  gap: 0.2vh;

  background: #050505;
}

.trolley-code {
  font-size: clamp(0.5rem, 0.8vw, 1.2rem);
  font-weight: 700;

  line-height: 1;
}

.trolley-buffer {
  font-size: clamp(0.5rem, 0.8vw, 1.2rem);

  line-height: 1;
}

.trolley-qty {
   font-size: clamp(0.5rem, 0.8vw, 1.2rem);
  font-weight: 700;

  line-height: 1;
}

/* =========================================================
COLORS
========================================================= */

.cyan-text {
  color: #00d0ff;
}

.green-text {
  color: #7dff71;
}

.yellow-text {
  color: yellow;
}

.white-text {
  color: white;
}

</style>