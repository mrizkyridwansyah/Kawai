<template>
  <div class="kpi-summary-strip mb-0">
    <!-- 1. Total Locations -->
    <div class="kpi-item">
      <div class="kpi-title">
        <i class="fa fa-boxes-stacked text-primary me-1"></i> Total Locations
      </div>
      <div class="kpi-val">{{ (kpis.TotalLocations ?? 0).toLocaleString() }}</div>
      <div class="kpi-note">All slots</div>
    </div>

    <!-- 2. Occupied Address -->
    <div class="kpi-item">
      <div class="kpi-title">
        <i class="fa fa-box-open text-warning me-1"></i> Occupied
      </div>
      <div class="kpi-val text-dark">{{ (kpis.OccupiedLocations ?? 0).toLocaleString() }}</div>
      <div class="kpi-note">Active inventory</div>
    </div>

    <!-- 3. Empty Address -->
    <div class="kpi-item">
      <div class="kpi-title">
        <i class="fa fa-layer-group text-success me-1"></i> Empty Address
      </div>
      <div class="kpi-val text-success">{{ (kpis.EmptyLocations ?? 0).toLocaleString() }}</div>
      <div class="kpi-note">Ready for storage</div>
    </div>

    <!-- 4. Total Item (Distinct item count) -->
    <div
      class="kpi-item kpi-item-clickable"
      @click="openItemModal"
      title="Click to view item details on rack"
    >
      <div class="kpi-title d-flex justify-content-between align-items-center">
        <span><i class="fa fa-boxes-packing text-info me-1"></i> Total Item</span>
        <span class="badge py-0 px-1" style="font-size: 9px; background: rgba(14, 165, 233, 0.15); color: #0284c7; border: 1px solid rgba(2, 132, 199, 0.3);">
          <i class="fa fa-list me-1"></i>Detail
        </span>
      </div>
      <div class="kpi-val text-dark">
        {{ totalItemCount }} <span class="kpi-unit">Items</span>
      </div>
      <div class="kpi-note text-primary">
        <i class="fa fa-arrow-up-right-from-square me-1" style="font-size: 9px;"></i>
        <span>{{ itemsList.length > 0 ? `${itemsList.length} active item types` : `${kpis.TotalStockRows ?? 0} lot records` }}</span>
      </div>
    </div>

    <!-- 5. Address Availability with Compact Donut -->
    <div class="kpi-item kpi-avail-item">
      <div class="donut-wrapper flex-shrink-0">
        <svg width="42" height="42" viewBox="0 0 36 36" class="donut-svg">
          <path
            class="donut-bg"
            d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
            fill="none"
            stroke="#e2e8f0"
            stroke-width="3.8"
          />
          <path
            class="donut-ring"
            :stroke-dasharray="`${emptyPercentage}, 100`"
            d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
            fill="none"
            stroke="#16a34a"
            stroke-width="3.8"
            stroke-linecap="round"
          />
          <text x="18" y="21" class="donut-text" text-anchor="middle">
            {{ emptyPercentage }}%
          </text>
        </svg>
      </div>
      <div class="flex-grow-1 min-w-0">
        <div class="kpi-title">
          Availability <span class="badge ms-1 py-0 px-1" style="font-size: 9px; background: rgba(34, 197, 94, 0.15); color: #16a34a; border: 1px solid rgba(22, 163, 74, 0.3);">{{ emptyPercentage }}% Free</span>
        </div>
        <div class="kpi-sub-text">
          {{ kpis.EmptyLocations ?? 0 }} of {{ registeredTotal }} empty
        </div>
        <div class="progress mt-1" style="height: 4px; max-width: 140px;">
          <div
            class="progress-bar bg-success"
            role="progressbar"
            :style="{ width: `${emptyPercentage}%` }"
          ></div>
        </div>
      </div>
    </div>

    <!-- Modal: Item List -->
    <div
      v-if="isItemModalOpen"
      class="modal fade show d-block item-list-modal-backdrop"
      tabindex="-1"
      role="dialog"
      @click.self="isItemModalOpen = false"
    >
      <div class="modal-dialog modal-dialog-centered modal-lg" role="document">
        <div class="modal-content shadow border-0 rounded-3">
          <!-- Modal Header -->
          <div class="modal-header py-2 px-3 text-white" style="background-color: #002060;">
            <div class="d-flex align-items-center">
              <i class="fa fa-boxes-packing me-2 fs-5"></i>
              <h6 class="modal-title fw-bold mb-0 text-white">Item List on Rack (Total: {{ itemsList.length }} Items)</h6>
            </div>
            <button
              type="button"
              class="btn-close btn-close-white"
              aria-label="Close"
              @click="isItemModalOpen = false"
            ></button>
          </div>

          <!-- Modal Body -->
          <div class="modal-body p-3" style="max-height: 60vh; overflow-y: auto;">
            <div v-if="itemsList.length === 0" class="text-center py-4 text-muted">
              <i class="fa fa-box-open fa-2x mb-2 opacity-50"></i>
              <div>No stock items found for the current filter.</div>
            </div>
            <div v-else class="table-responsive">
              <table class="table table-sm table-striped table-hover align-middle mb-0">
                <thead class="table-light">
                  <tr>
                    <th class="text-center" style="width: 45px;">No</th>
                    <th style="min-width: 140px;">Item Code</th>
                    <th style="min-width: 250px;">Item Name</th>
                    <th class="text-end" style="width: 120px;">Total Qty</th>
                    <th class="text-center" style="width: 90px;">Lot Rows</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, idx) in itemsList" :key="item.ItemCode">
                    <td class="text-center text-muted small">{{ idx + 1 }}</td>
                    <td class="fw-bold text-primary">{{ item.ItemCode }}</td>
                    <td class="text-dark small">{{ item.ItemName || '-' }}</td>
                    <td class="text-end fw-bold text-success">
                      {{ Number(item.TotalQty || 0).toLocaleString() }} <small class="text-muted fw-normal">PCS</small>
                    </td>
                    <td class="text-center">
                      <span class="badge bg-secondary rounded-pill px-2">
                        {{ item.LotCount || 1 }} lots
                      </span>
                    </td>
                  </tr>
                </tbody>
                <tfoot class="table-light fw-bold">
                  <tr>
                    <td colspan="3" class="text-end">Total Accumulated:</td>
                    <td class="text-end text-success">
                      {{ totalQtySum.toLocaleString() }} <small class="text-muted fw-normal">PCS</small>
                    </td>
                    <td class="text-center">{{ totalLotsSum }} lots</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          <!-- Modal Footer -->
          <div class="modal-footer py-2 px-3 bg-light">
            <button
              type="button"
              class="btn btn-sm btn-secondary"
              @click="isItemModalOpen = false"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  kpis: {
    type: Object,
    default: () => ({
      TotalLocations: 0,
      EmptyLocations: 0,
      OccupiedLocations: 0,
      UnmappedLocations: 0,
      TotalStockQty: 0,
      TotalStockRows: 0,
      TotalItemCount: 0,
      EmptyPercentage: 0,
      Items: [],
    }),
  },
});

