<template>
  <div class="mb-3">
    <label class="form-label">Trade Code</label>
    <input-text
      placeholder="Trade Code"
      v-model="model.Trade_Code"
      :disabled="mode === 'add' || 'edit'"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Location Code</label>
    <input-text
      placeholder="Location Code"
      v-model="model.Location_Code"
      :errors="errors.Location_Code"
      :disabled="mode === 'edit'"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Location Name</label>
    <input-text
      placeholder="Location Name"
      v-model="model.Location_Name"
      :errors="errors.Location_Name"
    />
  </div>
  <div class="mt-4 mb-3">
    <v-button-submit-modal
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id", "trade_code", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      Trade_Code: "",
      Location_Code: "",
      Location_Name: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useDeliveryPlace();
    },
  },
  mounted: function () {
    if (this.mode === "edit" && this.id) {
      this.loadDetail(this.$route.query.trade_code, this.id);
    } else if (this.mode === "add") {
      this.resetForm();
    }
  },
  watch: {
    trade_code: function (val) {
      this.model.Trade_Code = val;
    },
    mode: function (val) {
      if (val === "edit") {
        this.loadDetail(this.$route.query.trade_code, this.id);
      } else {
        this.resetForm();
      }
    },
  },
  methods: {
    loadDetail: function () {
      this.ds
        .loadDetail(this.$route.query.trade_code, this.id)
        .then((dt) => (this.model = dt.Data));
    },
    resetForm: function () {
      if (this.mode === "edit") {
        this.model.Location_Code = this.model.Location_Code; // Jangan reset LocationCode di Edit
      } else {
        // Kosongkan form untuk mode Add
        this.model = {
          Trade_Code: this.$route.query.trade_code || "",
          Location_Code: "", // Kosongkan LocationCode
          Location_Name: "",
        };
      }
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.model.Trade_Code = this.$route.query.trade_code;

      if (this.mode === "add") this.create();
      else this.update();
    },
    create: function () {
      this.ds
        .create(this.model)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
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
