<template>
  <table>
    <tr>
      <td><label class="form-label">Factory</label></td>
      <td style="padding-left: 15px" colspan="5">
        <input-factory
          class="form-control"
          v-model="model.FactoryCode"
          disabled="true"
          style-code="width: 110px"
          style-desc="width: 250px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Warehouse Code</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px" colspan="5">
        <input-text
          placeholder="Warehouse Code"
          v-model="model.WarehouseCode"
          :disabled="mode === 'edit'"
          :errors="errors?.WarehouseCode"
          :maxlength="15"
          style="width: 130px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Warehouse Name</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px" colspan="5">
        <input-text
          placeholder="Warehouse Name"
          v-model="model.WarehouseName"
          :errors="errors?.WarehouseName"
          :disabled="mode === 'edit'"
          :maxlength="50"
          style="width: 370px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Adm Group</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px" colspan="5">
        <input-trade
          placeholder="Adm Group"
          v-model="model.AdmGroup"
          :trade-cls="['1', '2', '3']"
          :errors="errors?.AdmGroup"
          style-code="width: 130px"
          style-desc="width: 250px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Use End Date</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <input-date
          v-model="model.UseEndDate"
          :errors="errors?.UseEndDate"
          style-date="width: 120px"
        />
      </td>
      <td style="padding-top: 5px;">
        <label class="form-label">Stock Cls</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <input-cls
          type-data="ItemStockControlCls"
          placeholder="Stock Cls"
          v-model="model.StockControlCls"
          :errors="errors?.StockControlCls"
          style-code="width: 100px"
          style-desc="width: 50px"
        />
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <label class="form-label">NG Cls</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <input-cls
          type-data="NGCls"
          placeholder="NG Cls"
          v-model="model.NGCls"
          :errors="errors?.NGCls"
          style-code="width: 100px"
          style-desc="width: 50px"
        />
      </td>
    </tr>
  </table>
  <div style="float: right" class="mt-6">
    <v-button-submit
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id", "factory", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      FactoryCode: "",
      WarehouseCode: "",
      WarehouseName: "",
      AdmGroup: "",
      UseEndDate: null,
      StockControlCls: "",
      NGCls: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useWarehouse();
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
    factory: function (val) {
      this.model.FactoryCode = val;
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
      this.ds.loadDetail(this.id).then((dt) => {
        this.model = dt.Data;
      });
    },
    resetForm: function () {
      // Kosongkan form untuk mode Add
      this.model = {
        FactoryCode: this.factory || "",
        WarehouseCode: "",
        WarehouseName: "",
        AdmGroup: "",
        UseEndDate: null,
        StockControlCls: "",
        NGCls: "",
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.model.FactoryCode = this.factory;
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
