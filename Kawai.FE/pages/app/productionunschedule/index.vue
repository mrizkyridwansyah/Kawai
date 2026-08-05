<template>
  <v-frame title="Production Un Schedule" icon="dumpster-fire">
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
          <label class="form-label">Qty Input</label>
          <div>
            <input-money v-model="filter.QtyInput" />
          </div>
          
        </div>
        
        <div class="filter-item">
          <label class="form-label">Poduction Date</label>
          <div>
            <input-date v-model="filter.ProductionDate" />
          </div>
          
        </div>

      </div>
        <!--remarks di modal aja taronya-->
        <!-- <div class="col-md-12 row">
            <div class="col-md-6">
              <div></div>
            </div>
            <div class="col-md-6">
               <div style="padding-left: 10px;padding-top: 10px;" class="row">
                 <div style="width: 10%;"><label class="form-label">Remarks</label></div>
                 <div class="col-md-7"><input-text
                      multiline
                      v-model="filter.Remarks"
                    /></div>
                    
               </div>
            </div>
        </div> -->
       
      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-calculate-reset :search="search" :reset="reset" />
            <span
              v-b-tooltip.hover
              :title="hasNotEnoughStock ? 'Tidak dapat melakukan Confirmation karena masih ada material dengan status tidak mencukupi.' : ''"
            >
            <v-button-submit-confirm :add="add" cClass="mr-1" style="margin-left: 10px;" :disabled="!menuPrivAllowUpdate || hasNotEnoughStock" />
            </span>
            <span
              v-b-tooltip.hover
              :title="'Hasil produksi unschedule'"
            >
            <button
              class="btn btn-secondary btn-elevate btn-search"
              style="margin-left: 5px;"
              @click="toResult">
              <font-awesome-icon icon="arrow-right" />
              <span class="ml-2">History</span>
            </button>
          </span>
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
          <div class="card-head">
              <div class="card-title">Material Requirement</div>
          </div>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">No</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">BOM Qty</th>
                <th class="text-center">Qty Requirement</th>
                <th class="text-center">Current Stock</th>
                <th class="text-center">Remaining</th>
                <th class="text-center">Status</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists || []"
                
              >
                <tr>
                  <td>{{ item.No }}</td>
                  <td>
                    <div style="display: flex; justify-content: space-between">
                      {{ item.ItemCode }}
                    </div>
                  </td>
                  <td>{{ item.ItemName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.BomQty) }}
                  </td>
                   <td class="text-right">
                    {{ $func.formatMoney(item.QtyRequirement) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.CurrentStock) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.Remaining) }}
                  </td>
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
                  
                </tr>
              
              </template>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>

  <v-modal
    ref="modalsubmit"
    id="modal-form-produnschedule"
    :title="title"
    size="md"
  >
    <modal-form-produnschedule
      ref="formProdUns"
      :filter="this.filter"
      :id="idSelected"
      :mode="modalMode"
      @save="saveData"
      
    />
  </v-modal>
