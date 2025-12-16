<template>
  <v-frame title="Part Material Supply Request (WOMIN)" icon="cart-flatbed">
    <template #frame-content>
      <div class="row">
        <label
          style="white-space: nowrap"
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Schedule Date</label
        >
        <div class="col-xl-2 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <input-date v-model="filter.PeriodFrom" />
        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Until Date</label
        >
        <div class="col-xl-2 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <input-date v-model="filter.PeriodUntil" />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Factory</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <filter-factory-privileges
            class="form-control"
            v-model="filter.FactoryCode"
          />
        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Line</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <filter-manufacture
            class="form-control"
            v-model="filter.ManufactureCode"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Machine</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <filter-line-factory
            class="form-control"
            :company="filter.FactoryCode"
            :manufacture="filter.ManufactureCode"
            v-model="filter.LineCode"
          />
        </div>
        <label
          style="white-space: nowrap"
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Remaining Cls</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <input-remaining-cls
            class="form-control"
            placeholder="Search Remaining Cls"
            v-model="filter.RemainingCls"
          />
        </div>
      </div>
      <div class="row mt-1">
        <div class="col-xl-11 col-lg-11 col-md-6 col-sm-6 col-xs-6 mt-1">
          <v-button-search-reset :search="search" :reset="reset" />
        </div>
      </div>

      <v-table
        :filter="filter"
        :export-excel="true"
        :export-excel-action="exportExcel"
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
                <th class="text-center">Schedule Date</th>
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
              <tr v-for="(item, idx) in ds.data.Items || []" :key="idx">
                <td><input-checkbox /></td>
                <td>{{ item.PONumber }}</td>
                <td>{{ item.ReceiptNo }}</td>
                <td>{{ item.SupplierName }}</td>
                <td>{{ $func.formatDate(item.DNDate) }}</td>
                <td>{{ item.ItemCode }}</td>
                <td>{{ item.ItemName }}</td>
                <td>{{ item.DNNumber }}</td>
                <td>{{ item.BCType }}</td>
                <td>{{ item.BCNumber }}</td>
                <td>{{ $func.formatDate(item.BCDate) }}</td>
                <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
                <td>{{ item.UnitClsDescription }}</td>
                <td>{{ item.Currency }}</td>
                <td class="text-right">{{ $func.formatMoney(item.Price) }}</td>
                <td class="text-right">{{ $func.formatMoney(item.Amount) }}</td>
              </tr>
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
    breadcrumbs: [
      { title: "Stock Control", active: false, to: "" },
      { title: "Part Receipt Material", active: false, to: "" },
    ],
    filter: {
      keyword: null,
      keywordKey: "ReceiptNo",
      FactoryCode: null,
      ManufactureCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      LineCode: null,
      RemainingCls: null,
      sorts: {
        ReceiptNo: "asc",
      },
      sortItems: [
        {
          label: "Receipt No.",
          value: "ReceiptNo",
          selected: true,
          direction: "asc",
        },
        {
          label: "Supplier",
          value: "SupplierName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item",
          value: "ItemName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Delivery Date",
          value: "DNDate",
          selected: false,
          direction: "asc",
        },
      ],
    },
    debounce: null,
    lists: [],
  }),
  computed: {
    ds: function () {
      return useReceiptInquiry();
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
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
    this.search();
  },
  methods: {
    resetGrid: function () {
      this.ds.setFilter([]);
      this.ds.setPage(1);
      this.ds.setLength(10);
      this.ds.data.Items = [];
    },
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.FactoryCode,
          ManufactureCode: this.filter.ManufactureCode,
          LineCode: this.filter.LineCode,
          RemainingCls: this.filter.RemainingCls,
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom)
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil)
          ),
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
    },
    reset: function () {
      this.filter.FactoryCode = null;
      this.filter.ManufactureCode = null;
      this.filter.LineCode = null;
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
    exportExcel: function () {
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.FactoryCode,
          ManufactureCode: this.filter.ManufactureCode,
          LineCode: this.filter.LineCode,
          RemainingCls: this.filter.RemainingCls,
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom)
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil)
          ),
        },
      ];

      return new Promise((resolve, reject) => {
        this.ds
          .exportExcel(filters)
          .then((_) => {
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          });
      });
    },
  },
};
</script>

<style scoped>
.vdatetime {
  max-width: 60% !important;
}
</style>
