<template>
  <v-frame title="Part Receipt Material Input" icon="receipt">
    <template #frame-content>
      <div class="row">
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Factory</label>
            <input-factory-privileges v-model="filter.FactoryCode" />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">BC Number</label>
            <input-text v-model="model.BCNumber" :errors="errors?.BCNumber" />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Supplier</label>
            <input-trade
              placeholder="Supplier"
              v-model="filter.SupplierCode"
              :disabled="filter.ReceiptId != null"
            />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 mt-2">
          <div class="mb-3">
            <label class="form-label">BC Type</label>
            <input-cls
              type-data="BCType_Cls"
              placeholder="BC Type"
              v-model="model.BCType"
              :errors="errors?.BCType"
            />
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Delivery Date From</label>
            <input-date v-model="filter.PeriodFrom" />
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Delivery Date Until</label>
            <input-date v-model="filter.PeriodUntil" />
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">BC Date</label>
            <input-date v-model="model.BCDate" :errors="errors?.BCDate" />
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-3 col-sm-6 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">DN Date</label>
            <input-date v-model="model.DNDate" :errors="errors?.DNDate" />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 mt-2">
          <div class="mb-3">
            <label class="form-label">PO Number</label>
            <input-po
              v-model="filter.PONumber"
              :supplier-code="filter.SupplierCode"
              :type-date="filter.TypeDate"
              :period-from="filter.PeriodFrom"
              :period-until="filter.PeriodUntil"
              :show-option-all="true"
            />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Police No</label>
            <input-text v-model="model.VehicleNo" :errors="errors?.VehicleNo" />
          </div>
        </div>
        <div class="col-xl-5 col-lg-5 col-md-5 col-sm-8 col-8 mt-2">
          <div class="mb-3">
            <label class="form-label">Receipt No</label>
            <input-receipt
              :disabled="isNew"
              status="NEW"
              source-menu="RECEIPT PO"
              :supplier-code="filter.SupplierCode"
              v-model="filter.ReceiptId"
              :errors="errors?.ReceiptId"
            />
          </div>
        </div>
        <div class="col-xl-1 col-lg-1 col-md-1 col-sm-4 col-4 mt-4">
          <div class="mb-3">
            <label class="form-label"></label>
            <input-checkbox
              label="New"
              v-model="isNew"
              @click="(e) => changeNew(e)"
            />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 mt-2">
          <div class="mb-3">
            <label class="form-label">Transport By</label>
            <input-cls
              type-data="Transport_Cls"
              placeholder="Transport"
              v-model="model.Transport"
              :errors="errors?.Transport"
            />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">DN Number</label>
            <input-text v-model="model.DNNumber" :errors="errors?.DNNumber" />
          </div>
        </div>
        <div class="col-xl-4 col-lg-4 col-md-4 col-sm-8 col-8 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Register No</label>
            <input-text
              v-model="model.RegisterNo"
              :errors="errors?.RegisterNo"
            />
          </div>
        </div>
        <div class="col-xl-2 col-lg-2 col-md-2 col-sm-4 col-4 mt-8">
          <div class="mr-1" style="width: 100%">
            <label class="form-label"></label>
            <button
              class="btn btn-sm btn-blue btn-elevate mr-1"
              @click="searchPoDetail"
              :disabled="isLoading"
            >
              <div
                class="spinner-border spinner-border-sm text-light"
                role="status"
                v-if="isLoading"
              >
                <span class="visually-hidden">Loading...</span>
              </div>
              <font-awesome-icon v-else icon="search" />
              <span class="ml-2">Search</span>
            </button>
          </div>
        </div>
      </div>
      <hr />
      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-submit
            :submit="submit"
            cClass="mr-1"
            :is-loading="isLoading"
          />
          <v-button-print
            label="Print Label"
            class="mr-1"
            :print="printLabel"
            :is-loading="isLoading"
          />
          <v-button-print
            label="Print Receipt Report"
            :print="printReport"
            :is-loading="isLoading"
          />
        </div>
      </div>

      <v-table-input :data-items="listPODetail" ref="vtable">
        <template #table-content>
          <div class="detail-content">
            <table
              class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
              v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
              ref="table"
            >
              <thead>
                <tr>
                  <th class="text-center">
                    <input-checkbox @click="(e) => checkAll(e)" />
                  </th>
                  <th class="text-center">PO Number</th>
                  <th class="text-center">Item Code</th>
                  <th class="text-center">Item Name</th>
                  <th class="text-center">Unit</th>
                  <th class="text-center">PO Qty</th>
                  <th class="text-center">Receipt Qty</th>
                  <th class="text-center">Remaining Qty</th>
                  <th class="text-center">Qty DN</th>
                  <th class="text-center">Total Packing</th>
                  <th class="text-center">Qty Packing</th>
                  <th class="text-center">No. Seri</th>
                  <th class="text-center">Production Date</th>
                  <th class="text-center">Last Update</th>
                  <th class="text-center">Last User</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in listPODetail || []" :key="idx">
                  <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                      @click="(e) => check(e, item)"
                    />
                  </td>
                  <td>{{ item.PONumber }}</td>
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsName }}</td>
                  <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.TotalReceiptQty) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.RemainingQty) }}
                  </td>
                  <td>
                    <input-money
                      v-model="item.ReceiptQty"
                      :errors="errors?.[`Details[${i}].ReceiptQty`]"
                    />
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.TotalPacking) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.QtyPacking) }}
                  </td>
                  <td>{{ item.NoSeri }}</td>
                  <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                  <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                  <td>{{ item.LastUser }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </template>
      </v-table-input>

      <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12 mt-2">
        <div class="mr-1" style="width: 100%">
          <label class="form-label">Remarks</label>
          <input-text
            multiline
            v-model="model.Remarks"
            :errors="errors?.Remarks"
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
      { title: "Part Receipt Material", active: false, to: "" },
    ],
    isNew: true,
    filter: {
      FactoryCode: null,
      SupplierCode: null,
      TypeDate: "DELIVERY",
      PeriodFrom: null,
      PeriodUntil: null,
      PONumber: null,
      ReceiptId: null,
    },
    model: {
      Id: null,
      ReceiptNo: "",
      DNNumber: "",
      SupplierCode: null,
      DNDate: null,
      BCNumber: "",
      BCType: "",
      BCDate: null,
      VehicleNo: "",
      Transport: null,
      RegisterNo: null,
      Remarks: null,
      Details: [],
      IsManual: true,
    },
    listPODetail: [],
    debounce: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useReceipt();
    },
    dsPO: function () {
      return usePO();
    },
    notif: function () {
      return useNotification();
    },
  },
  watch: {
    "filter.PONumber": function () {
      this.listPODetail = [];
    },
    "filter.SupplierCode": function () {
      this.listPODetail = [];
    },
    "filter.ReceiptId": function () {
      if (this.filter.ReceiptId) this.getReceipt();
    },
  },
  mounted: function () {
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
  },
  methods: {
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    reset: function () {
      this.isNew = true;
      this.filter = {
        FactoryCode: null,
        SupplierCode: null,
        TypeDate: "DELIVERY",
        PeriodFrom: null,
        PeriodUntil: null,
        PONumber: null,
        ReceiptId: null,
      };
      this.model = {
        Id: null,
        ReceiptNo: "",
        DNNumber: "",
        SupplierCode: null,
        DNDate: null,
        BCNumber: "",
        BCType: "",
        BCDate: null,
        VehicleNo: "",
        Transport: null,
        RegisterNo: null,
        Remarks: null,
        Details: [],
        IsManual: true,
        SourceMenu: "RECEIPT PO"
      };
      this.listPODetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.PeriodUntil = today;
    },
    changeNew: function (e) {
      if (e.target.checked) this.reset();
    },
    checkAll: function (e) {
      this.listPODetail.map((x) => (x.Selected = e.target.checked));
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
    },
    printLabel: function () {
      this.ds
        .printLabel(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));      
    },
    printReport: function () {},
    submit: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listPODetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Silahkan pilih PO!");
        return;
      }

      this.model.Id = this.filter.ReceiptId;
      this.model.SupplierCode = this.filter.SupplierCode;
      this.model.Details = details.map((p) => {
        return {
          PONumber: p.PONumber,
          ItemCode: p.ItemCode,
          UnitClsCode: p.UnitClsCode,
          ExpectedQty: p.RemainingQty,
          TotalPacking: p.TotalPacking,
          ReceiptQty: p.ReceiptQty,
        };
      });

      if (this.isNew) {
        this.createReceipt();
      } else {
        this.updateReceipt();
      }
    },
    getReceipt: function () {
      this.ds.loadDetail(this.filter.ReceiptId).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.filter.SupplierCode = this.model.SupplierCode;
        this.filter.PeriodFrom = this.model.DeliveryDatePOFrom;
        this.filter.PeriodUntil = this.model.DeliveryDatePOUntil;
        this.filter.PONumber = this.model.PONumber;
        this.$nextTick(() => setTimeout(() => this.searchPoDetail(), 500));
      });
    },
    searchPoDetail: function () {
      if (!this.filter.SupplierCode) {
        toastDanger("Silahkan pilih Supplier!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          ReceiptId: this.filter.ReceiptId?.toString() || "0",
          PONumber: this.filter.PONumber,
          SupplierCode: this.filter.SupplierCode,
          DateFrom: this.filter.PeriodFrom,
          DateUntil: this.filter.PeriodUntil,
        },
      ];
      this.ds.listPODetail(filters).then((dt) => {
        this.listPODetail = dt.Data.Items;
        this.listPODetail.map((x) => (x.Selected = x.ReceiptDetailId > 0));
      });
    },
    createReceipt: function () {
      this.ds
        .create(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    updateReceipt: function () {
      this.ds
        .update(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
  },
};
</script>

<style scoped>
.detail-content {
  max-height: 30em;
  max-height: 60%;
}

.detail-content-view {
  /* padding-bottom: 5em; */
  height: 70%;
  max-height: 70%;
  overflow-y: scroll;
}
</style>
