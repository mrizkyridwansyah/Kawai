<template>
  <v-frame title="Inventory Report" icon="table">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Warehouse</label></td>
          <td style="padding-left: 15px" colspan="3">
            <filter-warehouse-privileges
              class="form-control"
              v-model="filter.warehouse"
              factory-code="ALL"
              style-code="width: 150px;"
              style-desc="width: 250px;"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Area</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-area-privileges
              class="form-control"
              v-model="filter.area"
              :warehouse="filter.warehouse"
              style-code="width: 150px;"
              style-desc="width: 250px;"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Period</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 200px">
            <input-month v-model="filter.period" style="width: 180px" />
          </td>
          <td style="padding-top: 5px" colspan="2">
            <div style="margin-left: -30px">
              <v-button-search-reset :search="search" :reset="reset" />
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <!-- TABLE -->
      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :ds="ds"
        :data-items="ds.data.Items"
        :export-excel="true"
        :frozen-column-left="3"
        :export-excel-action="exportExcel"
        :top-content-height="255"
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
                <th class="text-center">Warehouse</th>
                <th class="text-center">Product Code</th>
                <th class="text-center">Product Name</th>
                <th class="text-center">Lot No</th>
                <th class="text-center">Pre Month</th>
                <th class="text-center">Receipt</th>
                <th class="text-center">Supply</th>
                <th class="text-center">Loss / Reject</th>
                <th class="text-center">Current</th>
                <th class="text-center">Inventory</th>
                <th class="text-center">Remarks</th>
                <th class="text-center">User</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="(item, i) in ds.data.Items" :key="i">
                <td>{{ item.Warehouse }}</td>
                <td>{{ item.ProductCode }}</td>
                <td>{{ item.ProductName }}</td>
                <td>{{ item.LotNo }}</td>
                <td class="text-end">{{ $func.formatMoney(item.PreMonth) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Receipt) }}</td>
                <td class="text-end">{{ $func.formatMoney(item.Supply) }}</td>
                <td class="text-end">
                  {{ $func.formatMoney(item.LossReject) }}
                </td>
                <td class="text-end">{{ $func.formatMoney(item.Current) }}</td>
                <td class="text-end">
                  {{ $func.formatMoney(item.Inventory) }}
                </td>
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
        { Id: "ProductCode", Name: "Product Code" },
        { Id: "ProductName", Name: "Product Name" },
        { Id: "Warehouse", Name: "Warehouse" },
        { Id: "LotNo", Name: "Lot No" },
      ],

      filter: {
        warehouse: null,
        area: null,
        period: new Date(
          new Date().getFullYear(),
          new Date().getMonth(),
          1,
        ).toISOString(),

        keyword: null,

        sorts: {},

        sortItems: [
          {
            label: "Product Code",
            value: "ProductCode",
            selected: false,
            direction: "asc",
          },
          {
            label: "Product Name",
            value: "ProductName",
            selected: false,
            direction: "asc",
          },
          {
            label: "Warehouse",
            value: "Warehouse",
            selected: false,
            direction: "asc",
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
    "filter.keyword"() {
      this.search();
    },
    "filter.sorts"() {
      this.search();
    },
  },

  methods: {
    search() {
      const filters = [
        {
          WarehouseCode: this.filter.warehouse || "",
          AreaCode: this.filter.area || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
          Keyword: this.filter.keyword || "",
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

    exportExcel() {
      const filters = [
        {
          WarehouseCode: this.filter.warehouse || "",
          AreaCode: this.filter.area || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
          Keyword: this.filter.keyword || "",
        },
      ];
      return this.ds.exportExcel(filters);
    },
  },
};
</script>

<style scoped>
thead {
  white-space: nowrap;
}
</style>
