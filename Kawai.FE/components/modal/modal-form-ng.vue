<template>
  <div class="mb-3">
    <label class="form-label">NG Code</label>
    <input-text
      placeholder="NG Code"
      v-model="model.NGCode"
      :disabled="mode === 'edit'"
      :errors="errors?.NGCode"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Description</label>
    <input-text
      placeholder="Description"
      v-model="model.Description"
      :errors="errors?.Description"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Common</label>
    <input-dropdown
      :options="[
        { value: true, text: 'Yes' },
        { value: false, text: 'No' },
      ]"
      textField="text"
      valueField="value"
      placeholder="Common"
      v-model="model.IsCommon"
      :errors="errors?.IsCommon"
    />
  </div>
  <div class="mt-4 mb-3">
    <v-button-submit
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      NGCode: "",
      Description: "",
      IsCommon: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useNG();
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
        NGCode: "",
        Description: "",
        IsCommon: "",
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      if (this.mode === "add") this.create();
      else this.update();
    },
    create: function () {
      this.ds
        .create(this.model)
        .then((datas) => {
          toastSuccess("success");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
    update: function () {
      this.ds
        .update(this.model)
        .then((datas) => {
          toastSuccess("success");
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
