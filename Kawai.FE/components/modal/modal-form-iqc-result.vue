<template>
  <div class="row" style="max-height: 80vh; overflow-y: scroll">
    <div class="col-lg-12">
      <div class="mb-3">
        <label class="form-label">DN Number</label>
        <input-text v-model="model.DNNumber" :disabled="true" />
      </div>
      <div class="mb-3">
        <label class="form-label">Item Name</label>
        <input-text v-model="model.ItemName" :disabled="true" />
      </div>
      <div
        class="mb-3"
        v-if="mode == 'CONFIRM-SA' && this.model.ProcessUnapprove"
      >
        <div class="row">
          <div class="col-3">
            <label class="form-label">Qty Receipt</label>
            <input-money v-model="model.QtyReceipt" :disabled="true" />
          </div>
          <div class="col-3">
            <label class="form-label">Qty Sample</label>
            <input-money v-model="model.Qty" :disabled="true" />
          </div>
          <div class="col-3">
            <label class="form-label">Qty NG Total</label>
            <input-money v-model="model.TotalRealNG" :disabled="true" />
          </div>
          <div class="col-3">
            <label class="form-label">Qty Good</label>
            <input-money
              class="text-right"
              v-model="model.TotalGoodQty"
              :errors="errors?.TotalGoodQty"
            />
          </div>
        </div>
      </div>
      <div class="mb-3" v-else>
        <div class="row">
          <div class="col-4">
            <label class="form-label">Qty Receipt</label>
            <input-money v-model="model.QtyReceipt" :disabled="true" />
          </div>
          <div class="col-4">
            <label class="form-label">Qty Sample</label>
            <input-money v-model="model.Qty" :disabled="true" />
          </div>
          <div class="col-4">
            <label class="form-label">Qty NG</label>
            <input-money
              class="text-right"
              v-model="model.QtyNG"
              :errors="errors?.QtyNG"
              :disabled="mode != 'INPUT'"
            />
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label class="form-label">Remarks</label>
        <input-text
          v-model="model.Remarks"
          :errors="errors?.Remarks"
          multiline
          :disabled="mode != 'INPUT'"
        />
      </div>
      <div class="mb-3" v-if="mode == 'CONFIRM-SA' || mode == 'UNAPPROVE'">
        <label class="form-label">Remarks Unapprove</label>
        <input-text
          v-model="model.RemarksUnapprove"
          :errors="errors?.RemarksUnapprove"
          multiline
          :disabled="mode == 'CONFIRM-SA'"
        />
      </div>
      <div class="mb-3" v-if="mode == 'CONFIRM-SA'">
        <label class="form-label">Remarks SA</label>
        <input-text
          v-model="model.RemarksSA"
          :errors="errors?.RemarksSA"
          multiline
          :disabled="mode != 'CONFIRM-SA'"
        />
      </div>
    </div>
    <label class="form-label">Upload Evidence</label>
    <div class="col-lg-12" style="min-height: 20em">
      <input-image
        v-model="model.Attachment"
        :errors="errors?.Attachment"
        :initial-image="model.AttachmentFileBase64"
        :disabled="mode != 'INPUT'"
      />
    </div>
  </div>
  <div class="mt-4 mb-3" v-if="mode == 'INPUT'">
    <v-button-submit-modal
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
  <div class="mt-4 mb-3" v-else-if="mode == 'CONFIRM'">
    <div style="display: flex; justify-content: space-between">
      <button
        class="btn btn-danger rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => confirm('Rejected', 'Vendor')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        HOLD VENDOR
      </button>
      <button
        class="btn btn-danger rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => confirm('Rejected', 'Process')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        HOLD PROCESS
      </button>
      <button
        class="btn btn-green rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => confirm('SA')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        SA
      </button>
      <button
        class="btn btn-primary rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => confirm('Accepted')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        GOOD
      </button>
    </div>
  </div>
  <div class="mt-4 mb-3" v-else-if="mode == 'CONFIRM-SA'">
    <div style="display: flex; justify-content: space-between">
      <button
        class="btn btn-danger rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => approvalSA('Rejected', 'Vendor')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        HOLD VENDOR
      </button>
      <button
        class="btn btn-danger rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => approvalSA('Rejected', 'Process')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        HOLD PROCESS
      </button>
      <button
        class="btn btn-primary rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => approvalSA('Accepted')"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        GOOD
      </button>
    </div>
  </div>
  <div class="mt-4 mb-3" v-else-if="mode == 'UNAPPROVE'">
    <div style="display: flex; justify-content: center">
      <button
        class="btn btn-green rounded-pill"
        type="button"
        style="color: white; width: 12em"
        @click="() => cancelConfirm()"
        :disabled="
          (btnDisabled !== undefined && btnDisabled !== false) ||
          isLoading !== false
        "
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        >
          <span class="visually-hidden">Loading...</span>
        </div>
        SA
      </button>
    </div>
  </div>
