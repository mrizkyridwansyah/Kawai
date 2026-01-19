<template>
  <div class="row">
    <div class="col-lg-12">
      <div class="mb-3">
        <label class="form-label">DN Number</label>
        <input-text v-model="model.DNNumber" :disabled="true" />
      </div>
      <div class="mb-3">
        <label class="form-label">Item Name</label>
        <input-text v-model="model.ItemName" :disabled="true" />
      </div>
      <div class="mb-3">
        <div class="row">
          <div class="col-6">
            <label class="form-label">Qty Sample</label>
            <input-money v-model="model.Qty" :disabled="true" />
          </div>
          <div class="col-6">
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
        style="color: white; width: 10em"
        @click="() => confirm('Rejected')"
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
        HOLD
      </button>
      <button
        class="btn btn-primary rounded-pill"
        type="button"
        style="color: white; width: 10em"
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
      QtyNG: null,
      Remarks: "",
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
          QtyNG: dt.Data.QtyNG,
          Remarks: dt.Data.Remarks,
          AttachmentID: dt.Data.AttachmentID,
          Attachment: null,
          AttachmentName: dt.Data.AttachmentFileName,
          AttachmentFileBase64: dt.Data.AttachmentFileBase64,
        };
      });
    },
    resetForm: function () {
      this.model = {
        InspectionId: null,
        DNNumber: "",
        ItemName: "",
        Qty: null,
        QtyNG: null,
        Remarks: "",
        AttachmentID: null,
        Attachment: null,
        AttachmentName: "",
        AttachmentFileBase64: null,
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
    confirm: function (inspectionResult) {
      let payload = {
        InspectionId: this.id,
        InspectionResult: inspectionResult,
      };

      this.ds
        .confirm(payload)
        .then((datas) => {
          toastSuccess("Transaction on process!");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
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
