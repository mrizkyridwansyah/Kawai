<template>
  <v-frame title="Production Un Schedule History" icon="dumpster-fire">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- <div class="filter-item">
          <label class="form-label">Factory</label>
          <filter-factory-privileges
            class="form-control"
            v-model="filter.FactoryCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>
        <div class="filter-item">
          <label class="form-label">Process</label>
          <filter-trade-2
            class="form-control"
            :trade-cls="['1']"
            v-model="filter.ManufactureCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div> -->
       <div class="filter-item">
          <label class="form-label">Production Line</label>
          <filter-line-produnschedule
            class="form-control"
            v-model="filter.LineCode"
            style-code="width: 120px"
            style-desc="width: 300px"
           @selected="onLineSelected"
           
          />
        </div>
        <div class="filter-item">
          <label class="form-label">Parent Item</label>
          <filter-item-produnschedule
            class="form-control"
            v-model="filter.ParentItemCode"
            style-code="width: 120px"
            style-desc="width: 300px"
            @selected="onParentItemSelected"
             
          />
        </div>
       
        <div class="filter-item">
          <label class="form-label">Production Date</label>
          <div>
            <input-date v-model="filter.DateFrom" />
          </div>
          <label
            class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
            >To</label
          >
          <div>
            <input-date v-model="filter.DateTo" />
          </div>
        </div>

      </div>
      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button
                :action="back"
                label="Back"
                icon="arrow-left"
                cClass="ml-1 btn-secondary"
                :is-loading="isLoading"
              />
          <span style="padding: 3px;"></span>
          <v-button-search-reset :search="search" :reset="reset" />
         
        </div>
      </div>
      <hr />
      <v-table
        :filter="filter"
        :ds="ds"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
        class="vh-grid"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Result ID</th>
                <!-- <th class="text-center">Line Code</th> -->
                <th class="text-center">Line Name</th>
                <th class="text-center">Parent Item Code</th>
                <th class="text-center">Parent Item Name</th>
                <th class="text-center">Production Date</th>
                <th class="text-center">Lot No</th>
                <th class="text-center">Qty</th>
                <th class="text-center">Remarks</th>
                <th class="text-center">Status</th>
                 <th class="text-center">Details</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists || []" >
                <tr>
                  <td>{{ item.ProdResultID }}</td>
                  <!-- <td>{{ item.LineCode }}</td> -->
                  <td>{{ item.LineName }}</td>                  
                  <td>
                    <div style="display: flex; justify-content: space-between">
                      {{ item.ParentItemCode }}
                    </div>
                  </td>
                  <td>{{ item.ParentItemName }}</td>
                  <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                  <td>{{ item.LotNo }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.Qty) }}
                  </td>
                  <td>{{ item.Remarks }}</td>
                  <td class="text-center">
                    <span
                      :style="{
                        color: item.Status?.toLowerCase() === 'available' ? 'limegreen' : 'red',
                        // padding: '3px 8px',
                        // borderRadius: '4px',
                        fontWeight: 'bold',
                        textAlign:'center'
                        
                      }"
                    >
                      {{ item.Status }}
                    </span>
                  </td>
                 
                  <td class="text-center">
                    <button
                      class="btn btn-sm btn-outline-primary"
                      @click="showBarcode(item)"
                    >
                      <i class="fa fa-eye"></i> View
                    </button>
                  </td>
                </tr>
              
              </template>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
 
  <v-modal
    id="modal-form-produnschedule-detail"
    size="md"
    :title="title"
  >
    <modal-form-produnschedule-detail
      ref="formProdUns"
      :id="selectedProdResultID"
      
     
    />
  </v-modal>

