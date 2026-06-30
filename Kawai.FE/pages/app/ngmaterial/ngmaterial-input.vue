<template>
  <v-frame title="NG Claim Material Input" icon="receipt">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Supplier</label>
          <filter-trade-2
            v-model="filter.SupplierCode"
            :trade-cls="['2', '3']"
            :disabled="filter.ClaimId != null"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Claim Date</label>
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
            :disabled="isNew"
            status="NEW"
            :supplier-code="filter.SupplierCode"
            :period-from="filter.PeriodFrom"
            :period-until="filter.PeriodUntil"
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
              :disabled="!menuPrivAllowUpdate"
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
          <v-button-print
            label="Print Surat Jalan"
            class="mr-1"
            icon="print"
            :print="printSuratJalan"
            :is-loading="isLoading"
              :disabled="!menuPrivAllowUpdate"
          />
        </div>
      </div>
      <hr />
      <v-table-input
        :data-items="listPODetail"
        :frozen-column-left="3"
        ref="vtable"
        :top-content-height="400"
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
                <tr v-for="(item, idx) in listPODetail || []" :key="idx">
                  <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                      @click="(e) => check(e, item)"
                    />
                  </td>
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
                (!listPODetail || listPODetail.length === 0)
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
    menuPrivAllowUpdate: false,
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
    listPODetail: [],
    debounce: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useNGClaim();
    },
     dsMenu: function () {
      return useMenu();
    },
  },
  watch: {
    "filter.SupplierCode": function () {
      this.listPODetail = [];
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
      this.listPODetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.PeriodUntil = today;

      if (this.filter.ClaimId) this.getClaim();
    },
  },
  mounted: function () {
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "F01",
      )[0].AllowUpdate;
    });
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
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
      this.listPODetail = [];
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.PeriodUntil = today;
    },
    changeNew: function (e) {
      if (e.target.checked) this.reset();
    },
    checkAll: function (e) {
      this.listPODetail.map((x) => (x.Selected = e.target.checked));
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
    },
    printSuratJalan: function () {
      this.ds
        .PrintSuratJalan1(this.filter.SupplierCode, this.filter.ClaimId)
        .then((dt) => {
          toastSuccess("Download successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    submit: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listPODetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Please Choose Receipt No!");
        return;
      }

      this.model.ClaimId = this.filter.ClaimId;
      this.model.SupplierCode = this.filter.SupplierCode;
      this.model.Details = details.map((p) => {
        return {
          PONumber: p.ReceiptNumber,
          ReceiptNumber: p.ReceiptNumber,
          ItemCode: p.ItemCode,
          NGCode: p.NGCode,
          Qty: p.Qty,
        };
      });

      if (this.isNew) {
        this.createClaim();
      } else {
        this.updateClaim();
      }
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
        toastDanger("Please Select Supplier!");
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
      this.ds.listPODetail(filters).then((dt) => {
        this.listPODetail = dt.Data.Items;
        this.listPODetail.map((x) => (x.Selected = x.DetailID > 0));
      });
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
          toastDanger(err?.Message);
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
