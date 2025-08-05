<template>
  <header-menu title="Traceability" :breadcrumbs="this.breadcrumbs" />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-1">
        <div class="mr-1" style="width: 100%">
          <input-item-cls class="form-control" v-model="filter.itemCls" />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3">
        <div class="" style="width: 100%">
          <input-item
            class="form-control"
            placeholder="Search Parent Item"
            v-model="filter.parentItem"
            brand="ALL"
            item-type="ALL"
            :item-cls="filter.itemCls"
          />
        </div>
      </div>
      <!-- <div class="col-lg-3 col-md-3 col-sm-3 col-3">
        <div class="" style="width: 100%">
          <input-item
            class="form-control"
            placeholder="Search Lot No"
            v-model="filter.parentItem"
            brand="ALL"
            item-type="ALL"
            :item-cls="filter.itemCls"
          />
        </div>
      </div> -->
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
    </div>
  </div>
  <v-tree
    :tree-data="treeData"
    :columns="columns"
    :is-loading="ds.isLoading"
    :is-server-error="ds.isServerError"
    :is-network-error="ds.isNetworkError"
  />
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Report", active: false, to: "" },
      { title: "Traceability", active: true, to: "/report/traceability" },
    ],
    filter: {
      itemCls: null,
      item: null,
    },
    columns: [
      {
        text: "Item Code",
        dataField: "material_code",
        width: "150px",
        align: "left",
      },
      { text: "Description", dataField: "Material_Name", width: "250px" },
      { text: "Process", dataField: "Child_ProcessName", width: "250px" },
      { text: "Machine", dataField: "Child_LineName", width: "250px" },
      { text: "Date", dataField: "Date", width: "250px" },
      { text: "Qty", dataField: "Qty", width: "80px", align: "right" },
      { text: "Unit", dataField: "UnitDescs", width: "80px", align: "left" },
      {
        text: "Start Date",
        dataField: "Start_Date",
        width: "120px",
        align: "center",
      },
      {
        text: "End Date",
        dataField: "End_Date",
        width: "120px",
        align: "center",
      },
    ],
    rawData: [],
    treeData: [],
  }),
  computed: {
    ds: function () {
      return useTraceability();
    },
  },
  mounted: function () {
    this.search();
  },
  watch: {
    "filter.itemCls": function () {
      this.treeData = [];
    },
    "filter.itemType": function () {
      this.treeData = [];
    },
    "filter.brand": function () {
      this.treeData = [];
    },
    "filter.parentItem": function () {
      this.treeData = [];
    },
  },
  methods: {
    search: function () {
      this.ds.inquiry(true, 'BCRS00E0200E', 'EL.250224.1.0002').then((dt) => {
        this.rawData = dt.Data.Items;
        this.treeData = this.buildTree(this.rawData);
        console.log(this.treeData);
      });
    },
    reset: function () {
      this.filter.itemCls = null;
      this.filter.itemType = null;
      this.filter.brand = null;
      this.filter.parentItem = null;
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
