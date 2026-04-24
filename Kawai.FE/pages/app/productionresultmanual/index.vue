<template>
  <v-frame title="Production Result Manual Input" icon="dumpster-fire">
    <template #frame-content>
       <div class="filter-wrapper">
         

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Schedule Date</label>
           <div>
          <input-date v-model="filter.PeriodFrom" />
        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >To</label
        >
        <div>
          <input-date v-model="filter.PeriodUntil" />
        </div>
        </div>

        <div class="filter-item">
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
       
        </div>
         <div class="filter-item">
          <label class="form-label">Line</label>
          <filter-line-factory
              class="form-control"
              
              :company="filter.FactoryCode"
              :manufacture="filter.ManufactureCode"
              v-model="filter.LineCode"
              style-code="width: 120px"
                    style-desc="width: 300px"
            />
       
        </div>

         <div class="filter-item">
          <label class="form-label">Complete Cls</label>
           <filter-yes-no-all
              class="form-control"
              
              v-model="filter.CompleteCls"
              style="width: 120px"
            />
       
        </div>


         
     
      </div>
   <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
              <v-button-search-reset :search="search" :reset="reset" />
              <button
                class="btn btn-primary btn-elevate btn-search"
                style="margin-left: 5px"
                @click="newRequest"
              >
                <font-awesome-icon icon="arrow-right" />
                <span class="ml-2">To Material Consump</span>
              </button>
            </div>
            </div>
            <hr>
      <v-table
        :filter="filter"
        :ds="ds"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
        :top-content-height="100"
      >
     
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center"></th>
                <th class="text-center">Schedule Date</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Plan Qty</th>
                <th class="text-center">Result Qty</th>
                 <th class="text-center">Remaining Qty</th>
                <th class="text-center">Lot No</th>
                <th class="text-center">Barcode No</th>
                <th class="text-center">Barcode Qty</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists || []"
                :key="item.ProductionId"
              >
                <tr>
                  <td>
                    <input-checkbox
                      v-model="item.Selected"
                      @click="(e) => check(e, item)"
                    />
                  </td>
                  <td>{{ $func.formatDate(item.ScheduleDate) }}</td>
                  <td>
                    <div style="display: flex; justify-content: space-between">
                      
                        {{ item.ItemCode }}
                    </div>
                  </td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsDesc }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.PlanQty) }}
                  </td>
                  <td><input-money v-model="item.ResultQty" /></td>
                  <td class="text-right">
                  
                    {{ $func.formatMoney(item.RemainingQty) }}
                   
                 
                  </td>
                   <td>
                    <span>
                     {{ item.LotNo }}
                    </span>
                    <span
                        v-if="
                          item.Details.length > 0
                        "
                        :class="[
                          'toggle-button',
                          item.Expanded ? 'collapse' : 'expand',
                        ]"
                        @click="() => (item.Expanded = !item.Expanded)"
                      >
                        {{ item.Expanded ? "-" : "+" }}
                      </span>
                      </td>
                   <td> </td>
                   <td> </td>
                </tr>
                <tr
                  v-if="item.Expanded"
                  v-for="(dtl, idxx) in item.Details || []"
                  :key="dtl.ProdResultID"
                >
                  <td colspan="9"></td>                 
                  <td>{{ dtl.BarcodeNo }} </td>
                   <td class="text-right">
                  
                    {{ $func.formatMoney(dtl.BarcodeQty) }}
                   
                 
                  </td>
                   
                  
                </tr>
              </template>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    filter: {
      keyword: null,
      FactoryCode: null,
      ManufactureCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      LineCode: null,
      RemainingCls: null,
      sorts: {
        ProductionId: "asc",
      },
    },
    debounce: null,
    lists: [],
    productionIds: [],
    groupLists: [],
  }),
  computed: {
    ds: function () {
      return useProductionManual();
    },
  },
  watch: {
    "filter.FactoryCode": function () {
      this.resetGrid();
    },
    "filter.ManufactureCode": function () {
      this.resetGrid();
    },
    "filter.LineCode": function () {
      this.resetGrid();
    },
    "filter.RemainingCls": function () {
      this.resetGrid();
    },
    "filter.PeriodFrom": function () {
      this.resetGrid();
    },
    "filter.PeriodUntil": function () {
      this.resetGrid();
    },
  },
  mounted: function () {
    const f = this.ds.filter.Filters?.[0];

    if (this.$route.query.back && f) {
      this.filter.FactoryCode = f.FactoryCode;
      this.filter.ManufactureCode = f.ManufactureCode;
      this.filter.LineCode = f.LineCode;
      this.filter.CompleteCls = f.CompleteCls;
      this.filter.PeriodFrom = f.PeriodFrom ? new Date(f.PeriodFrom) : null;
      this.filter.PeriodUntil = f.PeriodUntil ? new Date(f.PeriodUntil) : null;

      // OPTIONAL: auto load
      this.search();
    } else {
      this.setDefaultFilter();
    }
  },
  methods: {
    resetGrid: function () {
      this.groupLists = [];
    },
    search: function () {
      let rangePeriodDays = this.$func.dateDiffInDays(
        this.filter.PeriodFrom,
        this.filter.PeriodUntil
      );

      if (new Date(this.filter.PeriodFrom) > new Date(this.filter.PeriodUntil)) {
        toastWarning("Periode Dari tidak boleh melewati Periode Sampai.");
        return;
      }

      if (rangePeriodDays > 30) {
        toastWarning("Jarak Periode hanya 30 hari.");
        return;
      }

      if(!this.filter.ManufactureCode) {
        toastWarning("Silahkan pilih process.");
        return;
      }

      if(!this.filter.LineCode) {
        toastWarning("Silahkan pilih line.");
        return;
      }

      if(!this.filter.CompleteCls) {
        toastWarning("Silahkan pilih filter Complete.");
        return;
      }

      this.ds.setPage(1);
      this.ds.setLength(1000);
      this.ds.setSort(this.filter.sorts);

      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.FactoryCode,
          ManufactureCode: this.filter.ManufactureCode,
          LineCode: this.filter.LineCode,
          CompleteCls: this.filter.CompleteCls,
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom)
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil)
          ),
        },
      ];
      this.ds.setFilter(filters);
      this.ds.load().then((dt) => {
        let grouped = {};

        dt.Data.Items.forEach((item) => {
          let key = [
            item.ProductionId,
            item.ScheduleDate,
            item.ItemCode,
            item.ItemName,
            item.UnitClsDesc,
            item.PlanQty,
          ].join("|");

          if (!grouped[key]) {
            let totalResultQty = dt.Data.Items.filter(
              (x) => x.ProductionId == item.ProductionId && x.ProdResultId != null
            ).reduce((a, b) => a + (b.ResultQty || 0), 0);

            grouped[key] = {
              ...item,
              ResultQty: totalResultQty,
              RemainingQty: item.PlanQty - totalResultQty,
              Selected: false,
              Expanded: true,
              Details: [],
            };
          }

          if (item.ProdResultId) {
            grouped[key].Details.push({
              ProdResultId: item.ProdResultId,
              LotNo: item.LotNo,
              BarcodeNo: item.BarcodeNo,
              BarcodeQty: item.BarcodeQty, 
            });
          }
        });

        this.groupLists = Object.values(grouped);
      });
    },
    reset: function () {
      this.filter.FactoryCode = null;
      this.filter.ManufactureCode = null;
      this.filter.LineCode = null;
      this.filter.CompleteCls = null;

      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.PeriodUntil = today;
      this.search();
    },
    check: function (e, item) {
        if (e.target.checked) {
          // uncheck semua dulu
          this.groupLists.forEach((x) => {
            x.Selected = false;
          });

          // pilih hanya yg ini
          item.Selected = true;
        } else {
          item.Selected = false;
        }
      },
    newRequest: function () {
      let selected = this.groupLists.filter((x) => x.Selected);

      if (selected.length == 0) {
        toastWarning("Please choose schedule!");
        return;
      }

      if (selected.length > 1) {
        toastWarning("Only one schedule can be selected!");
        return;
      }

      if (selected.filter((x) => x.ResultQty <= 0).length > 0) {
        toastWarning("Result Qty must be greater than ZERO!");
        return;
      }

      if (selected.filter((x) => x.ResultQty > x.RemainingQty).length > 0) {
        toastWarning("Result Qty cannot be greater than Remaining Qty!");
        return;
      }

      let newRequestPayload = selected.map((x) => {
        return {
          LineCode: this.filter.LineCode,
          ProdResultId: null,
          ProductionId: x.ProductionId,
          ScheduleDate: x.ScheduleDate,
          ItemCode: x.ItemCode,
          ResultQty: x.ResultQty,
        };
      });

      this.ds.setRequest(newRequestPayload);
      debugger;
      this.$router.push("/app/productionresultmanual/create");
    },
    viewRequest: function (selected, dtl) {
      let payloadrequest = [
        {
          LineCode: this.filter.LineCode,
          RProdResultIdd: dtl.RequestId,
          RequestNo: dtl.RequestNo,
          RequestDate: dtl.RequestDate,
          ProductionId: selected.ProductionId,
          ScheduleDate: selected.ScheduleDate,
          ItemCode: selected.ItemCode,
          ResultQty: selected.ResultQty,
        },
      ];

      this.ds.setRequest(payloadrequest);
      this.$router.push("/app/supply-request/womin/view");
    },

    setDefaultFilter: function () {
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.PeriodUntil = today;
      this.filter.CompleteCls = "ALL";
    },
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

.filter-wrapper:has(.filter-item:nth-child(11)):not(:has(.filter-item:nth-child(12))) {
  grid-template-rows: repeat(6, auto);
}

.filter-wrapper:has(.filter-item:nth-child(10)):not(:has(.filter-item:nth-child(11))) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(9)):not(:has(.filter-item:nth-child(10))) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(8)):not(:has(.filter-item:nth-child(9))) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(7)):not(:has(.filter-item:nth-child(8))) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(6)):not(:has(.filter-item:nth-child(7))) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(5)):not(:has(.filter-item:nth-child(6))) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(4)):not(:has(.filter-item:nth-child(5))) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(3)):not(:has(.filter-item:nth-child(4))) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(2)):not(:has(.filter-item:nth-child(3))) {
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

