<template>
  <div class="rack-grid-container">
    <!-- Empty state if no locations found -->
    <div v-if="!locations || locations.length === 0" class="text-center py-5 bg-white rounded-3 shadow-sm">
      <div class="text-muted mb-2">
        <i class="fa fa-folder-open fa-3x opacity-50"></i>
      </div>
      <h6 class="text-dark fw-bold">No Rack Locations Found</h6>
      <p class="text-muted small mb-0">Please adjust your warehouse or area filter selection.</p>
    </div>

    <!-- Grouped by Area or Flat Grid -->
    <div v-else>
      <div
        v-for="(areaGroup, areaKey) in groupedLocations"
        :key="areaKey"
        class="area-section mb-4"
      >
        <div class="d-flex align-items-center justify-content-between mb-2 pb-1 border-bottom">
          <div class="d-flex align-items-center">
            <span class="badge bg-primary me-2 px-2 py-1">Area</span>
            <span class="fw-bold text-dark fs-6">{{ areaKey }}</span>
            <span class="text-muted small ms-2">({{ areaGroup.length }} slots)</span>
          </div>
          <div class="small text-muted">
            <span class="text-success me-2">● {{ areaGroup.filter(l => l.Status === 'empty').length }} Empty</span>
            <span class="text-warning">● {{ areaGroup.filter(l => l.Status === 'occupied').length }} Occupied</span>
          </div>
        </div>

        <div class="row row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-6 row-cols-xl-8 g-2">
          <div
            v-for="loc in areaGroup"
            :key="loc.AddressCode"
            class="col"
          >
            <div
              class="rack-card h-100 p-2 rounded-2 text-center position-relative cursor-pointer transition-all"
              :class="cardStatusClass(loc)"
              @click="$emit('select-location', loc)"
              :title="`Click to inspect ${loc.AddressCode}`"
            >
              <!-- Slot Header -->
              <div class="d-flex justify-content-between align-items-center mb-1">
                <span class="badge badge-sm" :class="badgeStatusClass(loc)">
                  {{ badgeStatusText(loc) }}
                </span>
                <span v-if="loc.Status === 'occupied' && getItemCount(loc) > 0" class="badge bg-dark bg-opacity-75 text-white badge-sm">
                  {{ getItemCount(loc) }} {{ getItemCount(loc) === 1 ? 'item' : 'items' }}
                </span>
              </div>

              <!-- Address Code & Name -->
              <div class="fw-bold text-truncate slot-code mb-0 text-dark">
                {{ loc.AddressCode }}
              </div>
              <div class="text-muted text-truncate slot-desc mb-1">
                {{ loc.AddressName || '-' }}
              </div>

              <!-- Qty & Status Indicator (Total Items) -->
              <div class="slot-qty-box py-1 px-2 rounded-1 mt-auto">
                <span v-if="getItemCount(loc) > 0 || loc.TotalQty > 0 || loc.Status === 'occupied'" class="fw-bold" :class="loc.Status === 'occupied' ? 'text-warning-emphasis' : 'text-primary'">
                  {{ getItemCount(loc) }} <small class="fw-normal">{{ getItemCount(loc) === 1 ? 'item' : 'items' }}</small>
                </span>
                <span v-else class="text-success small fw-semibold">
                  <i class="fa fa-check-circle me-1"></i>Available
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  locations: {
    type: Array,
    default: () => [],
  },
});

defineEmits(['select-location']);

// Helper to get distinct item count (fallback to StockRowCount)
const getItemCount = (loc) => {
  if (loc.ItemCount !== undefined && loc.ItemCount !== null) return loc.ItemCount;
  return loc.StockRowCount || 0;
};

// Group locations by AreaCode / AreaName
const groupedLocations = computed(() => {
  const groups = {};
  for (const loc of props.locations) {
    const areaName = loc.AreaName || loc.AreaCode || 'Unassigned';
    if (!groups[areaName]) {
      groups[areaName] = [];
    }
    groups[areaName].push(loc);
  }
  return groups;
});

const badgeStatusText = (loc) => {
  if (loc.Status === 'occupied') return 'OCCUPIED';
  if (loc.Status === 'unmapped') return 'UNMAPPED';
  return 'EMPTY';
};

const cardStatusClass = (loc) => {
  if (loc.Status === 'occupied') return 'rack-card-occupied';
  if (loc.Status === 'unmapped') return 'rack-card-unmapped';
  return 'rack-card-empty';
};

const badgeStatusClass = (loc) => {
  if (loc.Status === 'occupied') return 'bg-warning text-dark';
  if (loc.Status === 'unmapped') return 'bg-secondary text-white';
  return 'bg-success text-white';
};
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.transition-all {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.rack-card {
  border: 1px solid #e2e8f0;
  background-color: #ffffff;
  display: flex;
  flex-direction: column;
}

.rack-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
  border-color: #3b82f6 !important;
}

.rack-card-empty {
  border-top: 3px solid #10b981;
}

.rack-card-occupied {
  border-top: 3px solid #f59e0b;
  background-color: #fffdfa;
}

.rack-card-unmapped {
  border-top: 3px solid #64748b;
  background-color: #f8fafc;
}

.slot-code {
  font-size: 0.85rem;
  letter-spacing: -0.01em;
}

.slot-desc {
  font-size: 0.72rem;
}

.slot-qty-box {
  background: rgba(0, 0, 0, 0.03);
  font-size: 0.78rem;
}

.badge-sm {
  font-size: 0.62rem;
  padding: 0.2em 0.45em;
  font-weight: 600;
}
</style>
