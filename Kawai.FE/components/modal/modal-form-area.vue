<template>
  <table>
    <tr>
      <td><label class="form-label">Warehouse</label></td>
      <td style="padding-left: 15px" colspan="7">
        <input-warehouse
          class="form-control"
          v-model="model.WarehouseCode"
          disabled="true"
          style-code="width: 120px"
          style-desc="width: 270px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Area Code</label>
      </td>
      <td style="padding-left: 15px; padding-top: 5px; width: 120px">
        <input-text
          placeholder=""
          :disabled="mode === 'edit'"
          v-model="model.AreaCode"
          :errors="errors?.AreaCode"
          style="width: 120px"
        />
      </td>
      <td style="padding-left: 10px; padding-top: 5px">
        <label class="form-label">Area Name</label>
      </td>
      <td style="padding-left: 15px; padding-top: 5px" colspan="5">
        <input-text
          v-model="model.AreaName"
          :errors="errors?.AreaName"
          style="width: 470px"
          maxlength="200"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Item Type</label>
      </td>
      <td
        style="padding-left: 15px; padding-top: 5px; width: 270px"
        colspan="2"
      >
        <input-cls
          type-data="ClasificationPart_Cls"
          v-model="model.ItemType"
          :errors="errors?.ItemType"
          style-code="width: 120px"
          style-desc="width: 150px"
        />
      </td>
      <td style="padding-left: 15px; padding-top: 5px; width: 120px">
        <label class="form-label">Picking Sequence</label>
      </td>
      <td style="padding-left: 15px; padding-top: 5px; width: 50px" colspan="2">
        <input-number
          v-model="model.PickingSequence"
          min="1"
          :errors="errors?.PickingSequence"
          style="width: 10px !important"
        />
      </td>
      <td colspan="2" style="width: 150px">&nbsp;</td>
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
  props: ["id", "warehouse", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      WarehouseCode: "",
      AreaCode: "",
      AreaName: "",
      ItemType: "",
      PickingSequence: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useArea();
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
          AreaCode: "", // Kosongkan AreaCode
          AreaName: "",
          ItemType: "",
          PickingSequence: null,
        };
      }
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.model.WarehouseCode = this.warehouse;
      //input area code tidak otomotatis lagi
      //this.model.AreaCode = this.mode === "edit" ? this.id : "#AUTO";

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
