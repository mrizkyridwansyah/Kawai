<template>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-data="ds.dataListScan"
    :ds-page="ds.setPageListScan"
    :ds-length="ds.setLengthListScan"
    :ds-load="ds.loadListScan"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="text-center">Barcode No</th>
            <th class="text-center">Item Code</th>
            <th class="text-center">Item Name</th>
            <th class="text-center">Qty</th>
            <th class="text-center">User Scan</th>
            <th class="text-center">Time Scan</th>
            
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.dataListScan.Items">
            <td class="text-center">{{ idx + 1 }}.</td>
            <td>{{ item.BarcodeNo }}</td>
            <td>{{ item.ItemCode }}</td>
            <td>{{ item.ItemName }}</td>
             <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
            <td>{{ item.UserScan }}</td>
            <td>{{ item.TimeScan }}</td> 
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
        BarcodeNo: "asc",
        ItemCode: "asc",
        TimeScan: "asc" 
      },
      sortItems: [
        {
          label: "Barcode No",
          value: "BarcodeNo",
          selected: true,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "ItemCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Time Scan",
          value: "TimeScan",
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
      this.ds.setSortListScan(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          IDSeqno: this.item.toString() || "0",
        },
      ];

      this.ds.setFilterListScan(filters);
      this.$nextTick(() => this.ds.loadListScan());
    },
    reset: function () {
      this.item = null;
      this.search();
    },
  },
};
</script>
