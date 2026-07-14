<template>
  <v-frame title="Part Material Supply Request (WOMIN)" icon="cart-flatbed">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Schedule Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
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
          <td style="padding-top: 5px; padding-left: 15px">
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
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Model</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-cls-2
              class="form-control"
              :show-option-all="true"
              default-option-all="ALL"
              type-data="Model_Cls"
              v-model="filter.Model"
              style-code="width: 100px"
              style-desc="width: 110px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Process</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-trade-2
              class="form-control"
              :trade-cls="['1']"
              v-model="filter.ManufactureCode"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Line</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-line-factory
              class="form-control"
              :company="filter.FactoryCode"
              :manufacture="filter.ManufactureCode"
              v-model="filter.LineCode"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Remaining Cls</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-yes-no-all
              class="form-control"
              v-model="filter.RemainingCls"
              style="width: 210px"
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
      <hr />

      <v-table
        :filter="filter"
        :ds="ds"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
        :top-content-height="290"
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
                <th class="text-center">Model</th>
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
                  <td>{{ $func.formatDate(item.ScheduleDate) }}</td>
                  <td>{{ item.Model }}</td>
                  <td>
                    <div style="display: flex; justify-content: space-between">
                      <span>
                        {{ item.ItemCode }}
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
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsDesc }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.PlanQty) }}
                  </td>
                  <td>
                    <input-money
                      v-model="item.RequestSetQty"
                      style="width: 100px"
                    />
                  </td>
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
                  <td colspan="3"></td>
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
      ManufactureCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      LineCode: null,
      Model: null,
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
      return useSupplyRequestWomin();
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
    "filter.Model": function () {
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
      this.filter.Model = f.Model;
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
      let rangePeriodDays = this.$func.dateDiffInDays(
        this.filter.PeriodFrom,
        this.filter.PeriodUntil,
      );

      if (
        new Date(this.filter.PeriodFrom) > new Date(this.filter.PeriodUntil)
      ) {
        toastWarning("Periode Dari tidak boleh melewati Periode Sampai.");
        return;
      }

      if (rangePeriodDays > 30) {
        toastWarning("Jarak Periode hanya 30 hari.");
        return;
      }

      if (!this.filter.ManufactureCode) {
        toastWarning("Silahkan pilih process.");
        return;
      }

      if (!this.filter.LineCode) {
        toastWarning("Silahkan pilih line.");
        return;
      }

      if (!this.filter.Model) {
        toastWarning("Silahkan pilih filter remaining.");
        return;
      }

      if (!this.filter.RemainingCls) {
        toastWarning("Silahkan pilih filter remaining.");
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
          Model: this.filter.Model,
          RemainingCls: this.filter.RemainingCls,
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom),
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil),
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
            let totalRequestQty = dt.Data.Items.filter(
              (x) => x.ProductionId == item.ProductionId && x.RequestId != null,
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
      this.filter.ManufactureCode = null;
      this.filter.LineCode = null;
      this.filter.Model = null;
      this.filter.RemainingCls = null;

      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.PeriodUntil = today;
      this.search();
    },
    check: function (e, item) {
      const isChecked = e.target.checked;
      // this.groupLists.forEach((x) => {
      //   x.Selected = false;
      // });
      item.Selected = isChecked;
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
          LineCode: this.filter.LineCode,
          Model: this.filter.Model,
          RequestId: null,
          ProductionId: x.ProductionId,
          ScheduleDate: x.ScheduleDate,
          ItemCode: x.ItemCode,
          RequestSetQty: x.RequestSetQty,
        };
      });

      this.ds.setRequest(newRequestPayload);
      this.$nextTick(() => {
        this.ds
          .checkValid()
          .then(() => {
            this.$router.push("/app/supply-request/womin/create");
          })
          .catch((err) => {
            console.log(err);
            toastDanger(err?.Message);
          })
          .finally(() => (this.isLoading = false));
      });
    },
    viewRequest: function (selected, dtl) {
      let payloadrequest = [
        {
          LineCode: this.filter.LineCode,
          Model: this.filter.Model,
          RequestId: dtl.RequestId,
          RequestNo: dtl.RequestNo,
          RequestDate: dtl.RequestDate,
          ProductionId: selected.ProductionId,
          ScheduleDate: selected.ScheduleDate,
          ItemCode: selected.ItemCode,
          RequestSetQty: selected.RequestSetQty,
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
        1,
      );
      this.filter.PeriodUntil = today;
      this.filter.Model = "ALL";
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
