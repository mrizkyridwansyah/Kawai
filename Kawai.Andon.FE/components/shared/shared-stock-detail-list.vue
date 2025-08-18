<template>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-page="ds.setPageDetail"
    :ds-length="ds.setLengthDetail"
    :ds-load="ds.inquiryDetail"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="text-center">Barcode No</th>
            <th class="text-center">Sublot No</th>
            <th class="text-center">Qty</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
            <td class="text-center">{{ idx + 1 }}.</td>
            <td>{{ item.BarcodeNo }}</td>
            <td class="text-center">{{ item.SublotNo }}</td>
            <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["warehouse", "location", "item", "lotno"],
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
        {
          label: "Sublot",
          value: "SublotNo",
          selected: false,
          direction: "asc",
        },
      ],
    },
  }),
  computed: {
    ds: function () {
      return useStock();
    },
  },
  watch: {
    warehouse: function () {
      this.search();
    },
    location: function () {
      this.search();
    },
    item: function () {
      this.search();
    },
    lotno: function () {
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
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ItemCode: this.item || "",
          WarehouseCode: this.warehouse || "",
          LocationCode: this.location || "",
          LotNo: this.lotno || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.inquiryDetail();
    },
    reset: function () {
      this.item = null;
      this.warehouse = null;
      this.location = null;
      this.lotno = null;
      this.search();
    },
  },
};
</script>
