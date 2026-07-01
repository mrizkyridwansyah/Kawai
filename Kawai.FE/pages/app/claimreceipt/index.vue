<template>
  <v-frame title="Claim Receipt Input" icon="receipt">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Factory</label>					   
          <filter-factory-privileges
            class="form-control"
            v-model="filter.FactoryCode"
            :disabled="filter.ReceiptId != null"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

        <div class="filter-item">						 
          <label class="form-label">Supplier</label>													 
          <filter-trade-2
            class="form-control"
            v-model="filter.SupplierCode"
            :trade-cls="['2', '3']"
            :disabled="filter.ReceiptId != null"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Delivery Date</label>
          <div>
            <input-date v-model="filter.PeriodFrom" />
          </div>
          <label
            class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
            >Until Date</label
          >
          <div> 
            <input-date v-model="filter.PeriodUntil" />
          </div>
        </div>

        <div class="filter-item">
          <label class="form-label">Claim No</label>											 
          <input-claim
            class="form-control"
            status="APPROVED"			  
            :supplier-code="filter.SupplierCode"				
            :period-from="filter.PeriodFrom"
            :period-until="filter.PeriodUntil"
            v-model="filter.PONumber"
            :errors="errors?.PONumber"
            style="width: 420px"
          />
        </div>

        <div class="filter-item">			 
          <label class="form-label">Receipt No</label>
          <input-receipt
            class="form-control"
            :disabled="isNew"
            status="NEW"
            source-menu="RECEIPT CLAIM"
            :factory-code="filter.FactoryCode"
            :supplier-code="filter.SupplierCode"
            v-model="filter.ReceiptId"
            :errors="errors?.ReceiptId"
            style="width: 375px"
          />
          <div class="col-xl-1 col-lg-1 col-md-1 col-sm-1 col-xs-1 mt-2">						 
            <input-checkbox
              label="New"
              v-model="isNew"
              @click="(e) => changeNew(e)"
            />
          </div>
        </div>

        <div class="filter-item">					 
          <label class="form-label">DN Number</label>												 
          <input-text
            v-model="model.DNNumber"
            maxlength="50"
            :errors="errors?.DNNumber"
            style="width: 420px"
          />
        </div>

        <div class="filter-item"> 
          <label class="form-label">BC Number</label>							   
          <input-text
            v-model="model.BCNumber"
            maxlength="50"
            :errors="errors?.BCNumber"
            style="width: 420px"
          />
        </div>

        <div class="filter-item">					 
          <label class="form-label">BC Type</label>													 
          <filter-cls-2
            class="form-control"
            type-data="BCType_Cls"
            v-model="model.BCType"
            :errors="errors?.BCType"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

        <div class="filter-item">				 
          <label class="form-label">BC Date</label>
          <div>				 
            <input-date v-model="model.BCDate" :errors="errors?.BCDate" />
          </div>
          <label
            class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
            >DN Date</label
          >
          <div>				 
            <input-date v-model="model.DNDate" :errors="errors?.DNDate" />
          </div>
            
        </div>
        <div class="filter-item">					 
          <label class="form-label">Police No</label>												 
          <input-text
            v-model="model.VehicleNo"
            :errors="errors?.VehicleNo"
            style="width: 420px"
            maxlength="15"
          />
        </div>
        <div class="filter-item">					 
          <label class="form-label">Transport By</label>											 
          <filter-cls-2
            class="form-control"
            type-data="Transport_Cls"
            v-model="model.Transport"
            :errors="errors?.Transport"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>
		
	     <div class="filter-item">								 
          <label class="form-label">Receipt Date</label>
           <input-date
                    v-model="model.ReceiptDate"
                    style-date="width: 115px"
                    :errors="errors?.ReceiptDate"
                  />
        </div>						 
        <div class="filter-item">	 
          <label class="form-label">Register No</label>											 
          <input-text
            v-model="model.RegisterNo"
            :errors="errors?.RegisterNo"
            :disabled="isNew"
            style="width: 420px"
          />
        </div>
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <button
            class="btn btn-sm btn-blue btn-elevate mr-1"
            @click="searchPoDetail"
            :disabled="isLoading"
          >
            <div
              class="spinner-border spinner-border-sm text-light"
              role="status"
              v-if="isLoading"
            >
              <span class="visually-hidden">Loading...</span>
            </div>
            <font-awesome-icon v-else icon="search" />
            <span class="ml-2">Search</span>
          </button>		  
          <v-button-submit
            :submit="submit"
            cClass="mr-1"
            :is-loading="isLoading"
												
          />
          <v-button-print
            label="Print Label"
            class="mr-1"
            :print="printLabel"
            :is-loading="isLoading"
												
          />
          <v-button-print
            label="Print Receipt Report"
							
            :print="printReport"
            :is-loading="isLoading"
												
          />
          <v-button
            :action="print"
            label="Print Label PDF"
            icon="file-pdf"
            cClass="ml-1 btn-green"
            :is-loading="isLoading"
												
          />
        </div>										
      </div>
      <hr />

      <v-table-input
        :data-items="listClaimDetail"
        ref="vtable"				 
        :top-content-height="425"
      >
        <template #table-content>
          <div class="detail-content" style="width: 100%">
            <table
              class="table table-striped table-bordered mb-0 align-middle v-fixed-table w-100"
              v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
              ref="table"
            >
              <thead>
                <tr>
                  <th class="text-center">
                    <input-checkbox @click="(e) => checkAll(e)" />
                  </th>
                  <th class="text-center">Claim Number</th>
                  <th class="text-center">Item Code</th>
                  <th class="text-center">Item Name</th>
                  <th class="text-center">Unit</th>
                  <th class="text-center">Claim Qty</th>
                  <th class="text-center">Receipt Qty</th>
                  <th class="text-center">Remaining Qty</th>
                  <th class="text-center">Qty DN</th>
                  <th class="text-center">Total Packing</th>
                  <th class="text-center">Qty Packing</th>
                  <th class="text-center">No. Seri</th>
                  <th class="text-center">Production Date</th>
                  <th class="text-center">Last Update</th>
                  <th class="text-center">Last User</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in listClaimDetail || []" :key="idx">
                  <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                      @click="(e) => check(e, item)"
                    />
                  </td>
                  <td>{{ item.PONumber }}</td>
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.Qty) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.TotalReceiptQty) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.RemainingQty) }}
                  </td>
                  <td>
															 
                    <input-money-small
                      v-model="item.ReceiptQty"
                      :errors="errors?.[`Details[${idx}].ReceiptQty`]"
                      style="width: 100px"
                    />

                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.TotalPacking) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.QtyPacking) }}
                  </td>
						
										
                  <td>
                      <input-money-small
                        v-model="item.NoSeri"
                        :errors="errors?.[`Details[${idx}].NoSeri`]"
                        style="width: 100px"
                      />
                    </td>												
											
						
						 
                  <td>{{ $func.formatDate(item.ProductionDate) }}</td>
                  <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                  <td>{{ item.LastUser }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </template>
      </v-table-input>
			

		   
      <label class="form-label">Remarks</label>				 
      <input-text multiline v-model="model.Remarks" :errors="errors?.Remarks" />
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
     isNew: true,
	 noSeri: 0,		  
	 menuPrivAllowUpdate: false,					
					   
					  
    filter: {
      FactoryCode: null,
      SupplierCode: null,
      TypeDate: "DELIVERY",
      PeriodFrom: null,
      PeriodUntil: null,
      PONumber: null,
      ReceiptId: null,
    },
    model: {
      Id: null,
      ReceiptNo: "",
      DNNumber: "",
      FactoryCode: null,
      SupplierCode: null,
      DNDate: null,
      BCNumber: "",
      BCType: "",
      BCDate: null,
	  ReceiptDate: null,				
      VehicleNo: "",
      Transport: null,
      RegisterNo: null,
      Remarks: null,
      Details: [],
      IsManual: true,
    },
    listClaimDetail: [],
    debounce: null,
    isLoading: false,
    prevRegisterNo: "",
    errors: {},
						
							   
  }),
  computed: {
    ds: function () {
      return useReceipt();
    },
    notif: function () {
      return useNotification();
    },
    dsMenu: function () {
      return useMenu();
    },		 
  },

  watch: {
    "filter.PONumber": function () {
      this.listClaimDetail = [];
    },
    "filter.FactoryCode": function () {
      this.listClaimDetail = [];
    },
    "filter.SupplierCode": function () {
      this.listClaimDetail = [];
    },
    "filter.ReceiptId": function () {
      if (this.filter.ReceiptId) this.getReceipt();
      else {
        this.isNew = true;
        let today = new Date();
        this.filter.PeriodFrom = new Date(
          today.getFullYear(),
          today.getMonth(),
          1,
        );
        this.filter.PeriodUntil = today;
        this.filter.PONumber = null;
        this.listClaimDetail = [];
        this.filter.ReceiptId = null;
        this.model = {
          Id: null,
          ReceiptNo: "",
          DNNumber: "",
          FactoryCode: null,
          SupplierCode: null,
          DNDate: today,
          BCNumber: "",
          BCType: "",
          BCDate: today,
		  ReceiptDate: today,					 
          VehicleNo: "",
          Transport: null,
          RegisterNo: null,
          Remarks: null,
          Details: [],
          IsManual: true,
          SourceMenu: "RECEIPT CLAIM",
        };
      }
    },
  },
  mounted: function () {
	this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "E05",
      )[0].AllowUpdate;
    });
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
	this.model.ReceiptDate = today;							   
    this.model.BCDate = today;
    this.model.DNDate = today;
  },
  methods: {
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    reset: function () {
      this.isNew = true;
      this.filter = {
        FactoryCode: null,
        SupplierCode: null,
        TypeDate: "DELIVERY",
        PeriodFrom: null,
        PeriodUntil: null,
        PONumber: null,
        ReceiptId: null,
      };
      this.model = {
        Id: null,
        ReceiptNo: "",
        DNNumber: "",
        FactoryCode: null,
        SupplierCode: null,
        DNDate: null,
        BCNumber: "",
        BCType: "",
        BCDate: null,
	    ReceiptDate: null,				  
        VehicleNo: "",
        Transport: null,
        RegisterNo: null,
        Remarks: null,
        Details: [],
        IsManual: true,
        SourceMenu: "RECEIPT CLAIM",
      };
      this.listClaimDetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.PeriodUntil = today;
    },
    changeNew: function (e) {
      if (e.target.checked) {
        this.isNew = true;
        this.listClaimDetail = [];
        this.filter.ReceiptId = null;
        this.model = {
          Id: null,
          ReceiptNo: "",
          DNNumber: "",
          FactoryCode: null,
          SupplierCode: null,
          DNDate: null,
          BCNumber: "",
          BCType: "",
          BCDate: null,
		  ReceiptDate: null,					
          VehicleNo: "",
          Transport: null,
          RegisterNo: null,
          Remarks: null,
          Details: [],
          IsManual: true,
          SourceMenu: "RECEIPT CLAIM",
        };
        // this.reset();
      }
    },
    checkAll: function (e) {
      this.listClaimDetail.map((x) => {
        x.Selected = e.target.checked;
        x.ReceiptQty = e.target.checked ? x.RemainingQty : 0;
      });
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
      item.ReceiptQty = e.target.checked ? item.RemainingQty : 0;
	 if (item.Selected) {
        let lastNoSeriInGrid = this.listClaimDetail.filter(
          (p) => (p.NoSeri ?? 0) > 0,
        );
        if (lastNoSeriInGrid.length > 0) {
          const maxNoSeri = Math.max(
            ...lastNoSeriInGrid.map((p) => p.NoSeri ?? 0),
          );

          this.noSeri = maxNoSeri + 1;
        } else {
          this.noSeri = 1;
        }

        item.NoSeri = this.noSeri;
      } else {
        item.NoSeri = 0;
      }
    },
    printLabel: function () {
	  this.isLoading = true;						
      this.ds
        .printLabel(this.filter.ReceiptId)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.isNew = false;
          this.filter.ReceiptId = dt.Data["Receipt Header"].Id;
          // this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },

    print: function () {
      if (!this.filter.ReceiptId) {
        toastDanger("Silahkan pilih Receipt No!");
        return;
      }

	   this.isLoading = true;						
      this.ds
        .print(this.filter.ReceiptId)
        .then((data) => {
          toastSuccess(data || "Print Label berhasil!");
        })
        .catch((err) => toastDanger(err.Message));
												 
    },
						 
    printReport: function () {},
    submit: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listClaimDetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Silahkan pilih CLAIM!");
        return;
      }

      this.model.Id = this.filter.ReceiptId;
      this.model.FactoryCode = this.filter.FactoryCode;
      this.model.SupplierCode = this.filter.SupplierCode;
      this.model.Details = details.map((p) => {
        return {
          PONumber: p.PONumber,
          ItemCode: p.ItemCode,
          UnitClsCode: p.UnitClsCode,
          ExpectedQty: p.RemainingQty,
          TotalPacking: p.TotalPacking,
          ReceiptQty: p.ReceiptQty,
		  NoSeri: p.NoSeri,				   
        };
      });

   let totalBarcodePrint = details.reduce(
        (total, item) => total + Math.ceil(item.ReceiptQty / item.QtyPacking),
        0,
      );
      if (totalBarcodePrint >= 100) {
        let modalMessage = `<div style="font-size: medium">Total barcode yang akan dicetak sebanyak <strong>${this.$func.formatMoney(Math.ceil(totalBarcodePrint))} Barcode</strong>.
            <br>Anda yakin akan <strong>MELANJUTKAN</strong>?</div>`;
        confirmSubmit(
          () =>
            new Promise((resolve) => {
              if (this.isNew) {
                this.createReceipt();
              } else {
                this.updateReceipt();
              }
              resolve();
            }),
          () => (this.isLoading = false),
          modalMessage,
        );
      } else {
        if (this.isNew) {
          this.createReceipt();
        } else {
          this.updateReceipt();
        }
      }
    },
    getReceipt: function () {
      this.ds.loadDetail(this.filter.ReceiptId).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.filter.FactoryCode = this.model.FactoryCode;
        this.filter.SupplierCode = this.model.SupplierCode;
        this.filter.PeriodFrom = this.model.DeliveryDatePOFrom;
        this.filter.PeriodUntil = this.model.DeliveryDatePOUntil;
        this.filter.PONumber = this.model.PONumber;
        this.prevRegisterNo = this.model.RegisterNo;
        this.$nextTick(() => setTimeout(() => this.searchPoDetail(), 500));
      });
    },
    searchPoDetail: function () {
      if (!this.filter.SupplierCode) {
        toastDanger("Silahkan pilih Supplier!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          FactoryCode: this.filter.FactoryCode,
          ReceiptId: this.filter.ReceiptId?.toString() || "0",
          PONumber: this.filter.PONumber.toString(),
          SupplierCode: this.filter.SupplierCode,
          DateFrom: this.filter.PeriodFrom,
          DateUntil: this.filter.PeriodUntil,
        },
      ];
      this.ds.listClaimDetail(filters).then((dt) => {
        this.listClaimDetail = dt.Data.Items;
        this.listClaimDetail.map((x) => (x.Selected = x.ReceiptDetailId > 0));
      });
    },
    createReceipt: function () {
      this.ds
        .createclaim(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.isNew = false;
          this.filter.ReceiptId = dt.Data["Receipt Header"].Id;
          // this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    updateReceipt: function () {
      if (this.prevRegisterNo != this.model.RegisterNo) {
        confirmSubmit(
          () =>
            new Promise((resolve) => {
            this.checkIsDetailUpdate();
              resolve();
            }),
          () => (this.isLoading = false),
          `You change the <strong>Register No</strong>. Are you sure to <strong>CONTINUE</strong> changes?`,
        );
      } else {
       this.checkIsDetailUpdate();
      }
    },
    checkIsDetailUpdate: function () {
      this.ds
        .checkIsDetailUpdate(this.model)
        .then((dt) => {
          if (dt.Data.IsUpdateDetails) {
            let modalMessage = `<div style="font-size: medium">Anda mengubah <strong>${dt.Data.TypeConfirmationDesc}</strong>.
                <br><strong>Barcode Label Saat ini</strong> akan menjadi <strong class="text-danger">TIDAK VALID</strong>
                <br>Anda yakin akan <strong>MELANJUTKAN</strong> perubahan?</div>`;

            confirmSubmit(
              () =>
                new Promise((resolve) => {
                  this.update();
                  resolve();
                }),
              () => (this.isLoading = false),
              modalMessage,
            );
          } else if (
            !dt.Data.IsUpdateDetails &&
            dt.Data.TypeConfirmation == 3
          ) {
            let modalMessage = `<div style="font-size: medium">Anda sudah melakukan <strong>SCAN RECEIVING MOBILE</strong>.
                <br>Perubahan hanya berlaku untuk informasi <strong>Header</strong> saja.
                <br>Anda yakin akan <strong>MELANJUTKAN</strong> perubahan?</div>`;

            confirmSubmit(
              () =>
                new Promise((resolve) => {
                  this.update();
                  resolve();
                }),
              () => (this.isLoading = false),
              modalMessage,
            );
          } else {
            this.update();
          }
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    update: function () {
      this.ds
        .updateclaim(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.isNew = false;
          this.filter.ReceiptId = dt.Data["Receipt Header"].Id;
          this.prevRegisterNo = this.model.RegisterNo;
							
          // this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
  },
};
</script>

<style scoped>
.detail-content {
  max-height: 30em;
  max-height: 60%;
}

.detail-content-view {
  /* padding-bottom: 5em; */
  height: 70%;
  max-height: 70%;
  overflow-y: scroll;
}
thead {
  white-space: nowrap;
}
/* GANTI CSS .filter-wrapper lama dengan ini */

.filter-wrapper {
  display: grid;
  grid-template-columns: repeat(2, minmax(320px, 1fr));
  grid-auto-flow: column; /* isi atas ke bawah dulu */
  gap: 4px 20px;
  width: 100%;
  align-items: center;
}

.filter-wrapper:has(.filter-item:nth-child(14)) {
  grid-template-rows: repeat(9, auto);
}

.filter-wrapper:has(.filter-item:nth-child(13)):not(
    :has(.filter-item:nth-child(14))
  ) {
  grid-template-rows: repeat(8, auto);
}

.filter-wrapper:has(.filter-item:nth-child(12)):not(
    :has(.filter-item:nth-child(13))
  ) {
  grid-template-rows: repeat(7, auto);
}

.filter-wrapper:has(.filter-item:nth-child(11)):not(
    :has(.filter-item:nth-child(12))
  ) {
  grid-template-rows: repeat(6, auto);
}

.filter-wrapper:has(.filter-item:nth-child(10)):not(
    :has(.filter-item:nth-child(11))
  ) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(9)):not(
    :has(.filter-item:nth-child(10))
  ) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(8)):not(
    :has(.filter-item:nth-child(9))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(7)):not(
    :has(.filter-item:nth-child(8))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(6)):not(
    :has(.filter-item:nth-child(7))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(5)):not(
    :has(.filter-item:nth-child(6))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(4)):not(
    :has(.filter-item:nth-child(5))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(3)):not(
    :has(.filter-item:nth-child(4))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(2)):not(
    :has(.filter-item:nth-child(3))
  ) {
  grid-template-rows: repeat(1, auto);
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
  min-height: 32px;
  width: 100%;
}

.filter-item label {
  width: 70px;
  min-width: 70px;
  white-space: nowrap;
}

/* MOBILE = turun kebawah normal */
@media (max-width: 1035px) {
  .filter-wrapper {
    grid-template-columns: 1fr !important;
    grid-template-rows: auto !important;
    grid-auto-flow: row !important;
    gap: 6px;
  }

  .filter-item {
    width: 100%;
  }
}

.button-section {
  /* garis panjang bawah */
  width: 100%;
}
</style>
