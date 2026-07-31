<template>
  <v-frame title="Production Material NG Claim Input" icon="receipt">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->

          <div class="filter-item">
          <label class="form-label">Claim No</label>
          <input-productionclaim
            class="form-control"
            :disabled="isNew"
            status="DRAFT"
            v-model="filter.ClaimId"
            :errors="errors?.ClaimId"
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

         <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Claim Date</label>
          <div>
            <input-date v-model="model.ClaimDate" />
          </div>
        </div>

        <div class="filter-item">
          <label class="form-label">Factory</label>
          <filter-factory-privileges
            class="form-control"
            v-model="filter.FactoryCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

         <div class="filter-item">
          <label class="form-label">Process</label>
          <filter-trade-2
            class="form-control"
            :trade-cls="['1']"
            v-model="filter.ManufactureCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>
        <div class="filter-item">
          <label class="form-label">Line</label>
          <filter-line-factory
            class="form-control"
            :company="filter.FactoryCode"
            :manufacture="filter.ManufactureCode"
            v-model="filter.LineCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

     

        <div class="filter-item">
          <label class="form-label">Picking No</label>
          <input-pickingno
                    class="form-control"
                    v-model="filter.PickingNo"
                    :line="filter.LineCode"
                    type-date="ALL"
                    :show-option-all="true"
                    :claim-id="filter.ClaimId"
                    style="width: 420px"
                  />
           
        </div>
        

        <div class="filter-item">
          <label class="form-label">Priority</label>
          <input-priority
            v-model="model.Priority"
            maxlength="50"
            :errors="errors?.Priority"
            style="width: 420px"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Status</label>
          <input-text
            v-model="model.Status"
            maxlength="50"
           :errors="errors?.Status"
            disabled="true"
            style="width: 420px"
          />
        </div>

     
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <button
            class="btn btn-sm btn-blue btn-elevate mr-1"
            @click="searchNGDetail"
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
            :submit="submitDraft"
            :disabled="!menuPrivAllowUpdate"
             label="Save Draft"
            icon="save"
            cClass="mr-1"
            :is-loading="isLoading"
          />
           <v-button
                :action="remove"
                label="Delete"
                icon="trash"
                cClass="mr-1 btn-danger"
                :is-loading="isLoading"
                :disabled="!menuPrivAllowUpdate"
          />
         <v-button-submit
            :submit="submitprocess"
           :disabled="isNew || !menuPrivAllowUpdate"
             label="Submit Claim"
            icon="check"
            cClass="mr-1"
            :is-loading="isLoading"
          />
        </div>
      </div>
      <hr />
      <v-table-input
        :data-items="listNGDetail"
        :frozen-column-left="3"
        ref="vtable"
        :top-content-height="450" 
      >
        <template #table-content>
          <div class="detail-content">
            <table
              class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
              v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
              ref="table"
            >
              <thead>
                <tr>
                  <th class="text-center">
                    <input-checkbox @click="(e) => checkAll(e)" />
                  </th>
                  <th class="text-center">Picking No</th>
                  <th class="text-center">Barcode No</th>
                  <th class="text-center">Item Code</th>
                  <th class="text-center">Item Name</th>
                  <th class="text-center">Lot No</th>
                  <th class="text-center">Unit</th>
                  <th class="text-center">Qty NG</th>
                  <th class="text-center">Reason</th>
                  <th class="text-center">Remarks</th>
                  <th class="text-center">Evidence</th>
                  <th class="text-center">Last Update</th>
                  <th class="text-center">Last User</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in listNGDetail || []" :key="idx">
                  <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                      @click="(e) => check(e, item)"
                    />
                  </td>
                  <td>{{ item.PickingNo }}</td>
                  <td>{{ item.BarcodeNo }}</td>
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.LotNo }}</td>
                  <td>{{ item.UnitClsName }}</td>
                  <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
                 <td>
                   <input-text
                        v-model="item.Reason"
                        :errors="errors?.[`Details[${idx}].Reason`]"
                        style="width: 200px"
                      />
                       </td>
                 <td>
                  <input-text
                        v-model="item.RemarksDetail"
                        :errors="errors?.[`Details[${idx}].RemarksDetail`]"
                        style="width: 200px"
                      />
                     </td>
                  <td>
                     <a
                        href="javascript:void(0);"
                        @click="() => showModal(item, 'VIEW')"
                      >
                        View
                      </a>

                  </td>
                  <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                  <td>{{ item.LastUser }}</td>
                </tr>
              </tbody>
            </table>
            <v-data-empty
              class="mt-3"
              v-if="
                !ds.isLoading &&
                !ds.isNetworkError &&
                !ds.isServerError &&
                (!listNGDetail || listNGDetail.length === 0)
              "
            />
          </div>
        </template>
      </v-table-input>
       <div>
        <label class="form-label">Remarks</label>
        <input-text
          multiline
          v-model="model.Notes"
          :errors="errors?.Notes"
        />
      </div>
    </template>
  </v-frame>
   <v-modal
    ref="modalIQC"
    id="modal-form-iqc-result"
    :title="title"
    size="md"
    @hidden="
      () => {
        this.$refs.formIQC.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-iqc-result
      ref="formIQC"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    isNew: true,
    menuPrivAllowUpdate: false,
    filter: {
        PickingNo: null,
        FactoryCode: null,
        ManufactureCode: null,
        LineCode: null,
        ClaimId: null,
    },
    model: {
      ClaimId: null,
      ClaimNo: "",
      LineCode: "",
      Priority: "",
      Status: "",
      ClaimDate: null,
      Notes: "",
      Details: [],
    },
    listNGDetail: [],
    Details: [],
    debounce: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useProductionNGClaim();
    },
     dsMenu: function () {
      return useMenu();
    },
  },
  watch: {
   
    "filter.LineCode": function () {
      this.listNGDetail = [];
    },
    "filter.ClaimId": function () {
      this.model = {
        ClaimId: null,
        ClaimNo: "",
        LineCode: "",
        Priority: "",
        Status: "",
        ClaimDate: null,
        Notes: "",
        Details: [],
      };
      this.listNGDetail = [];
      let today = new Date();
      if (this.filter.ClaimId) this.getClaim();
    },
  },
  mounted: function () {
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "P03",
      )[0].AllowUpdate;
    });
    let today = new Date();
    this.model.ClaimDate = today;

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
        PickingNo: null,
        FactoryCode: null,
        ManufactureCode: null,
        LineCode: null,
        ClaimId: null,
      };
      this.model = {
        ClaimId: null,
        ClaimNo: "",
        LineCode: "",
        Priority: "",
        Status: "",
        ClaimDate: today,
        Notes: "",
        Details: [],
      };
      this.listNGDetail = [];
      let today = new Date();
   
    },
    changeNew: function (e) {
      if (e.target.checked) this.reset();
    },
    checkAll: function (e) {
      this.listNGDetail.map((x) => (x.Selected = e.target.checked));
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
    },
    submitDraft: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listNGDetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Please Choose Picking No!");
        return;
      }
      if (!this.model.Priority) {
        toastDanger("Please choose priority!");
        return;
      }


      this.model.ClaimId = this.filter.ClaimId?.toString() || "0";
      this.model.LineCode = this.filter.LineCode;
      this.model.Details = details.map((p) => {
        return {
          PickingNo: p.PickingNo,
          LotNo: p.LotNo,
          BarcodeNo: p.BarcodeNo,
          ItemCode: p.ItemCode,
          RemarksDetail: p.RemarksDetail,
          Reason: p.Reason,
          Qty: p.Qty?.toString() || "0",

        };
      });

      if (this.isNew) {
        this.createClaim();
      } else {
        this.updateClaim();
      }
    },

    submitprocess: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listNGDetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Please Choose Picking No!");
        return;
      }

       if (!this.model.Priority) {
        toastDanger("Please choose priority!");
        return;
      }
      this.ds
        .submit(this.model)
        .then((dt) => {
          toastSuccess("Data Approved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    getClaim: function () {
      this.ds.loadDetail(this.filter.ClaimId).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.filter.PickingNo = this.model.PickingNo;
        this.filter.FactoryCode = this.model.FactoryCode;
        this.filter.ManufactureCode = this.model.ManufactureCode;
        this.filter.LineCode = this.model.LineCode;
        this.$nextTick(() => setTimeout(() => this.searchNGDetail(), 500));
      });
    },
    searchNGDetail: function () {
      if (!this.filter.LineCode) {
        toastDanger("Please Select Line!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          FactoryCode: this.filter.FactoryCode,
          ClaimId: this.filter.ClaimId?.toString() || "0",
          LineCode: this.filter.LineCode,
          PickingNo: this.filter.PickingNo,
          ManufactureCode: this.filter.ManufactureCode,
          
        },
      ];
      this.ds.listNGDetail(filters).then((dt) => {
        this.listNGDetail = dt.Data.Items;
        this.listNGDetail.map((x) => (x.Selected = x.DetailID > 0));
      });
    },
    showModal: function (dt, mode) {
      this.title = "Evidence Detail";
      this.modalMode = mode;
      this.idSelected = dt.InspectionID;
      this.$bvModal.show("modal-form-iqc-result");
    },
    close: function () {
      this.$bvModal.hide("modal-form-iqc-result");
      this.search();
    },
    remove: function () {
      if (!this.filter.ClaimId) {
        toastDanger("Silahkan pilih Claim No!");
        return;
      }

      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(this.filter.ClaimId)
              .then((dt) => {
                toastSuccess("Data deleted successfully!");
                resolve();
                this.reset();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                resolve();
                //toastDanger(err?.Message);
              });
          }),
        null,

        "",
      );
    },
    createClaim: function () {
      this.ds
        .create(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          //toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    updateClaim: function () {
      this.ds
        .update(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
         // toastDanger(err?.Message);
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

/* jumlah baris otomatis sesuai jumlah item */

.filter-wrapper:has(.filter-item:nth-child(10)) {
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
