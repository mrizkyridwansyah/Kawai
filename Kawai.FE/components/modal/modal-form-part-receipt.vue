<template>
  <div class="row">
    <div class="col-xs-12 col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">DN Number</label>
        <div class="row">
          <div class="col-10">
            <input-text
              placeholder="DN Number"
              v-model="model.DNNumber"
              :disabled="mode === 'view'"
              :errors="errors?.DNNumber"
              show-suffix
            />
          </div>
          <div class="col-1" style="margin-left: -1em">
            <button
              class="form-submit bg-primary"
              style="height: 100%; width: 4.5em; border-radius: 0.5em"
              @click="loadDN"
              :disabled="mode === 'view'"
            >
              <v-icon name="search" width="16px" />
            </button>
          </div>
        </div>
      </div>
    </div>
    <div class="col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">BC Number</label>
        <input-text
          placeholder="BC Number"
          v-model="model.BCNumber"
          :disabled="!isManual || mode === 'view'"
          :errors="errors?.BCNumber"
        />
      </div>
    </div>
    <div class="col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">BC Type</label>
        <input-cls
          type-data="BCType_Cls"
          placeholder="BC Type"
          v-model="model.BCType"
          :disabled="!isManual || mode === 'view'"
          :errors="errors?.BCType"
        />
      </div>
    </div>
    <div class="col-xs-12 col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">DN Date</label>
        <input-date
          placeholder="DN Date"
          v-model="model.DNDate"
          :disabled="!isManual || mode === 'view'"
          :errors="errors?.DNDate"
        />
      </div>
    </div>
  </div>
  <div class="row">
    <div class="col-xs-12 col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">Supplier</label>
        <input-trade
          placeholder="Supplier"
          v-model="model.SupplierCode"
          :disabled="!isManual || mode === 'view'"
          :errors="errors?.SupplierCode"
        />
      </div>
    </div>
    <div class="col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">Vehicle No</label>
        <input-text
          placeholder="Vehicle No"
          v-model="model.VehicleNo"
          :disabled="!isManual || mode === 'view'"
          :errors="errors?.VehicleNo"
        />
      </div>
    </div>
    <div class="col-sm-12 col-md-4 col-lg-3 col-xl-3">
      <div class="mb-3">
        <label class="form-label">BC Date</label>
        <input-date
          placeholder="BC Date"
          v-model="model.BCDate"
          :disabled="!isManual || mode === 'view'"
          :errors="errors?.BCDate"
        />
      </div>
    </div>
  </div>
  <hr />
  <div :class="
    { 'detail-content': mode === 'add'},
    { 'detail-content-view': mode === 'view'}
  ">
    <table class="table table-striped mb-0 align-middle v-fixed-table">
      <thead>
        <tr>
          <th class="text-center">#</th>
          <th class="text-center">PO Number</th>
          <th class="text-center" style="width: 40em">Item Name</th>
          <th class="text-center">Unit</th>
          <th class="text-center" style="width: 12em">Expected Qty</th>
          <th class="text-center" style="width: 12em">Total Packing</th>
          <th class="text-center" style="width: 12em">Received Qty</th>
          <th class="text-center" style="width: 1em" v-if="mode !== 'add'">IQC Result</th>
          <th class="text-center"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in this.model.Details">
          <td class="text-center">{{ i + 1 }}.</td>
          <td v-if="isManual">
            <div class="row">
              <div class="col-10">
                <input-text
                  placeholder="PO Number"
                  v-model="item.PONumber"
                  :disabled="mode === 'view'"
                  :errors="errors?.[`Details[${i}].PONumber`]"
                  
                  show-suffix
                />
              </div>
              <div class="col-1" style="margin-left: -1em">
                <button
                  class="form-submit bg-primary"
                  style="height: 100%; width: 4.5em; border-radius: 0.5em"
                  @click="loadPO(i)"
                  :disabled="mode === 'view'"
                >
                  <v-icon name="search" width="16px" />
                </button>
              </div>
            </div>
          </td>
          <td v-else>{{ item.PONumber }}</td>
          <td>{{ item.ItemName }}</td>
          <td>{{ item.UnitClsName }}</td>
          <td class="text-right">{{ $func.formatMoney(item.ExpectedQty) }}</td>
          <td class="text-right">{{ $func.formatMoney(item.TotalPacking) }}</td>
          <td>
            <input-money
              v-model="item.ReceiptQty"
              :errors="errors?.[`Details[${i}].ReceiptQty`]"
              :disabled="mode === 'view'"
            />
          </td>
          <td v-if="mode !== 'add'">
            <input-cls
              v-model="item.IQCResult"
              type-data="IQCResult"
              placeholder="IQC Result"
              :errors="errors?.[`Details[${i}].IQCResult`]"
              :disabled="mode === 'view'"
            />
          </td>
          <td>
            <a
              v-if="isManual && mode === 'add'"
              href="javascript:void(0)"
              @click="model.Details.splice(i, 1)"
            >
              <v-icon name="x" width="20" color="red" />
            </a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
  <div
    class="mt-6"
    style="padding-bottom: 4em"
    v-if="isManual && mode === 'add'"
  >
    <a href="javascript:void(0)" @click="addDetail(1)"> [+] Tambah Baris </a>
    <span class="mx-3"> | </span>
    <a href="javascript:void(0)" @click="addDetail(5)"> [+5] Tambah 5 Baris </a>
  </div>
  <div class="submit-wrapper">
    <v-button-submit
      v-if="mode==='add'"
      :nomargintop="true"
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>

  <v-modal id="shared-delivery-note-list" title="List Delivery Note" size="lg">
    <shared-delivery-note-list
      :data="this.dataDN"
      :list="this.dsDN"
      :actions="[
        {
          href: 'javascript:void(0);',
          icon: 'edit',
          label: 'Select',
          event: (item) => selectDN(item),
        },
      ]"
    />
  </v-modal>

  <v-modal id="shared-po-list-detail" title="List PO Detail" size="lg">
    <shared-po-list-detail
      :filters="[{ SupplierCode: this.model.SupplierCode }]"
      :list="this.dsPO"
      :actions="[
        {
          href: 'javascript:void(0);',
          icon: 'edit',
          label: 'Select',
          event: (item) => selectPO(item),
        },
      ]"
    />
  </v-modal>
