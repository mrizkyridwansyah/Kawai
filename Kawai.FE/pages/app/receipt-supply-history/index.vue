<template>
  <header-menu
    title="Receipt / Supply History"
    :breadcrumbs="this.breadcrumbs"
  />
  <div class="row">
    <div class="col-lg-4 col-md-4 col-sm-12 mt-1">
      <div class="mr-1">
        <input-warehouse class="form-control" v-model="filter.warehouse" />
      </div>
    </div>
    <div class="col-lg-4 col-md-4 col-sm-12 mt-1">
      <div class="mr-1">
        <input-item class="form-control" v-model="filter.item" />
      </div>
    </div>
    <div class="col-lg-4 col-md-4 col-sm-12 mt-1">
      <div class="mr-1">
        <input-lot-no
          class="form-control"
          v-model="filter.lotno"
          :warehouse="filter.warehouse"
          area="ALL"
          address="ALL"
          :item="filter.item"
        />
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <input-month v-model="filter.period" class="mr-1" placeholder="Period" />
      <v-button-search-reset :search="search" :reset="reset" />
    </div>
  </div>
  <v-tree
    :tree-data="treeData"
    :columns="columns"
    :is-loading="ds.isLoading"
    :is-server-error="ds.isServerError"
    :is-network-error="ds.isNetworkError"
    child-key="children"
  />
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Stock Control", active: false, to: "" },
      {
        title: "Receip Supply History",
        active: true,
        to: "/receipt-supply-history",
      },
    ],
    filter: {
      keyword: null,
      period: new Date(
        new Date().getFullYear(),
        new Date().getMonth(),
        1
      ).toISOString(),
      warehouse: null,
      item: null,
      lotno: null,
      sorts: {
        LotNo: "asc",
      },
      sortItems: [
        {
          label: "Lot No",
          value: "LotNo",
          selected: true,
          direction: "asc",
        },
      ],
    },
    columns: [
      {
        text: "Lot No",
        dataField: "LotNo",
        width: "200px",
        align: "left",
      },
      { text: "Date", dataField: "TransactionDate", width: "150px" },
      {
        text: "Transaction Type",
        dataField: "TransactionType",
        width: "150px",
      },
      { text: "Qty", dataField: "QtyTrans", width: "150px", align: "right" },
      {
        text: "Doc. Reference",
        dataField: "DocReference",
        width: "150px",
        align: "left",
      },
      { text: "Remarks", dataField: "Remarks", width: "300px", align: "left" },
      { text: "User", dataField: "LastUser", width: "150px", align: "left" },
    ],
    rawData: [],
    treeData: [],
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useReceiptSupplyHistory();
    },
  },
  watch: {
    "filter.period": function () {
      this.treeData = [];
    },
    "filter.warehouse": function () {
      this.treeData = [];
    },
    "filter.item": function () {
      this.treeData = [];
    },
    "filter.lotno": function () {
      this.treeData = [];
    },
  },
  mounted: function () {
    // this.search();
  },
  methods: {
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          WarehouseCode: this.filter.warehouse || "",
          ItemCode: this.filter.item || "",
          LotNo: this.filter.lotno || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => {
        this.rawData = dt.Data;
        this.treeData = this.buildTree(this.rawData);
        console.log(this.treeData);
      });
    },
    reset: function () {
      this.filter.warehouse = null;
      this.filter.item = null;
      this.filter.lotno = null;
      this.filter.period = null;
      this.search();
    },
    buildTree: function (data) {
      const grouped = {};

      data.forEach((item) => {
        const lotNo = item.LotNo;

        if (!grouped[lotNo]) {
          grouped[lotNo] = {
            LotNo: lotNo,
            children: [],
          };
        }

        const childItem = { ...item };
        delete childItem.LotNo;
        childItem.TransactionDate = this.$func.formatDateTime(
          childItem.TransactionDate
        );
        childItem.QtyTrans = this.$func.formatNumber(childItem.QtyTrans);
        grouped[lotNo].children.push(childItem);
      });

      // Ubah object ke array
      return Object.values(grouped);
    },
  },
};
</script>
