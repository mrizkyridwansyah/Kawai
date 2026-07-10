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
        <label class="form-label">Line Code</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px" colspan="5">
        <input-text
          v-model="model.LineCode"
          :disabled="mode === 'edit'"
          :errors="errors?.LineCode"
          :maxlength="15"
          style="width: 130px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Line Name</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px" colspan="5">
        <input-text
          v-model="model.LineName"
          :errors="errors?.LineName"
          :disabled="mode === 'edit'"
          :maxlength="50"
          style="width: 370px"
        />
      </td>
    </tr>
       <tr>
             <td style="padding-top: 5px">
             <label class="form-label">IP Printer</label></td>
           <td style="padding-left: 15px;padding-top: 4px" colspan="7">
             <input-ip
               class="form-control"
               v-model="model.IPPrinter"
                style-code="width: 140px"
               style-desc="width: 270px"
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
      LineCode: "",
      LineName: "",
      IPPrinter: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useManufactureLine();
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
        LineCode: "",
        LineName: "",
        IPPrinter: "",
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.model.FactoryCode = this.factory;
       this.update();
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