</template>
<script>
export default {
  props: ["id", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    dataDN: null,
    dataPO: null,
    idxPOLoad: 0,
    isManual: true,
    model: {
      Id: null,
      ReceiptNo: "",
      IsManual: true,
      DNNumber: "",
      SupplierCode: "",
      DNDate: null,
      BCNumber: "",
      BCType: "",
      BCDate: null,
      VehicleNo: "",
      Details: [],
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds() {
      return useReceipt();
    },
    dsDN() {
      return useDeliveryNote();
    },
    dsPO() {
      return usePO();
    },
  },
  mounted() {
    if (this.mode === "view" && this.id) {
      this.loadDetail(this.id);
    } else if (this.mode === "add") {
      this.resetForm();
    }
  },
  watch: {
    mode(val) {
      if (val === "view") {
        this.loadDetail(this.id);
      } else {
        this.isManual = true;
        this.resetForm();
      }
    },
    "model.DNNumber"(val) {
      if (!val && !this.isManual && this.mode === "add") this.resetForm();
    },
    "model.SupplierCode"(val) {
      if (!val && this.isManual && this.mode === "add") this.model.Details = [];
    },
  },
  methods: {
    deepClone(obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    addDetail(c = 1) {
      for (let i = 0; i < c; i++) {
        this.model.Details.push({
          Id: null,
          PONumber: null,
          ItemCode: null,
          UnitClsCode: null,
          ExpectedQty: null,
          TotalPacking: null,
          ReceiptQty: null,
          IQCResult: null,
        });
      }
    },
    loadDetail() {
      this.ds.loadDetail(this.id).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.ds.listDetail(this.id).then((dtl) => {
          this.model.Details = this.deepClone(dtl.Data || []);
        });
      });
    },
    loadDN() {
      this.dsDN.setSort({ DNNumber: "asc" });
      this.dsDN.load();
      this.$bvModal.show("shared-delivery-note-list");
    },
    selectDN(dt) {
      this.dataDN = dt;
      this.model.DNNumber = dt.DNNumber;
      this.model.SupplierCode = dt.SupplierCode;
      this.model.SupplierName = dt.SupplierName;
      this.model.DNDate = dt.DNDate;
      this.model.BCNumber = dt.BCNumber;
      this.model.BCType = dt.BCType;
      this.model.BCDate = dt.BCDate;
      this.model.VehicleNo = dt.VehicleNo;
      this.isManual = false;

      this.dsDN.listDetail(dt.DNNumber).then((dtl) => {
        this.model.Details = this.deepClone(dtl.Data || []);
      });

      this.$bvModal.hide("shared-delivery-note-list");
    },
    loadPO(idx) {
      if (!this.model.SupplierCode) {
        toastWarning("Please choose supplier");
        return;
      }
      this.idxPOLoad = idx;
      this.$bvModal.show("shared-po-list-detail");
    },
    selectPO(dt) {
      this.dataPO = dt;

      const duplicate = this.model.Details.some(
        (p, i) =>
          p.PONumber === dt.PONumber &&
          p.ItemCode === dt.ItemCode &&
          i !== this.idxPOLoad
      );

      if (duplicate) {
        toastWarning("Data PO & Item sudah ada didalam list!");
        return;
      }

      const detail = this.model.Details[this.idxPOLoad];
      detail.PONumber = dt.PONumber;
      detail.ItemCode = dt.ItemCode;
      detail.ItemName = dt.ItemName;
      detail.UnitClsCode = dt.UnitClsCode;
      detail.UnitClsName = dt.UnitClsName;
      detail.ExpectedQty = dt.Qty;
      detail.TotalPacking = dt.TotalPacking;

      this.$bvModal.hide("shared-po-list-detail");
    },
    resetForm() {
      this.model = {
        Id: null,
        ReceiptNo: "",
        DNNumber: "",
        SupplierCode: "",
        DNDate: null,
        BCNumber: "",
        BCType: "",
        BCDate: null,
        VehicleNo: "",
        Details: [],
      };
      this.dataDN = null;
      this.dataPO = null;
      this.errors = {};
    },
    submit() {
      if (this.mode === "add") this.create();
      // else this.update();
    },
    create() {
      this.model.IsManual = this.isManual;
      this.ds
        .create(this.model)
        .then((dt) => {
          console.log(dt);
          toastSuccess(dt.Message);
          this.$emit("submitted");
        })
        .catch((err) => {
          console.error(err);
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
  },
};
</script>
<style scoped>
.detail-content {
  /* padding-bottom: 5em; */
  height: 60%;
  max-height: 60%;
  overflow-y: scroll;
}

.detail-content-view {
  /* padding-bottom: 5em; */
  height: 70%;
  max-height: 70%;
  overflow-y: scroll;
}
</style>
