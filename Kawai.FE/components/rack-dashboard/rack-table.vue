<template>
  <div class="rack-table-container bg-white d-flex flex-column h-100 min-h-0 overflow-hidden">
    <!-- Empty State -->
    <div v-if="!locations || locations.length === 0" class="text-center py-5 my-auto">
      <div class="text-muted mb-2">
        <i class="fa fa-folder-open fa-3x opacity-50"></i>
      </div>
      <h6 class="text-dark fw-bold">No Rack Locations Found</h6>
      <p class="text-muted small mb-0">Please adjust your warehouse or area filter selection.</p>
    </div>

    <!-- Table View -->
    <template v-else>
      <div class="table-scroll-wrapper flex-fill">
        <table class="table table-hover align-middle mb-0 custom-rack-table">
          <thead class="table-dark">
          <tr>
            <th scope="col" style="width: 100px;">Warehouse</th>
            <th scope="col" style="min-width: 150px;">Warehouse Name</th>
            <th scope="col" style="width: 100px;">Area</th>
            <th scope="col" style="min-width: 130px;">Address</th>
            <th scope="col" style="min-width: 160px;">Address Name</th>
            <th scope="col" style="width: 120px;" class="text-center">Status</th>
            <th scope="col" style="width: 130px;" class="text-end">Stock Qty</th>
            <th scope="col" style="width: 110px;" class="text-center">Item Types</th>
            <th scope="col" style="width: 100px;" class="text-center">Action</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="loc in paginatedLocations"
            :key="`${loc.WarehouseCode}-${loc.AreaCode}-${loc.AddressCode}`"
            class="cursor-pointer"
            @click="$emit('select-location', loc)"
          >
            <!-- Warehouse -->
            <td class="fw-semibold text-secondary small">
              {{ loc.WarehouseCode }}
            </td>

            <!-- Warehouse Name -->
            <td class="text-dark small text-truncate" style="max-width: 220px;" :title="loc.WarehouseName">
              {{ loc.WarehouseName }}
            </td>

            <!-- Area -->
            <td>
              <span class="badge bg-light text-dark border">
                {{ loc.AreaCode || loc.AreaName }}
              </span>
            </td>

            <!-- Address Code -->
            <td>
              <button
                type="button"
                class="btn btn-link p-0 fw-bold text-primary text-decoration-none address-link"
                @click.stop="$emit('select-location', loc)"
              >
                <i class="fa fa-location-dot me-1 text-primary opacity-75"></i>
                {{ loc.AddressCode }}
              </button>
            </td>

            <!-- Address Name -->
            <td class="small text-muted text-truncate" style="max-width: 200px;" :title="loc.AddressName">
              {{ loc.AddressName || '-' }}
            </td>

            <!-- Status -->
            <td class="text-center">
              <span class="badge px-2 py-1 rounded-pill status-pill" :class="statusBadgeClass(loc)">
                {{ statusLabel(loc) }}
              </span>
            </td>

            <!-- Total Qty -->
            <td class="text-end fw-bold" :class="loc.TotalQty > 0 ? 'text-success' : 'text-muted'">
              {{ Number(loc.TotalQty || 0).toLocaleString() }}
              <span class="small fw-normal text-muted">PCS</span>
            </td>

            <!-- Item Types (Distinct) -->
            <td class="text-center">
              <span v-if="(loc.ItemCount || loc.StockRowCount) > 0" class="badge bg-secondary text-white rounded-pill px-2" :title="`${loc.StockRowCount || 0} total stock rows`">
                {{ loc.ItemCount || loc.StockRowCount }} {{ (loc.ItemCount || loc.StockRowCount) === 1 ? 'item' : 'items' }}
              </span>
              <span v-else class="text-muted small">-</span>
            </td>

            <!-- Action -->
            <td class="text-center" @click.stop>
              <button
                type="button"
                class="btn btn-sm btn-outline-primary px-2 py-1 inspect-btn"
                @click="$emit('select-location', loc)"
                title="Inspect detail"
              >
                <i class="fa fa-eye me-1"></i>Detail
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      </div>

      <!-- Pagination footer if items > pageSize -->
      <div v-if="locations.length > pageSize" class="d-flex justify-content-between align-items-center p-2 px-3 border-top bg-light flex-shrink-0 flex-wrap gap-2">
        <div class="small text-muted">
          Showing <b>{{ (currentPage - 1) * pageSize + 1 }}</b> to <b>{{ Math.min(currentPage * pageSize, locations.length) }}</b> of <b>{{ locations.length }}</b> entries
        </div>
        <nav aria-label="Table pagination">
          <ul class="pagination pagination-sm mb-0">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <button class="page-link" @click="currentPage--" :disabled="currentPage === 1">Previous</button>
            </li>
            <li
              v-for="page in totalPages"
              :key="page"
              class="page-item"
              :class="{ active: currentPage === page }"
              v-show="Math.abs(page - currentPage) <= 2 || page === 1 || page === totalPages"
            >
              <button class="page-link" @click="currentPage = page">{{ page }}</button>
            </li>
            <li class="page-item" :class="{ disabled: currentPage === totalPages }">
              <button class="page-link" @click="currentPage++" :disabled="currentPage === totalPages">Next</button>
            </li>
          </ul>
        </nav>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';

