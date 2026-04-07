<template>
  <v-frame title="Stock Inquiry By Category" icon="boxes-stacked">
    <template #frame-content>

       <table width="100%">
        <tr style="height: 38px">
          <td style="width: 10%"><label class="form-label">Category</label></td>
          <td style="width: 34%">
            <filter-cls-2
              class="form-control"
              type-data="ClasificationPart_Cls"
              v-model="filter.category"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
          <td style="width: 1%"></td>
          <td style="width: 10%">
            <label class="form-label">Lot No</label>
          </td>
          <td style="width: 34%">
             <filter-lot-by-stock
              class="form-control"
              v-model="filter.lotno"
              warehouse="ALL"
              area="ALL"
              address="ALL"
              :item="filter.item"
              :category="filter.category"
              :show-option-all="true"
              style="width: 200px"
            />
          </td>

          <td style="width: 10%"></td>
        </tr>
        <tr style="height: 38px">
          <td style="width: 10%"><label class="form-label">Item</label></td>
          <td style="width: 34%">
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
          <td style="width: 1%"></td>
          <td style="width: 10%"><label class="form-label">Stock Status</label></td>
          <td style="width: 34%">
             <filter-status-receipt
              class="form-control"
              v-model="filter.status"
              style="width: 110px"
            />
          </td>

          <td style="width: 10%"></td>
        </tr>
        <tr style="height: 38px">
          <td style="width: 10%"><label class="form-label">Warehouse</label></td>
          <td style="width: 34%">
            <filter-warehouse-by-stock
              class="form-control"
              v-model="filter.warehouse"
              :item-code="filter.item"
              :show-option-all="true"
              style-code="width: 150px"
              style-desc="width: 300px"
            />
          </td>
          <td style="width: 1%"></td>
          <td style="width: 10%"></td>
          <td style="width: 34%"></td>

          <td style="width: 10%"></td>
        </tr>
      </table>


      <table>
        
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
      <v-tree-group
        :tree-data="treeData"
        :columns="columns"
        child-key="children"
        :group-by-fields="groupByFields"
        :frozen-column-left="3"
        :start-collapse-level="1"
        :is-loading="ds.isLoading"
        :is-server-error="ds.isServerError"
        :is-network-error="ds.isNetworkError"
        :refresh="onSearch"
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
      :status="this.detail.status"
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
       status: null,
    },
    detail: {
      warehouse: null,
      item: null,
      area: null,
      address: null,
      lotno: null,
       status: null,
    },
    columns: [],
    rawData: [],
    groupByFields: [
      ["ItemCode", ["ItemCode", "ItemName"]],
      ["WarehouseCode", ["WarehouseCode", "WarehouseName"]],
      ["AreaCode", ["AreaCode", "AreaName"]],
      ["LotNo", ["LotNo","Status"]],
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
     "filter.status": function () {
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
        { text: "Stock Status", dataField: "Status", width: "max-content" ,
          align: "center" },
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

       if ((this.filter.status || "") == "") {
        toastDanger("Silahkan pilih status");
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
          Status: this.filter.status || "",
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
      this.filter.status = null;
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
      this.detail.status = row.children[0].Status;
      this.$bvModal.show("modal-detail");
    },
  },
};
</script>
