<template>
  <table>
  
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Stop Point</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <filter-stoppoint
          style-code="width: 140px"
              style-desc="width: 240px"
          v-model="filter.stoppoint"
        />
      </td>
    </tr>
  </table>
  <div style="float: right" class="mt-4 mb-3">
    <v-button-submit
      :submit="submit"
      label="Setting"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: {
  id: String,
  btnDisabled: Boolean,
  mode: String,
  dataSetting: {
    type: Array,
    default: () => []
  }
     },
  data: () => ({
    isLoading: false,
    filter: {
      
      stoppoint: null,
       
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
      this.resetForm();
  },
  watch: {
    mode: function (val) {
        this.resetForm();    
    },
  },
  methods: {
    resetForm: function () {
       this.filter = {
        stoppoint: "",
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.settingdata();
     
    },
    settingdata: function () {
     const payload = {
          StopPoint: this.filter.stoppoint,
          Address: this.dataSetting
     };
      this.ds
        .settingdata(payload)
        .then(() => {
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
