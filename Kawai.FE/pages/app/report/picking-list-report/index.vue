<template>
  <v-frame title="Picking List Report" icon="table">
    <template #frame-content>
      <table class="filter-table">
        <tr>
          <td><label class="form-label">Customer</label></td>
          <td colspan="4">
            <filter-trade-cust
              class="form-control"
              v-model="filter.customer"
              :placeholder="'ALL'"
              style-code="width: 250px;"
              style-desc="width: 320px;"
            />
          </td>
        </tr>
        <tr>
          <td><label class="form-label">Date</label></td>
          <td>
            <input-date v-model="filter.dateFrom" style-date="width: 130px !important" />
          </td>
          <td class="to-label"><label class="form-label">To</label></td>
          <td>
            <input-date v-model="filter.dateTo" style-date="width: 130px !important" />
          </td>
          <td></td>
        </tr>
        <tr>
          <td><label class="form-label">Shipping Instruction No.</label></td>
          <td>
            <filter-shipping-instruction
              class="form-control"
              v-model="filter.shippingInstructionNo"
              :customer="filter.customer"
              :date-from="filter.dateFrom"
              :date-to="filter.dateTo"
              :placeholder="'Select Shipping Instruction No'"
              style-code="width: 270px;"
            />
          </td>
          <td colspan="3">
            <v-button-search-reset :search="search" :reset="reset" />
          </td>
        </tr>
      </table>
      <div class="mt-2">
        <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :ds="ds"
        :export-excel="true"
        :export-excel-action="exportExcel"
      >
        syncing
        <template #table-content>
          <table
            class="table table-bordered mb-0 align-middle report-table"
            style="min-width: 100%; width: max-content"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Cust. Code</th>
                <th class="text-center">Cust. Name</th>
                <th class="text-center">Shipping Instruction No.</th>
                <th class="text-center">Shipping Instruction Date</th>
                <th class="text-center">Part Number</th>
                <th class="text-center">Description</th>
                <th class="text-center">Serial No.</th>
                <th class="text-center">Address</th>
                <th class="text-center">Picking Date</th>
                <th class="text-center">Time</th>
                <th class="text-center">Picking By</th>
              </tr>
            </thead>

            <tbody>
              <tr v-if="!ds.data.Items?.length">
                <td colspan="11" class="text-center text-muted">No data</td>
              </tr>

              <tr v-for="(item, i) in ds.data.Items" :key="i">
                <td>{{ item.CustCode || item.Cust_Code || "" }}</td>
                <td>{{ item.Trade_Name || item.Trade_Name || "" }}</td>
                <td>{{ item.SINo || item.SI_No || item.SI_NO || "" }}</td>
                <td>{{ formatDate(item.SIDate || item.SI_Date, "") }}</td>
                <td>{{ item.PartNumber || item.Item_Code || item.ProductCode || "" }}</td>
                <td>{{ item.Description || item.Item_Name || item.ProductName || "" }}</td>
                <td>{{ item.SerialNo || item.Serial_No || item.LotNo || "" }}</td>
                <td>{{ item.Address || "" }}</td>
                <td>{{ formatDate(item.PickingDate || item.Picking_Date, "") }}</td>
                <td>{{ item.PickingTime || item.Picking_Time || "" }}</td>
                <td>{{ item.Picking_Name || item.PickingBy || item.Picking_By || item.LastUser || "" }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
      </div>
    </template>
  </v-frame>
</template>

<script>
export default {
  data() {
    return {
      keywordKeys: [
        { Id: "Cust_Code", Name: "Cust. Code" },
        { Id: "Trade_Name", Name: "Cust. Name" },
        { Id: "SI_NO", Name: "Shipping Instruction No." },
        { Id: "Serial_No", Name: "Serial No." },
      ],

      filter: {
        customer: 'ALL',
        dateFrom: new Date(
          new Date().getFullYear(),
          new Date().getMonth(),
          1
        ),
        dateTo: new Date(
          new Date().getFullYear(),
          new Date().getMonth() + 1,
          0
        ),
        shippingInstructionNo: "ALL",
        keyword: null,

        sorts: {},

        sortItems: [
          // {
          //   label: "Cust. Code",
          //   value: "Cust_Code",
          //   selected: false,
          //   direction: "asc",
          // },
          // {
          //   label: "Cust. Name",
          //   value: "Trade_Name",
          //   selected: false,
          //   direction: "asc",
          // },
          {
            label: "Serial No.",
            value: "Serial_No",
            selected: false,
            direction: "asc",
          },
        ],
      },
    };
  },

  computed: {
    ds() {
      return usePickingListReport();
    },
  },

  mounted() {
    this.search();
  },

  watch: {
    "filter.keyword"() {
      this.search();
    },
    "filter.sorts"() {
      this.search();
    },
  },

  methods: {
    formatDate(value, fallback = "-") {
      if (!value) return fallback;
      const dt = new Date(value);
      if (Number.isNaN(dt.getTime())) return fallback;

      const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      const day = String(dt.getDate()).padStart(2, '0');
      const month = months[dt.getMonth()];
      const year = String(dt.getFullYear()).slice(-2);

      return `${day}-${month}-${year}`;
    },
    search() {
      const filters = [
        {
          CustCode: this.filter.customer || "ALL",
          DateFrom: this.filter.dateFrom
            ? this.$func.asUtcStringDateOnly(new Date(this.filter.dateFrom))
            : "",
          DateTo: this.filter.dateTo
            ? this.$func.asUtcStringDateOnly(new Date(this.filter.dateTo))
            : "",
          ShippingNo: this.filter.shippingInstructionNo || "ALL",
          Keyword: this.filter.keyword || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.setSort(this.filter.sorts);
      this.ds.load();
    },

    reset() {
      const today = new Date();
      this.filter.customer = 'ALL';
      this.filter.dateFrom = new Date(today.getFullYear(), today.getMonth(), 1);
      this.filter.dateTo = new Date(today.getFullYear(), today.getMonth() + 1, 0);
      this.filter.shippingInstructionNo = "ALL";
      this.filter.keyword = null;
      this.filter.sorts = {};
      this.search();
    },

    exportExcel() {
      const filters = [
          {
            CustCode: this.filter.customer || "ALL",
            DateFrom: this.filter.dateFrom
              ? this.$func.asUtcStringDateOnly(new Date(this.filter.dateFrom))
              : "",
            DateTo: this.filter.dateTo
              ? this.$func.asUtcStringDateOnly(new Date(this.filter.dateTo))
              : "",
            ShippingNo: this.filter.shippingInstructionNo || "ALL",
            Keyword: this.filter.keyword || "",
          },
        ];
        return this.ds.exportExcel(filters);
    },
  },
};
</script>

<style>
.filter-table td {
  padding-top: 5px;
  vertical-align: middle;
}

.filter-table td:first-child {
  width: 190px;
}

.filter-table td:not(:first-child) {
  padding-left: 12px;
}

.to-label {
  width: 28px;
}

.report-table thead th {
  white-space: nowrap;
  background-color: #8cb9e6;
  color: #20374f;
  font-size: 12px;
}

.report-table td {
  white-space: nowrap;
  font-size: 12px;
  padding: 4px 8px;
}
</style>
