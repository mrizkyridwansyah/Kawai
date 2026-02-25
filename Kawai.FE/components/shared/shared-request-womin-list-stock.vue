<template>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-data="ds.dataListStock"
    :ds-page="ds.setPageListStock"
    :ds-length="ds.setLengthListStock"
    :ds-load="ds.loadListStock"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="text-center">Warehouse</th>
            <th class="text-center">Area</th>
            <th class="text-center">Address</th>
            <th class="text-center">Item</th>
            <th class="text-center">Barcode No</th>
            <th class="text-center">Qty</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.dataListStock.Items">
            <td class="text-center">{{ idx + 1 }}.</td>
            <td>{{ item.WarehouseCode }}</td>
            <td>{{ item.AreaCode }}</td>
            <td>{{ item.AddressCode }}</td>
            <td>{{ item.ItemName }}</td>
            <td>{{ item.BarcodeNo }}</td>
            <td class="text-right">{{ $func.formatMoney(item.CurrentQty) }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["item", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        WarehouseCode: "asc",
        AreaCode: "asc",
        AddressCode: "asc",
        BarcodeNo: "asc",
      },
      sortItems: [
        {
          label: "Warehouse Code",
          value: "WarehouseCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Area Code",
          value: "AreaCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Address Code",
          value: "AddressCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Barcode No",
          value: "BarcodeNo",
          selected: true,
          direction: "asc",
        },
      ],
    },
  }),
  computed: {
    ds: function () {
      return useSupplyRequestWomin();
    },
  },
  watch: {
    item: function () {
      this.search();
    },
    counter: function () {
      this.search();
    },
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    search: function () {
      this.ds.setSortListStock(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ItemCode: this.item || "",
        },
      ];

      this.ds.setFilterListStock(filters);
      this.$nextTick(() => this.ds.loadListStock());
    },
    reset: function () {
      this.item = null;
      this.search();
    },
  },
};
</script>
