<template>
  <v-frame title="Trade Master" icon="database">
    <template #frame-content>
      <ul
        class="nav nav-tabs nav-tabs-inverse"
        style="background-color: rgb(0, 32, 96) !important"
      >
        <li class="nav-item">
          <a href="#general-tab" data-bs-toggle="tab" class="nav-link active">
            General
          </a>
        </li>
        <li class="nav-item">
          <a href="#other-tab" data-bs-toggle="tab" class="nav-link">
            Others
          </a>
        </li>
      </ul>
      <div class="tab-content rounded-bottom p-3 card rounded-0 border-0">
        <!-- BEGIN tab-pane -->
        <div class="tab-pane fade active show" id="general-tab">
          <div class="row g-2">
            <!-- LEFT PANEL -->
            <div class="col-12 col-lg-6">
              <div class="fieldset">
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                    <col style="width: 10%" />
                  </colgroup>

                  <tr>
                    <td colspan="2">
                      <label>Trade Code</label>
                    </td>
                    <td colspan="2">
                      <div class="d-flex">
              <input-text
                v-model="model.Trade_Code"
                :disabled="isTradeCodeDisabled"
                @keyup.enter="onTradeCodeEnter"
                maxlength="15"
              />
              <button
                class="form-submit bg-primary"
                style="margin-left: 2px; width: 3em; border-radius: 0.5em"
                @click="loadItem"
              >
                <v-icon name="search" width="16px" />
              </button>
            </div>


                      
                    </td>

                    <td></td>

                    <td colspan="2" style="text-align: right">
                      <label>SAP Code</label>
                    </td>
                    <td colspan="3">
                      <input-text v-model="model.SAP_Code" maxlength="15" />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2">
                      <label>Trade Cls</label>
                    </td>
                    <td colspan="4">
                      <filter-cls
                      
                        ddl-width="80px"
                        desc-width="50px"
                        type-data="Trade_Cls"
                        v-model="model.Trade_Cls"
                      />
                    </td>

                    <td></td>

                    <td colspan="3">
                      <input-checkbox
                        v-model="model.Affiliate_Cls"
                        label="Affiliate Company"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2">
                      <label>Warehouse Code</label>
                    </td>
                    <td colspan="8">
                      <filter-warehouse
                      
                        v-model="model.Subcon_WH_Code"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2">
                      <label>Trade Name</label>
                    </td>
                    <td colspan="8">
                      <input-text v-model="model.Trade_Name" maxlength="70" />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Trade ABBR</label></td>
                    <td colspan="5">
                      <input-text
                        style="width: 250px"
                        v-model="model.Trade_Abbr"
                        maxlength="20"
                      />
                    </td>
                    <td></td>
                    <td></td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Contact Person</label></td>
                    <td colspan="8">
                      <input-text
                        v-model="model.Contact_Person"
                        maxlength="50"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Address 1</label></td>
                    <td colspan="8">
                      <input-text v-model="model.Address1" maxlength="100" />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Address 2</label></td>
                    <td colspan="8">
                      <input-text v-model="model.Address2" maxlength="100" />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>City</label></td>
                    <td colspan="8"><input-text v-model="model.City" /></td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Postel Code</label></td>
                    <td colspan="8">
                      <input-text
                        style="width: 150px"
                        v-model="model.Postal_Code"
                        maxlength="10"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Country</label></td>
                    <td colspan="8">
                      <input-text v-model="model.Country" maxlength="50" />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Country Cls</label></td>
                    <td colspan="2">
                       <input-cls-desc
                      
                        type-data="Country_Cls"
                        v-model="model.Country_Cls"
                      />
                    </td>
                    <td></td>
                    <td></td>
                    <td><label>Epte</label></td>
                    <td colspan="3">
                      <input-cls-desc
                       
                        type-data="Epte_Cls"
                        v-model="model.Epte_Cls"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Telephone / Fax</label></td>
                    <td colspan="2">
                      <input-text v-model="model.Telephone" maxlength="50" />
                    </td>

                    <td style="text-align: center"><label> / </label></td>
                    <td colspan="3">
                      <input-text v-model="model.Fax" maxlength="50" />
                    </td>

                    <td></td>
                    <td></td>
                    <td></td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Region Cls</label></td>
                    <td colspan="8">
                      <filter-cls
                        
                        ddl-width="100px"
                        desc-width="180px"
                        type-data="Region_Cls"
                        v-model="model.Region_Cls"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Closing Day</label></td>
                    <td colspan="2">
                      <input-text
                        style="width: 50px"
                        v-model="model.Closing_Day"
                        maxlength="2"
                      />
                    </td>
                    <td><label>Pay Day</label></td>

                    <td>
                      <input-text
                        style="width: 50px"
                        v-model="model.Pay_Day"
                        maxlength="2"
                      />
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                  </tr>
                  <tr>
                    <td colspan="2">
                      <input-checkbox
                        label="Invoice To"
                        v-model="isInvoiceToChecked"
                      />
                    </td>
                    <td colspan="8">
                      <filter-trade
                       
                        :trade-cls="['2', '3']"
                        v-model="model.Invoice_To"
                        :disabled="isInvoiceToChecked"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Invoice Pay</label></td>
                    <td>
                      <input-money
                        style="width: 50px"
                        v-model="model.InvoicePay_Days"
                      />
                    </td>
                    <td>
                      <label>Day</label>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>Insurance Covered</label></td>
                    <td colspan="8">
                      <filter-cls
                       
                        ddl-width="100px"
                        desc-width="180px"
                        type-data="Insurance_Cls"
                        v-model="model.Insurance_Cls"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>PO Cls</label></td>
                    <td colspan="2">
                        <input-cls-desc
                        
                        v-model="model.PO_Cls"
                        type-data="PO_Cls"
                        style="width: 120px"
                      />
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                  </tr>
                  <tr>
                    <td colspan="2"><label>NG Cls</label></td>
                    <td colspan="2">
                      <input-cls-desc
                       
                        v-model="model.NG_Cls"
                        type-data="NG_Cls"
                        style="width: 120px"
                      />
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                  </tr>
                </table>
              </div>
            </div>
            <div class="col-12 col-lg-6">
              <div class="fieldset">
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                  </colgroup>
                  <tr>
                    <td><label>NPWP No</label></td>
                    <td colspan="5">
                      <input-text v-model="model.NPWP_No" maxlength="20" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>NPWP Name</label></td>
                    <td colspan="5">
                      <input-text v-model="model.NPWP_Name" maxlength="100" />
                    </td>
                  </tr>
                  <tr rowspan="2">
                    <td><label>NPWP Address</label></td>
                    <td colspan="5">
                      <input-text
                        v-model="model.NPWP_Address"
                        maxlength="200"
                        multiline
                      />
                    </td>
                  </tr>

                  <tr>
                    <td><label>NPWP City</label></td>
                    <td colspan="5">
                      <input-text v-model="model.NPWP_City" maxlength="50" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>NNPPKP No</label></td>
                    <td colspan="5">
                      <input-text v-model="model.NNPPKP" maxlength="20" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>NITKU</label></td>
                    <td colspan="5">
                      <input-text v-model="model.NITKU" maxlength="30" />
                    </td>
                  </tr>
                </table>
              </div>
              <div class="fieldset" style="margin-top: 5px">
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                  </colgroup>
                  <tr>
                    <td><label>Price Condition</label></td>
                    <td colspan="5">
                      <filter-cls
                       
                        ddl-width="100px"
                        desc-width="180px"
                        type-data="PriceCondition_Cls"
                        v-model="model.Price_Condition"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Payment Term</label></td>
                    <td colspan="5">
                      <filter-cls
                         
                        ddl-width="100px"
                        desc-width="180px"
                        type-data="PaymentTerm_Cls"
                        v-model="model.POPayment_Terms"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Transportation</label></td>
                    <td colspan="3">
                      <filter-cls
                        
                        ddl-width="100px"
                        desc-width="180px"
                        type-data="Transportation_Cls"
                        v-model="model.Transportation_Cls"
                      />
                    </td>
                    <td><label>Payment Day</label></td>
                    <td><input-money v-model="model.POPayment_Day" /></td>
                  </tr>
                </table>
              </div>
              <div class="fieldset" style="margin-top: 5px">
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                  </colgroup>
                  <tr>
                    <td
                      colspan="6"
                      style="text-align: center; border: 1px solid black"
                    >
                      <label style="font-weight: bold; font-size: 16px"
                        >Delivery Place</label
                      >
                    </td>
                  </tr>
                  <v-button-add :add="add" cClass="mt-1" />
                  <tr>
                    <td colspan="6"> 
                      <div class="table-scroll-wrapper">
                        <v-table-input
                          :data-items="items"
                          ref="vtable"
                          style="width: 100%"
                        >
                          <template #table-content>
                            <div class="detail-content">
                              <table
                                class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                                v-if="
                                  !ds.isLoading &&
                                  !ds.isNetworkError &&
                                  !ds.isServerError
                                "
                                ref="table"
                              >
                                <thead>
                                  <tr>
                                    <th class="text-center">No</th>
                                    <th class="text-center">Code</th>
                                    <th class="text-center">Location Name</th>
                                    <th class="text-center"></th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr v-for="(item, i) in items || []" :key="i">
                                    <td class="text-center">
                                      {{ i + 1 }}
                                    </td>
                                    <td>
                                      <div class="col-10">
                                        <input-text
                                          
                                          v-model="item.Location_Code"
                                          :errors="
                                            errors?.[
                                              `Details[${i}].Location_Code`
                                            ]
                                          "
                                        />
                                      </div>
                                    </td>
                                    <td>
                                      <div class="col-10">
                                        <input-text
                                          
                                          v-model="item.Location_Name"
                                          :errors="
                                            errors?.[
                                              `Details[${i}].Location_Name`
                                            ]
                                          "
                                        />
                                      </div>
                                    </td>

                                    <td>
                                      <a
                                        href="javascript:void(0)"
                                        @click="items.splice(i, 1)"
                                      >
                                        <v-icon
                                          name="x"
                                          width="20"
                                          color="red"
                                        />
                                      </a>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                              <v-data-empty
                                class="mt-3"
                                v-if="
                                  !ds.isLoading &&
                                  items.length == 0 &&
                                  !ds.isNetworkError &&
                                  !ds.isServerError
                                "
                              />
                            </div>
                          </template>
                        </v-table-input>
                      </div>
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </div>
          <!-- LEFT PANEL -->
        </div>
        <div class="tab-pane fade" id="other-tab">
          <div class="row g-2">
            <!-- LEFT PANEL -->
            <div class="col-12 col-lg-6">
              <div class="fieldset">
                <div class="fieldset-title">PO Case Mark</div>
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                  </colgroup>

                  <tr>
                    <td><label>Line 1</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POCaseMark1" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 2</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POCaseMark2" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 3</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POCaseMark3" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 4</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POCaseMark4" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 5</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POCaseMark5" maxlength="25" />
                    </td>
                  </tr>
                </table>
              </div>
              <div class="fieldset" style="margin-top: 7px">
                <div class="fieldset-title">PO Marking</div>
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                  </colgroup>

                  <tr>
                    <td><label>Line 1</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POMarking1" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 2</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POMarking2" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 3</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POMarking3" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 4</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POMarking4" maxlength="25" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Line 5</label></td>
                    <td colspan="5">
                      <input-text v-model="model.POMarking5" maxlength="25" />
                    </td>
                  </tr>
                </table>
              </div>
              <div class="fieldset" style="margin-top: 7px">
                <div class="fieldset-title">BC Information</div>
                <table class="form-table" style="width: 100%">
                  <colgroup>
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                    <col style="width: 16.6%" />
                  </colgroup>

                  <tr>
                    <td><label>BC Type</label></td>
                    <td colspan="5">
                      <filter-cls
                        
                        ddl-width="100px"
                        desc-width="180px"
                        type-data="BCType_Cls"
                        v-model="model.Type_BC"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Kode KPPBC</label></td>
                    <td colspan="5">
                      <input-text v-model="model.CODE_KPPBC" maxlength="6" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>No Izin</label></td>
                    <td colspan="5">
                      <input-text v-model="model.No_Izin" maxlength="50" />
                    </td>
                  </tr>
                  <tr>
                    <td><label>Tanggal</label></td>
                    <td colspan="5">
                      <input-date
                       
                        v-model="model.NoIzin_Date"
                        :errors="errors?.NoIzin_Date"
                      />
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
       
      <v-button-submit :submit="submit" cClass="mr-1" :is-loading="isLoading" />
      <button
        class="btn btn-sm btn-red btn-elevate mr-1"
        @click="reset"
        :disabled="isLoading"
      >
        <div
          class="spinner-border spinner-border-sm text-light"
          role="status"
          v-if="isLoading"
        > 
          <span class="visually-hidden">Loading...</span>
        </div>
        <font-awesome-icon v-else icon="rotate-left" />
        <span class="ml-2">Clear</span>
      </button>
    </template>
  </v-frame>
    
   <v-modal id="shared-trade" title="List Trade" size="lg">
    <shared-trade
      :list="this.dsTrade"
      :refresh="refreshTradeList"
      :actions="[
        {
          href: 'javascript:void(0);',
          icon: 'edit',
          label: 'Select',
          event: (item) => selectItem(item),
        },
      ]"
    />
  </v-modal>
