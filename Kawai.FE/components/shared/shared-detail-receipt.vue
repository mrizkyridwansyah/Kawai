<template>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-data="ds.dataDetail"
    :ds-page="ds.setPageDetail"
    :ds-length="ds.setLengthDetail"
    :ds-load="ds.loadDetail"
    :default-height="380"
    :max-height="380"
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
            <th class="text-center">Lot No</th>
            <th class="text-center">Qty</th>
            <th class="text-center">Scan Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.dataDetail.Items">
            <td class="text-center">{{ idx + 1 }}.</td>
            <td>{{ item.WarehouseCode }}</td>
            <td>{{ item.AreaCode }}</td>
            <td>{{ item.AddressCode }}</td>
            <td>{{ item.ItemName }}</td>
            <td>{{ item.BarcodeNo }}</td>
            <td>{{ item.LotNo }}</td>
            <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
            <td
              v-if="item.IsVerified"
              class="text-primary"
              style="font-weight: bold"
            >
              Sudah Scan
            </td>
            <td v-else class="text-danger" style="font-weight: bold">
              Belum Scan
            </td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["receiptDetailId", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        BarcodeNo: "asc",
        WarehouseCode: "asc",
        AreaCode: "asc",
        AddressCode: "asc",
      },
      sortItems: [
        {
          label: "Barcode No",
          value: "BarcodeNo",
          selected: true,
          direction: "asc",
        },
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
      ],
    },
  }),
  computed: {
    ds: function () {
      return useReceiptInquiry();
    },
  },
  watch: {
    receiptDetailId: function () {
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
      this.ds.setSortDetail(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ReceiptDetailId: this.receiptDetailId.toString() || null,
        },
      ];

      this.ds.setFilterDetail(filters);
      this.ds.loadDetail();
    },
    reset: function () {
      this.receiptDetailId = null;
      this.search();
    },
  },
};
</script>