const isItemModalOpen = ref(false);

const openItemModal = () => {
  isItemModalOpen.value = true;
};

const itemsList = computed(() => {
  return props.kpis.Items || [];
});

const totalItemCount = computed(() => {
  if (props.kpis.TotalItemCount !== undefined && props.kpis.TotalItemCount !== null && props.kpis.TotalItemCount > 0) {
    return Number(props.kpis.TotalItemCount).toLocaleString();
  }
  return itemsList.value.length.toLocaleString();
});

const totalQtySum = computed(() => {
  return itemsList.value.reduce((acc, curr) => acc + (Number(curr.TotalQty) || 0), 0);
});

const totalLotsSum = computed(() => {
  return itemsList.value.reduce((acc, curr) => acc + (Number(curr.LotCount) || 0), 0);
});

const registeredTotal = computed(() => {
  const empty = Number(props.kpis.EmptyLocations || 0);
  const occupied = Number(props.kpis.OccupiedLocations || 0);
  const total = empty + occupied;
  return total > 0 ? total : Number(props.kpis.TotalLocations || 0);
});

const emptyPercentage = computed(() => {
  if (props.kpis.EmptyPercentage !== undefined && props.kpis.EmptyPercentage !== null) {
    return Math.round(props.kpis.EmptyPercentage);
  }
  if (registeredTotal.value === 0) return 0;
  return Math.round(((props.kpis.EmptyLocations || 0) / registeredTotal.value) * 100);
});
</script>

<style scoped>
.kpi-summary-strip {
  display: flex;
  flex-wrap: wrap;
  background: #fff;
  border: 1px solid #dcdcdc;
  border-radius: 4px;
  padding: 5px 10px;
  gap: 6px;
  align-items: center;
  font-family: Arial, sans-serif;
}

.kpi-item {
  flex: 1 1 120px;
  min-width: 100px;
  padding: 2px 8px;
  border-right: 1px solid #edf2f7;
}

.kpi-item-clickable {
  cursor: pointer;
  transition: all 0.2s ease;
  border-radius: 4px;
}

.kpi-item-clickable:hover {
  background-color: #f1f5f9;
}

.kpi-item:last-child {
  border-right: none;
}

.kpi-avail-item {
  flex: 1.5 1 200px;
  min-width: 180px;
  display: flex;
  align-items: center;
}

.kpi-title {
  font-size: 10px;
  font-weight: 600;
  color: #526484;
  text-transform: uppercase;
  letter-spacing: 0.3px;
  margin-bottom: 1px;
  white-space: nowrap;
}

.kpi-val {
  font-size: 16px;
  font-weight: 700;
  line-height: 1.15;
  color: #1e293b;
}

.kpi-unit {
  font-size: 10px;
  font-weight: 400;
  color: #64748b;
}

.kpi-note {
  font-size: 9px;
  color: #94a3b8;
  margin-top: 1px;
  white-space: nowrap;
}

.kpi-sub-text {
  font-size: 10px;
  font-weight: 500;
  color: #334155;
  white-space: nowrap;
}

.donut-wrapper {
  width: 42px;
  height: 42px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  margin-right: 14px;
}

.donut-svg {
  transform: rotate(-90deg);
}

.donut-text {
  transform: rotate(90deg);
  transform-origin: 18px 18px;
  font-size: 9px;
  fill: #1e293b;
  font-weight: 700;
  font-family: Arial, sans-serif;
}

.item-list-modal-backdrop {
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1055;
}

@media (max-width: 768px) {
  .kpi-item {
    border-right: none;
    border-bottom: 1px solid #edf2f7;
    padding-bottom: 8px;
  }
}
</style>
