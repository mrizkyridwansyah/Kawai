<template>
  <table width="100%">
  <tr  style="height: 38px;">
    <td style="width: 10%;"><label class="form-label">From Factory</label></td>
    <td style="width: 34%;">
      <filter-factory-privileges
                  class="form-control"
                   :disabled="true"
                  v-model= "filter.fromfactory"
                  style-code="width: 110px"
                    style-desc="width: 250px"
                /></td>
     <td style="width: 1%;"></td>
    <td style="width: 10%;"><label class="form-label">To Factory</label></td>
    <td style="width: 34%;"> <filter-factory-privileges
                  class="form-control"
                  v-model="filter.tofactory"
                  style-code="width: 110px"
                    style-desc="width: 250px"
                /> </td>
   
     <td style="width: 10%;"></td>
  </tr>
  <tr  style="height: 38px">
    <td style="width: 10%;"><label class="form-label">From Process</label></td>
    <td style="width: 34%;">
      <filter-trade-2
                    class="form-control"
                    placeholder=" "
                     :disabled="true"
                    v-model="filter.fromprocess"
                    
                    :trade-cls="['1']"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  /></td>
     <td style="width: 1%;"></td>
    <td style="width: 10%;"><label class="form-label">To Process</label></td>
    <td style="width: 34%;"> <filter-trade-2
                    class="form-control"
                    placeholder=" "
                    v-model="filter.tosupplier"
                    
                    :trade-cls="['1']"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  /></td>
   
     <td style="width: 10%;"></td>
  </tr>
  <tr  style="height: 38px">
    <td style="width: 10%;"><label class="form-label">From Line</label></td>
    <td style="width: 34%;">
      <filter-line-factory
       class="form-control"
                       :company="filter.fromfactory"
                      :manufacture="filter.fromprocess"
                         :disabled="true"
                      v-model="filter.fromline"
                      style-code="width: 110px"
                        style-desc="width: 250px"
                    /></td>
     <td style="width: 1%;"></td>
    <td style="width: 10%;"><label class="form-label">To Line</label></td>
    <td style="width: 34%;">  <filter-line-factory
       class="form-control"
                      :company="filter.tofactory"
                      :manufacture="filter.tosupplier"
                      v-model="filter.tolinecode"
                      style-code="width: 110px"
                        style-desc="width: 250px"
                    /> </td>
   
     <td style="width: 10%;"></td>
  </tr>
      </table>
  <div style="float: right" class="mt-4 mb-3">
    <v-button-submit
       :submit="submit" 
      label="Copy"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id","btnDisable","mode","factory","process","line","item"]
 
,
  data: () => ({
    isLoading: false,
    filter: {
      
       fromfactory: null,
       fromprocess: null,
       fromline: null,
       tofactory: null,
       tosupplier: null,
       tolinecode: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useBOMWorkstation();
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
       tofactory: null,
       tosupplier: null,
       tolinecode: null,
      };
    this.filter.fromprocess=this.process;
    this.filter.fromfactory= this.factory;
    this.filter.tofactory= this.factory;
    this.filter.fromline = this.line;
    this.errors = {}; // Reset errors
    },
    submit: function () {
      this.copydata();
     
    },
    copydata: function () {
      if (!this.filter.tosupplier) {
        toastWarning("Please select process!");
        return;
      }
      if (!this.filter.tolinecode) {
        toastWarning("Please select line!");
        return;
      }
     const payload = {
         FromLine: this.filter.fromline,
         ToLine: this.filter.tolinecode,
         ItemCode: this.item,
     };
      this.ds
        .copydata(payload)
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
