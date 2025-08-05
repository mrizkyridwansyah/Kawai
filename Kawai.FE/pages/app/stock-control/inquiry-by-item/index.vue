<template>
  <header-menu title="Stock Inquiry By Item" :breadcrumbs="this.breadcrumbs" />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-2">
        <div class="" style="width: 100%">
          <input-item-by-stock
            class="form-control"
            placeholder="Search Item"
            v-model="filter.item"
            warehouse-code="ALL"
            location-code="ALL"
            area-code="ALL"
            item-type-code="ALL"
            brand-code="ALL"
            :show-option-all="false"
          />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-1">
        <div class="mr-1" style="width: 100%">
          <input-warehouse-by-stock
            class="form-control"
            placeholder="Search Warehouse"
            v-model="filter.warehouse"
            :item-code="filter.item"
            :show-option-all="true"
          />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-1">
        <div class="mr-1" style="width: 100%">
          <input-location-by-stock
            class="form-control"
            placeholder="Search Location"
            v-model="filter.location"
            :warehouse-code="filter.warehouse"
            :item-code="filter.item"
            :show-option-all="true"
          />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3">
        <div class="" style="width: 100%">
          <input-lot-by-stock
            class="form-control"
            placeholder="Search Lot No"
            v-model="filter.lotno"
            :warehouse-code="filter.warehouse"
            :location-code="filter.location"
            area-code="ALL"
            item-type-code="ALL"
            brand-code="ALL"
            :item-code="filter.item"
            :show-option-all="true"
          />
        </div>
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
    </div>
  </div>
  <v-tree-group
    :tree-data="treeData"
    :columns="columns"
    child-key="children"
    :group-by-fields="groupByFields"
    :frozen-column-left="3"
    :is-loading="ds.isLoading"
    :is-server-error="ds.isServerError"
    :is-network-error="ds.isNetworkError"
  >
    <template #paging-tree>
      <v-table-pagination
        v-if="
          !ds.isLoading &&
          ds.data.Items.length > 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
        class="mt-3"
        :table="ds.data"
        :page-change="ds.setPage"
        :length-change="ds.setLength"
      />
    </template>
  </v-tree-group>

  <v-modal title="Detail Stock" class="modal-lg" id="modal-detail">
    <shared-stock-detail-list
      :warehouse="this.detail.warehouse"
      :location="this.detail.location"
      :item="this.detail.item"
      :lotno="this.detail.lotno"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Report", active: false, to: "" },
      {
        title: "Stock Inquiry By Item",
        active: true,
        to: "/stock-control/inquiry-by-item",
      },
    ],
    filter: {
      item: null,
      warehouse: null,
      location: null,
      lotno: null,
    },
    detail: {
      warehouse: null,
      item: null,
      location: null,
      lotno: null,
    },
    columns: [],
    rawData: [],
    groupByFields: [
      ["ItemCode", ["ItemCode", "ItemName"]],
      ["WarehouseName", ["WarehouseCode", "WarehouseName"]],
      ["LocationName", ["LocationCode", "LocationName"]],
      ["LotNo", ["LotNo"]],
    ],
    sumFields: ["BeginQty", "ReceiptQty", "SupplyQty", "CurrentQty"],
    treeData: [],
  }),
  computed: {
    ds: function () {
      return useStockByItem();
    },
  },
  mounted: function () {
    this.getColumns();
    this.search();
  },
  watch: {
    "filter.item": function () {
      this.treeData = [];
    },
    "filter.lotno": function () {
      this.treeData = [];
    },
    "ds.data.Items": function () {
      console.log(this.ds.data.Items);
      this.rawData = this.ds.data.Items;
      this.treeData = this.buildTree(this.rawData);
    },
  },
  methods: {
    getColumns: function () {
      this.columns = [
        {
          text: "Item Code",
          dataField: "ItemCode",
          width: "200px",
          align: "left",
        },
        { text: "Item Name", dataField: "ItemName", width: "200px" },
        { text: "Warehouse", dataField: "WarehouseName", width: "200px" },
        { text: "Location", dataField: "LocationName", width: "200px" },
        { text: "Lot No", dataField: "LotNo", width: "200px" },
        {
          text: "Begin",
          dataField: "BeginQty",
          width: "100px",
          align: "right",
        },
        {
          text: "Receipt",
          dataField: "ReceiptQty",
          width: "100px",
          align: "right",
        },
        {
          text: "Supply",
          dataField: "SupplyQty",
          width: "100px",
          align: "right",
        },
        {
          text: "Current",
          dataField: "CurrentQty",
          width: "100px",
          align: "right",
        },
        {
          text: "Detail",
          isRender: true,
          showAtLevel: [this.groupByFields.length - 1],
          action: (row) => this.openModal(row), 
          /* 
          showAtLevel: [
            this.groupByFields.length - 2,
            this.groupByFields.length - 1,
          ],
          action: (row) => {
            if (row.Level == this.groupByFields.length - 1) this.openModal(row);
            else if (row.Level == this.groupByFields.length - 2) alert("ini summary");
          }, 
          */
          width: "100px",
          align: "center",
        },
      ];
    },
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ItemCode: this.filter.item || "",
          WarehouseCode: this.filter.warehouse || "",
          LocationCode: this.filter.location || "",
          LotNo: this.filter.lotno || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.inquiry().then((dt) => {
        this.rawData = dt.Data.Items;
        this.treeData = this.buildTree(this.rawData);
      });
    },
    reset: function () {
      this.filter.warehouse = null;
      this.filter.location = null;
      this.filter.item = null;
      this.filter.lotno = null;
      this.search();
    },
    buildTree: function (items, level = 0, path = []) {
      if (!this.groupByFields[level]) return items;

      const [groupField, displayFields] = this.groupByFields[level];
      const grouped = {};

      for (const item of items) {
        const key = (item[groupField] || "").trim();
        if (!grouped[key]) grouped[key] = [];
        grouped[key].push(item);
      }

      const result = [];

      for (const [key, groupItems] of Object.entries(grouped)) {
        const node = {
          key: [...path, key].join(">"),
          Level: level,
          children: this.buildTree(groupItems, level + 1, [...path, key]),
        };

        // set semua kolom dari columns ke ''
        this.columns.forEach((col) => {
          const field = col.dataField;
          node[field] = "";
        });

        // isi kolom hasil groupBy
        node[groupField] = key;

        // isi hanya displayFields dengan data dari item pertama
        for (const field of displayFields) {
          node[field] = groupItems[0][field];
        }

        // sumFields (BeginQty, ReceiptQty, dll)
        for (const sumField of this.sumFields) {
          node[sumField] = this.$func.formatMoney(
            groupItems.reduce(
              (acc, item) => acc + Number(item[sumField] || 0),
              0
            )
          );
        }

        result.push(node);
      }

      return result;
    },
    openModal(row) {
      this.detail.warehouse = row.children[0].WarehouseCode;
      this.detail.location = row.children[0].LocationCode;
      this.detail.item = row.children[0].ItemCode;
      this.detail.lotno = row.children[0].LotNo;
      this.$bvModal.show("modal-detail");
    },
  },
};
</script>
