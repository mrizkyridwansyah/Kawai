<template>
  <div
    v-if="isOpen"
    class="modal fade show d-block"
    tabindex="-1"
    style="background: rgba(0, 0, 0, 0.5); z-index: 1055"
    @click.self="$emit('close')"
  >
    <div class="modal-dialog modal-lg modal-dialog-centered">
      <div class="modal-content border-0 shadow-lg rounded-3">
        <!-- Modal Header -->
        <div class="modal-header bg-light py-3 border-bottom">
          <div class="d-flex align-items-center">
            <div class="p-2 rounded-2 bg-primary bg-opacity-10 text-primary me-2">
              <i class="fa fa-boxes-stacked"></i>
            </div>
            <div>
              <h6 class="modal-title fw-bold text-dark mb-0">
                Rack Address Detail: {{ detail?.AddressCode || '-' }}
              </h6>
              <span class="text-muted small">
                {{ detail?.WarehouseName || detail?.WarehouseCode }} / {{ detail?.AreaName || detail?.AreaCode }}
              </span>
            </div>
          </div>
          <button
            type="button"
            class="btn-close"
            @click="$emit('close')"
          ></button>
        </div>

        <!-- Modal Body -->
        <div class="modal-body p-4">
          <!-- Loading state -->
          <div v-if="isLoading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
            <div class="text-muted small mt-2">Loading rack items...</div>
          </div>

          <!-- Content when loaded -->
          <div v-else-if="detail">
            <!-- Metadata summary cards -->
            <div class="row g-2 mb-3">
              <div class="col-sm-3 col-6">
                <div class="p-2 bg-light rounded-2 border text-center">
                  <span class="text-muted small d-block" style="font-size: 11px;">Status</span>
                  <span
                    class="badge mt-1"
                    :class="detail.Status === 'occupied' ? 'bg-warning text-dark' : 'bg-success'"
                  >
                    {{ detail.Status === 'occupied' ? 'OCCUPIED' : (detail.Status === 'unmapped' ? 'UNREGISTERED' : 'EMPTY') }}
                  </span>
                </div>
              </div>
              <div class="col-sm-3 col-6">
                <div class="p-2 bg-light rounded-2 border text-center">
                  <span class="text-muted small d-block" style="font-size: 11px;">Rack Name</span>
                  <span class="fw-bold text-dark small text-truncate d-block mt-1" :title="detail.AddressName">{{ detail.AddressName || '-' }}</span>
                </div>
              </div>
              <div class="col-sm-3 col-6">
                <div class="p-2 bg-light rounded-2 border text-center">
                  <span class="text-muted small d-block" style="font-size: 11px;">Total Quantity</span>
                  <span class="fw-bold text-dark fs-7 d-block mt-1">
                    {{ Number(detail.TotalQty || 0).toLocaleString() }} <small class="text-muted fw-normal">PCS</small>
                  </span>
                </div>
              </div>
              <div class="col-sm-3 col-6">
                <div class="p-2 bg-light rounded-2 border text-center">
                  <span class="text-muted small d-block" style="font-size: 11px;">Distinct Items</span>
                  <span class="fw-bold text-primary fs-7 d-block mt-1">
                    {{ distinctItemList.length }} <small class="text-muted fw-normal">items ({{ stockList.length }} lots)</small>
                  </span>
                </div>
              </div>
            </div>

            <!-- View Toggle & Section Header -->
            <div class="d-flex justify-content-between align-items-center mb-2 pb-1 border-bottom">
              <div class="d-flex align-items-center gap-2">
                <span class="fw-bold text-dark small">
                  <i class="fa fa-boxes-stacked text-primary me-1"></i> Stock Items
                </span>
                <span class="badge bg-secondary-subtle text-secondary border rounded-pill small">
                  {{ viewMode === 'distinct' ? `${distinctItemList.length} distinct item(s)` : `${stockList.length} barcode record(s)` }}
                </span>
              </div>
              <!-- Toggle View Mode: Distinct vs All Barcodes -->
              <div class="btn-group btn-group-sm" role="group">
                <button
                  type="button"
                  class="btn btn-sm py-0 px-2"
                  :class="viewMode === 'distinct' ? 'btn-primary' : 'btn-outline-secondary'"
                  @click="viewMode = 'distinct'"
                  title="Grouped by distinct Item Code & Name"
                >
                  <i class="fa fa-layer-group me-1"></i> Distinct Items ({{ distinctItemList.length }})
                </button>
                <button
                  type="button"
                  class="btn btn-sm py-0 px-2"
                  :class="viewMode === 'all' ? 'btn-primary' : 'btn-outline-secondary'"
                  @click="viewMode = 'all'"
                  title="Show all barcode and lot transaction rows"
                >
                  <i class="fa fa-barcode me-1"></i> All Records ({{ stockList.length }})
                </button>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="!stockList || stockList.length === 0" class="text-center py-4 text-muted border rounded-2 bg-light">
              <i class="fa fa-info-circle me-1"></i> No items currently placed in this rack location.
            </div>

            <!-- 1. DISTINCT ITEMS TABLE (DEFAULT) -->
            <div v-else-if="viewMode === 'distinct'" class="table-responsive border rounded-2">
              <table class="table table-sm table-hover mb-0 align-middle">
                <thead class="table-light">
                  <tr>
                    <th style="width: 40px" class="text-center">#</th>
                    <th style="width: 130px">Item Code</th>
                    <th>Item Name</th>
                    <th style="width: 160px">Lot No</th>
                    <th style="width: 90px" class="text-center">Lots / Barcodes</th>
                    <th style="width: 120px" class="text-end">Total Qty</th>
                    <th style="width: 90px" class="text-center">Status</th>
                    <th style="width: 80px" class="text-center">Detail</th>
                  </tr>
                </thead>
                <tbody>
                  <template v-for="(item, idx) in distinctItemList" :key="item.ItemCode">
                    <tr class="cursor-pointer" @click="toggleExpand(item.ItemCode)">
                      <td class="text-center text-muted small">{{ idx + 1 }}</td>
                      <td class="fw-semibold text-primary font-monospace small">
                        {{ item.ItemCode }}
                      </td>
                      <td class="text-dark small fw-medium">
                        {{ item.ItemName || '-' }}
                      </td>
                      <td>
                        <span class="badge bg-secondary font-monospace" :title="item.LotSummary">
                          {{ item.LotSummary || '-' }}
                        </span>
                      </td>
                      <td class="text-center">
                        <span class="badge bg-light text-dark border font-monospace">
                          {{ item.BarcodeCount }} barcode{{ item.BarcodeCount === 1 ? '' : 's' }}
                        </span>
                      </td>
                      <td class="text-end fw-bold text-dark small">
                        {{ Number(item.TotalQty || 0).toLocaleString() }}
                        <small class="fw-normal text-muted">{{ item.Unit || 'PCS' }}</small>
                      </td>
                      <td class="text-center">
                        <span class="badge bg-success bg-opacity-75">
                          {{ item.StatusReceipt || 'OK' }}
                        </span>
                      </td>
                      <td class="text-center" @click.stop="toggleExpand(item.ItemCode)">
                        <button
                          type="button"
                          class="btn btn-xs btn-outline-secondary py-0 px-2 rounded-1"
                          style="font-size: 11px;"
                        >
                          <i
                            class="fa"
                            :class="isExpanded(item.ItemCode) ? 'fa-chevron-up' : 'fa-chevron-down'"
                          ></i>
                          {{ isExpanded(item.ItemCode) ? 'Close' : 'Details' }}
                        </button>
                      </td>
                    </tr>

                    <!-- Expandable Barcode Breakdown Rows -->
                    <tr v-if="isExpanded(item.ItemCode)" class="bg-light">
                      <td colspan="8" class="p-2 ps-4 bg-body-tertiary border-start border-3 border-primary">
                        <div class="small fw-bold text-secondary mb-1">
                          <i class="fa fa-barcode me-1"></i> Barcode / Lot Details for {{ item.ItemCode }} ({{ item.ItemName }}):
                        </div>
                        <table class="table table-sm table-bordered bg-white mb-0 small">
                          <thead class="table-light">
                            <tr style="font-size: 11px;">
                              <th>Barcode No</th>
                              <th>Lot No</th>
                              <th class="text-end">Qty</th>
                              <th class="text-center">Status</th>
                              <th>Expired Date</th>
                              <th>Receipt Date</th>
                            </tr>
                          </thead>
                          <tbody>
                            <tr v-for="(b, bIdx) in item.Barcodes" :key="bIdx">
                              <td class="font-monospace text-primary fw-semibold">{{ b.BarcodeNo || '-' }}</td>
                              <td class="font-monospace">{{ b.LotNo || '-' }}</td>
                              <td class="text-end fw-bold">
                                {{ Number(b.Qty ?? b.StockQty ?? 0).toLocaleString() }}
                                <small class="text-muted fw-normal">{{ b.Unit || 'PCS' }}</small>
                              </td>
                              <td class="text-center">
                                <span class="badge bg-success bg-opacity-75">{{ b.StatusReceipt || 'Available' }}</span>
                              </td>
                              <td class="text-muted">{{ b.ExpiredDate ? b.ExpiredDate.substring(0, 10) : '-' }}</td>
                              <td class="text-muted">{{ b.ReceiptDate ? b.ReceiptDate.substring(0, 10) : '-' }}</td>
                            </tr>
                          </tbody>
                        </table>
                      </td>
                    </tr>
                  </template>
                </tbody>
              </table>
            </div>

            <!-- 2. ALL RECORDS FLAT TABLE -->
            <div v-else class="table-responsive border rounded-2">
              <table class="table table-sm table-hover mb-0 align-middle">
                <thead class="table-light">
                  <tr>
                    <th style="width: 40px" class="text-center">#</th>
                    <th>Barcode No</th>
                    <th>Item Code</th>
                    <th>Item Name</th>
                    <th>Lot No</th>
                    <th class="text-end">Stock Qty</th>
                    <th class="text-center">Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, idx) in stockList" :key="idx">
                    <td class="text-center text-muted small">{{ idx + 1 }}</td>
                    <td class="font-monospace small text-muted">
                      {{ item.BarcodeNo || '-' }}
                    </td>
                    <td class="fw-semibold text-primary font-monospace small">
                      {{ item.ItemCode }}
                    </td>
                    <td class="text-dark small">{{ item.ItemName || '-' }}</td>
                    <td>
                      <span class="badge bg-secondary font-monospace">
                        {{ item.LotNo || '-' }}
                      </span>
                    </td>
                    <td class="text-end fw-bold text-dark small">
                      {{ Number(item.Qty ?? item.StockQty ?? 0).toLocaleString() }} <small class="fw-normal text-muted">{{ item.Unit || 'PCS' }}</small>
                    </td>
                    <td class="text-center">
                      <span class="badge bg-success bg-opacity-75">
                        {{ item.StatusReceipt || 'OK' }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Modal Footer -->
        <div class="modal-footer bg-light py-2 border-top">
          <button
            type="button"
            class="btn btn-secondary btn-sm"
            @click="$emit('close')"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false,
  },
  isLoading: {
    type: Boolean,
    default: false,
  },
  detail: {
    type: Object,
    default: null,
  },
});

