<template>
  <v-frame title="Stock Inquiry by Rack Address" icon="boxes-stacked">
    <template #frame-content>
      <div class="rack-dashboard-page-container">
        <!-- Filter Table (Standard Kawai 2-Column with Auto-Description) -->
        <table class="filter-table mb-2" style="width: auto; align-self: flex-start;">
          <tr>
            <!-- Warehouse -->
            <td style="padding-top: 3px; vertical-align: middle;">
              <label class="form-label mb-0">Warehouse</label>
            </td>
            <td style="padding-top: 3px; padding-left: 15px; vertical-align: middle;">
              <rack-dashboard-rack-filter-select
                v-model="store.selectedWarehouse"
                :options="warehouseOptions"
                :selected-desc="selectedWarehouseName"
                desc-placeholder="Warehouse Description"
                @change="store.setWarehouse(store.selectedWarehouse)"
              />
            </td>

            <!-- Status with Description -->
            <td style="padding-top: 3px; padding-left: 25px; vertical-align: middle;">
              <label class="form-label mb-0">Status</label>
            </td>
            <td style="padding-top: 3px; padding-left: 15px; vertical-align: middle;">
              <rack-dashboard-rack-filter-select
                v-model="store.selectedStatus"
                :options="statusOptions"
                :selected-desc="selectedStatusDescription"
                desc-placeholder="Status Description"
                @change="store.setStatus(store.selectedStatus)"
              />
            </td>
          </tr>

          <tr>
            <!-- Area -->
            <td style="padding-top: 3px; vertical-align: middle;">
              <label class="form-label mb-0">Area</label>
            </td>
            <td style="padding-top: 3px; padding-left: 15px; vertical-align: middle;">
              <rack-dashboard-rack-filter-select
                v-model="store.selectedArea"
                :options="areaOptions"
                :selected-desc="selectedAreaName"
                desc-placeholder="Area Description"
                @change="store.setArea(store.selectedArea)"
              />
            </td>

            <!-- Item (Disimpan di bawah Status) -->
            <td style="padding-top: 3px; padding-left: 25px; vertical-align: middle;">
              <label class="form-label mb-0">Item</label>
            </td>
            <td style="padding-top: 3px; padding-left: 15px; vertical-align: middle;">
              <rack-dashboard-rack-filter-select
                v-model="store.selectedItem"
                :options="itemOptions"
                :selected-desc="selectedItemName"
                desc-placeholder="Item Description"
                @change="store.setItem(store.selectedItem)"
              />
            </td>
          </tr>

          <tr>
            <td style="padding-top: 5px" colspan="4">
              <div class="d-flex flex-fill">
                <v-button-search-reset
                  class="mr-1"
                  :search="onSearch"
                  :reset="onReset"
                />
              </div>
            </td>
          </tr>
        </table>
        <hr class="my-2" />

        <!-- Unified Dashboard Panel (Header + Attached KPI Strip + Scrollable Content) -->
        <div class="unified-dashboard-panel d-flex flex-column">
          <!-- Unified Search & Toolbar Panel Header (Ref: app/master/warehouse v-table) -->
          <div
            class="ui-sortable-handle panel-table-header flex-shrink-0"
            style="
              background-color: #333;
              display: flex;
              padding: 1em;
              border-start-start-radius: 0.5em;
              border-start-end-radius: 0.5em;
              align-items: center;
              justify-content: space-between;
              gap: 12px;
            "
          >
            <!-- Left Side: Sort button (Table View only, Ref: master/warehouse) + Search Input stretching up to location badge -->
            <div class="d-flex align-items-center flex-grow-1">
              <!-- Sort Button shown only in Table View -->
              <v-button-sort
                v-if="store.activeView === 'table'"
                class="me-2 flex-shrink-0"
                v-model="sorts"
                :items="sortItems"
              />

              <!-- Search Field following http://localhost:3000/app/master/warehouse (fills width up to badge location) -->
              <div class="input-group flex-grow-1">
                <input
                  type="text"
                  placeholder="Search..."
                  class="form-control"
                  style="padding: 4px 10px; font-size: 13px;"
                  v-model="searchQuery"
                  @keyup.enter="onSearch"
                />
              </div>
            </div>

            <!-- Right Controls: Locations Counter & View Switcher -->
            <div class="d-flex align-items-center gap-2 flex-shrink-0">
              <span class="text-white small me-2 text-nowrap">
                <i class="fa fa-cubes me-1 text-info"></i> {{ store.locations?.length || 0 }} locations
              </span>

              <!-- Grid vs Table View Switcher -->
              <div class="btn-group btn-group-sm" role="group" aria-label="View switcher">
                <button
                  type="button"
                  class="btn fw-semibold py-1 px-2"
                  :class="store.activeView === 'grid' ? 'btn-primary' : 'btn-dark border-secondary text-white'"
                  @click="store.setActiveView('grid')"
                >
                  <i class="fa fa-th-large me-1"></i> Grid View
                </button>
                <button
                  type="button"
                  class="btn fw-semibold py-1 px-2"
                  :class="store.activeView === 'table' ? 'btn-primary' : 'btn-dark border-secondary text-white'"
                  @click="store.setActiveView('table')"
                >
                  <i class="fa fa-table-list me-1"></i> Table View
                </button>
              </div>
            </div>
          </div>

          <!-- Inside Panel Body: KPI Strip (fixed at top of panel body) -->
          <div class="panel-body-top-wrapper flex-shrink-0">
            <rack-dashboard-rack-kpi :kpis="store.kpis" />
          </div>

          <!-- Scrollable Content Section (Grid View / Table View) -->
          <div
            class="content-scroll-area flex-fill"
            :class="store.activeView === 'table' ? 'view-table' : 'view-grid'"
          >
            <!-- Loading State -->
            <div v-if="store.isLoadingLocations" class="text-center py-5 bg-white rounded-3 shadow-sm my-auto">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
              <div class="text-muted small mt-2">Loading rack locations...</div>
            </div>

            <!-- Main Display: Grid View or Table View -->
            <template v-else>
              <!-- Grid View -->
              <rack-dashboard-rack-grid
                v-if="store.activeView === 'grid'"
                :locations="store.locations"
                @select-location="handleSelectLocation"
              />

              <!-- Table View -->
              <rack-dashboard-rack-table
                v-else-if="store.activeView === 'table'"
                :locations="store.locations"
                :sorts="sorts"
                @select-location="handleSelectLocation"
              />
            </template>
          </div>
        </div>
      </div>

      <!-- Center Modal Detail Inspector (Default & Only Detail Display) -->
      <rack-dashboard-rack-detail-modal
        :is-open="store.isDetailModalOpen"
        :is-loading="store.isLoadingDetail"
        :detail="store.detail"
        @close="store.closeDetailModal"
      />
    </template>
  </v-frame>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRackDashboard } from '@/stores/data-sources/stock-control/rack-dashboard';

