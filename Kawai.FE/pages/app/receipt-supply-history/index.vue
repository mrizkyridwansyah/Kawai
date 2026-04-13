<template>
  <v-frame title="Receipt / Supply Inquiry" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Warehouse</label></td>
          <td style="padding-left: 15px">
            <filter-warehouse-privileges
              class="form-control"
              v-model="filter.warehouse"
              factory-code="ALL"
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
            <filter-area-privileges
              class="form-control"
              
              v-model="filter.area"
              :warehouse="filter.warehouse"
              :include-temp="true"
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
            <filter-item
              class="form-control"
              
              v-model="filter.item"
              style-code="width: 160px"
              style-desc="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Lot No</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-lot-no
              class="form-control"
              v-model="filter.lotno"
              :warehouse="filter.warehouse"
              area="ALL"
              address="ALL"
              :show-option-all="true"
              :item="filter.item"
              style="width: 200px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Period</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-month
              v-model="filter.period"
              class="mr-1"
              style="width: 150px"
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
      <div ref="historyContent" class="mt-4">
        <div class="elevated-tree-container shadow-sm bg-white rounded">
          <v-tree
            :tree-data="treeData"
            :columns="columns"
            :frozen-column-left="2"
            :is-loading="ds.isLoading"
            :is-server-error="ds.isServerError"
            :is-network-error="ds.isNetworkError"
            child-key="children"
          />
        </div>
      </div>
    </template>
  </v-frame>
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
      area: null,
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
    columns: [],
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
      this.rawData = [];
      this.treeData = [];
    },
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
  },
  mounted: function () {
    this.getColumns();
  },
  methods: {
    removeTableStripedClass() {
      const container = this.$refs.historyContent;
      if (!container) return;
      const stripedTables = container.querySelectorAll(".table-striped");
      stripedTables.forEach((el) => {
        el.classList.remove("table-striped");
      });
    },
    getColumns: function () {
      this.columns = [
        {
          text: "Lot No",
          dataField: "LotNo",
          width: "200px",
          align: "left",
        },
        { text: "Date", dataField: "TransactionDate", width: "150px" },
        {
          text: "Pre Month",
          dataField: "PreMonth",
          width: "100px",
          align: "right",
        },
        {
          text: "Receipt",
          dataField: "Receipt",
          width: "100px",
          align: "right",
        },
        { text: "Supply", dataField: "Supply", width: "100px", align: "right" },
        { text: "Reject", dataField: "Reject", width: "100px", align: "right" },
        {
          text: "Current",
          dataField: "Current",
          width: "100px",
          align: "right",
        },
        {
          text: "From Area",
          dataField: "FromAreaName",
          width: "max-content",
        },
        {
          text: "To Area",
          dataField: "ToAreaName",
          width: "max-content",
        },
        {
          text: "Doc. Reference",
          dataField: "DocReference",
          width: "max-content",
          align: "left",
        },
        {
          text: "Remarks",
          dataField: "Remarks",
          width: "200px",
          align: "left",
        },
        { text: "User", dataField: "LastUser", width: "200px", align: "left" },
      ];
    },
    search: function () {
      this.$nextTick(() => {
        this.removeTableStripedClass();
      });
      this.ds.setSort(this.filter.sorts);

      let filters = [
        {
          WarehouseCode: this.filter.warehouse || "",
          AreaCode: this.filter.area || "",
          ItemCode: this.filter.item || "",
          LotNo: this.filter.lotno || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => {
        this.rawData = dt.Data;
        this.treeData =
          this.rawData.length > 0 ? this.buildTree(this.rawData) : [];
      });
    },

    reset: function () {
      this.filter.warehouse = null;
      this.filter.area = null;
      this.filter.item = null;
      this.filter.lotno = null;
      this.filter.period = null;
      this.search();
    },
    buildTree: function (data, level = 0) {
      const grouped = {};
      data.forEach((item) => {
        const lotNo = item.LotNo;

        if (!grouped[lotNo]) {
          grouped[lotNo] = {
            LotNo: lotNo,
            PreMonth: this.$func.formatNumber(
              data.filter((x) => x.LotNo == lotNo)[0].PreMonth
            ),
            Receipt: this.$func.formatNumber(
              data
                .filter((x) => x.LotNo == lotNo)
                .reduce((a, b) => a + parseFloat(b.Receipt), 0)
            ),
            Supply: this.$func.formatNumber(
              data
                .filter((x) => x.LotNo == lotNo)
                .reduce((a, b) => a + parseFloat(b.Supply), 0)
            ),
            Reject: this.$func.formatNumber(
              data
                .filter((x) => x.LotNo == lotNo)
                .reduce((a, b) => a + parseFloat(b.Reject), 0)
            ),
            Current: this.$func.formatNumber(
              data.filter((x) => x.LotNo == lotNo)[0].Current
            ),
            children: [],
          };
        }

        const childItem = { ...item };
        delete childItem.LotNo;
        delete childItem.PreMonth;
        // delete childItem.Receipt;
        // delete childItem.Supply;
        // delete childItem.Reject;
        delete childItem.Current;

        childItem.TransactionDate = this.$func.formatDateTime(
          childItem.TransactionDate
        );
        childItem.Receipt = this.$func.formatNumber(childItem.Receipt);
        childItem.Supply = this.$func.formatNumber(childItem.Supply);
        childItem.Reject = this.$func.formatNumber(childItem.Reject);

        grouped[lotNo].children.push(childItem);
      });

      const treeChildren = Object.values(grouped);
      console.log(treeChildren);
      let totalPreMonth = treeChildren.reduce(
        (a, b) => a + parseFloat(String(b.PreMonth).replace(/,/g, "")),
        0
      );
      let totalReceipt = treeChildren.reduce(
        (a, b) => a + parseFloat(String(b.Receipt).replace(/,/g, "")),
        0
      );
      let totalSupply = treeChildren.reduce(
        (a, b) => a + parseFloat(String(b.Supply).replace(/,/g, "")),
        0
      );
      let totalReject = treeChildren.reduce(
        (a, b) => a + parseFloat(String(b.Reject).replace(/,/g, "")),
        0
      );
      let totalCurrent = treeChildren.reduce(
        (a, b) => a + parseFloat(String(b.Current).replace(/,/g, "")),
        0
      );

      // Tambahkan 1 parent global summary di atas semua LotNo
      const parentSummary = {
        LotNo: "SUMMARY",
        PreMonth: this.$func.formatNumber(totalPreMonth),
        Receipt: this.$func.formatNumber(totalReceipt),
        Supply: this.$func.formatNumber(totalSupply),
        Reject: this.$func.formatNumber(totalReject),
        Current: this.$func.formatNumber(totalCurrent),
        children: treeChildren,
      };

      return [parentSummary];
    },
  },
};
</script>
