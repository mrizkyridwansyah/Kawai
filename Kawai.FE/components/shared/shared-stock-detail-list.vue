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
        class="table table-striped mb-0 align-middle v-fixed-table w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="text-center">Address</th>
            <th class="text-center">Barcode No</th>
            <!-- <th class="text-center">Sublot No</th> -->
            <th class="text-center">Qty</th>
            <th class="text-center">Status</th>
            <th class="text-center">Last Update</th>
            <th class="text-center">Last User</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
            <td class="text-center">{{ idx + 1 }}.</td>
            <td>{{ item.AddressName }}</td>
            <td>{{ item.BarcodeNo }}</td>
            <!-- <td class="text-right">{{ item.SublotNo }}</td> -->
            <td class="text-right">{{ $func.formatMoney(item.CurrentQty) }}</td>
            <td>{{ item.Status }}</td>
            <td class="text-left">
              {{ $func.formatDateTime(item.LastUpdate) }}
            </td>
            <td class="text-left">{{ item.LastUser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["warehouse", "area", "address", "item", "lotno", "counter"],
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
        // {
        //   label: "Sublot",
        //   value: "SublotNo",
        //   selected: false,
        //   direction: "asc",
        // },
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
    area: function () {
      this.search();
    },
    address: function () {
      this.search();
    },
    item: function () {
      this.search();
    },
    lotno: function () {
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
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ItemCode: this.item || "",
          WarehouseCode: this.warehouse || "",
          AreaCode: this.area || "",
          AddressCode: this.address || "",
          LotNo: this.lotno || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.inquiryDetail();
    },
    reset: function () {
      this.item = null;
      this.warehouse = null;
      this.area = null;
      this.address = null;
      this.lotno = null;
      this.search();
    },
  },
};
</script>