const props = defineProps({
  locations: {
    type: Array,
    default: () => [],
  },
  sorts: {
    type: Object,
    default: () => ({}),
  },
});

defineEmits(['select-location']);

const currentPage = ref(1);
const pageSize = ref(20);

// Reset to page 1 when locations or sorts change
watch([() => props.locations, () => props.sorts], () => {
  currentPage.value = 1;
});

const totalPages = computed(() => {
  return Math.ceil((props.locations?.length || 0) / pageSize.value) || 1;
});

const sortedLocations = computed(() => {
  if (!props.locations || props.locations.length === 0) return [];
  if (!props.sorts || Object.keys(props.sorts).length === 0) return props.locations;
  const list = [...props.locations];
  const sortKeys = Object.entries(props.sorts);
  if (sortKeys.length === 0) return props.locations;

  list.sort((a, b) => {
    for (const [key, dir] of sortKeys) {
      let valA = a[key];
      let valB = b[key];
      if (valA === undefined || valA === null) valA = '';
      if (valB === undefined || valB === null) valB = '';

      if (typeof valA === 'number' && typeof valB === 'number') {
        if (valA !== valB) {
          return dir === 'desc' ? valB - valA : valA - valB;
        }
      } else {
        const comp = String(valA).localeCompare(String(valB), undefined, { numeric: true });
        if (comp !== 0) {
          return dir === 'desc' ? -comp : comp;
        }
      }
    }
    return 0;
  });
  return list;
});

const paginatedLocations = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return sortedLocations.value.slice(start, start + pageSize.value);
});

function statusLabel(loc) {
  if (loc.Status === 'unmapped' || !loc.IsRegistered) return 'Unregistered';
  if (loc.Status === 'occupied') return 'Occupied';
  return 'Empty';
}

function statusBadgeClass(loc) {
  if (loc.Status === 'unmapped' || !loc.IsRegistered) {
    return 'badge-unregistered';
  }
  if (loc.Status === 'occupied') {
    return 'bg-success bg-opacity-10 text-success border border-success';
  }
  return 'bg-secondary bg-opacity-10 text-secondary border border-secondary';
}
</script>

<style scoped>
.rack-table-container {
  width: 100%;
}

.table-scroll-wrapper {
  flex: 1;
  min-height: 0;
  overflow: auto;
  position: relative;
}

.custom-rack-table {
  width: 100%;
  min-width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.custom-rack-table thead th {
  position: sticky;
  top: 0;
  z-index: 20;
  background-color: #1b566a;
  color: #fff;
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 10px 12px;
  border-bottom: 2px solid #174a5c;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  white-space: nowrap;
}

.custom-rack-table tbody td {
  padding: 10px 12px;
  font-size: 13px;
  border-bottom: 1px solid #e9ecef;
  background-color: #fff;
}

.custom-rack-table tbody tr:hover td {
  background-color: #f1f8fa !important;
}

.address-link {
  font-size: 13px;
}
.address-link:hover {
  text-decoration: underline !important;
}

.inspect-btn {
  font-size: 11px;
  border-radius: 6px;
}

.badge-unregistered {
  background-color: #f3e8ff;
  color: #7e22ce;
  border: 1px solid #d8b4fe;
}

.status-pill {
  font-size: 11px;
  font-weight: 600;
}

/* Custom scrollbar for table scroll wrapper */
.table-scroll-wrapper::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}
.table-scroll-wrapper::-webkit-scrollbar-track {
  background: #f1f5f9;
}
.table-scroll-wrapper::-webkit-scrollbar-thumb {
  background-color: #cbd5e1;
  border-radius: 4px;
}
.table-scroll-wrapper::-webkit-scrollbar-thumb:hover {
  background-color: #94a3b8;
}
</style>
