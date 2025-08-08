<template>
  <div class="mb-3">
    <label class="form-label">Item Cls</label>
    <input-cls
    type-data="ItemFinishGoodCls"
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
    <label class="form-label">Item</label>
    <input-item
      placeholder="Item"
      v-model="model.ChildItemCode"
      :brand="model.BrandCode"
      :item-type="model.ItemTypeCode"
      :item-cls="model.ItemCls"
      :errors="errors?.ChildItemCode"
    />
  </div>
  <div class="mb-3">
    <label class="form-label">Qty</label>
    <input-money placeholder="Qty " v-model="model.Qty" :errors="errors?.Qty" />
  </div>
  <div class="mb-3">
    <label class="form-label">Unit Classification</label>
    <input-uom
      placeholder="Unit Classification"
      v-model="model.UnitClsCode"
      :errors="errors?.UnitClsCode"
    />
  </div>
  <div class="row">
    <div class="col-md-6 col-lg-6 col-6">
      <label class="form-label">Start Date</label>
      <input-date
        placeholder="Start Date"
        v-model="model.StartDate"
        :errors="errors?.StartDate"
      />
    </div>
    <div class="col-md-6 col-lg-6 col-6">
      <label class="form-label">End Date</label>
      <input-date
        placeholder="End Date"
        v-model="model.EndDate"
        :errors="errors?.EndDate"
      />
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
  props: ["dataSelected", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      ParentItemCode: "",
      ItemCls: "",
      ItemTypeCode: "",
      BrandCode: "",
      ChildItemCode: "",
      Qty: 0,
      UnitClsCode: "",
      StartDate: null,
      EndDate: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useBom();
    },
  },
  mounted: function () {
    if (this.mode === "edit" && this.dataSelected.childItemCode) {
      this.loadDetail(
        this.dataSelected.parentItemCode,
        this.dataSelected.childItemCode,
        this.dataSelected.startDate
      );
    } else if (this.mode === "add") {
      this.resetForm();
    }
  },
  watch: {
    mode: function (val) {
      if (val === "edit") {
        this.loadDetail(
          this.dataSelected.parentItemCode,
          this.dataSelected.childItemCode,
          this.dataSelected.startDate
        );
      } else {
        this.resetForm();
      }
    },
  },
  methods: {
    loadDetail: function () {
      this.ds
        .loadDetail(
          this.dataSelected.parentItemCode,
          this.dataSelected.childItemCode,
          this.dataSelected.startDate
        )
        .then((dt) => (this.model = dt.Data));
    },
    resetForm: function () {
      // Kosongkan form untuk mode Add
      this.model = {
        ParentItemCode: "",
        ItemCls: "",
        ItemTypeCode: "",
        BrandCode: "",
        ChildItemCode: "",
        Qty: 0,
        UnitClsCode: "",
        StartDate: null,
        EndDate: null,
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.model.ParentItemCode = this.dataSelected.parentItemCode;
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
      this.model.PrevStartDate = this.dataSelected.startDate;
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
