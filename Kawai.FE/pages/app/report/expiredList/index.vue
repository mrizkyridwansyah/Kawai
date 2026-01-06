<template>
  <v-frame title="Expired List" icon="receipt">
    <template #frame-content>
  
      <div class="row mt-3">
        <label class="col-md-1 col-form-label">Expired Until</label>
        <div class="col-md-3">
          <input-date v-model="filter.ExpiredUntil" />
        </div>
      </div>
  
      <div class="row mt-2">
        <div class="col-md-12">
          <v-button-search-reset :search="search" :reset="reset" />
        </div>
      </div>
  
      <v-table
        :filter="filter"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :frozen-column-left="3"
        :data-items="ds.data.Items"
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
                <th class="text-center">Warehouse Code</th>
                <th class="text-center">Warehouse Name</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Part Number</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Lot No</th>
                <th class="text-center">Receipt Date</th>
                <th class="text-center">Manufacture Date</th>
                <th class="text-center">Expired Day</th>
                <th class="text-center">Qty</th>
                <th class="text-center">Expired Date</th>
                <th class="text-center">Status</th>
              </tr>
            </thead>
  
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items || []" :key="idx">
                <td>{{ item.WHCode }}</td>
                <td>{{ item.WHName }}</td>
                <td>{{ item.ItemCode }}</td>
                <td>{{ item.PartNo }}</td>
                <td>{{ item.ItemName }}</td>
                <td>{{ item.Unit }}</td>
                <td>{{ item.LotNo }}</td>
                <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                <td>{{ $func.formatDate(item.ManufactureDate) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.ExpiredDay) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Qty) }}</td>
                <td>{{ $func.formatDate(item.ExpiredDate) }}</td>
                <td>{{ item.Status }}</td>
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
      { title: "Report", active: false, to: "" },
      { title: "Expired List", active: false, to: "" },
    ],

    /* ===============================
     * FILTER
     * =============================== */
    filter: {
      keyword: null,
      ExpiredUntil: null,
      sorts: {
        ItemCode: "asc",
      },
      sortItems: [
        { label: "Warehouse Code", value: "WHCode", selected: true, direction: "asc" },
        { label: "Item Code", value: "ItemCode", selected: false, direction: "asc" },
        { label: "Lot No", value: "LotNo", selected: false, direction: "asc" },
        { label: "Receipt Date", value: "ReceiptDate", selected: false, direction: "asc" },
        { label: "Manufacture Date", value: "ManufactureDate", selected: false, direction: "asc" },
        { label: "Expired Day", value: "ExpiredDay", selected: false, direction: "asc" },
        { label: "Expired Date", value: "ExpiredDate", selected: false, direction: "asc" },
        { label: "Status", value: "Status", selected: false, direction: "asc" },
      ],
    },

    debounce: null,
    lists: [],
  }),

  /* ===============================
   * STORE
   * =============================== */
  computed: {
    ds() {
      return useExpiredList();
    },
  },  

  watch: {
    "filter.ExpiredUntil": function () {
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
    this.filter.ExpiredUntil = today;
    this.search();
  },

  methods: {
    resetGrid: function () {
      this.ds.setFilter([]);
      this.ds.setPage(1);
      this.ds.setLength(5);
      this.ds.data.Items = [];
    },
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ExpiredUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.ExpiredUntil)
          ),
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
    },
    reset: function () {
      let today = new Date();
      this.filter.ExpiredUntil = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.ExpiredUntil = today;
      this.search();
    },
    exportExcel: function () {
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ExpiredUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.ExpiredUntil)
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