</template>

<script>
export default {
  props: ["id", "btnDisabled", "mode"],
  data: () => ({
    isInvoiceToChecked: false,
    isTradeCodeDisabled: false,
    isLoading: false,
    items: [],
    dataItem: null,
    idxItemLoad: 0,
    debounce: null,
    errors: {},
    refreshTradeList: 0,
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
      InvoicePay_Days: null,
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
      POPayment_Day: null,
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
      NoIzin_Date: null,
      NITKU: "",
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useTrade();
    },
    dsTrade: function () {
      return useTrade();
    },
  },
  mounted() {
    const trade_code = this.$route.query.tradecode;
    if (trade_code) {
      this.model.Trade_Code = trade_code;
      this.$nextTick(() => this.onTradeCodeEnter());
    }
  },
  watch: {
    // HANDLE BIND DATA AWAL & UPDATE
    "model.Invoice_To": {
      immediate: true,
      handler(v) {
        // hanya true kalau benar-benar ada value
        this.isInvoiceToChecked = v !== "";
      },
    },

    // HANDLE USER INTERACTION
    isInvoiceToChecked(v) {
      if (!v) {
        this.model.Invoice_To = ""; // KEMBALIKAN KE FORMAT BACKEND
      }
    },
  },
  methods: {
   loadItem: function (idx) {
      this.refreshTradeList++;
      this.$bvModal.show("shared-trade");
    },
     selectItem: function (dt) {
  // set Trade Code dari modal
  this.model.Trade_Code = dt.Trade_Code;

  // tutup modal
  this.$bvModal.hide("shared-trade");

  // trigger logic utama (sama seperti tekan Enter)
  this.$nextTick(() => {
    this.onTradeCodeEnter();
  });
},
    onTradeCodeEnter() {
      this.ds
        .loadDetail(this.model.Trade_Code)
        .then((res) => {
          // hanya akan masuk jika HTTP 200
          this.model = res.Data;
          this.isTradeCodeDisabled = true;

          this.ds.loadDelivery(this.model.Trade_Code).then((res2) => {
            this.items = (res2.Data?.DeliveryList || []).map((x) => ({
              Id: x.Id ?? null,
              Location_Code: x.Location_Code || "",
              Location_Name: x.Location_Name || "",
            }));
          });
        })
        .catch((err) => {
          // ✅ ERROR DARI BACKEND MASUK KE SINI
          const message =
            err.response?.data?.Message || "Trade Code didn't Exists";

          toastDanger("Trade Code wajib diisi");

          // reset state
          this.isTradeCodeDisabled = false;
          this.model = {};
          this.items = [];
        });
    },

    reset: function () {
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
        InvoicePay_Days: null,
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
        POPayment_Day: null,
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
        NoIzin_Date: null,
        NITKU: "",
        items: [],
      };
      this.items = [];
      this.isTradeCodeDisabled = false;
    },

    submit: async function () {
      // VALIDASI TRADE CODE
      if (!this.model.Trade_Code || this.model.Trade_Code.trim() === "") {
        toastDanger("Trade Code wajib diisi");
        return;
      }
      if (this.isLoading) return;

      this.isLoading = true;
      this.errors = {};

      try {
        // payload sesuai DTO backend
        const payload = {
          ...this.model,
          DeliveryList: this.items.map((x) => ({
            Location_Code: x.Location_Code,
            Location_Name: x.Location_Name,
          })),
        };
        debugger;
        // CREATE / UPDATE (backend pakai InsUpd)
        await this.ds.submit(payload);

        toastSuccess("Data berhasil disimpan");

        // optional: disable Trade Code setelah save
        this.isTradeCodeDisabled = true;
      } catch (err) {
        this.errors = err?.Errors || {};
        toastDanger(err?.Message || "Gagal menyimpan data");
      } finally {
        this.isLoading = false;
      }
    },
    add: function () {
      let obj = {
        Id: null,
        Location_Code: "",
        Location_Name: "",
      };

      this.items.push(obj);
    },
  },
};
</script>
<style>
.form-table {
  border-collapse: separate;
}

.form-table td {
  padding: 1px 2px;
}
.fieldset {
  position: relative;
  border: 2px solid grey;
  border-radius: 0.5em;
  padding: 2em 1em 1em;
}
.table-scroll-wrapper {
  max-height: 180px; /* ± 3 baris */
  overflow-y: auto;
}
.v-fixed-table {
  table-layout: fixed;
  width: 100%;
}

/* Header tetap */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 2;
}
.fieldset-title {
  position: absolute;
  top: -0.8em;
  left: 1em;
  background: white;
  padding: 0 0.5em;
  font-weight: bold;
}
</style>
