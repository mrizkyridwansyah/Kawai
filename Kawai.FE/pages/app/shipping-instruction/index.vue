<template>
  <v-frame title="Shipping Instruction" icon="receipt">
    <template #frame-content>
      <div class="shipping-instruction-page">
        <table class="filter-table">
          <tr>
            <td style="padding-top: 5px">
              <label class="form-label">Customer</label>
            </td>
            <td style="padding-top: 5px; padding-left: 15px" colspan="3">
             <filter-trade-cust
                class="form-control"
                v-model="filter.customer"
                :placeholder="'Select Customer'"
                style-code="width: 170px;"
                style-desc="width: 250px;"
              />
            </td>
          </tr>
          <tr>
            <td><label class="form-label">Delivery Date</label></td>
            <td style="padding-left: 15px; padding-top: 5px">
              <input-date v-model="filter.deliveryDateFrom" style-date="width: 130px !important" />
            </td>
            <td style="padding-top: 5px">
              <label class="form-label">To</label>
            </td>
            <td style="padding-left: 15px; padding-top: 5px">
              <input-date v-model="filter.deliveryDateTo" style-date="width: 130px !important" />
            </td>
          </tr>
          <tr>
            <td><label class="form-label">Order Number</label></td>
            <td colspan="3">
              <filter-order-entry
                class="form-control"
                v-model="filter.orderNumber"
                :customer="filter.customer"
                :date-from="filter.deliveryDateFrom"
                :date-to="filter.deliveryDateTo"
                :on-select="onOrderEntrySelect"
                :placeholder="'Select Order Number'"
              />
            </td>
          </tr>
          <tr>
            <td style="padding-top: 5px">
              <label class="form-label">Shipping Instruction No.</label>
            </td>
            <td colspan="3">
              <table>
                <tr>
                  <td>
                      <input
                        type="text"
                        v-model="filter.shippingInstructionNo"
                        class="form-control form-control-sm field-compact input-white"
                        :readonly="true"
                      />     
                  </td>
                  <td>              
                      <label class="ms-2 d-flex align-items-center new-check-label">
                        <input type="checkbox" v-model="filter.isNew" />
                        <span class="ms-1">New</span>
                      </label>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td><label class="form-label">Shipping Instruction Date</label></td>
            <td colspan="3"> 
              <div class="d-flex align-items-center">
                <input-date v-model="filter.shippingInstructionDate" style-date="width: 130px !important" />
                 <v-button-search-reset :search="search" :reset="reset" :disabled="isLoadingSearch" />
              </div>
            </td>
          </tr>
          <tr>
            <td class="pt-2" colspan="2" style="padding-top: 5px">
              <div class="d-flex flex-fill">
                <v-button-submit
                  :submit="submit"
                  cClass="mr-1"
                  :is-loading="isLoading"
                />
                
                <v-button-print
                  label="Print"
                  class="mr-1"
                  :print="print"
                  :is-loading="isLoading"
                />
              </div>
            </td>
          </tr>
        </table>

        <div class="mt-2">
          <v-table-input :data-items="shippingRows" ref="vtable">
            <template #table-content>
              <div class="detail-content" style="width: 100%">
                <table class="table table-bordered mb-0 align-middle v-fixed-table w-100 shipping-table">
                  <thead>
              <tr>
                <th class="text-center check-col">
                  <input
                    type="checkbox"
                    :checked="isAllRowsSelected()"
                    @change="onSelectAllRows($event)"
                  />
                </th>
                      <th class="text-center">Part Number</th>
                      <th class="text-center">Description</th>
                      <th class="text-center">Unit</th>
                      <th class="text-center">Qty Shipping</th>
                      <th class="text-center">Delivery Date</th>
                      <th class="text-center">Qty Stock</th>
                      <th class="text-center">Qty Picking</th>
                      <th class="text-center">Serial No</th>
                      <th class="text-center">Address</th>
                      <th class="text-center">Picking Date</th>
                      <th class="text-center">Time</th>
                      <th class="text-center">Picking By</th>
                    </tr>
                  </thead>
                  <tbody>
                    <template v-for="(item, idx) in shippingRows" :key="idx">
                      <tr>
                        <td class="text-center">
                          <input type="checkbox" v-model="item.selected" @change="onRowSelectionChange(item)" />
                        </td>
                        <td>{{ item.partNumber }}</td>
                        <td>{{ item.description }}</td>
                        <td>{{ item.unit }}</td>
                        <td class="text-right">{{ item.qtyShipping }}</td>
                        <td>{{ item.deliveryDate }}</td>
                        <td class="text-right">{{ item.qtyStock }}</td>
                        <td class="text-right">{{ item.qtyPicking }}</td>
                        <td>
                          <a href="javascript:void(0)" @click="openDetail(item)">
                            {{ serialSummary(item.serials) }}
                          </a>
                          <button
                            v-if="hasMultipleSerials(item)"
                            type="button"
                            class="serial-toggle-btn ms-1"
                            @click.stop="toggleSerialDetail(item)"
                            :aria-label="item.serialExpanded ? 'Collapse serial details' : 'Expand serial details'"
                          >
                            {{ item.serialExpanded ? '-' : '+' }}
                          </button>
                        </td>
                        <td>{{ item.serials[0]?.address || '' }}</td>
                        <td>{{ item.serials[0]?.pickingDate || '' }}</td>
                        <td>{{ item.serials[0]?.time || '' }}</td>
                        <td>{{ item.serials[0]?.pickingBy || '' }}</td>
                      </tr>
                      <tr
                        v-for="(serial, serialIdx) in serialDetailRows(item)"
                        :key="`${idx}-${serialIdx}`"
                        class="detail-row"
                      >
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td class="serial-highlight">{{ serial.serialNo }}</td>
                        <td class="serial-highlight">{{ serial.address }}</td>
                        <td class="serial-highlight">{{ serial.pickingDate }}</td>
                        <td class="serial-highlight">{{ serial.time }}</td>
                        <td class="serial-highlight">{{ serial.pickingBy }}</td>
                      </tr>
                    </template>
                  </tbody>
                </table>
              </div>
            </template>
          </v-table-input>
        </div>
      </div>
    </template>
  </v-frame>

  <v-modal title="Detail Picking" class="modal-lg" id="modal-shipping-detail" @hidden="closeDetailModal">
    <div class="detail-modal" v-if="selectedRow">
      <table class="detail-form mb-3">
        <tr>
          <td><label class="form-label">Part Number</label></td>
          <td>
            <input class="form-control form-control-sm" :value="selectedRow.partNumber" readonly />
          </td>
        </tr>
        <tr>
          <td><label class="form-label">Description</label></td>
          <td>
            <input class="form-control form-control-sm" :value="selectedRow.description" readonly />
          </td>
        </tr>
      </table>

      <table class="table table-bordered mb-2 align-middle shipping-table">
        <thead>
          <tr>
            <th class="text-center check-col"></th>
            <th class="text-center">Serial No</th>
            <th class="text-center">Address</th>
            <th class="text-center">Picking Date</th>
            <th class="text-center">Time</th>
            <th class="text-center">Picking By</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(serial, idx) in selectedRow.serials" :key="idx">
            <td class="text-center">
              <input
                type="checkbox"
                v-model="serial.selected"
                :disabled="serial.isUpdating || serial.isPicking"
                @change="onSerialSelectionChange(selectedRow, serial)"
              />
            </td>
            <td>{{ serial.serialNo }}</td>
            <td>{{ serial.address }}</td>
            <td>{{ serial.pickingDate }}</td>
            <td>{{ serial.time }}</td>
            <td>{{ serial.pickingBy }}</td>
          </tr>
        </tbody>
      </table>

      <div class="text-end">
        <v-button class="btn-success" icon="save" label="Submit" @click="submitDetail" :disabled="isSubmittingDetail" />
      </div>
    </div>
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    filter: {
      customer: 'ALL',
      deliveryDateFrom: null,
      deliveryDateTo: null,
      orderNumber: null,
      shippingInstructionNo: null,
      shippingInstructionDate: null,
      isNew: true,
    },
    shippingRows: [],
    selectedRow: null,
    isLoading: false,
    isLoadingSearch: false,
    isSubmitting: false,
    isSubmittingDetail: false,
  }),
  mounted() {
    const today = new Date();
    this.filter.deliveryDateFrom = new Date(today.getFullYear(), today.getMonth(), 1);
    this.filter.deliveryDateTo = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    this.filter.shippingInstructionDate = today;
  },
  watch: {
    "filter.isNew"(isNew) {
      if (isNew) this.filter.shippingInstructionNo = null;
      this.shippingRows = (this.shippingRows || []).map((row) => ({
        ...row,
        selected: this.isPickingChecked(row.isPicking),
        initialSelected: this.isPickingChecked(row.isPicking),
        serialExpanded: this.isPickingChecked(row.isPicking),
      }));
    },
    "filter.orderNumber"(value) {
      if (!value) this.filter.shippingInstructionNo = null;
    },
  },
  methods: {
    isPickingChecked(value) {
      if (value === true || value === false) return value;
      const normalized = String(value ?? '').trim().toLowerCase();
      return normalized === '1' || normalized === 'true' || normalized === 'y' || normalized === 'yes';
    },
    onOrderEntrySelect(selected) {
      if (!selected) {
        this.filter.customer = 'ALL';
        this.filter.isNew = true;
        this.filter.shippingInstructionNo = null;
        return;
      }

      // this.filter.customer = selected.Cust_Code || 'ALL';
      const selectedSiNo = (selected.SI_NO || selected.SI_No || '').trim();
      const hasSiNo = !!selectedSiNo;
      
      this.filter.isNew = !hasSiNo;
      this.filter.shippingInstructionNo =  selectedSiNo;
    },
    mapRows(items = []) {
      return (items || []).map((item) => ({
        isPicking: item.IsPicking ?? item.isPicking ?? 0,
        selected: this.isPickingChecked(item.IsPicking ?? item.isPicking ?? 0),
        initialSelected: this.isPickingChecked(item.IsPicking ?? item.isPicking ?? 0),
        serialExpanded: this.isPickingChecked(item.IsPicking ?? item.isPicking ?? 0),
        siNo: item.SINo || item.SI_NO || item.SI_No || '',
        siDate: this.formatDate(item.SIDate || item.SI_Date),
        poNo: item.PONo || item.PO_No || '',
        seqNo: Number(item.SeqNo || item.PO_SeqNo || 0),
        itemCode: item.ItemCode || item.Item_Code || '',
        serialNoFrom: item.SerialNoFrom || item.SerialNo_From || '',
        serialNoTo: item.SerialNoTo || item.SerialNo_To || '',
        partNumber: item.PartNumber || item.ItemCode || item.Item_Code || '-',
        description: item.Description || item.Item_Name || '-',
        unit: item.Unit || item.Unit_Desc || '-',
        qtyShipping: Number(item.QtyShipping || item.Qty_Shipping || 0),
        deliveryDate: this.formatDate(item.DeliveryDate || item.PO_DelivDate),
        qtyStock: Number(item.QtyStock || item.Qty_Stock || 0),
        qtyPicking: Number(item.QtyPicking || item.Qty_Picking || 0),
        serials: (item.Serials || []).map((serial) => ({
          isPicking: serial.IsPicking ?? serial.isPicking ?? 0,
          isUpdating: false,
          selected: this.isPickingChecked(serial.IsPicking ?? serial.isPicking ?? 0),
          serialNo: serial.SerialNo || serial.Serial_No || '-',
          address: serial.Address || '',
          pickingDate: this.formatDate(serial.PickingDate || serial.Picking_Date, ''),
          time: serial.PickingTime || serial.Picking_Time || this.formatTime(serial.PickingDate || serial.Picking_Date, ''),
          pickingBy: serial.PickingBy || serial.Picking_By || '',
        })),
      }));
    },
    formatDate(value, fallback = '-') {
      if (!value) return fallback;
      const dt = new Date(value);
      if (Number.isNaN(dt.getTime())) return fallback;

      const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      const day = String(dt.getDate()).padStart(2, '0');
      const month = months[dt.getMonth()];
      const year = String(dt.getFullYear()).slice(-2);

      return `${day}-${month}-${year}`;
    },
    formatTime(value, fallback = '') {
      if (!value) return fallback;
      const dt = new Date(value);
      if (Number.isNaN(dt.getTime())) return fallback;

      const hour = String(dt.getHours()).padStart(2, '0');
      const minute = String(dt.getMinutes()).padStart(2, '0');
      return `${hour}:${minute}`;
    },
    serialSummary(serials = []) {
      if (!serials.length) return '-';
      if (serials.length === 1) return serials[0].serialNo;
      return `${serials[0].serialNo}-${serials[serials.length - 1].serialNo}`;
    },
    hasMultipleSerials(item) {
      return (item?.serials?.length || 0) > 1;
    },
    onSelectAllRows(event) {
      const checked = !!event?.target?.checked;
      this.shippingRows = (this.shippingRows || []).map((row) => ({
        ...row,
        selected: checked,
        serialExpanded: checked,
      }));
    },
    isAllRowsSelected() {
      const rows = this.shippingRows || [];
      if (!rows.length) return false;
      return rows.every((row) => !!row.selected);
    },
    onRowSelectionChange(item) {
      item.serialExpanded = !!item?.selected;
    },
    toggleSerialDetail(item) {
      if (!this.hasMultipleSerials(item)) return;
      item.serialExpanded = !item.serialExpanded;
    },
    serialDetailRows(item) {
      if (!this.hasMultipleSerials(item)) return [];
      return item.serialExpanded ? item.serials : [];
    },
    async search() {
      const poNo = this.filter.orderNumber || '';

      if (!poNo) {
        toastWarning('Pilih Order Number terlebih dahulu.');
        return;
      }

      this.shippingRows = [];
      this.selectedRow = null;
      this.isLoadingSearch = true;
      try {
        const response = await this.$http.get('/shipping-instruction/list', {
          params: {
            poNo,
            isNew: this.filter.isNew,
          },
        });

        const items = response?.data?.Data || [];
        this.shippingRows = this.mapRows(items);

        if (!this.shippingRows.length) {
          toastInfo('Data tidak ditemukan.');
        }
      } catch (error) {
        this.shippingRows = [];
      } finally {
        this.isLoadingSearch = false;
      }
    },
    reset() {
      const today = new Date();
      this.filter.customer = 'ALL';
      this.filter.deliveryDateFrom = new Date(today.getFullYear(), today.getMonth(), 1);
      this.filter.deliveryDateTo = new Date(today.getFullYear(), today.getMonth() + 1, 0);
      this.filter.orderNumber = null;
      this.filter.shippingInstructionNo = null;
      this.filter.shippingInstructionDate = today;
      this.filter.isNew = true;

      this.shippingRows = [];
      this.selectedRow = null;
    },
    async submit() {
      const selectedRows = (this.shippingRows || []).filter((row) => row.selected && !row.initialSelected);
      if (!selectedRows.length) {
        toastWarning('Pilih minimal 1 data baru pada grid.');
        return;
      }

      if (!this.filter.shippingInstructionDate) {
        toastWarning('Shipping Instruction Date harus diisi.');
        return;
      }

      const siDate = this.$func.asUtcStringDateOnly(new Date(this.filter.shippingInstructionDate));
      const payload = selectedRows.map((row) => ({
        PONo: row.poNo,
        POSeqNo: row.seqNo,
        ItemCode: row.itemCode,
        SerialNoFrom: row.serialNoFrom,
        SerialNoTo: row.serialNoTo,
        SIDate: siDate,
      }));

      if (payload.some((x) => !x.PONo || !x.POSeqNo || !x.ItemCode)) {
        toastWarning('Data PO/Item pada baris terpilih belum valid.');
        return;
      }

      this.isSubmitting = true;
      try {
        await this.$http.post('/shipping-instruction/submit', payload);
        toastSuccess('Submit Shipping Instruction berhasil.');
        await this.search();
      } finally {
        this.isSubmitting = false;
      }
    },
    print() {
      toastSuccess('Print Shipping Instruction berhasil.');
    },
    openDetail(item) {
      this.selectedRow = item;
      this.$bvModal.show('modal-shipping-detail');
    },
    closeDetailModal() {
      if (this.selectedRow?.serials?.length) {
        this.selectedRow.serials.forEach((serial) => {
          serial.selected = this.isPickingChecked(serial.isPicking);
          serial.isUpdating = false;
        });
      }
      this.selectedRow = null;
    },
    onSerialSelectionChange(row, serial) {
      if (!serial?.selected) return;

      if ((Number(row?.qtyStock) || 0) <= 0) {
        toastWarning(`Tidak bisa memilih serial. Qty Stock untuk item ${row?.itemCode || '-'} masih 0.`);
        serial.selected = false;
      }
    },
    async submitDetail() {
      const row = this.selectedRow;
      if (!row) return;

      const serials = (row.serials || []).filter(
        (serial) => serial.selected && !this.isPickingChecked(serial.isPicking)
      );

      if (!serials.length) {
        toastWarning('Pilih minimal 1 serial untuk dipicking.');
        return;
      }

      if ((Number(row.qtyStock) || 0) <= 0) {
        toastWarning(`Tidak bisa picking. Qty Stock untuk item ${row.itemCode || '-'} masih 0.`);
        return;
      }

      const payload = serials.map((serial) => ({
        PONo: row.poNo,
        POSeqNo: row.seqNo,
        ItemCode: row.itemCode,
        SerialNo: serial.serialNo,
      }));

      if (payload.some((x) => !x.PONo || !x.POSeqNo || !x.ItemCode || !x.SerialNo)) {
        toastWarning('Data serial tidak valid untuk proses picking.');
        return;
      }

      this.isSubmittingDetail = true;
      serials.forEach((serial) => {
        serial.isUpdating = true;
      });

      try {
        await this.$http.post('/shipping-instruction/picking', payload);

        serials.forEach((serial) => {
          serial.isPicking = true;
          serial.selected = true;
        });

        row.qtyPicking = Number(row.qtyPicking || 0) + serials.length;
        toastSuccess('Detail Picking berhasil disubmit.');
      } finally {
        serials.forEach((serial) => {
          serial.isUpdating = false;
        });
        this.isSubmittingDetail = false;
      }
    },
  },
};
</script>

