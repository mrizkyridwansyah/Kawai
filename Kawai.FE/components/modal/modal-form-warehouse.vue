<template>
  <div>
    <div class="mb-3">
      <label class="form-label">Warehouse Code</label>
      <input-text
        placeholder="Warehouse Code"
        v-model="model.WarehouseCode"
        :disabled="mode === 'edit'"
        :errors="errors?.WarehouseCode"
      />
    </div>
    <div class="mb-3">
      <label class="form-label">Warehouse Name</label>
      <input-text
        placeholder="Warehouse Name"
        v-model="model.WarehouseName"
        :errors="errors?.WarehouseName"
      />
    </div>
    <div class="mb-3">
      <label class="form-label">Adm Group</label>
      <input-text
        placeholder="Adm Group"
        v-model="model.AdmGroup"
        :errors="errors?.AdmGroup"
      />
    </div>
    <div class="mb-4">
      <label class="form-label">Use End Date</label>
      <input-date v-model="model.UseEndDate" :errors="errors?.UseEndDate" />
    </div>
    <div class="mb-3">
      <div class="row">
        <div class="col-sm-6">
          <input-checkbox
            label="Stock Control Cls"
            v-model="model.StockControlCls"
            :errors="errors?.StockControlCls"
          />
        </div>
        <div class="col-sm-6">
          <input-checkbox
            label="NG Cls"
            v-model="model.NGCls"
            :errors="errors?.NGCls"
          />
        </div>
      </div>
    </div>
  </div>
  <div>
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
      WarehouseCode: "",
      WarehouseName: "",
      AdmGroup: "",
      UseEndDate: null,
      StockControlCls: false,
      NGCls: false,
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
        WarehouseCode: "",
        WarehouseName: "",
        AdmGroup: "",
        UseEndDate: null,
        StockControlCls: false,
        NGCls: false,
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      if (this.mode === "add") this.create();
      else this.update();
    },
    create: function () {
      this.model.StockControlCls = this.model.StockControlCls ? "01" : "02";
      this.model.NGCls = this.model.NGCls ? "01" : "02";
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
      this.model.StockControlCls = this.model.StockControlCls ? "01" : "02";
      this.model.NGCls = this.model.NGCls ? "01" : "02";
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

