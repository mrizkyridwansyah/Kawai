<template>
  <div class="row" style="max-height: 80vh; overflow-y: scroll">
    <div class="col-lg-12">
      <div class="mb-3">
        <label class="form-label">DN Number xxx</label>
        <input-text v-model="model.DNNumber" :disabled="true" />
      </div>
      <div class="mb-3">
        <label class="form-label">Item Name</label>
        <input-text v-model="model.ItemName" :disabled="true" />
      </div>
      <div class="mb-3">
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
            <label class="form-label">Qty NG Input</label>
            <input-money v-model="model.QtyNG" :disabled="true" />
          </div>
        </div>
      </div>
      <div class="mb-3">
        <div class="row">
          <div class="col-4" v-if="this.model.ProcessUnapprove || this.model.InspectionResult == 'SA'">
            <label class="form-label">Qty NG Total (After)</label>
            <input-money v-model="model.TotalRealNG" :disabled="true" />
          </div>
          <div class="col-4" v-if="this.model.InspectionResult == 'SA'">
            <label class="form-label">Qty Good (SA)</label>
            <input-money
              class="text-right"
              v-model="model.TotalGoodQty"
              :errors="errors?.TotalGoodQty"
              :disabled="true"
            />
          </div>
        </div>
      </div>
      <div class="mb-3">
        <label class="form-label">Remarks QC Input</label>
        <input-text
          v-model="model.Remarks"
          :errors="errors?.Remarks"
          multiline
          :disabled="true"
        />
      </div>
      <div class="mb-3" v-if="this.model.ProcessUnapprove">
        <label class="form-label">Remarks Unapprove</label>
        <input-text
          v-model="model.RemarksUnapprove"
          :errors="errors?.RemarksUnapprove"
          multiline
          :disabled="true"
        />
      </div>
      <div class="mb-3" v-if="this.model.InspectionResult == 'SA'">
        <label class="form-label">Remarks SA</label>
        <input-text
          v-model="model.RemarksSA"
          :errors="errors?.RemarksSA"
          multiline
          :disabled="true"
        />
      </div>
    </div>
    <label class="form-label">Upload Evidence</label>
    <div class="col-lg-12" style="min-height: 20em">
      <input-image
        v-model="model.Attachment"
        :errors="errors?.Attachment"
        :initial-image="model.AttachmentFileBase64"
        :disabled="true"
      />
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
