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
                    :status="$route.query.id ? '' : 'NEW'"
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
                    :disabled="$route.query.id"
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
            <input-date
              v-model="model.BCDate"
              style-date="width: 120px"
              :errors="errors?.BCDate"
            />
          </td>
          <td style="padding-top: 5px">
            <label class="form-label">DN Date</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <input-date
              v-model="model.DNDate"
              style-date="width: 120px"
              :errors="errors?.DNDate"
            />
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
          <td>
            <table>
              <tr>
                <td style="padding-left: 15px; padding-top: 5px">
                  <input-text
                    v-model="model.VehicleNo"
                    :errors="errors?.VehicleNo"
                    style="width: 130px"
                    maxlength="15"
                  />
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
                  <label class="form-label">Receipt Date</label>
                </td>
                <td style="padding-left: 5px; padding-top: 5px">
                  <input-date
                    v-model="model.ReceiptDate"
                    style-date="width: 120px"
                    :errors="errors?.ReceiptDate"
                  />
                </td>
              </tr>
            </table>
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
              <v-button-add :add="add" cClass="ml-1" />
              <v-button-submit
                :submit="submit"
                cClass="ml-1"
                :is-loading="isLoading"
              />
              <v-button
                :action="remove"
                label="Delete"
                icon="trash"
                cClass="ml-1 btn-danger"
                :is-loading="isLoading"
              />
              <v-button-print
                label="Print Label"
                class="ml-1"
                :print="printLabel"
                :is-loading="isLoading"
              />

              <v-button
                :action="printBarcodesUsingJob"
                label="Print Label PDF"
                icon="file-pdf"
                cClass="ml-1 btn-green"
                :is-loading="isLoading"
              />
            </div>
          </td>
        </tr>
      </table>

      <hr />

      <div>
        <v-table-input
          :data-items="items"
          ref="vtable"
          :top-content-height="400"
        >
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
                    <th class="text-center">No. Seri</th>
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
                    <td>
                      <input-money-small
                        v-model="item.NoSeri"
                        :errors="errors?.[`Details[${idx}].NoSeri`]"
                        style="width: 100px"
                      />
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
    noSeri: 0,
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
      ReceiptDate: null,
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
        let today = new Date();
        this.isNew = true;
        this.items = [];
        this.filter.ReceiptId = null;
        this.model = {
          Id: null,
          ReceiptNo: "",
          DNNumber: "",
          FactoryCode: null,
          SupplierCode: null,
          DNDate: today,
          BCNumber: "",
          BCType: "",
          BCDate: today,
          ReceiptDate: today,
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
  mounted: function () {
    if (this.$route.query.id) {
      this.filter.ReceiptId = this.$route.query.id;
      this.isNew = false;
      this.getReceipt();
    }
    let today = new Date();
    this.model.ReceiptDate = today;
    this.model.BCDate = today;
    this.model.DNDate = today;
  },
  methods: {
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },

    remove: function () {
      if (!this.filter.ReceiptId) {
        toastDanger("Silahkan pilih Receipt No!");
        return;
      }

      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.dsReceipt
              .remove(this.filter.ReceiptId)
              .then((dt) => {
                toastSuccess("Data deleted successfully!");
                resolve();
                this.reset();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                resolve();
                toastDanger(err?.Message);
              });
          }),
        null,

        "",
      );
    },

    add: function () {
      let lastNoSeriInGrid = this.items.filter((p) => (p.NoSeri ?? 0) > 0);
      if (lastNoSeriInGrid.length > 0) {
        const maxNoSeri = Math.max(
          ...lastNoSeriInGrid.map((p) => p.NoSeri ?? 0),
        );

        this.noSeri = maxNoSeri + 1;
      } else {
        this.noSeri = 1;
      }

      let obj = {
        Id: null,
        ReceiptId: null,
        ItemCode: null,
        ItemName: null,
        UnitClsCode: null,
        UnitClsName: null,
        ReceiptQty: 0,
        TotalPacking: 0,
        NoSeri: this.noSeri,
        QtyPacking: null,
      };

      this.items.push(obj);
    },
    reset: function () {
      let today = new Date();
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
        DNDate: today,
        BCNumber: "",
        BCType: "",
        BCDate: today,
        ReceiptDate: today,
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
        let today = new Date();
        this.isNew = true;
        this.items = [];
        this.filter.ReceiptId = null;
        this.model = {
          Id: null,
          ReceiptNo: "",
          DNNumber: "",
          FactoryCode: null,
          SupplierCode: null,
          DNDate: today,
          BCNumber: "",
          BCType: "",
          BCDate: today,
          ReceiptDate: today,
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
      this.isLoading = true;
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

    print: function () {
      if (this.model.Id == null || this.model.Id == undefined) {
        toastDanger("Silahkan pilih Receipt No!");
        return;
      }
      this.isLoading = true;
      this.dsReceipt
        .print(this.model.Id)
        .then((data) => {
          toastSuccess(data || "Print Label berhasil!");
        })
        .catch((err) => toastDanger(err.Message))
        .finally(() => (this.isLoading = false));
    },
    printBarcodesUsingJob: function () {
      if (!this.filter.ReceiptId) {
        toastDanger("Silahkan pilih Receipt No!");
        return;
      }

      this.isLoading = true;

      this.dsReceipt
        .printBarcodesUsingJob(this.filter.ReceiptId)
        .then((data) => {
          if (data.Message != "-") toastInfo(data.Message);
        })
        .catch((err) => toastDanger(err.Message))
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

      if (this.items.some((x) => x.ReceiptQty / x.QtyPacking > 100)) {
        let totalBarcodePrint = this.items.reduce(
          (total, item) => total + item.ReceiptQty / item.QtyPacking,
          0,
        );
        let modalMessage = `<div style="font-size: medium">Total barcode yang akan dicetak sebanyak <strong>${this.$func.formatMoney(Math.ceil(totalBarcodePrint))} Barcode</strong>.
            <br>Anda yakin akan <strong>MELANJUTKAN</strong>?</div>`;
        confirmSubmit(
          () =>
            new Promise((resolve) => {
              if (this.isNew) {
                this.createReceipt();
              } else {
                this.updateReceipt();
              }
              resolve();
            }),
          () => (this.isLoading = false),
          modalMessage,
        );
      } else {
        if (this.isNew) {
          this.createReceipt();
        } else {
          this.checkIsDetailUpdate();
        }
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
    checkIsDetailUpdate: function () {
      this.ds
        .checkIsDetailUpdate(this.model)
        .then((dt) => {
          if (dt.Data.IsUpdateDetails) {
            let modalMessage = `<div style="font-size: medium">Anda mengubah <strong>${dt.Data.TypeConfirmationDesc}</strong>.
                <br><strong>Barcode Label Saat ini</strong> akan menjadi <strong class="text-danger">TIDAK VALID</strong> 
                <br>Anda yakin akan <strong>MELANJUTKAN</strong> perubahan?</div>`;

            confirmSubmit(
              () =>
                new Promise((resolve) => {
                  this.updateReceipt();
                  resolve();
                }),
              () => (this.isLoading = false),
              modalMessage,
            );
          } else if (
            !dt.Data.IsUpdateDetails &&
            dt.Data.TypeConfirmation == 3
          ) {
            let modalMessage = `<div style="font-size: medium">Anda sudah melakukan <strong>SCAN RECEIVING MOBILE</strong>.
                <br>Perubahan hanya berlaku untuk informasi <strong>Header</strong> saja. 
                <br>Anda yakin akan <strong>MELANJUTKAN</strong> perubahan?</div>`;

            confirmSubmit(
              () =>
                new Promise((resolve) => {
                  this.updateReceipt();
                  resolve();
                }),
              () => (this.isLoading = false),
              modalMessage,
            );
          } else {
            this.updateReceipt();
          }
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
          this.getReceipt();

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