definePageMeta({
  layout: 'app',
});

const store = useRackDashboard();
const searchQuery = ref('');

// Sort Items & Sorts Model for Table View (Ref: master/warehouse v-table)
const sortItems = ref([
  { label: 'Address Code', value: 'AddressCode', selected: true, direction: 'asc' },
  { label: 'Address Name', value: 'AddressName', selected: false, direction: 'asc' },
  { label: 'Area Code', value: 'AreaCode', selected: false, direction: 'asc' },
  { label: 'Status', value: 'Status', selected: false, direction: 'asc' },
  { label: 'Total Qty', value: 'TotalQty', selected: false, direction: 'desc' },
]);
const sorts = ref({ AddressCode: 'asc' });

// Dropdown options formatted as Code | Name (Standard Kawai DDL format)
const warehouseOptions = computed(() => {
  const list = [
    {
      Code: 'ALL',
      Name: 'All Warehouses',
      DDLDescription: 'ALL',
    },
  ];
  (store.warehouses || []).forEach((w) => {
    const code = w.WarehouseCode || w.Code;
    const name = w.WarehouseName || w.Name || code;
    list.push({
      Code: code,
      Name: name,
      DDLDescription: `${code} | ${name}`,
    });
  });
  return list;
});

const areaOptions = computed(() => {
  const list = [
    {
      Code: 'ALL',
      Name: 'All Areas',
      DDLDescription: 'ALL',
    },
  ];
  (store.areas || []).forEach((a) => {
    const code = a.AreaCode || a.Code;
    const name = a.AreaName || a.Name || code;
    list.push({
      Code: code,
      Name: name,
      DDLDescription: `${code} | ${name}`,
    });
  });
  return list;
});

