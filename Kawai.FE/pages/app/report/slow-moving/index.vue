<template>
    <v-frame title="Slow Moving Report (Red Stock, Yellow Stock)" icon="table">
        <template #frame-content>

            <!-- FILTER -->
            <div class="row">
                <label
                class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
                >Factory</label
                >
                <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
                <filter-factory-privileges
                class="form-control"
                    v-model="filter.factoryCode"
                    :disabled="null"
                />
                </div>
            </div>

            <div class="row mt-1">
                <label class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1">
                    Warehouse
                </label>
                <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
                    <filter-warehouse-privileges
                        class="form-control"
                        v-model="filter.warehouseCode"
                        factory-code="ALL"
                    />
                </div>
            </div>

            <div class="row mt-1">
                <label class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1">
                    Period
                </label>
                <div class="col-xl-3 col-lg-3 col-md-6 col-sm-6 col-xs-6">
                    <input-month
                        v-model="filter.period"
                        class="mr-1"
                        placeholder="Period"
                    />
                </div>

                <div class="col-xl-5 col-lg-5 col-md-4 col-sm-4 col-xs-5">
                    <v-button-search-reset :search="search" :reset="reset" />
                </div>
            </div>
            
            <!-- TABLE -->
            <v-table
              :filter="filter"
              :export-excel="true"
              :export-excel-action="exportExcel"
              :frozen-column-left="2"
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
                        <th class="text-center" rowspan="2">Long Stock</th>
                        <th class="text-center" rowspan="2">Warehouse</th>
                        <th class="text-center" rowspan="2">Item Code</th>
                        <th class="text-center" rowspan="2">Item Name</th>
                        <th class="text-center" :colspan="periods.length"> Period</th>
                        <th class="text-center" rowspan="2">Unit</th>
                        <th class="text-center" rowspan="2">Remarks</th>
                    </tr>
                    <tr>
                        <th class="text-center"
                            v-for="p in periods"
                            :key="p"
                        >
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
                        <td class="text-end"
                            v-for="p in periods"
                            :key="p"
                            >
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
        { Id: 'ItemCode', Name: 'Item Code' },
        { Id: 'ItemName', Name: 'Item Name' },
        { Id: 'WarehouseCode', Name: 'Warehouse Code' },
        { Id: 'LongStock', Name: 'Long Stock' },
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
                label: 'Warehouse Code',
                value: 'WarehouseCode',
                selected: false,
                direction: 'asc',
            },
            {
                label: 'Warehouse Name',
                value: 'WarehouseName',
                selected: false,
                direction: 'asc',
            },
            {
                label: "Item Code",
                value: "ItemCode",
                selected: false,
                direction: "asc",
            },
            {
                label: 'Item Name',
                value: 'ItemName',
                selected: false,
                direction: 'asc',
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
        return Object.keys(this.ds.data.Items[0].PeriodQty || {}).sort((a, b) => Number(b) - Number(a));;
    }
  },

  mounted: function () {
    this.filter.period = new Date(
        new Date().getFullYear(),
        new Date().getMonth(),
        1
      ).toISOString();

    //this.search();
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
            FactoryCode: this.filter.factoryCode || '',
            WarehouseCode: this.filter.warehouseCode || '',
            Period: this.$func.asUtcStringDateOnly(
                new Date(this.filter.period)
            ),
            Keyword: this.filter.keyword || '',
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
                FactoryCode: this.filter.factoryCode || '',
                WarehouseCode: this.filter.warehouseCode || '',
                Period: this.$func.asUtcStringDateOnly(
                    new Date(this.filter.period)
                ),
                Keyword: this.filter.keyword || '',
            },
        ];
        
        return this.ds.exportExcel(filters);
    },

    formatPeriod(p) {
        // p = "202509"
        const year = p.substring(2, 4);   // "25"
        const month = p.substring(4, 6);  // "09"

        const monthName = new Date(`2000-${month}-01`)
        .toLocaleDateString('en-US', { month: 'short' });

        return `${monthName}-${year}`;
    },

    formatQty(val) {
        if (val === null || val === undefined) return '0.0000';

        return new Intl.NumberFormat('en-US', {
        minimumFractionDigits: 4,
        maximumFractionDigits: 4
        }).format(val);
    }
  },
};
</script>
