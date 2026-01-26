<template>
  <table>
    <tr>
      <td><label class="form-label">Code</label></td>
      <td style="padding-left: 15px">
        <input-text
          v-model="model.Code"
          :disabled="mode === 'edit'"
          :errors="errors?.Code"
          style="width: 200px;"
          maxlength="20"
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
       TableName: "",   // 🔥 WAJIB
      Code: "",
      Description: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useClassification();
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
    setTableName(tableName) {
    this.model.TableName = tableName;
  },
    loadDetail() {
  this.ds
    .loadDetail({
      id: this.id,
      tableName: this.model.TableName
    })
    .then(dt => {
      this.model = {
        ...dt.Data,
        TableName: this.model.TableName
      };
    });
},
    resetForm: function () {
      // Kosongkan form untuk mode Add
      this.model = {
         TableName: this.model.TableName || "",  // 🔥 PERTAHANKAN
       Code: "",
      Description: "" 
         
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
