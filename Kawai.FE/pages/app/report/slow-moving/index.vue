<template>
  <v-frame title="Slow Moving Report (Red Stock, Yellow Stock)" icon="table">
    <template #frame-content>
      <!-- FILTER -->
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Factory</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-factory-privileges
              class="form-control"
              v-model="filter.factoryCode"
              :disabled="null"
              style-code="width: 170px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">WareHouse</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-warehouse-privileges
              class="form-control"
              v-model="filter.warehouseCode"
              factory-code="ALL"
              style-code="width: 170px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Period</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 200px">
            <input-month
              v-model="filter.period"
              class="mr-1"
              style="width: 215px"
            />
          </td>

          <td colspan="4" style="padding-top: 5px">
            <div style="margin-left: -40px">
              <v-button-search-reset :search="search" :reset="reset" />
            </div>
          </td>
        </tr>
        <tr></tr>
      </table>
      <hr />
      <!-- TABLE -->
      <v-table
        :filter="filter"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :frozen-column-left="2"
        :data-items="ds.data.Items"
        :ds="ds"
        :top-content-height="290"
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
                <th class="text-center" rowspan="2">Long Stock</th>
                <th class="text-center" rowspan="2">Warehouse</th>
                <th class="text-center" rowspan="2">Item Code</th>
                <th class="text-center" rowspan="2">Item Name</th>
                <th class="text-center" :colspan="periods.length">Period</th>
                <th class="text-center" rowspan="2">Unit</th>
                <th class="text-center" rowspan="2">Remarks</th>
              </tr>
              <tr>
                <th class="text-center" v-for="p in periods" :key="p">
                  {{ formatPeriod(p) }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items || []" :key="idx">
                <td>{{ item.LongStock }}</td>
                <td>{{ item.WHCode }} - {{ item.WHName }}</td>
                <td>{{ item.Item_Code }}</td>
                <td>{{ item.Item_Name }}</td>
                <td class="text-end" v-for="p in periods" :key="p">
                  {{ formatQty(item.PeriodQty?.[p]) }}
                </td>
                <td>{{ item.Unit_Name }}</td>
                <td>{{ item.Remarks }}</td>
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
      {
        title: "Slow Moving",
        active: true,
        to: "/report/slow-moving",
      },
    ],
    keywordKeys: [
      { Id: "ItemCode", Name: "Item Code" },
      { Id: "ItemName", Name: "Item Name" },
      { Id: "WarehouseCode", Name: "Warehouse Code" },
      { Id: "LongStock", Name: "Long Stock" },
    ],
    filter: {
      factoryCode: null,
      warehouseCode: null,
      period: null,

      keyword: null,

      sorts: {
        WarehouseCode: "asc",
        ItemCode: "asc",
      },

      sortItems: [
        {
          label: "Warehouse Code",
          value: "WarehouseCode",
          selected: false,
          direction: "asc",
        },
        {
          label: "Warehouse Name",
          value: "WarehouseName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "ItemCode",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Name",
          value: "ItemName",
          selected: false,
          direction: "asc",
        },
      ],
    },
  }),

  computed: {
    ds: function () {
      return useSlowMoving();
    },
    periods() {
      if (!this.ds?.data?.Items?.length) return [];
      return Object.keys(this.ds.data.Items[0].PeriodQty || {}).sort(
        (a, b) => Number(b) - Number(a),
      );
    },
  },

  mounted: function () {
    this.filter.period = new Date(
      new Date().getFullYear(),
      new Date().getMonth(),
      1,
    ).toISOString();

    //this.search();
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
          FactoryCode: this.filter.factoryCode || "",
          WarehouseCode: this.filter.warehouseCode || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
          Keyword: this.filter.keyword || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.setSort(this.filter.sorts);
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
    },

    reset() {
      this.filter.factoryCode = null;
      this.filter.warehouseCode = null;
      this.filter.keyword = null;
      this.filter.sorts = {};
      this.search();
    },

    exportExcel() {
      const filters = [
        {
          FactoryCode: this.filter.factoryCode || "",
          WarehouseCode: this.filter.warehouseCode || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
          Keyword: this.filter.keyword || "",
        },
      ];

      return this.ds.exportExcel(filters);
    },

    formatPeriod(p) {
      // p = "202509"
      const year = p.substring(2, 4); // "25"
      const month = p.substring(4, 6); // "09"

      const monthName = new Date(`2000-${month}-01`).toLocaleDateString(
        "en-US",
        { month: "short" },
      );

      return `${monthName}-${year}`;
    },

    formatQty(val) {
      if (val === null || val === undefined) return "0.0000";

      return new Intl.NumberFormat("en-US", {
        minimumFractionDigits: 4,
        maximumFractionDigits: 4,
      }).format(val);
    },
  },
};
</script>
