<template>
  <v-frame title="Inventory Report" icon="table">
    <template #frame-content>

      <!-- FILTER -->
      <div class="row">
        <label class="form-label col-form-label col-xl-1 col-lg-1 col-md-2">
          Warehouse
        </label>
        <div class="col-xl-5 col-lg-5 col-md-10">
          <filter-warehouse-privileges
            class="form-control"
            v-model="filter.warehouse"
            factory-code="ALL"
          />
        </div>

        <label class="form-label col-form-label col-xl-1 col-lg-1 col-md-2">
          Area
        </label>
        <div class="col-xl-5 col-lg-5 col-md-10">
          <filter-area-privileges
            class="form-control"
            v-model="filter.area"
            :warehouse="filter.warehouse"
          />
        </div>
      </div>

      <div class="row mt-1">
        <label class="form-label col-form-label col-xl-1 col-lg-1 col-md-2">
          Period
        </label>
        <div class="col-xl-3 col-lg-3 col-md-6">
          <input-month v-model="filter.period" />
        </div>
        <div class="col-xl-8 col-lg-8 col-md-6">
          <v-button-search-reset :search="search" :reset="reset" />
        </div>
      </div>

      <!-- TABLE -->
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
            class="table table-striped table-bordered mb-0 align-middle"
            style="min-width: 100%; width: max-content"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th>Warehouse</th>
                <th>Product Code</th>
                <th>Product Name</th>
                <th>Lot No</th>
                <th class="text-end">Pre Month</th>
                <th class="text-end">Receipt</th>
                <th class="text-end">Supply</th>
                <th class="text-end">Loss / Reject</th>
                <th class="text-end">Current</th>
                <th class="text-end">Inventory</th>
                <th>Remarks</th>
                <th>User</th>
              </tr>
            </thead>

            <tbody>
              <tr v-if="!ds.data.Items?.length">
                <td colspan="12" class="text-center text-muted">No data</td>
              </tr>

              <tr v-for="(item, i) in ds.data.Items" :key="i">
                <td>{{ item.Warehouse }}</td>
                <td>{{ item.ProductCode }}</td>
                <td>{{ item.ProductName }}</td>
                <td>{{ item.LotNo }}</td>
                <td class="text-end">{{ $func.formatMoney(item.PreMonth) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Receipt) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Supply) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.LossReject) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Current) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Inventory) }}</td>
                <td>{{ item.Remarks }}</td>
                <td>{{ item.LastUser }}</td>
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
  data() {
    return {
      keywordKeys: [
        { Id: 'ProductCode', Name: 'Product Code' },
        { Id: 'ProductName', Name: 'Product Name' },
        { Id: 'Warehouse', Name: 'Warehouse' },
        { Id: 'LotNo', Name: 'Lot No' },
      ],

      filter: {
        warehouse: null,
        area: null,
        period: new Date(
          new Date().getFullYear(),
          new Date().getMonth(),
          1
        ).toISOString(),

        keyword: null,

        sorts: {},

        sortItems: [
          {
            label: 'Product Code',
            value: 'ProductCode',
            selected: false,
            direction: 'asc',
          },
          {
            label: 'Product Name',
            value: 'ProductName',
            selected: false,
            direction: 'asc',
          },
          {
            label: 'Warehouse',
            value: 'Warehouse',
            selected: false,
            direction: 'asc',
          },
        ],
      },
    };
  },

  computed: {
    ds() {
      return useInventoryReport();
    },
  },

  mounted() {
    this.search();
  },

  watch: {
    'filter.keyword'() {
      this.search();
    },
    'filter.sorts'() {
      this.search();
    },
  },

  methods: {
    search() {
      const filters = [
        {
          WarehouseCode: this.filter.warehouse || '',
          AreaCode: this.filter.area || '',
          Period: this.$func.asUtcStringDateOnly(
            new Date(this.filter.period)
          ),
          Keyword: this.filter.keyword || '',
        },
      ];

      this.ds.setFilter(filters);
      this.ds.setSort(this.filter.sorts);
      this.ds.load();
    },

    reset() {
      this.filter.warehouse = null;
      this.filter.area = null;
      this.filter.keyword = null;
      this.filter.sorts = {};
      this.search();
    },

    exportExcel(filters) {
      return this.ds.exportExcel(filters);
    },
  },
};
</script>

<style>
thead {
  white-space: nowrap;
}
</style>