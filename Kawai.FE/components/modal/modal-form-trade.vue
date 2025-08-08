<template>
  <!-- BEGIN nav-pills -->
  <ul class="nav nav-pills mb-2">
    <li class="nav-item">
      <a href="#nav-pills-tab-1" data-bs-toggle="tab" class="nav-link active">
        <span class="d-sm-none">General</span>
        <span class="d-sm-block d-none">General</span>
      </a>
    </li>
    <li class="nav-item">
      <a href="#nav-pills-tab-2" data-bs-toggle="tab" class="nav-link">
        <span class="d-sm-none">Additional Info</span>
        <span class="d-sm-block d-none">Additional Info</span>
      </a>
    </li>
    <li class="nav-item">
      <a href="#nav-pills-tab-3" data-bs-toggle="tab" class="nav-link">
        <span class="d-sm-none">Others</span>
        <span class="d-sm-block d-none">Others</span>
      </a>
    </li>
  </ul>
  <!-- END nav-pills -->
  <!-- BEGIN tab-content -->
  <div class="tab-content p-3 rounded-top panel rounded-0 m-0">
    <!-- BEGIN tab-pane -->
    <div class="tab-pane fade active show" id="nav-pills-tab-1">
      <div class="row">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Trade Code</label>
            <input-text
              placeholder="Trade Code"
              v-model="model.Trade_Code"
              :disabled="mode === 'edit'"
              :errors="errors?.Trade_Code"
            />
          </div>
        </div>
        <div class="col-12 col-md-9">
          <div class="mb-3">
            <label class="form-label">Trade Name</label>
            <input-text
              placeholder="Trade Name"
              v-model="model.Trade_Name"
              :errors="errors?.Trade_Name"
            />
          </div>
        </div>
      </div>
      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Trade Cls</label>
            <input-cls
              type-data="Trade_Cls"
              placeholder="Search Trade Cls"
              class="form-control"
              v-model="model.Trade_Cls"
            />
          </div>
        </div>
        <div class="col-12 col-md-6">
          <div class="mb-3">
            <label class="form-label">Warehouse</label>
            <input-warehouse
              class="form-control"
              v-model="model.Subcon_WH_Code"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">SAP Code</label>
            <input-text
              placeholder="SAP Code"
              v-model="model.SAP_Code"
              :errors="errors?.SAP_Code"
            />
          </div>
        </div>
      </div>
      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Trade ABBR</label>
            <input-text
              placeholder="Trade ABBR"
              v-model="model.Trade_ABBR"
              :errors="errors?.Trade_ABBR"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Contact Person</label>
            <input-text
              placeholder="Contact Person"
              v-model="model.Contact_Person"
              :errors="errors?.Contact_Person"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Address 1</label>
            <input-text
              placeholder="Address 1"
              v-model="model.Address1"
              :errors="errors?.Address1"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Address 2</label>
            <input-text
              placeholder="Address 2"
              v-model="model.Address2"
              :errors="errors?.Address2"
            />
          </div>
        </div>
      </div>

      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">City</label>
            <input-text
              placeholder="City"
              v-model="model.City"
              :errors="errors?.City"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Postal Code</label>
            <input-text
              placeholder="Postal Code"
              v-model="model.Postal_Code"
              :errors="errors?.Postal_Code"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Country</label>
            <input-text
              placeholder="Country"
              v-model="model.Country"
              :errors="errors?.Country"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Country Cls</label>
            <input-cls
              type-data="Country_Cls"
              placeholder="Search Country Cls"
              class="form-control"
              v-model="model.Country_Cls"
            />
          </div>
        </div>
      </div>

      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Telephone</label>
            <input-text
              placeholder="Telephone"
              v-model="model.Telephone"
              :errors="errors?.Telephone"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Fax</label>
            <input-text
              placeholder="Fax"
              v-model="model.Fax"
              :errors="errors?.Fax"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Region Cls</label>
            <input-cls
              class="form-control"
              type-data="Region_Cls"
              placeholder="Search Region Cls"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">EPTE Cls</label>
            <input-cls
              class="form-control"
              type-data="Epte_Cls"
              placeholder="Search Epte Cls"
              v-model="model.Epte_Cls"
            />
          </div>
        </div>
      </div>

      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-1">
          <div class="mb-3">
            <label class="form-label">Closing Days</label>
            <input-text
              placeholder="Closing Day"
              v-model="model.Closing_Day"
              :errors="errors?.Closing_Day"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">Pay Days</label>
            <input-text
              placeholder="Pay Day"
              v-model="model.Pay_Day"
              :errors="errors?.Pay_Day"
            />
          </div>
        </div>
        <div class="col-12 col-md-1">
          <div class="mb-3">
            <label class="form-label">Invoice To</label>
            <input-text
              placeholder="Invoice To"
              v-model="model.Invoice_To"
              :errors="errors?.Invoice_To"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">Invoice Pay Days</label>
            <input-text
              placeholder="Invoice Pay Days"
              v-model="model.InvoicePay_Days"
              :errors="errors?.InvoicePay_Days"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">Insurance Covered</label>
            <input-cls
              type-data="Insurance_Cls"
              placeholder="Search Insurance Cls"
              class="form-control"
              v-model="model.Insurance_Cls"
            />
          </div>
        </div>
        <div class="col-12 col-md-1">
          <div class="mb-3">
            <label class="form-label">PO Cls</label>
            <input-cls
              class="form-control"
              v-model="model.PO_Cls"
              type-data="PO_Cls"
              placeholder="Search PO Cls"
            />
          </div>
        </div>
        <div class="col-12 col-md-1">
          <div class="mb-3">
            <label class="form-label">NG Cls</label>
            <input-cls
              class="form-control"
              v-model="model.NG_Cls"
              type-data="NG_Cls"
              placeholder="Search NG Cls"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">Affiliate Cls</label>
            <input-cls
              class="form-control"
              type-data="Affiliate_Cls"
              placeholder="Search Affiliate Cls"
              v-model="model.Affiliate_Cls"
            />
          </div>
        </div>
      </div>

      <!-- END -->
    </div>

    <!-- END tab-pane -->
    <!-- BEGIN tab-pane -->
    <div class="tab-pane fade" id="nav-pills-tab-2">
      <div class="row">
        <div class="col-12 col-md-4">
          <div class="mb-3">
            <label class="form-label">NPWP No</label>
            <input-text
              placeholder="NPWP No"
              v-model="model.NPWP_No"
              :errors="errors?.NPWP_No"
            />
          </div>
        </div>
      </div>
      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-4">
          <div class="mb-3">
            <label class="form-label">NPWP Name</label>
            <input-text
              placeholder="NPWP Name"
              v-model="model.NPWP_Name"
              :errors="errors?.NPWP_Name"
            />
          </div>
        </div>
        <div class="col-12 col-md-8">
          <div class="mb-3">
            <label class="form-label">NPWP Address</label>
            <input-text
              placeholder="NPWP Address"
              v-model="model.NPWP_Address"
              :errors="errors?.NPWP_Address"
            />
          </div>
        </div>
      </div>
      <div class="row" style="margin-top: -7px">
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">NPWP City</label>
            <input-text
              placeholder="NPWP City"
              v-model="model.NPWP_City"
              :errors="errors?.NPWP_City"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">NPPKP No</label>
            <input-text
              placeholder="NPPKP No"
              v-model="model.NPPKP_No"
              :errors="errors?.NPPKP_No"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">NITKU</label>
            <input-text
              placeholder="NITKU"
              v-model="model.NITKU"
              :errors="errors?.NITKU"
            />
          </div>
        </div>
      </div>

      <div class="row" style="margin-top: 15px">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Price Condition</label>
            <input-text
              placeholder="Price Condition"
              v-model="model.Price_Condition"
              :errors="errors?.Price_Condition"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Payment Terms</label>
            <input-paymentterm-cls
              class="form-control"
              v-model="model.POPayment_Terms"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Transportation Cls</label>
            <input-transportation-cls
              class="form-control"
              v-model="model.Transportation_Cls"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">Payment Days</label>
            <input-text
              placeholder="Payment Days"
              v-model="model.POPayment_Day"
              :errors="errors?.POPayment_Day"
            />
          </div>
        </div>
      </div>

      <!-- END -->
    </div>
    <!-- END tab-pane -->

    <div class="tab-pane fade" id="nav-pills-tab-3">
      <div class="row">
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">PO Case Mark 1</label>
            <input-text
              placeholder="PO Case Mark 1"
              v-model="model.POCaseMark1"
              :errors="errors?.POCaseMark1"
            />
          </div>
        </div>
        <div class="col-12 col-md-3">
          <div class="mb-3">
            <label class="form-label">PO Case Mark 2</label>
            <input-text
              placeholder="PO Case Mark 2"
              v-model="model.POCaseMark2"
              :errors="errors?.POCaseMark2"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Case Mark 3</label>
            <input-text
              placeholder="PO Case Mark 3"
              v-model="model.POCaseMark3"
              :errors="errors?.POCaseMark3"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Case Mark 4</label>
            <input-text
              placeholder="PO Case Mark 4"
              v-model="model.POCaseMark4"
              :errors="errors?.POCaseMark4"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Case Mark 5</label>
            <input-text
              placeholder="PO Case Mark 5"
              v-model="model.POCaseMark5"
              :errors="errors?.POCaseMark5"
            />
          </div>
        </div>
      </div>
      <div class="row" style="margin-top: 15px">
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Marking 1</label>
            <input-text
              placeholder="PO Marking 1"
              v-model="model.POMarking1"
              :errors="errors?.POMarking1"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Marking 2</label>
            <input-text
              placeholder="PO Marking 2"
              v-model="model.POMarking2"
              :errors="errors?.POMarking2"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Marking 3</label>
            <input-text
              placeholder="PO Marking 3"
              v-model="model.POMarking3"
              :errors="errors?.POMarking3"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Marking 4</label>
            <input-text
              placeholder="PO Marking 4"
              v-model="model.POMarking4"
              :errors="errors?.POMarking4"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Marking 5</label>
            <input-text
              placeholder="PO Marking 5"
              v-model="model.POMarking5"
              :errors="errors?.POMarking5"
            />
          </div>
        </div>

        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">PO Marking 6</label>
            <input-text
              placeholder="PO Marking 6"
              v-model="model.POMarking6"
              :errors="errors?.POMarking6"
            />
          </div>
        </div>
      </div>

      <div class="row" style="margin-top: 15px">
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">BC Type</label>
            <input-bctype-cls class="form-control" v-model="model.Type_BC" />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">Kode KPPBC</label>
            <input-text
              placeholder="Kode KPPBC"
              v-model="model.CODE_KPPBC"
              :errors="errors?.CODE_KPPBC"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">NO Izin</label>
            <input-text
              placeholder="No Izin"
              v-model="model.No_Izin"
              :errors="errors?.No_Izin"
            />
          </div>
        </div>
        <div class="col-12 col-md-2">
          <div class="mb-3">
            <label class="form-label">Tanggal</label>
            <input-date
              placeholder="No Izin Date"
              v-model="model.NoIzin_Date"
              :errors="errors?.NoIzin_Date"
            />
          </div>
        </div>
      </div>

      <!-- END -->
    </div>
  </div>
  <!-- END tab-content -->
  <div class="submit-wrapper">
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
      Trade_Code: "",
      Trade_Cls: "",
      Trade_Name: "",
      Trade_Abbr: "",
      Contact_Person: "",
      Address1: "",
      Address2: "",
      City: "",
      Country: "",
      Country_Cls: "",
      Epte_Cls: "",
      Region_Cls: "",
      Postal_Code: "",
      Telephone: "",
      Fax: "",
      Closing_Day: "",
      Pay_Day: "",
      InvoicePay_Days: "",
      Affiliate_Cls: "",
      Insurance_Cls: "",
      NPWP_No: "",
      NPWP_Name: "",
      NPWP_Address: "",
      NPWP_City: "",
      NPPKP_No: "",
      Invoice_To: "",
      PO_Cls: "",
      Price_Condition: "",
      POPayment_Day: "",
      POPayment_Terms: "",
      Transportation_Cls: "",
      POCaseMark1: "",
      POCaseMark2: "",
      POCaseMark3: "",
      POCaseMark4: "",
      POCaseMark5: "",
      POMarking1: "",
      POMarking2: "",
      POMarking3: "",
      POMarking4: "",
      POMarking5: "",
      POMarking6: "",
      Subcon_WH_Code: "",
      NG_Cls: "",
      SAP_Code: "",
      Type_BC: "",
      No_Izin: "",
      CODE_KPPBC: "",
      NoIzin_Date: "",
      NITKU: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useTrade();
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
        Trade_Code: "",
        Trade_Cls: "",
        Trade_Name: "",
        Trade_Abbr: "",
        Contact_Person: "",
        Address1: "",
        Address2: "",
        City: "",
        Country: "",
        Country_Cls: "",
        Epte_Cls: "",
        Region_Cls: "",
        Postal_Code: "",
        Telephone: "",
        Fax: "",
        Closing_Day: "",
        Pay_Day: "",
        InvoicePay_Days: "",
        Affiliate_Cls: "",
        Insurance_Cls: "",
        NPWP_No: "",
        NPWP_Name: "",
        NPWP_Address: "",
        NPWP_City: "",
        NPPKP_No: "",
        Invoice_To: "",
        PO_Cls: "",
        Price_Condition: "",
        POPayment_Day: "",
        POPayment_Terms: "",
        Transportation_Cls: "",
        POCaseMark1: "",
        POCaseMark2: "",
        POCaseMark3: "",
        POCaseMark4: "",
        POCaseMark5: "",
        POMarking1: "",
        POMarking2: "",
        POMarking3: "",
        POMarking4: "",
        POMarking5: "",
        POMarking6: "",
        Subcon_WH_Code: "",
        NG_Cls: "",
        SAP_Code: "",
        Type_BC: "",
        No_Izin: "",
        CODE_KPPBC: "",
        NoIzin_Date: "",
        NITKU: "",
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
