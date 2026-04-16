<template>
  <v-frame title="NG Claim Material Approval" icon="receipt">
    <template #frame-content>
      <div class="row">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Supplier</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
       
           <filter-trade-2
            v-model="filter.SupplierCode"
            :trade-cls="['2', '3']"
            :disabled="filter.ClaimId != null"
             style-code="width: 120px"
                    style-desc="width: 275px"
          />

        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >BC Number</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <input-text
            v-model="model.BCNumber"
            :errors="errors?.BCNumber"
            :disabled="true"
             style="width: 400px"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Claim Date</label
        >
        <div class="col-xl-2 col-lg-2 col-md-10 col-sm-10 col-xs-10">
          <input-date v-model="filter.PeriodFrom" />
        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Until Date</label
        >
        <div class="col-xl-2 col-lg-2 col-md-10 col-sm-10 col-xs-10">
          <input-date v-model="filter.PeriodUntil" />
        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >BC Type</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
           
          <filter-cls-2
                    class="form-control"
                    type-data="BCType_Cls"
                    v-model="model.BCType"
                    :errors="errors?.BCType"
                    style-code="width: 120px"
                    style-desc="width: 240px"
                     :disabled="true"
                  />

        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Claim No</label
        >
        <div class="col-xl-4 col-lg-4 col-md-9 col-sm-9 col-xs-11">
          <input-claim
            class="form-control"
            status="ALL"
            :supplier-code="filter.SupplierCode"
            :period-from="filter.PeriodFrom"
            :period-until="filter.PeriodUntil"
            v-model="filter.ClaimId"
            :errors="errors?.ClaimId"
          />
        </div>
        <div class="col-xl-1 col-lg-1 col-md-1 col-sm-1 col-xs-1 mt-2"></div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >BC Date</label
        >
        <div class="col-xl-2 col-lg-2 col-md-10 col-sm-10 col-xs-10">
          <input-date
            v-model="model.BCDate"
            :errors="errors?.BCDate"
            :disabled="true"
          />
        </div>
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >DN Date</label
        >
        <div class="col-xl-2 col-lg-2 col-md-10 col-sm-10 col-xs-10">
          <input-date
            v-model="model.DNDate"
            :errors="errors?.DNDate"
            :disabled="true"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Police No</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <input-text
            v-model="model.VehicleNo"
            :errors="errors?.VehicleNo"
               style="width: 120px"
            :disabled="true"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Transport By</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
           <filter-cls-2
                    class="form-control"
                    type-data="Transport_Cls"
                    v-model="model.Transport"
                    :errors="errors?.Transport"
                    style-code="width: 120px"
                    style-desc="width: 120px"
                     :disabled="true"
                  />

          
          
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >DN Number</label
        >
        <div class="col-xl-5 col-lg-5 col-md-10 col-sm-10 col-xs-12">
          <input-text
            v-model="model.DNNumber"
            :errors="errors?.DNNumber"
            :disabled="true"
            style="width: 325px"
          />
        </div>
        <div class="col-xl-2 col-lg-2 col-md-8 col-sm-8 col-xs-10"></div>
      </div>
      <hr />
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
            :disabled="model.Status == 'APPROVED'"
            label="Approved"
            icon="check"
            cClass="mr-1"
            :is-loading="isLoading"
          />

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
        </div>
      </div>

      <v-table-input :data-items="listNGClaimDetail" ref="vtable">
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
                    <input-checkbox
                      @click="(e) => checkAll(e)"
                      :disabled="true"
                    />
                  </th>
                  <th class="text-center">PO Number</th>
                  <th class="text-center">Receipt Number</th>
                  <th class="text-center">Receipt Date</th>
                  <th class="text-center">Item Code</th>
                  <th class="text-center">Item Name</th>
                  <th class="text-center">Unit</th>
                  <th class="text-center">Qty</th>
                  <th class="text-center">NG Code</th>
                  <th class="text-center">NG Description</th>
                  <th class="text-center">Last Update</th>
                  <th class="text-center">Last User</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in listNGClaimDetail || []" :key="idx">
                  <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                      @click="(e) => check(e, item)"
                      :disabled="true"
                    />
                  </td>
                  <td>{{ item.PONumber }}</td>
                  <td>{{ item.ReceiptNumber }}</td>
                  <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsName }}</td>
                  <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
                  <td>{{ item.NGCode }}</td>
                  <td>{{ item.NGDescs }}</td>
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
    (!listNGClaimDetail || listNGClaimDetail.length === 0)
  "
