
<template>
  <v-frame title="Production Quality Judgement" icon="dumpster-fire">
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
        </div>
          <v-button-submit :submit="submit" />
            </div>
            <hr>
      <v-table
        :filter="filter"
        :ds="ds"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
        :default-height="120"
          :max-height="120"
      >
     
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Schedule Date</th>
                <th class="text-center">Barcode No</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Serial No</th>
                <th class="text-center">Good</th>
                <th class="text-center">NG</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists || []"
                :key="item.ProductionId"
              >
                <tr>
                  <td>{{ $func.formatDate(item.ScheduleDate) }}</td>         
                  <td>{{ item.BarcodeNo }} </td>
                  <td>
                    <div style="display: flex; justify-content: space-between">
                      
                        {{ item.ItemCode }}
                    </div>
                  </td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsDesc }}</td>
                  <td>{{ item.LotNo }}</td>
                  <td>
                    <input-checkbox
                      v-model="item.Good"
                      :disabled="item.SavedGood || item.SavedNG"
                      @click="check(item, 'Good')"
                    />
                  </td>

                  <td>
                    <input-checkbox
                      v-model="item.NG"
                      :disabled="item.SavedGood || item.SavedNG"
                      @click="check(item, 'NG')"
                    />
                  </td>
                  <td>{{ item.LastUpdate }}</td>
                  <td>{{ item.LastUser }}</td>
                </tr>
                <tr
                  v-if="item.Expanded"
                  v-for="(dtl, idxx) in item.Details || []"
                  :key="dtl.ProdResultId"
                >
                  
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
import axios from "axios";

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
    CompleteCls: null,
    sorts: {
      ProductionId: "asc",
    },
  },

  model: {  
    ProdResultID: null,
    ResultType: null,
    ItemCode: null,
  },

  isLoading: false, 
  debounce: null,
  lists: [],
  productionIds: [],
  groupLists: [],
}),
  computed: {
    ds: function () {
      return useProductionqualityjudgement();
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
        toastWarning("The From Period cannot exceed the To Period.");
        return;
      }

      if (rangePeriodDays > 30) {
        toastWarning("Period Distance is only 30 days.");
        return;
      }

      if(!this.filter.ManufactureCode) {
        toastWarning("Please select process.");
        return;
      }

      if(!this.filter.LineCode) {
        toastWarning("Please select line.");
        return;
      }

      if(!this.filter.CompleteCls) {
        toastWarning("Please select Complete.");
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

  console.log("API Items =", dt.Data.Items.length);

  this.groupLists = dt.Data.Items.map(item => ({
    ...item,

  SavedGood: item.Good,
  SavedNG: item.NG,
    Selected: false,
    Expanded: false,
    Details: [],
    ResultQty: item.ResultQty || 0,
    RemainingQty: item.RemainingQty || 0
  }));

  console.log("Grid Items =", this.groupLists.length);
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
    
    submit: async function () {
      this.isLoading = true;

      const selected = this.groupLists.filter(
        (x) => x.Good === true || x.NG === true
      );

      if (selected.length == 0) {
        this.isLoading = false;
        toastDanger("Silahkan pilih data!");
        return;
      }

      let payload = selected.map((p) => ({
        ProdResultID: p.ProdResultId,
        ResultType: p.Good ? "GOOD" : "NG",
        ItemCode: p.ItemCode,
        BarcodeNo: p.BarcodeNo,
      }));

      try {
        console.log("SEND TO API:", payload);

        const res = await axios.post(
          "/productionjudgement/quality-judgement/save",
          payload
        );

    console.log("SUCCESS:", res.data);
        toastSuccess("Saved successfully");
        await this.search();
      } catch (err) {
        console.error(err);
        toastDanger("Save failed");
      } finally {
        this.isLoading = false;
      }
    },

   check(item, type) {
      // hanya lock data yang sudah tersimpan di DB
      if (item.SavedGood || item.SavedNG) {
        return;
      }

      if (type === "Good") {
        item.Good = !item.Good;

        if (item.Good) {
          item.NG = false;
        }
      }

      if (type === "NG") {
        item.NG = !item.NG;

        if (item.NG) {
          item.Good = false;
        }
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
      this.$router.push("/api/productionjudgement/quality-judgement/save");
    },
    viewRequest: function (selected, dtl) {
      let payloadrequest = [
        {
          LineCode: this.filter.LineCode,
          RProdResultId: dtl.RequestId,
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


