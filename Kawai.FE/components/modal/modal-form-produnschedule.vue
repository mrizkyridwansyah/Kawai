<template>
  <table style="width: 100%;">
    <tr >
      <td colspan="2" >
        <div class="alert warning">
          <label class="form-label" style="font-weight: bold;color: brown;">
            Setelah disubmit, hasil produksi akan terbentuk dan stock material akan dikurangi sesuai BOM.
          </label>

        </div>
      </td>
    </tr>
    <tr><td colspan="2">&nbsp;</td></tr>
    <tr>
      <td><label class="form-label">Line</label></td>
      <td style="padding-left: 15px">
        <div  class="col-md-12">
        <input-text v-model="filter.LineName" disabled />

        </div>
      </td>
    </tr>
     <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Parent Item</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <div  class="col-md-5">
            <input-text v-model="filter.ParentItemCode" disabled  />
        </div>
        
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Description</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
       <input-text v-model="filter.ParentItemName" disabled  />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Qty Input</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <div  class="col-md-5">
          <input-number v-model="filter.QtyInput"  />
        </div>
      </td>
    </tr>
     <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Production Date</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <div  class="col-md-12">
           <input-date v-model="filter.ProductionDate"  />
          
        </div>
      </td>
    </tr>
    <tr>
       <td style="padding-top: 5px">
        <label class="form-label">Remarks</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px;padding-bottom: 10px;">
        <input-text
                      multiline
                      v-model="filter.Remarks"
                      :errors="errors?.Remarks"
                    />
      </td>
    </tr>
    <tr style="border-top: 1px solid grey;">
      <td colspan="2">
        <label class="check-line">
            <input type="checkbox" v-model="isConfirmed">
              <span>Saya sudah memeriksa parent item, qty produksi, dan kecukupan stock material.</span>
            </input>
        </label>
      </td>
    </tr>

  </table>
  <div style="float: right" class="mt-4 mb-3">
    <v-button-submit
      :submit="submit"
      :disabled="submitDisabled"
      :is-loading="isLoading"
    />
  </div>
</template>
<style>
  .alert.warning
  {
      background: #f3d4a4;
      border-color: #f3d4a4;
      color: #8a4600;
  }

  .alert
  {
      align-items: flex-start;
      gap: 10px;
      padding: 12px 14px;
      border-radius: 8px;
      margin-bottom: 14px;
      border: 1px solid transparent;
      font-size: 12px;
  }

  .check-line {
      display: flex;
      align-items: flex-start;
      gap: 9px;
      margin-top: 16px;
      font-size: 13px;
  }
</style>
<script>
export default {
  emits: ["save"],
  props: {
    filter: {
      type: Object,
      required: true
    },
    id: String,
    btnDisabled: Boolean,
    mode: String
  },
  data: () => ({
    isLoading: false,
    isConfirmed: false,
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useProductionUnschedule();
    },
    submitDisabled() {
      return (this.btnDisabled ?? false) || !this.isConfirmed;
    }
  },
  mounted() {
    console.log("Modal mounted");
  },
  methods: {
    submit() {
      const data = {
        LineCode: this.filter.LineCode,
        ParentItemCode: this.filter.ParentItemCode,
        QtyInput: String(this.filter.QtyInput ?? ""),
        ProductionDate: this.$func.asUtcStringDateOnly(
          new Date(this.filter.ProductionDate)
        ),
        Remarks: this.filter.Remarks,
      };

      this.$emit("save", data);
      // this.$emit("submitted");
    }
      
    // save() {
    //   this.isLoading = true;

    //   const data = {
    //     LineCode: this.filter.LineCode,
    //     ParentItemCode: this.filter.ParentItemCode,
    //     QtyInput: String(this.filter.QtyInput ?? ""),
    //     ProductionDate: this.$func.asUtcStringDateOnly(
    //       new Date(this.filter.ProductionDate)
    //     ),
    //     Remarks: this.filter.Remarks,
    //   };

    //   this.ds
    //     .save(data)      // <-- bukan create()
    //     .then(() => {
    //       toastSuccess("Data saved successfully");
    //       this.$emit("submitted");
    //     })
    //     .catch((err) => {
    //       this.errors = err?.Errors;
    //       toastDanger(err?.Message);
    //     })
    //     .finally(() => {
    //       this.isLoading = false;
    //     });
    // }
  }
    
};
</script>
