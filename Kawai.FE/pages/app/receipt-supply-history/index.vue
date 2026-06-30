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
              style-code="width: 160px"
              style-desc="width: 300px"
            />
          </td>
          <td style="padding-left: 15px">
            <label class="form-label">Item</label>
          </td>
          <td style="padding-left: 15px" colspan="3">
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
            <label class="form-label">Area</label>
          </td>
          <td style="padding-left: 15px">
            <filter-area-privileges
              class="form-control"
              v-model="filter.area"
              :warehouse="filter.warehouse"
              :include-temp="true"
              :show-option-all="true"
              style-code="width: 160px"
              style-desc="width: 300px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
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
              style="width: 265px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
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
        <tr></tr>
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
      <hr />
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
            :top-content-height="290"
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
        1,
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
    buildTree: function (data) {
      // 1. Group by AreaCode
      const areaGroups = {};
      data.forEach((item) => {
        const areaName = item.AreaCode || "N/A";
        if (!areaGroups[areaName]) {
          areaGroups[areaName] = [];
        }
        areaGroups[areaName].push(item);
      });

      const level2Nodes = [];
      let totalPreMonth = 0;
      let totalReceipt = 0;
      let totalSupply = 0;
      let totalReject = 0;
      let totalCurrent = 0;

      for (const [areaName, areaItems] of Object.entries(areaGroups)) {
        // 2. Group areaItems by LotNo
        const lotGroups = {};
        areaItems.forEach((item) => {
          const lotNo = item.LotNo || "N/A";
          if (!lotGroups[lotNo]) {
            lotGroups[lotNo] = [];
          }
          lotGroups[lotNo].push(item);
        });

        const level3Nodes = [];
        let areaPreMonth = 0;
        let areaReceipt = 0;
        let areaSupply = 0;
        let areaReject = 0;
        let areaCurrent = 0;

        for (const [lotNo, lotItems] of Object.entries(lotGroups)) {
          const lotReceipt = lotItems.reduce(
            (sum, x) => sum + parseFloat(x.Receipt || 0),
            0,
          );
          const lotSupply = lotItems.reduce(
            (sum, x) => sum + parseFloat(x.Supply || 0),
            0,
          );
          const lotReject = lotItems.reduce(
            (sum, x) => sum + parseFloat(x.Reject || 0),
            0,
          );

          const lotPreMonth = parseFloat(lotItems[0].PreMonth || 0);
          const lotCurrent = parseFloat(lotItems[0].Current || 0);

          areaPreMonth += lotPreMonth;
          areaReceipt += lotReceipt;
          areaSupply += lotSupply;
          areaReject += lotReject;
          areaCurrent += lotCurrent;

          const childItems = lotItems.map((item) => {
            const child = { ...item };
            delete child.LotNo;
            delete child.PreMonth;
            delete child.Current;

            child.TransactionDate = this.$func.formatDateTime(
              child.TransactionDate,
            );
            child.Receipt = this.$func.formatNumber(child.Receipt);
            child.Supply = this.$func.formatNumber(child.Supply);
            child.Reject = this.$func.formatNumber(child.Reject);
            return child;
          });

          level3Nodes.push({
            LotNo: lotNo,
            PreMonth: this.$func.formatNumber(lotPreMonth),
            Receipt: this.$func.formatNumber(lotReceipt),
            Supply: this.$func.formatNumber(lotSupply),
            Reject: this.$func.formatNumber(lotReject),
            Current: this.$func.formatNumber(lotCurrent),
            children: childItems,
          });
        }

        totalPreMonth += areaPreMonth;
        totalReceipt += areaReceipt;
        totalSupply += areaSupply;
        totalReject += areaReject;
        totalCurrent += areaCurrent;

        level2Nodes.push({
          LotNo: areaName,
          PreMonth: this.$func.formatNumber(areaPreMonth),
          Receipt: this.$func.formatNumber(areaReceipt),
          Supply: this.$func.formatNumber(areaSupply),
          Reject: this.$func.formatNumber(areaReject),
          Current: this.$func.formatNumber(areaCurrent),
          children: level3Nodes,
        });
      }

      const parentSummary = {
        LotNo: "SUMMARY",
        PreMonth: this.$func.formatNumber(totalPreMonth),
        Receipt: this.$func.formatNumber(totalReceipt),
        Supply: this.$func.formatNumber(totalSupply),
        Reject: this.$func.formatNumber(totalReject),
        Current: this.$func.formatNumber(totalCurrent),
        children: level2Nodes,
      };

      return [parentSummary];
    },
  },
};
</script>