/>
          </div>
        </template>
      </v-table-input>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    isNew: true,
    filter: {
      SupplierCode: null,
      PeriodFrom: null,
      PeriodUntil: null,
      ClaimId: null,
    },
    model: {
      ClaimId: null,
      ClaimNo: "",
      DNNumber: "",
      SupplierCode: null,
      DNDate: null,
      BCNumber: "",
      BCType: "",
      BCDate: null,
      VehicleNo: "",
      Transport: null,
      Details: [],
    },
    listNGClaimDetail: [],
    debounce: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useNGClaim();
    },
  },
  watch: {
    "filter.SupplierCode": function () {
      this.listNGClaimDetail = [];
    },
    "filter.ClaimId": function () {
      this.model = {
        ClaimId: null,
        ClaimNo: "",
        DNNumber: "",
        SupplierCode: null,
        DNDate: null,
        BCNumber: "",
        BCType: "",
        BCDate: null,
        VehicleNo: "",
        Transport: null,
        Details: [],
      };
      this.listNGClaimDetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.PeriodUntil = today;

      if (this.filter.ClaimId) this.getClaim();
    },
  },
  mounted: function () {
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
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
        SupplierCode: null,
        PeriodFrom: null,
        PeriodUntil: null,
        ClaimId: null,
      };
      this.model = {
        ClaimId: null,
        ClaimNo: "",
        DNNumber: "",
        SupplierCode: null,
        DNDate: null,
        BCNumber: "",
        BCType: "",
        BCDate: null,
        VehicleNo: "",
        Transport: null,
        Details: [],
      };
      this.listNGClaimDetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1
      );
      this.filter.PeriodUntil = today;
    },

    checkAll: function (e) {
      this.listNGClaimDetail.map((x) => (x.Selected = e.target.checked));
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
    },
    submit: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listNGClaimDetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Silahkan pilih PO!");
        return;
      }

      this.model.ClaimId = this.filter.ClaimId;
      this.model.SupplierCode = this.filter.SupplierCode;
      this.model.Details = details.map((p) => {
        return {
          PONumber: p.PONumber,
          ReceiptNumber: p.ReceiptNumber,
          ItemCode: p.ItemCode,
          NGCode: p.NGCode,
          Qty: p.Qty,
        };
      });

      this.approvedClaim();
    },
    getClaim: function () {
      this.ds.loadDetail(this.filter.ClaimId).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.filter.SupplierCode = this.model.SupplierCode;
        this.filter.PeriodFrom = this.model.ClaimDateFrom;
        this.filter.PeriodUntil = this.model.ClaimDateTo;
        this.$nextTick(() => setTimeout(() => this.searchPoDetail(), 500));
      });
    },
    searchPoDetail: function () {
      if (!this.filter.SupplierCode) {
        toastDanger("Silahkan pilih Supplier!");
        return;
      }
      if (!this.filter.ClaimId) {
        toastDanger("Silahkan pilih Claim Number!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          FactoryCode: this.filter.FactoryCode,
          ClaimId: this.filter.ClaimId?.toString() || "0",
          SupplierCode: this.filter.SupplierCode,
          DateFrom: this.filter.PeriodFrom,
          DateUntil: this.filter.PeriodUntil,
        },
      ];
      this.ds.listNGClaimDetail(filters).then((dt) => {
        this.listNGClaimDetail = dt.Data.Items;
        this.listNGClaimDetail.map((x) => (x.Selected = x.DetailID > 0));
      });
    },

    approvedClaim: function () {
      this.ds
        .approve(this.model)
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
</style>
