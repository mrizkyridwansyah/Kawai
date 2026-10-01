<template>
  <table class="mb-3">
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Work Station</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <filter-workstation
          class="form-control"
          v-model="filter.workstation"
          :show-option-all="true"
          default-option-all="ALL"
          style-code="width: 150px"
          style-desc="width: 250px"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Group Part</label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <filter-group-part-privileges
          class="form-control"
          v-model="filter.group"
          :show-option-all="true"
          default-option-all="ALL"
          style-code="width: 150px"
          style-desc="width: 250px"
        />
      </td>
    </tr>
  </table>

  <div class="table-wrapper">
    <table class="table table-striped table-bordered mb-0 align-middle w-100">
      <thead>
        <tr>
          <th class="text-center" style="width: 40px">
            <input-checkbox
              no-group
              :modelValue="isAllChecked"
              @update:modelValue="(checked) => checkAll(checked)"
              :disabled="!items || items.length == 0"
            />
          </th>
          <th class="text-center">Item</th>
          <th class="text-center">Workstation</th>
          <th class="text-center">Child Cls</th>
          <th class="text-center">Barcode</th>
          <th class="text-center">Qty</th>
          <th class="text-center">Picking No.</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="isLoadingData">
          <td colspan="7" class="text-center">Loading...</td>
        </tr>
        <tr v-else v-for="item in items || []" :key="item.BarcodeNo">
          <td class="text-center">
            <input-checkbox
              no-group
              :modelValue="isChecked(item.BarcodeNo)"
              @update:modelValue="(checked) => check(checked, item)"
            />
          </td>
          <td>{{ item.ItemCode }} - {{ item.ItemName }}</td>
          <td>{{ item.WorkStationCode }} - {{ item.WorkStationName }}</td>
          <td>{{ item.GroupCode }} - {{ item.GroupName }}</td>
          <td>{{ item.BarcodeNo }}</td>
          <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
          <td>{{ item.PickingNo }}</td>
        </tr>
        <tr v-if="!isLoadingData && (!items || items.length == 0)">
          <td colspan="7" class="text-center">No data available</td>
        </tr>
      </tbody>
    </table>
  </div>
  <div class="d-flex justify-content-end mt-4 mb-3">
    <v-button
      :action="cancel"
      label="Cancel"
      icon="xmark"
      cClass="ml-1 btn-danger"
      :is-loading="isLoading"
    />
    <v-button
      :action="print"
      label="Print"
      icon="print"
      cClass="ml-1 btn-green"
      :is-loading="isLoading"
      :disabled="selectedPrint.length == 0"
    />
  </div>
</template>

<script>
export default {
  props: ["requestId", "counter"],
  emits: ["cancel", "printed"],
  data: () => ({
    isLoading: false,
    isLoadingData: false,
    items: [],
    selectedPrint: [],
    filter: {
      workstation: "ALL",
      group: "ALL",
    },
  }),
  computed: {
    ds: function () {
      return useSupplyRequestWomin();
    },
    isAllChecked: function () {
      return (
        !!this.items &&
        this.items.length > 0 &&
        this.selectedPrint.length == this.items.length
      );
    },
  },
  watch: {
    counter: function () {
      this.search();
    },
    "filter.workstation": function () {
      this.search();
    },
    "filter.group": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    search: function () {
      this.reset();
      this.items = [];
      if (!this.requestId) return;

      this.isLoadingData = true;
      this.ds
        .loadListScanBarcode(
          this.requestId,
          this.filter.workstation,
          this.filter.group,
        )
        .then((data) => (this.items = data.Data || []))
        .catch((err) => toastDanger(err?.Message))
        .finally(() => (this.isLoadingData = false));
    },
    reset: function () {
      this.selectedPrint = [];
    },
    isChecked: function (barcode) {
      return this.selectedPrint.includes(barcode);
    },
    check: function (checked, item) {
      if (checked) {
        if (!this.isChecked(item.BarcodeNo))
          this.selectedPrint.push(item.BarcodeNo);
      } else {
        this.selectedPrint = this.selectedPrint.filter(
          (b) => b != item.BarcodeNo,
        );
      }
    },
    checkAll: function (checked) {
      this.selectedPrint = checked
        ? (this.items || []).map((item) => item.BarcodeNo)
        : [];
    },
    cancel: function () {
      this.reset();
      this.$emit("cancel");
    },
    print: function () {
      if (this.selectedPrint.length == 0) {
        toastWarning("Please select barcode to print!");
        return;
      }

      this.isLoading = true;
      this.ds
        .print(this.selectedPrint)
        .then((data) => {
          if (data?.Message && data.Message != "-") toastInfo(data.Message);
          this.reset();
          this.$emit("printed");
        })
        .catch((err) => toastDanger(err?.Message))
        .finally(() => (this.isLoading = false));
    },
  },
};
</script>

<style scoped>
.table-wrapper {
  max-height: 450px;
  overflow-y: auto;
  width: 100%;
}

.table-wrapper thead th {
  position: sticky;
  top: 0;
  background: #fff;
  z-index: 10;
}
</style>
