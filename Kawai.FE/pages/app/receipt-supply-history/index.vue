<template>
  <header-menu
    title="Receipt / Supply History"
    :breadcrumbs="this.breadcrumbs"
  />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-4 col-md-4 col-sm-12 col-12">
        <div class="mr-1">
          <input-warehouse class="form-control" v-model="filter.warehouse" />
        </div>
      </div>
      <div class="col-lg-4 col-md-4 col-sm-12 col-12">
        <div class="mr-1">
          <input-item class="form-control" v-model="filter.item" />
        </div>
      </div>
      <div class="col-lg-4 col-md-4 col-sm-12 col-12">
        <div class="mr-1">
          <input-lot-no class="form-control" v-model="filter.lotno" />
        </div>
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <input-month v-model="filter.period" class="mr-1" placeholder="Period" />
      <v-button-search-reset :search="search" :reset="reset" />
    </div>
  </div>
  <!-- <v-tree
    :tree-data="treeData"
    :columns="columns"
    :is-loading="ds.isLoading"
    :is-server-error="ds.isServerError"
    :is-network-error="ds.isNetworkError"
  /> -->
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
      { text: "Date", dataField: "Date", width: "100px" },
      {
        text: "Transaction Type",
        dataField: "TransactionType",
        width: "150px",
      },
      { text: "Qty", dataField: "Qty", width: "150px", align: "right" },
      { text: "From Area", dataField: "FromArea", width: "150px" },
      { text: "To Area", dataField: "ToArea", width: "150px" },
      { text: "From Address", dataField: "FromAddress", width: "150px" },
      { text: "To Address", dataField: "ToAddress", width: "150px" },
      {
        text: "Doc. Reference",
        dataField: "DocReference",
        width: "150px",
        align: "left",
      },
      { text: "Remarks", dataField: "Remarks", width: "150px", align: "left" },
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
          Keyword: this.filter.keyword || "",
          ItemCode: this.filter.item || "",
          WarehouseCode: this.filter.warehouse || "",
          AreaCode: this.filter.area || "",
          LotNo: this.filter.lotno || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => {
        this.rawData = dt.Data.Items;
        this.treeData = this.buildTree(this.rawData);
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
      const map = {};
      const roots = [];

      data.forEach((item) => {
        map[item.ChildLvl] = { ...item, children: [] };
      });

      data.forEach((item) => {
        if (item.ParentLvl && map[item.ParentLvl]) {
          map[item.ParentLvl].children.push(map[item.ChildLvl]);
        } else {
          roots.push(map[item.ChildLvl]);
        }
      });

      return roots;
    },
  },
};
</script>
