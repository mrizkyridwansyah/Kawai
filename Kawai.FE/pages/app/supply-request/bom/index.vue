<template>
  <v-frame title="Part Material Supply Request (By BOM)" icon="cart-flatbed">
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
              placeholder=" "
              :trade-cls="['2', '3']"
              v-model="filter.SupplierCode"
              :show-option-all="true"
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
              placeholder=" "
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
              placeholder=" "
              :factory-code="filter.FactoryCode"
              v-model="filter.Warehouse"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Remaining Cls</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-remaining-cls
              class="form-control"
              placeholder=" "
              v-model="filter.RemainingCls"
              style="width: 110px"
            />
          </td>
        </tr>
        <tr>
          <td colspan="4" style="padding-top: 5px">
            <v-button-search-reset :search="search" :reset="reset" />
          </td>
        </tr>
      </table>

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
      SupplierCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      PONumber: null,
      Warehouse: null,
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
    "filter.SupplierCode": function () {
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
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
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
    exportExcel: function () {
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
