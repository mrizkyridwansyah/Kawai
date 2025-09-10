<template>
  <header-menu title="Quality Check (IQC)" :breadcrumbs="this.breadcrumbs" />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-2 col-md-2 col-sm-2 col-2 mr-6">
        <label class="form-label">Supplier</label>
        <input-trade
          class="form-control"
          placeholder="Search Supplier"
          v-model="filter.supplier"
          trade-cls="2"
          :show-option-all="true"
        />
      </div>
      <div class="col-lg-2 col-md-2 col-sm-2 col-2 mr-6">
        <label class="form-label">DN No</label>
        <input-dnno-supplier-from-to
          class="form-control"
          placeholder="Search DNNo No"
          v-model="filter.dnno"
          :supplier="filter.supplier"
          :receiptdatefrom="filter.receiptdatefrom"
          :receiptdateto="filter.receiptdateto"
          :show-option-all="true"
        />
      </div>
      <div class="col-lg-2 col-md-2 col-sm-2 col-2 mr-6">
        <label class="form-label">Item</label>
        <input-item
          class="form-control"
          placeholder="Search Item"
          v-model="filter.item"
          :show-option-all="true"
        />
      </div>
      <div class="col-lg-2 col-md-2 col-sm-2 col-2 mr-6">
        <label class="form-label">QC Status</label>
        <input-dropdown
          :options="[
            { value: '0', text: 'Pending' },
            { value: '1', text: 'NG' },
            { value: '2', text: 'Passed' },
          ]"
          textField="text"
          valueField="value"
          v-model="filter.qcstatus"
          placeholder=" QC Status"
        />
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-2 col-md-2 col-sm-2 col-2 mr-6">
        <label class="form-label">Receipt Date From</label>
        <input-date
          placeholder="ReceiptDateFrom"
          v-model="filter.receiptdatefrom"
          :errors="errors?.ReceiptDateFrom"
        />
      </div>
      <div class="col-lg-2 col-md-2 col-sm-2 col-2 mr-6">
        <label class="form-label">Receipt Date To</label>
        <input-date
          placeholder="ReceiptDateTo"
          v-model="filter.receiptdateto"
          :errors="errors?.ReceiptDateTo"
        />
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
    </div>
  </div>
  <v-table :filter="filter" :keyword-keys="keywordKeys" :ds="ds">
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle"
        style="width: 100%"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">DN No</th>
            <th class="text-center">Item Code</th>
            <th class="text-center">Description</th>
            <th class="text-center">Qty</th>
            <th class="text-center">Inspection Date</th>
            <th class="text-center">Inspector</th>
            <th class="text-center">QC Status</th>
            <th class="text-center">Photo</th>
            <th class="text-center">Remarks</th>
            <th class="text-center">Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
            <td>{{ item.DN_No }}</td>
            <td>{{ item.Item_Code }}</td>
            <td>{{ item.Item_Name }}</td>
            <td class="text-center">{{ item.Qty }}</td>
            <td class="text-center">
              {{ $func.formatDateTime(item.Inspection_Date) }}
            </td>
            <td class="text-center">{{ item.Inspection_User }}</td>
            <td class="text-center">
              <span
                class="qc-status"
                :class="item.QC_Status_Descs.toLowerCase()"
              >
                {{ item.QC_Status_Descs }}
              </span>
            </td>
            <td class="text-center">
              <a
                href="javascript:void(0)"
                class="ml-2 text-blue"
                @click="viewphoto(item)"
              >
                View Photo
              </a>
            </td>
            <td>{{ item.Remarks }}</td>
            <td class="text-center">
              <button
                type="button"
                class="btn btn-sm btn-green btn-elevate"
                @click="edit(item)"
              >
                Confirm
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
  <v-modal
    ref="modalQualityCheck"
    id="modal-form-qualitycheck-confirm"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formQualityCheck.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-qualitycheck-confirm
      ref="formQualityCheck"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>

  <v-modal
    ref="modalQualityCheckPhoto"
    id="modal-form-qualitycheck-photo"
    title="QC Photo"
    size="800"
    @hidden="
      () => {
        this.$refs.formQualityCheckPhoto.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-qualitycheck-photo
      ref="formQualityCheckPhoto"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Quality Control", active: false, to: "" },
      {
        title: "Quality Check (IQC)",
        active: true,
        to: "/app/quality/qualitycheck",
      },
    ],
    filter: {
      supplier: null,
      item: null,
      dnno: null,
      item: null,
      qcstatus: null,
      keyword: null,
      sorts: {
        Item_Name: "asc",
      },
      sortItems: [
        {
          label: "DN No",
          value: "DN_No",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "Item_Code",
          selected: true,
          direction: "desc",
        },
        {
          label: "Description",
          value: "Item_Name",
          selected: true,
          direction: "desc",
        },
        {
          label: "QC Status",
          value: "QC_Status",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
  }),
  computed: {
    ds: function () {
      return useQualityCheck();
    },
  },
  watch: {
    "filter.supplier": function () {
      this.ds.data.Items = [];
    },
    "filter.dnno": function () {
      this.ds.data.Items = [];
    },
    "filter.item": function () {
      this.ds.data.Items = [];
    },
    "filter.qcstatus": function () {
      this.ds.data.Items = [];
    },
    "filter.receiptdatefrom": function () {
      this.ds.data.Items = [];
    },
    "filter.receiptdateto": function () {
      this.ds.data.Items = [];
    },
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          SupplierCode: this.filter.supplier || "",
          DNNo: this.filter.dnno || "",
          ItemCode: this.filter.item || "",
          QCStatus: this.filter.qcstatus || "",
          ReceiptDateFrom: this.filter.receiptdatefrom || "",
          ReceiptDateTo: this.filter.receiptdateto || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.supplier = null;
      this.filter.dnno = null;
      this.filter.item = null;
      this.filter.qcstatus = null;
      this.filter.receiptdatefrom = null;
      this.filter.receiptdateto = null;

      this.search();
    },
    edit: function (dt) {
      this.title = "IQC Confirm";
      this.modalMode = "edit";
      this.idSelected = dt.Id;
      this.$bvModal.show("modal-form-qualitycheck-confirm");
    },
    viewphoto: function (dt) {
      this.title = "QC Photo";
      this.modalMode = "edit";
      this.idSelected = dt.Id;
      this.$bvModal.show("modal-form-qualitycheck-photo");
    },
    close: function () {
      this.search();
    },
  },
};
</script>

<style>
thead {
  white-space: nowrap;
}
.qc-status {
  display: inline-block;
  padding: 4px 14px;
  border-radius: 50px; /* Biar bulat/oval */
  font-size: 0.85rem;
  font-weight: bold;
  text-align: center;
  min-width: 70px;
}
.qc-status.passed {
  background-color: #d4f5df; /* hijau muda */
  color: #2e7d32; /* hijau teks */
}

.qc-status.ng {
  background-color: #fde2e2; /* merah muda */
  color: #c62828; /* merah teks */
}

.qc-status.pending {
  background-color: #eee346f5; /* kuning */
  color: #6d3c04e8; /* coklat teks */
}
</style>
