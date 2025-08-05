<template>
  <div class="mb-3">
    <label class="form-label">Item Code</label>
    <input-text
      placeholder="Item Code"
      v-model="model.ItemCode"
      :disabled="mode === 'edit'"
      :errors="errors?.ItemCode"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Item Name</label>
    <input-text
      placeholder="Item Name"
      v-model="model.ItemName"
      :errors="errors?.ItemName"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Item Group</label>
    <input-item-group
      placeholder="Item Group"
      v-model="model.ItemGroup"
      :errors="errors?.ItemGroup"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Item Cls</label>
    <input-item-cls
      placeholder="Item Cls"
      v-model="model.ItemCls"
      :errors="errors?.ItemCls"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Item Type</label>
    <input-item-type
      placeholder="Item Type"
      v-model="model.ItemTypeCode"
      :errors="errors?.ItemTypeCode"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Brand</label>
    <input-brand
      placeholder="Brand"
      v-model="model.BrandCode"
      :errors="errors?.BrandCode"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Unit Classification</label>
    <input-uom
      placeholder="Unit Classification"
      v-model="model.UnitClassificationCode"
      :errors="errors?.UnitClassificationCode"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Qty Packing</label>
    <input-money
      placeholder="Qty Packing"
      v-model="model.QtyPacking"
      :errors="errors?.QtyPacking"
    />
  </div>
  <div class="mb-3">
    <div class="col-6">
      <input-checkbox label="Control Stock" v-model="model.StockControlCls" />
    </div>
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
      ItemCode: "",
      ItemName: "",
      ItemGroup: "",
      ItemCls: "",
      ItemTypeCode: "",
      BrandCode: "",
      UnitClassificationCode: "",
      QtyPacking: 0,
      StockControlCls: false,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useItem();
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
        ItemCode: "",
        ItemName: "",
        ItemGroup: "",
        ItemCls: "",
        ItemTypeCode: "",
        BrandCode: "",
        UnitClassificationCode: "",
        QtyPacking: 0,
        StockControlCls: false,
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
