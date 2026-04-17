<template>
  <v-frame title="Part Material Receipt List" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Factory</label></td>
          <td style="padding-left: 15px" colspan="3">
            <filter-factory-privileges
              class="form-control"
              v-model="filter.FactoryCode"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Supplier</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-trade-2
              class="form-control"
              :trade-cls="['2', '3']"
              v-model="filter.SupplierCode"
              :show-option-all="true"
              style-code="width: 140px"
              style-desc="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Receipt Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
            <input-date
              v-model="filter.PeriodFrom"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px">
            <label class="form-label">To</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-date
              v-model="filter.PeriodUntil"
              style-date="width: 100px !important"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Receipt No.</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-receipt
              class="form-control"
              :factory-code="filter.FactoryCode"
              :supplier-code="filter.SupplierCode || '0'"
              :period-from="filter.PeriodFrom"
              :period-until="filter.PeriodUntil"
              :show-option-all="true"
              v-model="filter.ReceiptId"
              style="width: 300px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Complete Status</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-yes-no-all
              class="form-control"
              v-model="filter.CompleteStatus"
              style="width: 110px"
            />
          </td>
        </tr>
        <tr>
          <td colspan="4" style="padding-top: 5px">
            <div class="d-flex flex-fill">
              <v-button-search-reset :search="onSearch" :reset="reset" />
              <v-button
                :disabled="filter.ReceiptId == 'ALL'"
                :action="print"
                label="Print Label PDF"
                icon="file-pdf"
                cClass="ml-1 btn-green"
                :is-loading="isLoading"
              />
            </div>
          </td>
        </tr>
      </table>

      <v-table
        :filter="filter"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :frozen-column-left="3"
        :data-items="ds.data.Items"
        :ds="ds"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Receipt No</th>
                <th class="text-center">Supplier</th>
                <th class="text-center">Delivery Date</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">DN Number</th>
                <th class="text-center">PO Number</th>
                <th class="text-center">BC Type</th>
                <th class="text-center">BC No</th>
                <th class="text-center">BC Date</th>
                <th class="text-center">Qty DN</th>
                <th class="text-center">Qty Scan</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Currency</th>
                <th class="text-center">Price</th>
                <th class="text-center">Amount</th>
                <th class="text-center">Status IQC</th>
                <th class="text-center">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(item, idx) in ds.data.Items || []"
                :key="idx"
                :class="{ 'bg-danger': item.Qty > item.QtyScan }"
              >
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.ReceiptNo }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.SupplierName }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ $func.formatDate(item.DNDate) }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.ItemCode }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.ItemName }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.DNNumber }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.PONumber }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.BCType }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.BCNumber }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ $func.formatDate(item.BCDate) }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                  class="text-right"
                >
                  {{ $func.formatMoney(item.Qty) }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                  class="text-right"
                >
                  {{ $func.formatMoney(item.QtyScan) }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.UnitClsDescription }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.Currency }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                  class="text-right"
                >
                  {{ $func.formatMoney(item.Price) }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                  class="text-right"
                >
                  {{ $func.formatMoney(item.Amount) }}
                </td>
                <td
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                >
                  {{ item.StatusIQC }}
                </td>
                <td
                  class="text-center"
                  :class="{
                    'bg-danger': item.Qty > item.QtyScan,
                    'table-striped-row':
                      !(item.Qty > item.QtyScan) && idx % 2 === 0,
                  }"
                  :style="
                    !(item.Qty > item.QtyScan) && idx % 2 === 0
                      ? 'color: white !important'
                      : ''
                  "
                >
                  <a href="javascript:void(0)" @click="() => viewDetail(item)"
                    >View Detail</a
                  >
                </td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
  <v-modal title="Detail Receipt" class="modal-lg" id="modal-list-receipt">
    <shared-detail-receipt
      :receiptDetailId="this.selectedReceiptDetailId"
      :counter="this.counter"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    filter: {
      keyword: null,
      keywordKey: "ReceiptNo",
      FactoryCode: null,
      SupplierCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      ReceiptId: null,
      CompleteStatus: null,
      sorts: {
        ReceiptNo: "asc",
      },
      sortItems: [
        {
          label: "Receipt No.",
          value: "ReceiptNo",
          selected: true,
          direction: "asc",
        },
        {
          label: "Supplier",
          value: "SupplierName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item",
          value: "ItemName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Delivery Date",
          value: "DNDate",
          selected: false,
          direction: "asc",
        },
      ],
    },
    debounce: null,
    selectedReceiptDetailId: null,
    counter: 0,
    isLoading: false,
    lists: [],
  }),
  computed: {
    ds: function () {
      return useReceiptInquiry();
    },
    dsReceipt: function () {
      return useReceipt();
    },
  },
  watch: {
    "filter.FactoryCode": function () {
      this.resetGrid();
    },
    "filter.SupplierCode": function () {
      this.resetGrid();
    },
    "filter.PeriodFrom": function () {
      this.resetGrid();
    },
    "filter.PeriodUntil": function () {
      this.resetGrid();
    },
    "filter.ReceiptId": function () {
      this.resetGrid();
    },
    "filter.CompleteStatus": function () {
      this.resetGrid();
    },
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
    this.filter.CompleteStatus = "ALL";
  },
  methods: {
    resetGrid: function () {
      this.ds.setFilter([]);
      this.ds.setPage(1);
      this.ds.setLength(10);
      this.ds.data.Items = [];
    },
    validSearch: function () {
      if ((this.filter.SupplierCode || "") == "") {
        toastDanger("Silahkan pilih supplier");
        return false;
      }

      if ((this.filter.PeriodFrom || "") == "") {
        toastDanger("Silahkan pilih receipt date from");
        return false;
      }

      if ((this.filter.PeriodUntil || "") == "") {
        toastDanger("Silahkan pilih receipt date until");
        return false;
      }

      if (
        new Date(this.filter.PeriodFrom) > new Date(this.filter.PeriodUntil)
      ) {
        toastWarning("Periode Dari tidak boleh melewati Periode Sampai.");
        return;
      }

      if ((this.filter.ReceiptId || "") == "") {
        toastDanger("Silahkan pilih no. receipt");
        return false;
      }

      if ((this.filter.CompleteStatus || "") == "") {
        toastDanger("Silahkan pilih complete status");
        return false;
      }

      return true;
    },
    onSearch: function () {
      this.search(true);
    },
    search: function (cek) {
      if (cek && !this.validSearch()) return;

      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.FactoryCode,
          SupplierCode: this.filter.SupplierCode,
          ReceiptId:
            this.filter.ReceiptId == "ALL"
              ? "0"
              : this.filter.ReceiptId?.toString(),
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom),
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil),
          ),
          CompleteStatus: this.filter.CompleteStatus,
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
    },
    reset: function () {
      this.filter.FactoryCode = null;
      this.filter.SupplierCode = null;
      this.filter.CompleteStatus = "ALL";

      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.PeriodUntil = today;
      this.search(false);
    },
    print: function () {
      if ((this.filter.ReceiptId || "") == "") {
        toastDanger("Silahkan pilih no. receipt");
        return;
      }

      this.dsReceipt
        .print(this.filter.ReceiptId)
        .then((data) => {
          toastSuccess(data || "Print Label berhasil!");
        })
        .catch((err) => toastDanger(err.Message));
    },
    printBarcodesUsingJob: function () {
      if ((this.filter.ReceiptId || "") == "") {
        toastDanger("Silahkan pilih no. receipt");
        return;
      }

      this.dsReceipt
        .printBarcodesUsingJob(this.filter.ReceiptId)
        .then((data) => {
          if (data.Message != "-") toastInfo(data.Message);
        })
        .catch((err) => toastDanger(err.Message));
    },
    exportExcel: function () {
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.FactoryCode,
          SupplierCode: this.filter.SupplierCode,
          ReceiptId:
            this.filter.ReceiptId == "ALL"
              ? "0"
              : this.filter.ReceiptId?.toString(),
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom),
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil),
          ),
          CompleteStatus: this.filter.CompleteStatus,
        },
      ];

      return new Promise((resolve, reject) => {
        this.ds
          .exportExcel(filters)
          .then((_) => {
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          });
      });
    },
    viewDetail: function (item) {
      this.selectedReceiptDetailId = item.ReceiptDetailId;
      this.counter++;
      this.$bvModal.show("modal-list-receipt");
    },
  },
};
</script>

<style scoped>
.vdatetime {
  max-width: 60% !important;
}
.bg-danger {
  background-color: salmon !important;
}
.bg-danger a {
  color: #333;
}
.table-striped-row {
  background-color: #e9ecef;
}

.bg-danger a {
  background-color: rgba(250, 128, 114, 0.5) !important;
}
</style>
