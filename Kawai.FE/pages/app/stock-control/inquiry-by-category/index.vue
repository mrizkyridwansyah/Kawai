<template>
  <v-frame title="Stock Inquiry By Category" icon="boxes-stacked">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Category</label></td>
          <td style="padding-left: 15px">
            <filter-cls-2
              class="form-control"
              type-data="Grouping_Class_Part"
              v-model="filter.category"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px;">
            <label class="form-label">Warehouse</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-warehouse-by-stock
              class="form-control"
              v-model="filter.warehouse"
              :item-code="filter.item"
              :show-option-all="true"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px;">
            <label class="form-label">Item</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-item-by-stock
              class="form-control"
              v-model="filter.item"
              warehouse="ALL"
              area="ALL"
              address="ALL"
              :category="filter.category"
              :show-option-all="true"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px;">
            <label class="form-label">Lot No</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-lot-by-stock
              class="form-control"
              v-model="filter.lotno"
              :warehouse="filter.warehouse"
              area="ALL"
              address="ALL"
              :item="filter.item"
              :category="filter.category"
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
                :search="onSearch"
                :reset="reset"
              />
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <v-tree-group
        :tree-data="treeData"
        :columns="columns"
        child-key="children"
        :group-by-fields="groupByFields"
        :frozen-column-left="3"
        :start-collapse-level="0"
        :is-loading="ds.isLoading"
        :is-server-error="ds.isServerError"
        :is-network-error="ds.isNetworkError"
        :refresh="onSearch"
        :top-content-height="290"
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
      :counter="this.counter"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Report", active: false, to: "" },
      {
        title: "Stock Inquiry By Category",
        active: true,
        to: "/stock-control/inquiry-by-category",
      },
    ],
    filter: {
      category: null,
      item: null,
      lotno: null,
      warehouse: null,
    },
    detail: {
      warehouse: null,
      item: null,
      area: null,
      address: null,
      lotno: null,
    },
    counter:0,
    columns: [],
    rawData: [],
    groupByFields: [
      ["ItemCode", ["ItemCode", "ItemName"]],
      ["WarehouseCode", ["WarehouseCode", "WarehouseName"]],
      ["AreaCode", ["AreaCode", "AreaName"]],
      ["LotNo", ["LotNo"]],
    ],
    sumFields: ["BeginQty", "ReceiptQty", "SupplyQty", "CurrentQty"],
    treeData: [],
  }),
  computed: {
    ds: function () {
      return useStockByCategory();
    },
  },
  mounted: function () {
    this.getColumns();
    this.search(false);
  },
  watch: {
    "filter.item": function () {
      this.treeData = [];
    },
    "filter.lotno": function () {
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
        {
          text: "Item Code",
          dataField: "ItemCode",
          width: "max-content",
          align: "left",
        },
        { text: "Item Name", dataField: "ItemName", width: "max-content" },
        { text: "Warehouse", dataField: "WarehouseCode", width: "max-content" },
        { text: "Area", dataField: "AreaCode", width: "max-content" },
        { text: "Lot No", dataField: "LotNo", width: "max-content" },
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
    validSearch: function () {
      if ((this.filter.category || "") == "") {
        toastDanger("Silahkan pilih kategori");
        return false;
      }

      if ((this.filter.item || "") == "") {
        toastDanger("Silahkan pilih item");
        return false;
      }

      if ((this.filter.warehouse || "") == "") {
        toastDanger("Silahkan pilih warehouse");
        return false;
      }

      if ((this.filter.lotno || "") == "") {
        toastDanger("Silahkan pilih lot no");
        return false;
      }

      return true;
    },
    onSearch: function() {
      this.search(true);
    },
    search: function (cek) {
      if (cek && !this.validSearch()) return;

      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ItemCode: this.filter.item || "",
          WarehouseCode: this.filter.warehouse || "",
          Category: this.filter.category || "",
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
      this.filter.category = null;
      this.filter.item = null;
      this.filter.lotno = null;
      this.search(false);
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
              0,
            ),
          );
        }

        result.push(node);
      }

      return result;
    },
    openModal(row) {
      this.detail.warehouse = row.children[0].WarehouseCode;
      this.detail.area = row.children[0].AreaCode;
      this.detail.address = row.children[0].AddressCode;
      this.detail.item = row.children[0].ItemCode;
      this.detail.lotno = row.children[0].LotNo;
      this.counter++;
      this.$bvModal.show("modal-detail");
    },
  },
};
</script>