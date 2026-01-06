<template>
  <v-frame title="Stock Inquiry By Location" icon="boxes-stacked">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Warehouse</label></td>
          <td style="padding-left: 15px">
            <filter-warehouse-by-stock
              class="form-control"
              placeholder="Search Warehouse"
              v-model="filter.warehouse"
              item-code="ALL"
              :show-option-all="false"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Area</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-area-by-stock
              class="form-control"
              placeholder="Search Area"
              v-model="filter.area"
              :warehouse="filter.warehouse"
              item="ALL"
              :show-option-all="true"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Item</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-item-by-stock
              class="form-control"
              placeholder="Search Item"
              v-model="filter.item"
              :warehouse="filter.warehouse"
              :area="filter.area"
              address="ALL"
              category="ALL"
              :show-option-all="true"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Lot No</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-lot-by-stock
              class="form-control"
              placeholder="Search Lot No"
              v-model="filter.lotno"
              :warehouse="filter.warehouse"
              :area="filter.area"
              address="ALL"
              category="ALL"
              :item="filter.item"
              :show-option-all="true"
              style="width: 200px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px" colspan="2">
            <div class="d-flex flex-fill">
              <v-button-search-reset
                class="mr-1"
                :search="search"
                :reset="reset"
              />
            </div>
          </td>
        </tr>
      </table>
      <v-tree-group
        :tree-data="treeData"
        :columns="columns"
        child-key="children"
        :group-by-fields="groupByFields"
        :frozen-column-left="2"
        :start-collapse-level="0"
        :is-loading="ds.isLoading"
        :is-server-error="ds.isServerError"
        :is-network-error="ds.isNetworkError"
        :refresh="search"
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
    </template>
  </v-frame>

  <v-modal title="Detail Stock" class="modal-lg" id="modal-detail">
    <shared-stock-detail-list
      :warehouse="this.detail.warehouse"
      :area="this.detail.area"
      :address="this.detail.address"
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
        title: "Stock Inquiry By Area",
        active: true,
        to: "/stock-control/inquiry-by-area",
      },
    ],
    filter: {
      warehouse: null,
      area: null,
      item: null,
      lotno: null,
    },
    detail: {
      warehouse: null,
      item: null,
      area: null,
      lotno: null,
    },
    columns: [],
    rawData: [],
    groupByFields: [
      ["ItemName", ["ItemCode", "ItemName"]],
      ["AreaName", ["AreaCode", "AreaName"]],
      ["AddressName", ["AddressCode", "AddressName"]],
      ["LotNo", ["LotNo"]],
    ],
    sumFields: ["BeginQty", "ReceiptQty", "SupplyQty", "CurrentQty"],
    treeData: [],
  }),
  computed: {
    ds: function () {
      return useStockByArea();
    },
  },
  mounted: function () {
    this.getColumns();
    this.search();
  },
  watch: {
    "filter.warehouse": function () {
      this.rawData = [];
      this.treeData = [];
    },
    "filter.area": function () {
      this.rawData = [];
      this.treeData = [];
    },
    "filter.item": function () {
      this.rawData = [];
      this.treeData = [];
    },
    "filter.lotno": function () {
      this.rawData = [];
      this.treeData = [];
    },
    "ds.data.Items": function () {
      this.rawData = this.ds.data.Items;
      this.treeData = this.buildTree(this.rawData);
    },
  },
  methods: {
    getColumns: function () {
      this.columns = [
        // { text: "Warehouse Code", dataField: "WarehouseCode", width: "250px" },
        // { text: "Warehouse Name", dataField: "WarehouseName", width: "250px" },
        { text: "Item Name", dataField: "ItemName", width: "250px" },
        { text: "Area", dataField: "AreaName", width: "250px" },
        { text: "Address", dataField: "AddressName", width: "250px" },
        { text: "Lot No", dataField: "LotNo", width: "max-content" },
        {
          text: "Begin",
          dataField: "BeginQty",
          width: "max-content",
          align: "right",
        },
        {
          text: "Receipt",
          dataField: "ReceiptQty",
          width: "max-content",
          align: "right",
        },
        {
          text: "Supply",
          dataField: "SupplyQty",
          width: "max-content",
          align: "right",
        },
        {
          text: "Current",
          dataField: "CurrentQty",
          width: "max-content",
          align: "right",
        },
        {
          text: "Detail",
          isRender: true,
          showAtLevel: [this.groupByFields.length - 1],
          action: (row) => this.openModal(row),
          width: "max-content",
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
          AreaCode: this.filter.area || "",
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
      this.filter.area = null;
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
    openModal: function (row) {
      this.detail.warehouse = row.children[0].WarehouseCode;
      this.detail.area = row.children[0].AreaCode;
      this.detail.address = row.children[0].AddressCode;
      this.detail.item = row.children[0].ItemCode;
      this.detail.lotno = row.children[0].LotNo;
      this.$bvModal.show("modal-detail");
    },
  },
};
</script>