</template>
<script>
export default {
  data: () => ({
    debounce: null,
    lists: [],
    groupLists: [],
    filter: {
      keyword: null,
      // FactoryCode:null,
      // ManufactureCode: null, //proces
      LineCode: null,
      LineName:"",
      ParentItemCode:null,
      ParentItemName:null,
      ProductionDate:null,
      DateFrom: null,
      DateTo:null,
      menuPrivAllowUpdate: false,
      ProdResultID:null,
      sorts: {
        ProdResultID: "asc",
      },
    },
    debounce: null,
    lists: [],
    isLoading:false,
    //productionIds: [],
    groupLists: [],
    selectedProdResultID: null,
    title:null,
    //barcodeList: [],
  }),
  computed: {
    ds: function () {
      return useProductionUnschedule();
    },
      dsMenu: function () {
      return useMenu();
    },
  },
  watch: {
    //  "filter.FactoryCode": function () {
    //   this.resetGrid();
    // },
    // "filter.ManufactureCode": function () {
    //   this.resetGrid();
    // },
    "filter.LineCode": function () {
      this.resetGrid();
    },
    "filter.ParentItemCode": function () {
      this.resetGrid();
    },
     "filter.DateFrom": function () {
      this.resetGrid();
    },
    "filter.DateTo": function () {
      this.resetGrid();
    },
  },
  mounted: function () {
     this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "P03",
      )[0].AllowUpdate;
    });
    const req = this.ds.toResult;
   
    if (req) {
      // this.filter.FactoryCode = req.FactoryCode;
      // this.filter.ManufactureCode = req.ManufactureCode;
      this.filter.LineCode = req.LineCode;
      this.filter.ParentItemCode = req.ParentItemCode;
      this.filter.DateFrom = req.DateFrom;
      this.filter.DateTo = req.DateTo;
    }

    this.search();
  },
  methods: {
    resetGrid: function () {
      this.lists = [];
    },
    search() {
     
      let filters = [
        {
          LineCode: this.filter.LineCode,
          ParentItemCode: this.filter.ParentItemCode,
          DateFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.DateFrom),
          ),
          DateTo: this.$func.asUtcStringDateOnly(
            new Date(this.filter.DateTo),
          )
        },
      ];
      this.ds.setFilterDetail(filters);
      this.ds.loadDetail().then((dt) => {
          // console.log(dt.Data);
          // this.groupLists = dt.Data;

          let grouped = {};

        dt.Data.Items.forEach((item) => {
          let key = [
            item.ProdResultID,
            item.LineCode,
            item.LineName,
            item.ParentItemCode,
            item.ParentItemName,
            item.ProductionDate,
            item.LotNo,
            item.Qty,
            item.Remarks,
            item.Status
          ].join("|");

          if (!grouped[key]) {
            grouped[key] = {
              ...item,
            };
          }

      
        });

        this.groupLists = Object.values(grouped);

      });


      // this.ds.loadDetail().then((dt) => {
      //   this.groupLists = dt.Data;
      // });
    },
    reset: function () {
      // this.filter.FactoryCode = null;
      // this.filter.ManufactureCode = null;
      this.filter.LineCode = null;
      this.filter.LineName = null;
      this.filter.ParentItemCode = null;

      let today = new Date();
      this.filter.DateFrom = today;
      this.filter.DateTo = today;
      this.search();
    },
    back: function () {
      this.$router.push({
          path: "/app/productionunschedule",
          query: {
            back: 1,
          },
      });
    },
    onLineSelected(item) {
      this.filter.LineName = item?.LineName ?? "";
    },
    onParentItemSelected(item){
    
      this.filter.ParentItemName = item?.ParentItemName ?? "";
    },
    showBarcode(item) {
      this.title = "Detail Result"
      this.selectedProdResultID = item.ProdResultID;
      //this.filter.ProdResultID = item.ProdResultID;
      this.$nextTick(() => {
        this.$refs.formProdUns.loadData();
      });
      this.$bvModal.show("modal-form-produnschedule-detail");
    },
    // showBarcode(item) {
    //   this.barcodeFilter = {
    //     ProdResultID: item.ProdResultID,
    //   };
    //   this.title = "Detail Result";
    //   this.$bvModal.show("modal-form-produnschedule-detail");
    // },
    // loadBarcode(id) {
    //   this.ds.loadBarcode(id).then(dt => {
    //     this.barcodeList = dt.Data;
    //   });
    // }
  },
  
};
</script>

<style scoped>
.vdatetime {
  max-width: 60% !important;
}

.toggle-button {
  margin-left: 1em;
  display: inline-block;
  width: 14px;
  height: 14px;
  line-height: 12px;
  font-size: 10px;
  font-weight: bold;
  text-align: center;
  border: 1px solid;
  border-radius: 50%; /* full bulat */
  cursor: pointer;
  margin-right: 6px;
  user-select: none;
}

.toggle-button.expand {
  color: #007bff;
  border-color: #007bff;
  background-color: #e6f0ff;
}

.toggle-button.collapse {
  color: #dc3545;
  border-color: #dc3545;
  background-color: #ffe6e6;
}

.toggle-button:hover {
  opacity: 0.85;
}
thead {
  white-space: nowrap;
}
/* GANTI CSS .filter-wrapper lama dengan ini */

.filter-wrapper {
  display: grid;
  grid-template-columns: repeat(2, minmax(320px, 1fr));
  grid-auto-flow: column; /* isi atas ke bawah dulu */
  gap: 4px 20px;
  width: 100%;
  align-items: center;
}

.filter-wrapper:has(.filter-item:nth-child(12)) {
  grid-template-rows: repeat(6, auto);
}

.filter-wrapper:has(.filter-item:nth-child(11)):not(
    :has(.filter-item:nth-child(12))
  ) {
  grid-template-rows: repeat(6, auto);
}

.filter-wrapper:has(.filter-item:nth-child(10)):not(
    :has(.filter-item:nth-child(11))
  ) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(9)):not(
    :has(.filter-item:nth-child(10))
  ) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(8)):not(
    :has(.filter-item:nth-child(9))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(7)):not(
    :has(.filter-item:nth-child(8))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(6)):not(
    :has(.filter-item:nth-child(7))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(5)):not(
    :has(.filter-item:nth-child(6))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(4)):not(
    :has(.filter-item:nth-child(5))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(3)):not(
    :has(.filter-item:nth-child(4))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(2)):not(
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

.vh-grid{
  height: calc(-220px + 80vh) !important;
}

/* MOBILE = turun kebawah normal */
@media (max-width: 768px) {
  .filter-wrapper {
    grid-template-columns: 1fr !important;
    grid-template-rows: auto !important;
    grid-auto-flow: row !important;
    gap: 6px;
  }

  .filter-item {
    width: 100%;
  }
}

.button-section {
  /* garis panjang bawah */
  width: 100%;
}
</style>
