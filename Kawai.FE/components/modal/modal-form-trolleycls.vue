<template>
  <table>
    <tr>
      <td><label class="form-label">Trolley Cls</label></td>
      <td style="padding-left: 15px">
        <input-text
          v-model="model.Trolley_Cls"
          :disabled="mode === 'edit'"
          :errors="errors?.Trolley_Cls"
          style="width: 100px"
          maxlength="15"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Description</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <input-text
          v-model="model.Description"
          :errors="errors?.Description"
          style="width: 250px"
          maxlength="100"
        /> 
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Qty</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
       <input-money placeholder="Qty " v-model="model.Qty" style="width: 110px" />
      </td>
    </tr>
 
  </table>
  <div style="float: right" class="mt-4 mb-3">
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
      Qty: 0,
      Description: "",
      Trolley_Cls: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useTrolleyCls();
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
        Qty: 0,
        Description: "",
        Trolley_Cls: "",
        
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
          toastSuccess("Data saved successfully");
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
          toastSuccess("Data saved successfully");
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
