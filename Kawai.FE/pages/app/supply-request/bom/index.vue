<template>
  <v-frame title="Part Material Supply Request (Subcon)" icon="cart-flatbed">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">PO Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 160px">
            <input-date
              v-model="filter.PeriodFrom"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px">
            <label class="form-label">To</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-date
              v-model="filter.PeriodUntil"
              style-date="width: 100px !important"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Factory</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-factory-privileges
              class="form-control"
              v-model="filter.FactoryCode"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Supplier</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-trade-2
              class="form-control"
              
              :trade-cls="['3']"
              v-model="filter.SupplierCode"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">PO Number</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-po
              class="form-control"
              
              v-model="filter.PONumber"
              :factory-code="filter.FactoryCode"
              :supplier-code="filter.SupplierCode"
              type-date="PO"
              :period-from="filter.PeriodFrom"
              :period-until="filter.PeriodUntil"
              :show-option-all="true"
              style="width: 200px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">WH Subcon</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-warehouse-privileges
              class="form-control"
              
              disabled
              :factory-code="filter.FactoryCode"
              v-model="filter.Warehouse"
              style-code="width: 120px"
              style-desc="width: 240px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Remaining Cls</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-yes-no-all
              class="form-control"
              
              v-model="filter.RemainingCls"
              style="width: 110px"
            />
          </td>
        </tr>
        <tr>
          <td colspan="4" style="padding-top: 5px">
            <div class="d-flex flex-fill">
              <v-button-search-reset :search="search" :reset="reset" />
              <button
                class="btn btn-primary btn-elevate btn-search"
                style="margin-left: 5px"
                @click="newRequest"
              >
                <font-awesome-icon icon="arrow-right" />
                <span class="ml-2">To Material Request</span>
              </button>
            </div>
          </td>
        </tr>
      </table>

      <v-table
        :filter="filter"
        :ds="ds"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
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
                <th class="text-center">PO Date</th>
                <th class="text-center">PO Number</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name / Request No</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Plan Qty</th>
                <th class="text-center">Request Qty</th>
                <th class="text-center">Remaining Qty</th>
                <th class="text-center">Request User</th>
                <th class="text-center">Request Date</th>
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
                  <td>{{ $func.formatDate(item.PODate) }}</td>
                  <td>
                    <div style="display: flex; justify-content: space-between">
                      <span>
                        {{ item.PONumber }}
                      </span>
                      <span
                        v-if="item.Details.length > 0"
                        :class="[
                          'toggle-button',
                          item.Expanded ? 'collapse' : 'expand',
                        ]"
                        @click="() => (item.Expanded = !item.Expanded)"
                      >
                        {{ item.Expanded ? "-" : "+" }}
                      </span>
                    </div>
                  </td>
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsDesc }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.PlanQty) }}
                  </td>
                  <td><input-money v-model="item.RequestSetQty" /></td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.RemainingQty) }}
                  </td>
                  <td></td>
                  <td></td>
                </tr>
                <tr
                  v-if="item.Expanded"
                  v-for="(dtl, idxx) in item.Details || []"
                  :key="dtl.RequestId"
                >
                  <td colspan="4"></td>
                  <td>
                    <a
                      href="javascript:void(0)"
                      @click="() => viewRequest(item, dtl)"
                      >{{ dtl.RequestNo }}</a
                    >
                  </td>
                  <td>{{ idxx + 1 }}</td>
                  <td></td>
                  <td class="text-right">
                    {{ $func.formatMoney(dtl.RequestSetQty) }}
                  </td>
                  <td></td>
                  <td>{{ dtl.RegisterUser }}</td>
                  <td>{{ $func.formatDate(dtl.RequestDate) }}</td>
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
      SupplierCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      PONumber: null,
      Warehouse: null,
      RemainingCls: null,
      sorts: {
        ReceiptNo: "asc",
      },
    },
    debounce: null,
    lists: [],
    poNumbers: [],
    groupLists: [],
  }),
  computed: {
    ds: function () {
      return useSupplyRequestBOM();
    },
    dsSupplier: function () {
      return useTrade();
    },
  },
  watch: {
    "filter.FactoryCode": function () {
      this.resetGrid();
    },
    "filter.SupplierCode": function () {
      this.dsSupplier
        .loadDetail(this.filter.SupplierCode)
        .then((dt) => (this.filter.Warehouse = dt.Data.Subcon_WH_Code));
      this.resetGrid();
    },
    "filter.PeriodFrom": function () {
      this.resetGrid();
    },
    "filter.PeriodUntil": function () {
      this.resetGrid();
    },
    "filter.Warehouse": function () {
      this.resetGrid();
    },
    "filter.RemainingCls": function () {
      this.resetGrid();
    },
    "filter.PONumber": function () {
      this.resetGrid();
    },
  },
  mounted: function () {
    const f = this.ds.filter.Filters?.[0];

    if (this.$route.query.back && f) {
      this.filter.FactoryCode = f.FactoryCode;
      this.filter.SupplierCode = f.SupplierCode;
      this.filter.PONumber = f.PONumber;
      this.filter.Warehouse = f.Warehouse;
      this.filter.RemainingCls = f.RemainingCls;
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
      // let rangePeriodDays = this.$func.dateDiffInDays(
      //   this.filter.PeriodFrom,
      //   this.filter.PeriodUntil
      // );

      // if (rangePeriodDays > 30) {
      //   toastWarning("Range Period only 30 days.");
      //   return;
      // }

      this.ds.setPage(1);
      this.ds.setLength(1000);
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.FactoryCode,
          SupplierCode: this.filter.SupplierCode,
          PONumber: this.filter.PONumber,
          RemainingCls: this.filter.RemainingCls,
          Warehouse: this.filter.Warehouse,
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
            item.PONumber,
            item.PODate,
            item.ItemCode,
            item.ItemName,
            item.UnitClsDesc,
            item.PlanQty,
          ].join("|");

          if (!grouped[key]) {
            let totalRequestQty = dt.Data.Items.filter(
              (x) => x.PONumber == item.PONumber && x.RequestId != null
            ).reduce((a, b) => a + (b.RequestSetQty || 0), 0);

            grouped[key] = {
              ...item,
              RequestSetQty: totalRequestQty,
              RemainingQty: item.PlanQty - totalRequestQty,
              Selected: false,
              Expanded: true,
              Details: [],
            };
          }

          if (item.RequestId) {
            grouped[key].Details.push({
              RequestId: item.RequestId,
              RequestNo: item.RequestNo,
              RequestDate: item.RequestDate,
              RequestSetQty: item.RequestSetQty,
              RegisterUser: item.RegisterUser,
              RegisterDate: item.RegisterDate,
            });
          }
        });

        this.groupLists = Object.values(grouped);
      });
    },
    reset: function () {
      this.filter.FactoryCode = null;
      this.filter.SupplierCode = null;
      this.filter.Warehouse = null;
      this.filter.PONumber = null;
      this.filter.RemainingCls = null;

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
      item.Selected = e.target.checked;
    },
    newRequest: function () {
      let selected = this.groupLists.filter((x) => x.Selected);
      if (selected.length == 0) {
        toastWarning("Please choose schedule!");
        return;
      }

      if (selected.filter((x) => x.RequestSetQty <= 0).length > 0) {
        toastWarning("Request Qty must be greater than ZERO!");
        return;
      }

      if (selected.filter((x) => x.RequestSetQty > x.RemainingQty).length > 0) {
        toastWarning("Request Qty cannot be greater than Remaining Qty!");
        return;
      }

      let newRequestPayload = selected.map((x) => {
        return {
          WarehouseCode: this.filter.Warehouse,
          RequestId: null,
          PONumber: x.PONumber,
          PODate: x.PODate,
          ItemCode: x.ItemCode,
          RequestSetQty: x.RequestSetQty,
        };
      });

      this.ds.setRequest(newRequestPayload);
      this.$router.push("/app/supply-request/bom/create");
    },
    viewRequest: function (selected, dtl) {
      let payloadrequest = [
        {
          WarehouseCode: this.filter.Warehouse,
          RequestId: dtl.RequestId,
          RequestNo: dtl.RequestNo,
          RequestDate: dtl.RequestDate,
          PONumber: selected.PONumber,
          PODate: selected.PODate,
          ItemCode: selected.ItemCode,
          RequestSetQty: selected.RequestSetQty,
        },
      ];

      this.ds.setRequest(payloadrequest);
      this.$router.push("/app/supply-request/bom/view");
    },

    setDefaultFilter: function () {
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.PeriodUntil = today;
      this.filter.RemainingCls = "ALL";
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
</style>
