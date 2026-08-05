<template>
  <table style="width: 100%;">
    <tr>
      <td><label class="form-label">Prod Result ID</label></td>
      <td>
        <div  class="col-md-6">
          <input-text v-model="model.ProdResultID" disabled />
        </div>
      </td>
    </tr>
    <tr><td colspan="2" style="padding-bottom: 20px;"></td></tr>
    
  </table>
   <table class="table table-bordered table-sm">
    <thead>
      <tr style="background-color: #8ec5fc;">
        <th style="vertical-align: middle;text-align: center;">No</th>
        <th style="vertical-align: middle;text-align: center;">Barcode</th>
        <th style="vertical-align: middle;text-align: center;">Qty</th>
      </tr>
    </thead>

    <tbody>
      <tr
        v-for="(item,index) in barcodeList"
        :key="index"
      >
        <td class="text-center">{{ index + 1 }}</td>
        <td>{{ item.BarcodeNo }}</td>
        <td class="text-right">{{ item.Qty }}</td>
      </tr>
    </tbody>
  </table>
</template>

<script>
export default {
  // props: {
  //   filter: {
  //     type: Object,
  //     required: true
  //   },
  // },
  props: ["id"], 
  data: () => ({
    isLoading: false,
    model: {
      ProdResultID:null,
    },
    barcodeList: [],
    errorResponse: {},
    errors: {},
  }),
  
  computed: {
    ds() {
      return useProductionUnschedule();
    },
  },
  // mounted(){
    
  //   this.loadData();
  // },
  // mounted: function () {
  
  //   this.$nextTick(() => setTimeout(() => this.loadData(), 100));
  // },
  // watch: {
  //   mode: function () {
     
  //     this.$nextTick(() => setTimeout(() => this.loadData(), 100));
  //   },
  // },
   methods: {
    loadData() {
      if (!this.id) return;
      //this.ds.setFilter(this.filter,"",filterBarcode);
      this.ds
        .loadBarcode(this.id)
        .then((dt) => {
          debugger
          this.model = {ProdResultID:dt.Data[0].ProdResultID};
          this.barcodeList = dt.Data;
        })
        .catch((err) => {
          toastDanger(err?.Message);
        });
    }
  }
 
};
</script>