const statusOptions = computed(() => [
  {
    Code: 'ALL',
    Name: 'All Statuses',
    DDLDescription: 'ALL',
  },
  {
    Code: 'empty',
    Name: 'Empty Address (Available)',
    DDLDescription: 'EMPTY | Empty Address',
  },
  {
    Code: 'occupied',
    Name: 'Occupied Address (With Stock)',
    DDLDescription: 'OCCUPIED | Occupied Address',
  },
  {
    Code: 'unmapped',
    Name: 'Unregistered Address (Temporary / TMP)',
    DDLDescription: 'UNREGISTERED | Unregistered Address',
  },
]);

// Available Items from store (with fallback to KPI items)
const availableItems = computed(() => {
  if (store.allItems && store.allItems.length > 0) {
    return store.allItems;
  }
  return store.kpis?.Items || [];
});

const itemOptions = computed(() => {
  const list = [
    {
      Code: 'ALL',
      Name: 'All Items',
      DDLDescription: 'ALL',
    },
  ];
  (availableItems.value || []).forEach((i) => {
    const code = i.ItemCode;
    const name = i.ItemName || code;
    list.push({
      Code: code,
      Name: name,
      DDLDescription: `${code} | ${name}`,
    });
  });
  return list;
});

// Computed Description for Warehouse
const selectedWarehouseName = computed(() => {
  if (!store.selectedWarehouse || store.selectedWarehouse === 'ALL') return 'All Warehouses';
  const wh = store.warehouses.find(
    (w) => (w.WarehouseCode || w.Code) === store.selectedWarehouse
  );
  return wh ? (wh.WarehouseName || wh.Name) : store.selectedWarehouse;
});

// Computed Description for Area
const selectedAreaName = computed(() => {
  if (!store.selectedArea || store.selectedArea === 'ALL') return 'All Areas';
  const ar = store.areas.find(
    (a) => (a.AreaCode || a.Code) === store.selectedArea
  );
  return ar ? (ar.AreaName || ar.Name) : store.selectedArea;
});

// Computed Description for Status
const selectedStatusDescription = computed(() => {
  switch (store.selectedStatus) {
    case 'empty':
      return 'Empty Address (Available)';
    case 'occupied':
      return 'Occupied Address (With Stock)';
    case 'unmapped':
      return 'Unregistered Address (Temporary / TMP)';
    default:
      return 'All Statuses';
  }
});

// Computed Description for Item
const selectedItemName = computed(() => {
  if (!store.selectedItem || store.selectedItem === 'ALL') return 'All Items';
  const item = availableItems.value.find((i) => i.ItemCode === store.selectedItem);
  return item ? (item.ItemName || item.ItemCode) : store.selectedItem;
});

onMounted(async () => {
  await store.fetchWarehouses();
});

const onSearch = () => {
  store.setSearch(searchQuery.value);
};

const onReset = async () => {
  searchQuery.value = '';
  await store.resetFilters();
};

const handleSelectLocation = (loc) => {
  store.fetchDetail(loc.WarehouseCode, loc.AreaCode, loc.AddressCode);
};
</script>

<style scoped>
:deep(.panel.panel-inverse) {
  margin-bottom: 0 !important;
}

:deep(.panel-body) {
  padding: 8px 15px 10px 15px !important;
  overflow: hidden !important;
}

.rack-dashboard-page-container {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.filter-table {
  width: auto !important;
  align-self: flex-start !important;
}

.unified-dashboard-panel {
  width: 100%;
  height: calc(100vh - 290px);
  min-height: 0;
}

.panel-table-header {
  border-start-start-radius: 0.5em;
  border-start-end-radius: 0.5em;
  margin-bottom: 0 !important;
}

.panel-body-top-wrapper {
  border-left: 1px solid #d1d5db;
  border-right: 1px solid #d1d5db;
  background-color: #f8fafc;
  padding: 8px 10px 4px 10px;
}

.content-scroll-area {
  width: 100%;
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
  border-left: 1px solid #d1d5db;
  border-right: 1px solid #d1d5db;
  border-bottom: 1px solid #d1d5db;
  border-end-start-radius: 0.5em;
  border-end-end-radius: 0.5em;
  background-color: #f8fafc;
}

.content-scroll-area.view-grid {
  overflow-y: auto;
  overflow-x: auto;
  padding: 4px 10px 10px 10px;
}

.content-scroll-area.view-table {
  overflow: hidden;
  padding: 0;
}

/* Custom modern scrollbar for content */
.content-scroll-area::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}
.content-scroll-area::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 4px;
}
.content-scroll-area::-webkit-scrollbar-thumb {
  background-color: #cbd5e1;
  border-radius: 4px;
}
.content-scroll-area::-webkit-scrollbar-thumb:hover {
  background-color: #94a3b8;
}
</style>
