<template>
  <v-frame title="IQC Result Input" icon="list-check">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Factory</label></td>
          <td style="padding-left: 15px">
            <filter-factory-privileges
              class="form-control"
              v-model="filter.FactoryCode"
              style-code="width: 140px;"
              style-desc="width: 250px;"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Receipt Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-date v-model="filter.PeriodFrom" style-date="width:120px" />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label
              class="form-label"
              style="padding-top: 5px; padding-right: 15px"
              >To</label
            >
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-date v-model="filter.PeriodUntil" style-date="width:120px" />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Supplier</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <filter-trade-2
              class="form-control"
              v-model="filter.SupplierCode"
              :trade-cls="['2', '3']"
              :show-option-all="true"
              style-code="width: 140px;"
              style-desc="width: 250px;"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Status</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-iqc-status
              class="form-control"
              v-model="filter.Status"
              style="width: 140px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Source</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-iqc-source
              class="form-control"
              v-model="filter.Source"
              style="width: 140px"
            />
          </td>
        </tr>
        <tr>
          <td colspan="4" style="padding-top: 5px">
            <v-button-search-reset :search="search" :reset="resetFilter" />
          </td>
        </tr>
      </table>
      <hr />
      <div>
        <v-table-input
          :data-items="lists"
          :frozen-column-left="3"
          ref="vtable"
          :top-content-height="290"
        >
          <template #table-content>
            <div class="detail-content">
              <table
                class="table table-bordered mb-0 align-middle v-fixed-table"
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
                    <th class="text-center">Receipt Qty</th>
                    <th class="text-center">Sample Qty</th>
                    <th class="text-center">NG Qty</th>
                    <th class="text-center">Result Input & View</th>
                    <th class="text-center">Register User</th>
                    <th class="text-center">Register Date</th>
                    <th class="text-center">Inspection User</th>
                    <th class="text-center">Inspection Date</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, idx) in lists || []" :key="idx">
                    <td
                      class="text-center"
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{
                        item.Source == "Incoming Material"
                          ? "IQC Sample"
                          : item.Source
                      }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.SupplierName }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.DNNumber }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ $func.formatDate(item.DNDate) }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.ItemCode }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.ItemName }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.UnitClsDescription }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                      class="text-right"
                    >
                      {{ $func.formatMoney(item.QtyReceipt) }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                      class="text-right"
                    >
                      {{ $func.formatMoney(item.Qty) }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                      class="text-right"
                    >
                      {{ $func.formatMoney(item.QtyNG) }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                      class="text-center"
                    >
                      <a
                        href="javascript:void(0);"
                        v-if="item.StatusQC == 'CONFIRMED' || item.StatusQC == 'PENDING-SA'"
                        @click="() => showModal(item, 'VIEW')"
                      >
                        View
                      </a>
                      <a
                        href="javascript:void(0);"
                        v-else-if="
                          item.StatusQC == 'NEW' || item.StatusQC == 'INPUT'
                        "
                        @click="() => showModal(item, 'INPUT')"
                      >
                        Input
                      </a>
                      <a href="javascript:void(0);" v-else></a>
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.RegisterUserName }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ $func.formatDateTime(item.RegisterDate) }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ item.InspectorName }}
                    </td>
                    <td
                      :class="{
                        'bg-danger': item.StatusQC != 'NEW' && item.QtyNG > 0,
                        'bg-success': item.StatusQC != 'NEW' && item.QtyNG == 0,
                        'table-striped-row':
                          item.StatusQC == 'NEW' && (idx+1) % 2 == 0,
                      }"
                    >
                      {{ $func.formatDateTime(item.InspectionDate) }}
                    </td>
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
        this.debounce = setTimeout(() => (this.lists = []), 200);
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
    resetFilter: function () {
      let today = new Date();
      this.filter.PeriodFrom = new Date(
        today.getFullYear(),
        today.getMonth(),
        1,
      );
      this.filter.FactoryCode = null;
      this.filter.SupplierCode = null;
      this.filter.PeriodUntil = today;
      this.filter.Source = "ALL";
      this.filter.Status = "ALL";
    },
    load: function () {
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          SupplierCode: this.filter.SupplierCode || "",
          Source: this.filter.Source || "",
          StatusInspection: this.filter.Status || "",
          PeriodFrom: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodFrom),
          ),
          PeriodUntil: this.$func.asUtcStringDateOnly(
            new Date(this.filter.PeriodUntil),
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

      if (
        new Date(this.filter.PeriodFrom) > new Date(this.filter.PeriodUntil)
      ) {
        toastWarning("Periode Dari tidak boleh melewati Periode Sampai.");
        return;
      }

      this.$nextTick(() => setTimeout(() => this.load(), 500));
    },
    showModal: function (dt, mode) {
      this.title = "IQC Detail";
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

.bg-danger {
  background-color: salmon !important;
}
.bg-danger a {
  color: #333;
}

.bg-success {
  background-color: rgb(19, 222, 185) !important;
  color: #333 !important;
}
.bg-success a {
  color: #333;
}

.table-striped-row {
  background-color: #e9ecef;
}

.bg-danger a {
  background-color: rgba(250, 128, 114, 0.5) !important;
}

.table tbody td {
  border-bottom: 1px solid #ced4da;
}
</style>
