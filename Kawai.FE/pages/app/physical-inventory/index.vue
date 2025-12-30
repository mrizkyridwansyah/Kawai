<template>
  <v-frame title="Physical Inventory" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Warehouse</label></td>
          <td style="padding-left: 15px" colspan="3">
            <filter-warehouse-privileges
              class="form-control"
              v-model="filter.warehouse"
              factory-code="ALL"
              style-code="width: 170px;"
              style-desc="width: 250px;"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Item</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
          <filter-item
            class="form-control"
            placeholder="Search Item"
            v-model="filter.item"
              style-code="width: 170px;"
              style-desc="width: 250px;"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Period</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 200px">
            <input-month v-model="filter.period" style="width: 215px" />
          </td>
          <td style="padding-top: 5px" colspan="2">
            <div style="margin-left: -40px">
              <v-button-search-reset :search="search" :reset="reset" />
            </div>
          </td>
        </tr>
      </table>
      <v-table
        :filter="filter"
        :export-excel="false"
        :frozen-column-left="4"
        :data-items="ds.data.Items"
        :ds="ds"
        :use-header="false"
        :use-paging="false"
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
                <!-- <th class="text-center">Warehouse Code</th> -->
                <th class="text-center">Product Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Address</th>
                <th class="text-center">Pre Month Stock</th>
                <th class="text-center">Receipt Total</th>
                <th class="text-center">Supply Total</th>
                <th class="text-center">Loss / Reject</th>
                <th class="text-center">End Of Month Stock</th>
                <th class="text-center">Inventory</th>
                <th class="text-center">Diffrences</th>
                <th class="text-center">Reason</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items || []" :key="idx">
                <!-- <td>{{ item.WarehouseCode }}</td> -->
                <td>{{ item.ProductCode }}</td>
                <td>{{ item.ProductDesc }}</td>
                <td>{{ item.Unit }}</td>
                <td>{{ item.Address }}</td>
                <td class="text-right">
                  {{ $func.formatDecimal(item.PreMonthStock) }}
                </td>
                <td class="text-right">
                  {{ $func.formatDecimal(item.ReceiptTotal) }}
                </td>
                <td class="text-right">
                  {{ $func.formatDecimal(item.SupplyTotal) }}
                </td>
                <td class="text-right">{{ $func.formatDecimal(item.Loss) }}</td>
                <td class="text-right">
                  {{ $func.formatDecimal(item.EndOfMonthStock) }}
                </td>
                <td class="text-right">
                  <input
                    type="number"
                    step="0.01"
                    class="form-control form-control-sm text-right"
                    v-model.number="item.Inventory"
                    :ref="`inv-${idx}`"
                    @focus="item.Inventory = Number(item.Inventory)"
                    @blur="formatInventory(item)"
                    @keyup.enter="onEnterInventory(item, idx)"
                  />
                </td>
                <td class="text-right">
                  {{
                    $func.formatDecimal(item.Inventory - item.EndOfMonthStock)
                  }}
                </td>
                <td>
                  <input
                    type="string"
                    class="form-control form-control-sm"
                    placeholder="Input reason"
                    :ref="`reason-${idx}`"
                    v-model="item.Reason"
                    @focus="item._oldReason = item.Reason"
                    @keyup.enter="onEnterReason(item, idx)"
                  />
                </td>
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
      {
        title: "Physical Inventory",
        active: true,
        to: "/physical-inventory",
      },
    ],
    filter: {
      keyword: null,
      period: null,
      warehouseCode: null,
      item: null,
      sorts: {
        ItemCode: "asc",
      },
      sortItems: [
        {
          label: "Product Code",
          value: "Item_Code",
          selected: true,
          direction: "asc",
        },
      ],
    },
    debounce: null,
    lists: [],
  }),
  computed: {
    ds: function () {
      return usePhysicalInventory();
    },
  },
  watch: {
    "filter.period": function () {
      this.rawData = [];
      this.treeData = [];
    },
    "filter.warehouse": function () {
      this.rawData = [];
      this.treeData = [];
    },
    "filter.item": function () {
      this.rawData = [];
      this.treeData = [];
    },
  },
  mounted: function () {
    this.filter.period = new Date(
      new Date().getFullYear(),
      new Date().getMonth(),
      1
    ).toISOString();
    //this.getColumns();
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
          WarehouseCode: this.filter.warehouse || "",
          ItemCode: this.filter.item || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
    },
    reset: function () {
      this.filter.warehouse = null;
      this.filter.item = null;
      this.filter.period = null;
      this.search();
    },
    formatInventory(item) {
      let val = Number(item.Inventory || 0);

      // cegah -0.00
      if (Math.abs(val) < 0.005) val = 0;

      item.Inventory = Number(val.toFixed(2));
    },
    async onEnterInventory(item, idx) {
      console.log(idx);
      // pindah ke inventory row berikutnya
      this.$nextTick(() => {
        const nextRef = this.$refs[`inv-${idx + 1}`];
        if (nextRef && nextRef.length > 0) {
          const el = nextRef[0];
          el.focus();
          el.select();
        }
      });

      this.formatInventory(item);

      // kalau tidak berubah, skip update
      if (item._oldInventory !== item.Inventory) {
        await this.updateInventory(item);
      }
    },
    async onEnterReason(item, idx) {
      console.log(idx);
      this.$nextTick(() => {
        const nextRef = this.$refs[`reason-${idx + 1}`];
        if (nextRef && nextRef.length > 0) {
          nextRef[0].focus();
        }
      });

      if ((item._oldReason || "") !== (item.Reason || "")) {
        await this.updateInventory(item);
      }
    },
    async updateInventory(item) {
      const payload = {
        WarehouseCode: item.WarehouseCode,
        Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
        ProductCode: item.ProductCode,
        Inventory: item.Inventory,
        Reason: item.Reason || "",
      };

      try {
        await this.ds.updateInventory(payload);
        item._oldInventory = item.Inventory;
        item._oldReason = item.Reason;
        toastSuccess("Data saved successfully!");
      } catch (e) {
        toastDanger(e.Message);
        console.error(e);
      }
    },
  },
};
</script>
