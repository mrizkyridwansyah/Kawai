<template>
  <v-frame title="IQC Result Approval" icon="list-check">
    <template #frame-content>
      <div class="row">
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-12 col-12">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Factory</label>
            <input-factory-privileges v-model="filter.FactoryCode" />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-12 col-12">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Status</label>
            <input-iqc-status
              placeholder="Search Status"
              v-model="filter.Status"
            />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-12 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Supplier</label>
            <input-trade
              placeholder="Search Supplier"
              v-model="filter.SupplierCode"
              :trade-cls="['2', '3']"
              :show-option-all="true"
            />
          </div>
        </div>
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-12 col-12 mt-2">
          <div class="mr-1" style="width: 100%">
            <label class="form-label">Source</label>
            <input-iqc-source
              placeholder="Search Source"
              v-model="filter.Source"
            />
          </div>
        </div>
        <div class="col-12"></div>
        <div
          class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-xs-12 col-12 mt-2"
        >
          <label class="form-label">Trans Date</label>
          <div class="mr-1" style="width: 100%; display: flex">
            <input-date v-model="filter.PeriodFrom" />
            <label class="form-label ml-2 mr-2 mt-2">s/d</label>
            <input-date v-model="filter.PeriodUntil" class="ml-2" />
            <div class="d-flex ml-2">
              <div class="d-flex flex-fill">
                <button
                  class="btn btn-sm btn-blue btn-elevate mr-1"
                  @click="search"
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
              </div>
            </div>
          </div>
        </div>
      </div>

      <v-table-input :data-items="lists" :frozen-column-left="3" ref="vtable">
        <template #table-content>
          <div class="detail-content">
            <table
              class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
              v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
              ref="table"
            >
              <thead>
                <tr>
                  <th class="text-center">Source</th>
                  <th class="text-center">Supplier</th>
                  <th class="text-center">DN Number</th>
                  <th class="text-center">DN Date</th>
                  <th class="text-center">Item Code</th>
                  <th class="text-center">Item Name</th>
                  <th class="text-center">Unit</th>
                  <th class="text-center">Sample Qty</th>
                  <th class="text-center">NG Qty</th>
                  <th class="text-center">QC Status</th>
                  <th class="text-center">Approve</th>
                  <th class="text-center">Approval User</th>
                  <th class="text-center">Approval Date</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in lists || []" :key="idx">
                  <td class="text-center">
                    {{
                      item.Source == "Incoming Material"
                        ? "IQC Sample"
                        : item.Source
                    }}
                  </td>
                  <td>{{ item.SupplierName }}</td>
                  <td>{{ item.DNNumber }}</td>
                  <td>{{ $func.formatDate(item.DNDate) }}</td>
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>{{ item.UnitClsDescription }}</td>
                  <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.QtyNG) }}
                  </td>
                  <td>{{ item.InspectionResult }}</td>
                  <td class="text-center">
                    <a
                      href="javascript:void(0);"
                      v-if="
                        item.InspectionResult != null &&
                        item.InspectionResult != ''
                      "
                      @click="() => showModal(item, 'VIEW')"
                    >
                      View
                    </a>
                    <a
                      href="javascript:void(0);"
                      v-else
                      @click="() => showModal(item, 'CONFIRM')"
                    >
                      Confirm
                    </a>
                  </td>
                  <td>{{ item.ApprovalUserName }}</td>
                  <td>{{ $func.formatDateTime(item.ApprovalDate) }}</td>
                </tr>
              </tbody>
            </table>
            <v-data-empty
              class="mt-3"
              v-if="
                !ds.isLoading &&
                lists?.length == 0 &&
                !ds.isNetworkError &&
                !ds.isServerError
              "
            />
          </div>
        </template>
      </v-table-input>
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
    init: true,
    filter: {
      keyword: null,
      FactoryCode: null,
      SupplierCode: null,
      Status: null,
      Source: null,
      PeriodFrom: null,
      PeriodUntil: null,
    },
    lists: [],
    idSelected: null,
    title: "",
    modalMode: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useQualityCheck();
    },
    notif: function () {
      return useNotification();
    },
  },
  watch: {
    filter: {
      deep: true,
      handler: function (after) {
        if (this.debounce) clearTimeout(this.debounce);
        this.debounce = setTimeout(() => (this.lists = []), 800);
      },
    },
  },
  mounted: function () {
    let today = new Date();
    this.filter.PeriodFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.PeriodUntil = today;
    this.filter.Source = "ALL";
    this.filter.Status = "ALL";
  },
  methods: {
    load: function () {
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          SupplierCode: this.filter.SupplierCode || "",
          Source: this.filter.Source || "",
          StatusInspection: this.filter.Status || "",
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom)
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil)
          ),
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load().then((dt) => (this.lists = dt.Data.Items));
    },
    search: function () {
      if (!this.filter.SupplierCode) {
        toastDanger("Silahkan pilih Supplier!");
        return;
      }

      this.$nextTick(() => setTimeout(() => this.load(), 500));
    },
    showModal: function (dt, mode) {
      this.title = "IQC Result Approval";
      this.modalMode = mode;
      this.idSelected = dt.InspectionId;
      this.$bvModal.show("modal-form-iqc-result");
    },
    close: function () {
      this.$bvModal.hide("modal-form-iqc-result");
      this.search();
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
