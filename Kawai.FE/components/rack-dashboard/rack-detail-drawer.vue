<template>
  <div>
    <!-- Backdrop Overlay -->
    <transition name="fade">
      <div
        v-if="isOpen"
        class="drawer-overlay"
        @click="$emit('close')"
      ></div>
    </transition>

    <!-- Side Drawer Panel -->
    <transition name="slide">
      <aside v-if="isOpen" class="drawer-panel shadow-lg" role="dialog" aria-modal="true">
        <!-- Header -->
        <div class="drawer-header text-white p-3 d-flex justify-content-between align-items-start">
          <div>
            <div class="d-flex align-items-center gap-2">
              <span class="badge px-2 py-1" :class="statusBadgeClass">
                {{ statusLabel }}
              </span>
              <h5 class="mb-0 fw-bold font-monospace">{{ addressTitle }}</h5>
            </div>
            <div class="small text-white-50 mt-1">
              {{ pathBreadcrumb }}
            </div>
          </div>
          <button
            type="button"
            class="btn-close btn-close-white"
            aria-label="Close"
            @click="$emit('close')"
          ></button>
        </div>

        <!-- Drawer Body -->
        <div class="drawer-body p-3 overflow-y-auto">
          <!-- Loading State -->
          <div v-if="isLoading" class="text-center py-5">
            <div class="spinner-border text-primary mb-2" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
            <div class="text-muted small">Loading location details...</div>
          </div>

          <div v-else-if="detail">
            <!-- 6 Parameter Summary Grid -->
            <div class="row g-2 mb-3">
              <!-- Status Box -->
              <div class="col-4">
                <div class="p-2 rounded-2 bg-light border text-center h-100">
                  <div class="text-muted text-uppercase small fw-bold" style="font-size: 10px;">Status</div>
                  <div class="fw-bold fs-6 mt-1" :class="statusTextClass">{{ statusLabel }}</div>
                </div>
              </div>

              <!-- Total Stock Box -->
              <div class="col-4">
                <div class="p-2 rounded-2 bg-light border text-center h-100">
                  <div class="text-muted text-uppercase small fw-bold" style="font-size: 10px;">Total Stock</div>
                  <div class="fw-bold fs-6 mt-1 text-dark">
                    {{ Number(detail.TotalQty || 0).toLocaleString() }} <small class="text-muted fw-normal" style="font-size: 10px;">PCS</small>
                  </div>
                </div>
              </div>

              <!-- Distinct Items Box -->
              <div class="col-4">
                <div class="p-2 rounded-2 bg-light border text-center h-100">
                  <div class="text-muted text-uppercase small fw-bold" style="font-size: 10px;">Distinct Items</div>
                  <div class="fw-bold fs-6 mt-1 text-primary">
                    {{ distinctItemList.length || detail.ItemCount || 0 }}
                    <small class="text-muted fw-normal" style="font-size: 10px;">({{ detail.Stocks?.length || detail.StockRowCount || 0 }} lots)</small>
                  </div>
                </div>
              </div>

              <!-- Warehouse Box -->
              <div class="col-4">
                <div class="p-2 rounded-2 bg-light border text-center h-100">
                  <div class="text-muted text-uppercase small fw-bold" style="font-size: 10px;">Warehouse</div>
                  <div class="fw-bold text-truncate small mt-1 text-dark" :title="detail.WarehouseCode">
                    {{ detail.WarehouseCode }}
                  </div>
                </div>
              </div>

              <!-- Area Box -->
              <div class="col-4">
                <div class="p-2 rounded-2 bg-light border text-center h-100">
                  <div class="text-muted text-uppercase small fw-bold" style="font-size: 10px;">Area</div>
                  <div class="fw-bold text-truncate small mt-1 text-dark" :title="detail.AreaCode">
                    {{ detail.AreaCode }}
                  </div>
                </div>
              </div>

              <!-- Address Name Box -->
              <div class="col-4">
                <div class="p-2 rounded-2 bg-light border text-center h-100">
                  <div class="text-muted text-uppercase small fw-bold" style="font-size: 10px;">Address Name</div>
                  <div class="fw-bold text-truncate small mt-1 text-dark" :title="detail.AddressName">
                    {{ detail.AddressName || '-' }}
                  </div>
                </div>
              </div>
            </div>

            <!-- Unregistered Location Warning Alert -->
            <div
              v-if="detail.Status === 'unmapped' || !detail.IsRegistered"
              class="alert alert-warning border-warning d-flex align-items-start gap-2 py-2 px-3 mb-3 rounded-2"
              role="alert"
            >
              <i class="fa fa-triangle-exclamation text-warning mt-1"></i>
              <div class="small">
                <strong>Data Warning:</strong> This stock location exists in active stock transactions but is <strong>not registered</strong> in the Address master.
              </div>
            </div>

            <!-- Stock Rows Section Header & View Toggle -->
            <div class="d-flex justify-content-between align-items-center mb-2 pb-1 border-bottom">
              <div class="d-flex align-items-center gap-2">
                <span class="fw-bold text-dark small">
                  <i class="fa fa-boxes-stacked text-secondary me-1"></i> Stock Items
                </span>
                <span class="badge bg-secondary rounded-pill">
                  {{ viewMode === 'distinct' ? `${distinctItemList.length} distinct item(s)` : `${detail.Stocks?.length || 0} record(s)` }}
                </span>
              </div>
              <div class="btn-group btn-group-sm" role="group">
                <button
                  type="button"
                  class="btn btn-xs py-0 px-2"
                  :class="viewMode === 'distinct' ? 'btn-primary' : 'btn-outline-secondary'"
                  @click="viewMode = 'distinct'"
                  style="font-size: 11px;"
                >
                  Distinct ({{ distinctItemList.length }})
                </button>
                <button
                  type="button"
                  class="btn btn-xs py-0 px-2"
                  :class="viewMode === 'all' ? 'btn-primary' : 'btn-outline-secondary'"
                  @click="viewMode = 'all'"
                  style="font-size: 11px;"
                >
                  All ({{ detail.Stocks?.length || 0 }})
                </button>
              </div>
            </div>

            <!-- Empty State Message -->
            <div
              v-if="!detail.Stocks || detail.Stocks.length === 0"
              class="empty-box p-4 text-center rounded-3 my-3"
            >
              <div class="display-6 text-muted opacity-50 mb-2">□</div>
              <h6 class="fw-bold text-dark mb-1">This address is empty</h6>
              <p class="text-muted small mb-0">
                No positive stock quantity is currently recorded at this rack location.
              </p>
            </div>

            <!-- 1. Distinct Table (Default) -->
            <div v-else-if="viewMode === 'distinct'" class="table-responsive">
              <table class="table table-sm table-bordered table-hover align-middle mb-0 custom-drawer-table">
                <thead class="table-light">
                  <tr>
                    <th scope="col" style="font-size: 11px;">Item Code</th>
                    <th scope="col" style="font-size: 11px;">Item Name</th>
                    <th scope="col" style="font-size: 11px;" class="text-center">Lots</th>
                    <th scope="col" class="text-end" style="font-size: 11px;">Total Qty</th>
                    <th scope="col" class="text-center" style="font-size: 11px;">Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in distinctItemList" :key="item.ItemCode">
                    <td class="fw-bold text-primary font-monospace small">{{ item.ItemCode }}</td>
                    <td class="small text-dark text-truncate" style="max-width: 180px;" :title="item.ItemName">
                      {{ item.ItemName || '-' }}
                    </td>
                    <td class="text-center">
                      <span class="badge bg-light text-dark border font-monospace" style="font-size: 10px;">
                        {{ item.BarcodeCount }} lot{{ item.BarcodeCount === 1 ? '' : 's' }}
                      </span>
                    </td>
                    <td class="text-end fw-bold font-monospace small" :class="item.TotalQty > 0 ? 'text-success' : 'text-muted'">
                      {{ Number(item.TotalQty || 0).toLocaleString() }}
                      <span class="text-muted fw-normal" style="font-size: 10px;">{{ item.Unit || 'PCS' }}</span>
                    </td>
                    <td class="text-center">
                      <span class="badge bg-success bg-opacity-75">
                        {{ item.StatusReceipt || 'Available' }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- 2. Full Barcode Table -->
            <div v-else class="table-responsive">
              <table class="table table-sm table-bordered table-hover align-middle mb-0 custom-drawer-table">
                <thead class="table-light">
                  <tr>
                    <th scope="col" style="font-size: 11px;">Item Code</th>
                    <th scope="col" style="font-size: 11px;">Barcode No</th>
                    <th scope="col" style="font-size: 11px;">Lot No</th>
                    <th scope="col" class="text-end" style="font-size: 11px;">Qty</th>
                    <th scope="col" class="text-center" style="font-size: 11px;">Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(s, idx) in detail.Stocks" :key="idx">
                    <td class="fw-bold text-dark font-monospace small">{{ s.ItemCode }}</td>
                    <td class="font-monospace small text-muted text-truncate" style="max-width: 140px;" :title="s.BarcodeNo">
                      {{ s.BarcodeNo }}
                    </td>
                    <td class="font-monospace small text-muted text-truncate" style="max-width: 120px;" :title="s.LotNo">
                      {{ s.LotNo || '-' }}
                    </td>
                    <td class="text-end fw-bold font-monospace small" :class="s.Qty > 0 ? 'text-success' : 'text-muted'">
                      {{ Number(s.Qty || 0).toLocaleString() }}
                      <span class="text-muted fw-normal" style="font-size: 10px;">{{ s.Unit || 'PCS' }}</span>
                    </td>
                    <td class="text-center">
                      <span class="badge bg-light text-dark border small px-2">
                        {{ s.Status || 'Available' }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="drawer-footer p-2 bg-light border-top text-end">
          <button type="button" class="btn btn-sm btn-secondary px-3" @click="$emit('close')">
            Close
          </button>
        </div>
      </aside>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false,
  },
  detail: {
    type: Object,
    default: null,
  },
  isLoading: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(['close']);

// View mode: 'distinct' (default) | 'all'
const viewMode = ref('distinct');

// Distinct items grouped by ItemCode
const distinctItemList = computed(() => {
  const stocks = props.detail?.Stocks || props.detail?.stocks || [];
  if (stocks.length === 0) return [];

  const map = new Map();
  for (const row of stocks) {
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

// Handle ESC key to close drawer
function handleKeyDown(e) {
  if (e.key === 'Escape' && props.isOpen) {
    emit('close');
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleKeyDown);
});

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleKeyDown);
});