</template>
<script>
export default {
  data: () => ({
    filter: {
      keyword: null,
      // FactoryCode: null,
      // ManufactureCode: null, //proces
      LineCode: null,
      LineName:"",
      ParentItemCode:null,
      ParentItemName:null,
      ProductionDate: null,
      QtyInput:null,
      Remarks:null,
      sorts: {
        No: "asc",
      },
    },
    selectedItem: null,
    menuPrivAllowUpdate: false,
    debounce: null,
    lists: [],
    groupLists: [],
    idSelected: null,
    modalMode:null,
    title:null,
  }),
  computed: {
    ds: function () {
      return useProductionUnschedule();
    },
      dsMenu: function () {
      return useMenu();
    },

    hasNotEnoughStock() {
      return this.groupLists.some(
        x => (x.Status || "").toLowerCase() === "not enough"
      );
    }
  },
  watch: {
    // "filter.FactoryCode": function () {
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
     "filter.ProductionDate": function () {
      this.resetGrid();
    },
    "filter.QtyInput": function () {
      this.resetGrid();
    },
    "filter.Remarks": function () {
      this.resetGrid();
    },
  },
  mounted: function () {
     this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "P03",
      )[0].AllowUpdate;
    });

    const f = this.ds.filter.FiltersBack?.[0];
    
    if (this.$route.query.back && f) {
    
      // this.filter.FactoryCode = f.FactoryCode;
      // this.filter.ManufactureCode = f.ManufactureCode;
      this.filter.LineCode = f.LineCode;
      this.filter.LineName = f.LineName;
      this.filter.ParentItemCode = f.ParentItemCode;
      this.filter.ParentItemName = f.ParentItemName;
      this.filter.ProductionDate = new Date(f.ProductionDate);
      this.filter.Remarks = f.Remarks;
      this.filter.QtyInput = f.QtyInput;

      // OPTIONAL: auto load
      this.search();
    } else {
      this.setDefaultFilter();
    }
  },
  methods: {
    add: function () {
      this.title = "Production Un Schedule";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-produnschedule");
    },

    resetGrid: function () {
      this.groupLists = [];
    },
    onLineSelected(item) {
      this.filter.LineName = item?.LineName ?? "";
    },
    onParentItemSelected(item){
      
      this.filter.ParentItemName = item?.ParentItemName ?? "";
    },
    search: function () {
    //   let rangePeriodDays = this.$func.dateDiffInDays(
    //     this.filter.PeriodFrom,
    //     this.filter.PeriodUntil,
    //   );

      
      // if (!this.filter.ManufactureCode) {
      //   toastWarning("Silahkan pilih process.");
      //   return;
      // }

      if (!this.filter.LineCode) {
        toastWarning("Silahkan pilih line.");
        return;
      }

      if (!this.filter.ParentItemCode) {
        toastWarning("Silahkan pilih parent item");
        return;
      }

     if (!this.filter.QtyInput) {
        toastWarning("Silahkan input qty");
        return;
      }

    //   if (
    //     new Date(this.filter.ProductionDate) > new Date(today)
    //   ) {
    //     toastWarning("Periode Dari tidak boleh melewati Periode Sampai.");
    //     return;
    //   }


      this.ds.setPage(1);
      this.ds.setLength(1000);
      this.ds.setSort(this.filter.sorts);

      let FiltersBack = [
        {
          Keyword: this.filter.keyword || "",
          // FactoryCode: this.filter.FactoryCode,
          // ManufactureCode:this.filter.ManufactureCode,
          LineCode: this.filter.LineCode,
          LineName: this.filter.LineName,
          ParentItemCode: this.filter.ParentItemCode,
          ParentItemName: this.filter.ParentItemName,
          ProductionDate: this.$func.asUtcStringDateOnly(
            new Date(this.filter.ProductionDate),
          ),
          QtyInput: Number(this.filter.QtyInput ?? 0),
          //Remarks: this.filter.Remarks
        },
      ];


      let filters = [
        {
          Keyword: this.filter.keyword || "",
          LineCode: this.filter.LineCode,
          ParentItemCode: this.filter.ParentItemCode,
          ProductionDate: this.$func.asUtcStringDateOnly(
            new Date(this.filter.ProductionDate),
          ),
          QtyInput: String(this.filter.QtyInput ?? "0")
        },
      ];
      //console.log(filters[0]);
      this.ds.setFilter(filters,FiltersBack);
      this.ds.load().then((dt) => {
        let grouped = {};

        dt.Data.Items.forEach((item) => {
          let key = [
            item.No,
            item.ItemCode,
            item.ItemName,
            item.BOMQty,
            item.QtyRequirement,
            item.CurrentStock,
            item.Remaining,
            item.Status
          ].join("|");

          if (!grouped[key]) {
      

            grouped[key] = {
              ...item,
            //   ResultQty: totalResultQty,
            //   RemainingQty: item.PlanQty - totalResultQty,
              Selected: false,
              Expanded: false,
              //Details: [],
            };
          }

      
        });

        this.groupLists = Object.values(grouped);
      });
    },
    reset: function () {
      // this.filter.FactoryCode = null;
      // this.filter.ManufactureCode = null;
      this.filter.LineCode = null;
      this.filter.LineName = null;
      this.filter.ParentItemCode = null;
      this.filter.ParentItemName = null;
      this.filter.QtyInput = null;
      this.filter.Remarks=null;

      let today = new Date();
      this.filter.ProductionDate = today;
      this.search();
    },
    toResult: function () {
      let payload = {
          // FactoryCode:this.filter.FactoryCode,
          // ManufactureCode: this.filter.ManufactureCode,
          LineCode: this.filter.LineCode,
          LineName: this.filter.LineName,
          ProductionDate: this.filter.ProductionDate,
          DateFrom: this.filter.ProductionDate,
          DateTo: this.filter.ProductionDate,
          ParentItemCode: this.filter.ParentItemCode,
          ParentItemName: this.filter.ParentItemName,
          QtyInput : this.filter.QtyInput,
          Remarks: this.filter.Remarks

      };
      // console.log('payload');
      // console.log(payload);
      this.ds.setRequest(payload);
      this.$router.push("/app/productionunschedule/detail");
    },
    setDefaultFilter: function () {
      let today = new Date();
      this.filter.ProductionDate = today;
    },
    saveData(data) {
      //console.log(data);
      this.ds
        .save(data)
        .then(() => {
          toastSuccess("Data saved successfully");
          this.$bvModal.hide("modal-form-produnschedule");
          this.search();
          // this.close();
        })
        .catch((err) => {
          toastDanger(err?.Message);
        });
    }
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

.card-head {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 12px;
    padding: 5px 8px;
    border-bottom: 1px solid var(--border);
}

.card-title {
    font-size: 17px;
    font-weight: 800;
}
</style>