<style scoped>
.shipping-instruction-page {
  width: 100%;
}

.filter-table td {
  padding-top: 5px;
  vertical-align: middle;
}

.filter-table td:first-child {
  width: 180px;
}

.filter-table td:nth-child(2) {
  padding-left: 15px;
}

.field-compact {
  width: 360px;
}

.date-range-cell {
  display: flex;
  align-items: center;
}

.new-check-label {
  margin-bottom: 0;
}

.input-white {
  background-color: #fff !important;
}

.detail-content {
  max-height: 30em;
  max-height: 60%;
}

.shipping-table thead th {
  white-space: nowrap;
  padding: 4px 8px;
}

.shipping-table td {
  white-space: nowrap;
  padding: 3px 8px;
  font-size: 12px;
}

.shipping-table .check-col {
  width: 30px;
}

.serial-highlight {
  background-color: #f3f8cf !important;
}

.detail-row td {
  background-color: #fffff4;
}

.serial-toggle-btn {
  width: 18px;
  height: 18px;
  border: 1px solid #5f85a8;
  border-radius: 50%;
  background: #fff;
  color: #2f5f89;
  font-size: 13px;
  font-weight: 700;
  line-height: 14px;
  text-align: center;
  padding: 0;
  cursor: pointer;
  vertical-align: middle;
}

.serial-toggle-btn:hover {
  background: #e9f3ff;
}

.detail-modal {
  padding: 4px;
}

.detail-form td {
  padding: 4px;
}

.detail-form td:first-child {
  width: 130px;
}

.detail-form td:last-child {
  width: 280px;
}
</style>