defineEmits(['close']);

// View mode: 'distinct' (default) | 'all'
const viewMode = ref('distinct');

// Expanded item codes for barcode sub-tables
const expandedItemCodes = ref(new Set());

function isExpanded(code) {
  return expandedItemCodes.value.has(code);
}

function toggleExpand(code) {
  if (expandedItemCodes.value.has(code)) {
    expandedItemCodes.value.delete(code);
  } else {
    expandedItemCodes.value.add(code);
  }
}

const stockList = computed(() => {
  if (!props.detail) return [];
  return props.detail.Stocks || props.detail.stocks || props.detail.Items || [];
});

// Group stock items by distinct ItemCode and ItemName
const distinctItemList = computed(() => {
  if (!stockList.value || stockList.value.length === 0) return [];

  const map = new Map();
  for (const row of stockList.value) {
    const code = row.ItemCode || 'UNKNOWN';
    if (!map.has(code)) {
      map.set(code, {
        ItemCode: code,
        ItemName: row.ItemName || '-',
        Unit: row.Unit || 'PCS',
        TotalQty: 0,
        StatusReceipt: row.StatusReceipt || 'Available',
        Lots: new Set(),
        Barcodes: [],
      });
    }
    const item = map.get(code);
    item.TotalQty += Number(row.Qty ?? row.StockQty ?? 0);
    if (row.LotNo) item.Lots.add(row.LotNo);
    item.Barcodes.push(row);
  }

  return Array.from(map.values()).map(item => ({
    ...item,
    LotCount: item.Lots.size,
    LotSummary: Array.from(item.Lots).join(', '),
    BarcodeCount: item.Barcodes.length,
  }));
});
</script>
