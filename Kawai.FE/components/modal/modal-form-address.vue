<template>
  <table>
    <tr>
      <td><label class="form-label">Warehouse Code</label></td>
      <td style="padding-left: 15px" colspan="3">
        <input-warehouse
          class="form-control"
          v-model="model.WarehouseCode"
          disabled="true"
          style-code="width: 120px"
          style-desc="width: 250px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Area Code</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px" colspan="3">
        <input-area
          class="form-control"
          v-model="model.AreaCode"
          :warehouse="model.WarehouseCode"
          disabled="true"
          style-code="width: 150px"
          style-desc="width: 250px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Address Code</label>
      </td>
      <td style="padding-left: 15px; padding-top: 5px; width: 120px">
        <input-text
          placeholder="#AUTO"
          disabled="true"
          v-model="model.AddressCode"
          :errors="errors?.AddressCode"
          style="width: 120px"
        />
      </td>
      <td style="padding-left: 10px; padding-top: 5px">
        <label class="form-label">Address Name</label>
      </td>
      <td style="padding-left: 15px; padding-top: 5px">
        <input-text
          v-model="model.AddressName"
          :errors="errors?.AddressName"
          style="width: 420px"
          maxlength="200"
        />
      </td>
    </tr>
  </table>
  <div style="float: right;" class="mt-4 mb-3">
    <v-button-submit
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id", "warehouse", "area", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      WarehouseCode: "",
      AreaCode: "",
      AreaName: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useAddress();
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
    warehouse: function (val) {
      this.model.WarehouseCode = val;
    },
    area: function (val) {
      this.model.AreaCode = val;
    },
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
      if (this.mode === "edit") {
        this.model.AreaCode = this.model.AreaCode; // Jangan reset AreaCode di Edit
      } else {
        // Kosongkan form untuk mode Add
        this.model = {
          WarehouseCode: this.warehouse || "",
          AreaCode: this.area || "",
          AreaName: "",
        };
      }
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.model.WarehouseCode = this.warehouse;
      this.model.AreaCode = this.area;
      this.model.AddressCode = this.mode === "edit" ? this.id : "#AUTO";

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
