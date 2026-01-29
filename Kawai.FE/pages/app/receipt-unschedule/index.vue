<template>
  <v-frame title="Part Material Receipt Unschedule" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Factory</label></td>
          <td style="padding-left: 15px">
            <filter-factory-privileges
              class="form-control"
              v-model="filter.FactoryCode"
              :disabled="filter.ReceiptId != null"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
          <td style="padding-left: 15px">
            <label class="form-label">BC Number</label>
          </td>
          <td style="padding-left: 15px" colspan="3">
            <input-text
              v-model="model.BCNumber"
              :errors="errors?.BCNumber"
              style="width: 360px"
              maxlength="50"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Supplier</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <filter-trade-2
              class="form-control"
              v-model="filter.SupplierCode"
              :trade-cls="['2', '3']"
              :disabled="filter.ReceiptId != null"
              style-code="width: 120px"
              style-desc="width: 240px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
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
            <label class="form-label">Receipt No</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <table>
              <tr>
                <td>
                  <input-receipt
                    class="form-control"
                    :disabled="isNew"
                    status="NEW"
                    source-menu="RECEIPT UNSCHEDULE"
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
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">BC Date</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <input-date v-model="model.BCDate" style-date="width: 115px" />
          </td>
          <td style="padding-top: 5px">
            <label class="form-label">DN Date</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <input-date v-model="model.DNDate" style-date="width: 115px" />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">DN Number</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-text
              v-model="model.DNNumber"
              :errors="errors?.DNNumber"
              style="width: 360px"
              maxlength="50"
            />
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
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
            <label class="form-label">Police No</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <input-text
              v-model="model.VehicleNo"
              :errors="errors?.VehicleNo"
              style="width: 130px"
              maxlength="15"
            />
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <label class="form-label">Reference No</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px" colspan="3">
            <input-text
              v-model="model.ReferenceNo"
              :errors="errors?.ReferenceNo"
              style="width: 360px"
              maxlength="50"
            />
          </td>
        </tr>
        <tr>
          <td colspan="2" style="padding-top: 5px">
            <div class="d-flex flex-fill">
              <v-button-add :add="add" cClass="mr-1" />
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
            </div>
          </td>
        </tr>
      </table>

      <div style="width: 1150px">
        <v-table-input :data-items="items" ref="vtable">
          <template #table-content>
            <div class="detail-content">
              <table
                class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
                ref="table"
              >
                <thead>
                  <tr>
                    <th class="text-center">No</th>
                    <th class="text-center">Item Code</th>
                    <th class="text-center">Item Name</th>
                    <th class="text-center">Unit</th>
                    <th class="text-center">Qty DN</th>
                    <th class="text-center">Total Packing</th>
                    <th class="text-center">Qty Packing</th>
                    <th class="text-center">Last Update</th>
                    <th class="text-center">Last User</th>
                    <th class="text-center"></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, i) in items || []" :key="i">
                    <td class="text-center">
                      {{ i + 1 }}
                    </td>
                    <td>
                      <div class="row">
                        <div class="col-10">
                          <input-text-small
                            v-model="item.ItemCode"
                            disabled
                            :errors="errors?.[`Details[${i}].ItemCode`]"
                          />
                        </div>
                        <div class="col-1" style="margin-left: -1em">
                          <button
                            class="form-submit bg-primary"
                            style="
                              height: 100%;
                              width: 2.5em;
                              border-radius: 0.5em;
                            "
                            @click="loadItem(i)"
                          >
                            <v-icon name="search" width="16px" />
                          </button>
                        </div>
                      </div>
                    </td>
                    <td>{{ item.ItemName }}</td>
                    <td>{{ item.UnitClsName }}</td>
                    <td>
                      <input-money-small
                        v-model="item.ReceiptQty"
                        :errors="errors?.[`Details[${i}].ReceiptQty`]"
                      />
                    </td>
                    <td class="text-right">
                      {{
                        item.QtyPacking > 0
                          ? $func.formatMoney(
                              Math.ceil(
                                parseFloat(item.ReceiptQty) /
                                  parseFloat(item.QtyPacking),
                              ),
                            )
                          : "0"
                      }}
                    </td>
                    <td class="text-right">
                      {{ $func.formatMoney(item.QtyPacking) }}
                    </td>
                    <td></td>
                    <td></td>
                    <td>
                      <a href="javascript:void(0)" @click="items.splice(i, 1)">
                        <v-icon name="x" width="20" color="red" />
                      </a>
                    </td>
                  </tr>
                </tbody>
              </table>
              <v-data-empty
                class="mt-3"
                v-if="
                  !dsReceipt.isLoading &&
                  items.length == 0 &&
                  !dsReceipt.isNetworkError &&
                  !dsReceipt.isServerError
                "
              />
            </div>
          </template>
        </v-table-input>
      </div>

      <div
        class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12 mt-2"
        style="display: none"
      >
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

  <v-modal id="shared-item-list" title="List Item" size="lg">
    <shared-item-list
      :filters="[{ SupplierCode: this.filter.SupplierCode }]"
      :list="this.ds"
      :refresh="refreshItemList"
      :actions="[
        {
          href: 'javascript:void(0);',
          icon: 'edit',
          label: 'Select',
          event: (item) => selectItem(item),
        },
      ]"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    isNew: true,
    filter: {
      FactoryCode: null,
      SupplierCode: null,
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
      ReferenceNo: null,
      Remarks: null,
      Details: [],
    },
    items: [],
    dataItem: null,
    idxItemLoad: 0,
    debounce: null,
    isLoading: false,
    errors: {},
    refreshItemList: 0,
  }),
  computed: {
    ds: function () {
      return useReceiptUnschedule();
    },
    dsReceipt: function () {
      return useReceipt();
    },
    notif: function () {
      return useNotification();
    },
  },
  watch: {
    "filter.FactoryCode": function () {
      this.items = [];
    },
    "filter.SupplierCode": function () {
      this.items = [];
    },
    "filter.ReceiptId": function () {
      if (this.filter.ReceiptId) this.getReceipt();
      else {
        this.isNew = true;
        this.items = [];
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
          ReferenceNo: null,
          RegisterNo: null,
          Remarks: null,
          Details: [],
        };
      }
    },
  },
  mounted: function () {},
  methods: {
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    add: function () {
      let obj = {
        Id: null,
        ReceiptId: null,
        ItemCode: null,
        ItemName: null,
        UnitClsCode: null,
        UnitClsName: null,
        ReceiptQty: 0,
        TotalPacking: 0,
        QtyPacking: null,
      };

      this.items.push(obj);
    },
    reset: function () {
      this.isNew = true;
      this.filter = {
        FactoryCode: null,
        SupplierCode: null,
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
        ReferenceNo: null,
        RegisterNo: null,
        Remarks: null,
        Details: [],
      };
    },
    changeNew: function (e) {
      if (e.target.checked) {
        this.isNew = true;
        this.items = [];
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
          ReferenceNo: null,
          RegisterNo: null,
          Remarks: null,
          Details: [],
        };
        // this.reset();
      }
    },
    loadItem: function (idx) {
      this.idxItemLoad = idx;
      this.refreshItemList++;
      this.$bvModal.show("shared-item-list");
    },
    selectItem(dt) {
      this.dataItem = dt;

      if (dt.QtyPacking <= 0) {
        toastWarning("Qty Packing harus lebih dari 0!");
        return;
      }

      const duplicate = this.items.some(
        (p, i) => p.ItemCode === dt.ItemCode && i !== this.idxItemLoad,
      );

      if (duplicate) {
        toastWarning("Data Item sudah ada didalam list!");
        return;
      }

      const detail = this.items[this.idxItemLoad];
      detail.ItemCode = dt.ItemCode;
      detail.ItemName = dt.ItemName;
      detail.UnitClsCode = dt.UnitClsCode;
      detail.UnitClsName = dt.UnitClsName;
      detail.QtyPacking = dt.QtyPacking;

      this.$bvModal.hide("shared-item-list");
    },
    getReceipt: function () {
      this.dsReceipt.loadDetail(this.filter.ReceiptId).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.items = [];
        this.filter.SupplierCode = this.model.SupplierCode;
        this.$nextTick(() => setTimeout(() => this.search(), 500));
      });
    },
    printLabel: function () {
      if (this.model.Id == null || this.model.Id == undefined) {
        toastDanger("Silahkan pilih Receipt No!");
        return;
      }

      this.model.Remarks = "-";
      this.dsReceipt
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
    submit: function () {
      this.isLoading = true;
      this.errors = {};

      if (this.items.length == 0) {
        this.isLoading = false;
        toastDanger("Silahkan pilih Item!");
        return;
      }

      this.model.Id = this.filter.ReceiptId;
      this.model.FactoryCode = this.filter.FactoryCode;
      this.model.SupplierCode = this.filter.SupplierCode;
      this.model.Details = this.items;

      if (this.isNew) {
        this.createReceipt();
      } else {
        this.updateReceipt();
      }
    },
    search: function () {
      this.dsReceipt.listDetail(this.filter.ReceiptId).then((dt) => {
        this.items = dt.Data;
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
