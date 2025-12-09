<template>
  <v-frame title="Part Receipt Material Inquiry" icon="receipt">
    <template #frame-content>
      <div class="row">
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-12 col-12">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Factory</label>
            <input-factory-privileges v-model="filter.FactoryCode" />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-12 col-12">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Supplier</label>
            <input-trade
              placeholder="Search Supplier"
              :trade-cls="['2', '3']"
              v-model="filter.SupplierCode"
              :show-option-all="true"
            />
          </div>
        </div>
        <div
          class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-xs-12 col-12 mt-2"
        >
          <label class="form-label">Receipt Date</label>
          <div class="mr-1" style="width: 100%; display: flex">
            <input-date v-model="filter.PeriodFrom" />
            <label class="form-label ml-2 mr-2 mt-2">s/d</label>
            <input-date v-model="filter.PeriodUntil" class="ml-2 mr-2" />
            <v-button-search-reset
              class="ml-2 ms-1"
              :search="search"
              :reset="reset"
            />
          </div>
        </div>
      </div>

      <v-table
        :filter="filter"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :ds="ds"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Receipt No</th>
                <th class="text-center">Supplier</th>
                <th class="text-center">Delivery Date</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">DN Number</th>
                <th class="text-center">PO Number</th>
                <th class="text-center">BC Type</th>
                <th class="text-center">BC No</th>
                <th class="text-center">BC Date</th>
                <th class="text-center">Qty</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Currency</th>
                <th class="text-center">Price</th>
                <th class="text-center">Amount</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items || []" :key="idx">
                <td>{{ item.ReceiptNo }}</td>
                <td>{{ item.SupplierName }}</td>
                <td>{{ $func.formatDate(item.DNDate) }}</td>
                <td>{{ item.ItemCode }}</td>
                <td>{{ item.ItemName }}</td>
                <td>{{ item.DNNumber }}</td>
                <td>{{ item.PONumber }}</td>
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
      SupplierCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
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
    "filter.SupplierCode": function () {
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
          SupplierCode: this.filter.SupplierCode,
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
      this.filter.SupplierCode = null;

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
          SupplierCode: this.filter.SupplierCode,
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
