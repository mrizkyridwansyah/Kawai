<template>
  <table>
    <tr>
      <td><label class="form-label">Model Cls</label></td>
      <td style="padding-left: 15px">
        <input-text
          v-model="model.Model_Cls"
          :disabled="mode === 'edit'"
          :errors="errors?.Model_Cls"
          style="width: 130px"
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
          maxlength="25"
        /> 
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Cycle Time (menit/PCS)</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
       <input-money placeholder="CycleTime " v-model="model.CycleTime" :errors="errors?.CycleTime" style="width: 110px" />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px" colspan="2">
       <input-image
         v-model="model.ImageAttachment"
        :initial-image="model.ImageBase64"
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
      CycleTime: 0,
      Description: "",
      Model_Cls: "",
      ImageAttachment: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useModelCls();
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
            CycleTime: 0,
            Description: "",
            Model_Cls: "",
            ImageAttachment: null,
        
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
       this.update();
    },

    update: function () {
      let data = new FormData();
      data.append("Model_Cls", this.model.Model_Cls);
      data.append("Description", this.model.Description);
      data.append("CycleTime", this.model.CycleTime);
      data.append("ImageName", this.model.ImageName);

      if (this.model.ImageAttachment) {
        data.append("ImageAttachment", this.model.ImageAttachment);
      }

      this.ds
        .update(data)
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
