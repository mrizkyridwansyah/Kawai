<template>
  <table>
    <tr>
      <td><label class="form-label">Stop Point Code</label></td>
      <td style="padding-left: 15px">
        <input-text
          v-model="model.StopPointCode"
          :disabled="mode === 'edit'"
          :errors="errors?.StopPointCode"
          style="width: 200px;"
          maxlength="25"
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
          style="width: 350px;"
          maxlength="400"
        />
      </td>
    </tr>
      <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Picking Sequence</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
       <input-money
                        style="width: 50px"
                        v-model="model.PickingSeq"
                        :errors="errors?.PickingSeq"
       />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Active</label>
      </td>
      <td
        style="
          padding-top: 5px;
          padding-left: 15px;
          white-space: nowrap !important;
        "
      >
        <input-dropdown
          :options="[
            { value: true, text: 'Yes' },
            { value: false, text: 'No' },
          ]"
          textField="text"
          valueField="value"
          v-model="model.IsActive"
          :errors="errors?.IsActive"
          style="width: 100px;"
        />
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
      StopPointCode: "",
      Description: "",
      IsActive: null,
      PickingSeq: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useStopPoint();
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
          StopPointCode: "",
          Description: "",
          IsActive: null,
          PickingSeq: null,
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
