<template>
  <div>
    <div class="mb-3" style="display: none">
      <input-text v-model="model.Id" />
    </div>

    <div class="mb-3">
      <label class="form-label">QC Status</label>
      <input-dropdown
        :options="[
          { value: '1', text: 'NG' },
          { value: '2', text: 'Passed' },
        ]"
        textField="text"
        valueField="value"
        v-model="model.QC_Status"
      />
    </div>
    <div class="mb-3">
      <label class="form-label">Remarks</label>
      <input-text
        v-model="model.Remarks"
        :errors="errors?.Remarks"
      />
    </div>
  </div>
  <div>
    <div class="d-flex justify-content-end mt-4">
      <v-button-submit-modal
        :submit="submit"
        :label="Yes"
        :disabled="btnDisabled !== undefined && btnDisabled !== false"
        :is-loading="isLoading"
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
      Remarks: "",
      QC_Status: false,
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
    if (this.mode === "edit" && this.id) {
      this.loadDetail(this.id);
    } else if (this.mode === "add") {
      this.resetForm();
    }
  },
  watch: {
    mode: function (val) {
      if (val === "edit") {
        this.loadDetail(this.id);
      } else {
        this.resetForm();
      }
    },
  },
  methods: {
    loadDetail: function () {
      this.ds.loadDetail(this.id).then((dt) => (this.model = dt.Data));
    },
    resetForm: function () {
      // Kosongkan form untuk mode Add
      this.model = {
        Remarks: "",
        QC_Status: false,
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      if (this.mode === "add") this.create();
      else this.update();
    },

    update: function () {
      this.ds
        .update(this.model)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
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