const addressTitle = computed(() => {
  return props.detail?.AddressCode || 'Address Detail';
});

const pathBreadcrumb = computed(() => {
  if (!props.detail) return '';
  const wh = props.detail.WarehouseCode || '';
  const ar = props.detail.AreaCode || '';
  const addr = props.detail.AddressName || props.detail.AddressCode || '';
  return `${wh} / ${ar} / ${addr}`;
});

const statusLabel = computed(() => {
  if (!props.detail) return '';
  if (props.detail.Status === 'unmapped' || !props.detail.IsRegistered) return 'Unregistered';
  if (props.detail.Status === 'occupied') return 'Occupied';
  return 'Empty';
});

const statusTextClass = computed(() => {
  if (!props.detail) return '';
  if (props.detail.Status === 'unmapped' || !props.detail.IsRegistered) return 'text-purple';
  if (props.detail.Status === 'occupied') return 'text-success';
  return 'text-secondary';
});

const statusBadgeClass = computed(() => {
  if (!props.detail) return 'bg-secondary';
  if (props.detail.Status === 'unmapped' || !props.detail.IsRegistered) return 'bg-purple text-white';
  if (props.detail.Status === 'occupied') return 'bg-success text-white';
  return 'bg-secondary text-white';
});
</script>

<style scoped>
.drawer-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(11, 20, 36, 0.45);
  backdrop-filter: blur(1.5px);
  z-index: 1040;
}

.drawer-panel {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  width: min(600px, 95vw);
  background-color: #ffffff;
  z-index: 1050;
  display: flex;
  flex-direction: column;
}

.drawer-header {
  background: linear-gradient(110deg, #13273e, #1e3f63);
}

.drawer-body {
  flex: 1;
}

.empty-box {
  background-color: #f8fafc;
  border: 1px dashed #cbd5e1;
}

.custom-drawer-table thead th {
  background-color: #f1f5f9;
  color: #475569;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.text-purple {
  color: #7e22ce !important;
}

.bg-purple {
  background-color: #8b5cf6 !important;
}

/* Slide Transition */
.slide-enter-active,
.slide-leave-active {
  transition: transform 0.25s ease;
}
.slide-enter-from,
.slide-leave-to {
  transform: translateX(100%);
}

/* Fade Transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
