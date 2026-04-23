<template>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-data="ds.dataListSupplyScan"
    :ds-page="ds.setPageListSupplyScan"
    :ds-length="ds.setLengthListSupplyScan"
    :ds-load="ds.loadListSupplyScan"
    :default-height="280"
    :max-height="280"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="text-center">Item</th>
            <th class="text-center">Barcode No</th>
            <th class="text-center">Qty</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.dataListSupplyScan.Items">
            <td class="text-center">{{ idx + 1 }}.</td>
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
  props: ["item" , "productionid", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        BarcodeNo: "asc",
      },
      sortItems: [
        
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
      return useSupplyScan();
    },
  },
  watch: {
    item: function () {
      this.search();
    },
    productionid: function () {
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
      this.ds.setSortListSupplyScan(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ItemCode: this.item || "",
          ProductionId: this.productionid.toString() || "",
        },
      ];

      this.ds.setFilterListSupplyScan(filters);
      this.$nextTick(() => this.ds.loadListSupplyScan());
    },
    reset: function () {
      this.item = null;
       this.productionid = null;
      this.search();
    },
  },
};
</script>
