<template>
  <v-frame title="Part Material Receipt Input" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <table>
              <tr>
                <td><label class="form-label">Factory</label></td>
                <td style="padding-left: 15px" colspan="3">
                  <filter-factory-privileges
                    class="form-control"
                    v-model="filter.FactoryCode"
                    :disabled="filter.ReceiptId != null"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Supplier</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px" colspan="3">
                  <filter-trade-2
                    class="form-control"
                    v-model="filter.SupplierCode"
                    :trade-cls="['2', '3']"
                    :disabled="filter.ReceiptId != null"
                    style-code="width: 120px"
                    style-desc="width: 240px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Delivery Date</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px">
                  <input-date
                    v-model="filter.PeriodFrom"
                    style-date="width: 115px"
                  />
                </td>
                <td style="padding-top: 5px">
                  <label class="form-label">Until Date</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px">
                  <input-date
                    v-model="filter.PeriodUntil"
                    style-date="width: 115px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">PO Number</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px" colspan="3">
                  <input-po
                    class="form-control"
                    v-model="filter.PONumber"
                    :factory-code="filter.FactoryCode"
                    :supplier-code="filter.SupplierCode"
                    :type-date="filter.TypeDate"
                    :period-from="filter.PeriodFrom"
                    :period-until="filter.PeriodUntil"
                    :show-option-all="true"
                    style="width: 360px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Receipt No</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px" colspan="3">
                  <table>
                    <tr>
                      <td>
                        <input-receipt
                          class="form-control"
                          :disabled="isNew"
                          status="NEW"
                          source-menu="RECEIPT PO"
                          :factory-code="filter.FactoryCode"
                          :supplier-code="filter.SupplierCode"
                          v-model="filter.ReceiptId"
                          :errors="errors?.ReceiptId"
                          style="width: 300px"
                        />
                      </td>
                      <td style="padding-left: 15px">
                        <input-checkbox
                          label="New"
                          v-model="isNew"
                          @click="(e) => changeNew(e)"
                        />
                      </td>
                    </tr>
                  </table>
                  <div class="d-flex-fill"></div>
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">DN Number</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px" colspan="3">
                  <input-text
                    v-model="model.DNNumber"
                    :errors="errors?.DNNumber"
                    style="width: 360px"
                  />
                </td>
              </tr>
            </table>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <table>
              <tr>
                <td><label class="form-label">BC Number</label></td>
                <td style="padding-left: 15px" colspan="3">
                  <input-text
                    v-model="model.BCNumber"
                    :errors="errors?.BCNumber"
                    style="width: 360px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">BC Type</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px" colspan="3">
                  <filter-cls-2
                    class="form-control"
                    type-data="BCType_Cls"
                    v-model="model.BCType"
                    :errors="errors?.BCType"
                    style-code="width: 120px"
                    style-desc="width: 240px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">BC Date</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px">
                  <input-date
                    v-model="model.BCDate"
                    style-date="width: 115px"
                  />
                </td>
                <td style="padding-top: 5px">
                  <label class="form-label">DN Date</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px">
                  <input-date
                    v-model="model.DNDate"
                    style-date="width: 115px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Police No</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px" colspan="3">
                  <input-text
                    v-model="model.VehicleNo"
                    :errors="errors?.VehicleNo"
                    style="width: 360px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Transport By</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px" colspan="3">
                  <filter-cls-2
                    class="form-control"
                    type-data="Transport_Cls"
                    v-model="model.Transport"
                    :errors="errors?.Transport"
                    style-code="width: 120px"
                    style-desc="width: 240px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Register No</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px" colspan="3">
                  <input-text
                    v-model="model.RegisterNo"
                    :errors="errors?.RegisterNo"
                    style="width: 360px"
                  />
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
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
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td colspan="2" style="padding-top: 5px">
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
              <v-button
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

      <div style="width: 1150px">
        <v-table-input :data-items="listPODetail" ref="vtable">
          <template #table-content>
            <div class="detail-content" style="width: 100%">
              <table
                class="table table-striped table-bordered mb-0 align-middle v-fixed-table w-100"
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
                    <td class="text-right">
                      {{ $func.formatMoney(item.Qty) }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.TotalReceiptQty) }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.RemainingQty) }}
                    </td>
                    <td>
                      <input-money-small
                        v-model="item.ReceiptQty"
                        :errors="errors?.[`Details[${idx}].ReceiptQty`]"
                        style="width: 100px"
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
      </div>

      <div style="width: 1150px">
        <label class="form-label">Remarks</label>
        <input-text
          multiline
          v-model="model.Remarks"
          :errors="errors?.Remarks"
        />
      </div>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
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
      FactoryCode: null,
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
    "filter.FactoryCode": function () {
      this.listPODetail = [];
    },
    "filter.SupplierCode": function () {
      this.listPODetail = [];
    },
    "filter.ReceiptId": function () {
      if (this.filter.ReceiptId) this.getReceipt();
      else {
        this.isNew = true;
        let today = new Date();
        this.filter.PeriodFrom = new Date(
          today.getFullYear(),
          today.getMonth(),
          1,
        );
        this.filter.PeriodUntil = today;
        this.filter.PONumber = null;
        this.listPODetail = [];
        this.filter.ReceiptId = null;
        this.model = {
          Id: null,
          ReceiptNo: "",
          DNNumber: "",
          FactoryCode: null,
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
          SourceMenu: "RECEIPT PO",
        };
      }
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
        FactoryCode: null,
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
        SourceMenu: "RECEIPT PO",
      };
      this.listPODetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.PeriodUntil = today;
    },
    changeNew: function (e) {
      if (e.target.checked) {
        this.isNew = true;
        this.listPODetail = [];
        this.filter.ReceiptId = null;
        this.model = {
          Id: null,
          ReceiptNo: "",
          DNNumber: "",
          FactoryCode: null,
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
          SourceMenu: "RECEIPT PO",
        };
        // this.reset();
      }
    },
    checkAll: function (e) {
      this.listPODetail.map((x) => {
        x.Selected = e.target.checked;
        x.ReceiptQty = e.target.checked ? x.RemainingQty : 0;
      });
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
      item.ReceiptQty = e.target.checked ? item.RemainingQty : 0;
    },
    printLabel: function () {
      this.ds
        .printLabel(this.filter.ReceiptId)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.isNew = false;
          this.filter.ReceiptId = dt.Data["Receipt Header"].Id;
          // this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },

    print: function () {
      if (!this.filter.ReceiptId) {
        toastDanger("Silahkan pilih Receipt No!");
        return;
      }
      this.ds
        .print(this.filter.ReceiptId)
        .then((data) => {
          toastSuccess(data || "Print Label berhasil!");
        })
        .catch((err) => toastDanger(err.Message));
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
      this.model.FactoryCode = this.filter.FactoryCode;
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
        this.filter.FactoryCode = this.model.FactoryCode;
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
          FactoryCode: this.filter.FactoryCode,
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
          this.isNew = false;
          this.filter.ReceiptId = dt.Data["Receipt Header"].Id;
          // this.reset();
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
          this.isNew = false;
          this.filter.ReceiptId = dt.Data["Receipt Header"].Id;
          // this.reset();
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