</template>
<script>
export default {
  props: ["id", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      InspectionId: null,
      DNNumber: "",
      ItemName: "",
      Qty: null,
      QtyReceipt: null,
      QtyNG: null,
      TotalRealNG: null,
      InspectionResult: "",
      Remarks: "",
      RemarksSA: "",
      AttachmentID: null,
      Attachment: null,
      AttachmentName: "",
      AttachmentFileBase64: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useQualityCheck();
    },
  },
  mounted: function () {
    this.resetForm();
    this.$nextTick(() => setTimeout(() => this.loadDetail(), 500));
  },
  watch: {
    mode: function (val) {
      this.resetForm();
      this.$nextTick(() => setTimeout(() => this.loadDetail(), 500));
    },
  },
  methods: {
    loadDetail: function () {
      this.ds.loadDetail(this.id).then((dt) => {
        this.model = {
          InspectionId: dt.Data.InspectionId,
          DNNumber: dt.Data.DNNumber,
          ItemName: dt.Data.ItemName,
          Qty: dt.Data.Qty,
          QtyReceipt: dt.Data.QtyReceipt,
          QtyNG: dt.Data.QtyNG,
          TotalRealNG: dt.Data.TotalRealNG,
          TotalGoodQty: dt.Data.QtyGoodSA,
          InspectionResult: dt.Data.InspectionResult,
          Remarks: dt.Data.Remarks,
          RemarksSA: dt.Data.RemarksSA,
          RemarksUnapprove: dt.Data.RemarksUnapprove,
          AttachmentID: dt.Data.AttachmentID,
          // Attachment: null,
          AttachmentName: dt.Data.AttachmentFileName,
          AttachmentFileBase64: dt.Data.AttachmentFileBase64,
          ProcessUnapprove: dt.Data.ProcessUnapprove,
        };
      });
    },
    resetForm: function () {
      this.model = {
        InspectionId: null,
        DNNumber: "",
        ItemName: "",
        Qty: null,
        QtyReceipt: null,
        QtyNG: null,
        TotalRealNG: null,
        TotalGoodQty: null,
        InspectionResult: "",
        Remarks: "",
        RemarksSA: "",
        RemarksUnapprove: "",
        AttachmentID: null,
        Attachment: null,
        AttachmentName: "",
        AttachmentFileBase64: null,
        ProcessUnapprove: false,
      };
      this.errors = {};
    },
    submit: function () {
      let data = new FormData();
      data.append("Mode", this.mode);
      data.append("InspectionId", this.model.InspectionId);
      data.append("QtyNG", this.model.QtyNG);
      data.append("Remarks", this.model.Remarks);

      if (this.model.AttachmentID)
        data.append("AttachmentID", this.model.AttachmentID);

      if (this.model.AttachmentName)
        data.append("AttachmentName", this.model.AttachmentName);

      if (this.model.Attachment) {
        data.append("Attachment", this.model.Attachment);
      }

      this.save(data);
    },
    save: function (data) {
      this.ds
        .save(data)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
    confirm: function (inspectionResult, typeHold = "") {
      confirmSubmit(
        () =>
          new Promise((resolve) => {
            let payload = {
              InspectionId: this.id,
              InspectionResult: inspectionResult,
              TypeHold: typeHold ?? "",
            };

            this.ds
              .confirm(payload)
              .then((datas) => {
                if (datas.Code == 200) toastSuccess("Data saved successfully!");
                else toastInfo("Transaction on process!");

                this.$emit("submitted");
                resolve();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        `If you submit <strong>${inspectionResult.toUpperCase()} ${typeHold.toUpperCase()}</strong>, you CAN'T recover it. Are you sure to <strong>SUBMIT</strong> the data?`,
      );
    },
    approvalSA: function (inspectionResult, typeHold = "") {
      confirmSubmit(
        () =>
          new Promise((resolve) => {
            let payload = {
              InspectionId: this.id,
              InspectionResult: inspectionResult,
              RemarksSA: this.model.RemarksSA,
              TotalGoodQty: this.model.TotalGoodQty,
              ProcessUnapprove: this.model.ProcessUnapprove,
              TypeHold: typeHold ?? "",
            };

            this.ds
              .approvalSA(payload)
              .then((datas) => {
                let msg =
                  datas.Code == 200
                    ? "Data saved successfully!"
                    : "Transaction on process!";
                toastSuccess(msg);
                this.$emit("submitted");
                resolve();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        `If you submit <strong>${inspectionResult.toUpperCase()}</strong>, you CAN'T recover it. Are you sure to <strong>SUBMIT</strong> the data?`,
      );
    },
    cancelConfirm: function () {
      confirmSubmit(
        () =>
          new Promise((resolve) => {
            let payload = {
              InspectionId: this.id,
              RemarksUnapprove: this.model.RemarksUnapprove,
            };

            this.ds
              .cancelConfirm(payload)
              .then((datas) => {
                let msg =
                  datas.Code == 200
                    ? "Data saved successfully!"
                    : "Transaction on process!";
                toastSuccess(msg);
                this.$emit("submitted");
                resolve();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        `If you submit <strong>SA</strong>, this data need <strong>APPROVAL SA</strong> and you <strong>CAN'T</strong> recover it. Are you sure to <strong>SUBMIT</strong> the data?`,
      );
    },
  },
};
</script>

<style scoped>
.image-input-wrapper {
  width: 100%;
  height: 100%;
  max-height: 400px;
}
</style>
