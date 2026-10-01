import { defineStore, acceptHMRUpdate } from 'pinia';

const getHttp = () => {
  const nuxtApp = useNuxtApp();
  return nuxtApp.$http;
};

export const useRackDashboard = defineStore('RackDashboard', {
  state: () => ({
    isLoading: false,
    isLoadingLocations: false,
    isLoadingDetail: false,
    isServerError: false,
    isNetworkError: false,

    warehouses: [],
    areas: [],
    kpis: {
      TotalLocations: 0,
      EmptyLocations: 0,
      OccupiedLocations: 0,
      UnmappedLocations: 0,
      TotalStockQty: 0,
      TotalStockRows: 0,
      TotalItemCount: 0,
      EmptyPercentage: 0,
      Items: [],
    },
    locations: [],
    detail: null,

    // Filter states (empty string represents 'ALL')
    selectedWarehouse: '',
    selectedArea: '',
    selectedStatus: '',
    selectedItem: '',
    allItems: [],
    searchQuery: '',

    // View mode: 'grid' | 'table'
    activeView: 'grid',

    // Detail display mode: 'drawer' | 'modal' (Default: 'modal')
    detailDisplayMode: 'modal',

    // Modal state
    isDetailModalOpen: false,
  }),

  actions: {
    async fetchWarehouses() {
      this.isLoading = true;
      try {
        const http = getHttp();
        const { data } = await http.get('/rack-dashboard/warehouses');
        this.warehouses = data.Data || [];

        await this.fetchAreas();
        await this.fetchDashboardData();
        return this.warehouses;
      } catch (err) {
        console.error('Error fetching warehouses:', err);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async fetchAreas() {
      try {
        const http = getHttp();
        const params = {};
        if (this.selectedWarehouse && this.selectedWarehouse !== 'ALL') {
          params.warehouse = this.selectedWarehouse;
        }
        const { data } = await http.get('/rack-dashboard/areas', { params });
        this.areas = data.Data || [];

        if (this.selectedArea && this.selectedArea !== 'ALL') {
          const exists = this.areas.some(
            a => (a.AreaCode || a.Code) === this.selectedArea
          );
          if (!exists) this.selectedArea = '';
        }
        return this.areas;
      } catch (err) {
        console.error('Error fetching areas:', err);
        throw err;
      }
    },

    async fetchDashboardData() {
      this.isLoadingLocations = true;
      this.isServerError = false;
      this.isNetworkError = false;

      const params = {
        warehouse: this.selectedWarehouse || undefined,
        area: this.selectedArea || undefined,
        status: this.selectedStatus || undefined,
        search: this.searchQuery || undefined,
        item: this.selectedItem || undefined,
      };

      try {
        const http = getHttp();
        const [kpiRes, locRes] = await Promise.all([
          http.get('/rack-dashboard/kpis', { params }),
          http.get('/rack-dashboard/locations', { params }),
        ]);

        this.kpis = kpiRes.data.Data || {
          TotalLocations: 0,
          EmptyLocations: 0,
          OccupiedLocations: 0,
          UnmappedLocations: 0,
          TotalStockQty: 0,
          TotalStockRows: 0,
          TotalItemCount: 0,
          EmptyPercentage: 0,
          Items: [],
        };

        // Cache all items when unfiltered, so selecting an item doesn't collapse the dropdown options
        if (!this.selectedItem || this.allItems.length === 0) {
          if (this.kpis.Items && this.kpis.Items.length > 0) {
            this.allItems = this.kpis.Items;
          }
        }

        this.locations = locRes.data.Data || [];
      } catch (err) {
        console.error('Error fetching dashboard data:', err);
        if (err.code === 'ERR_NETWORK') this.isNetworkError = true;
        if (err.code === 'ERR_BAD_RESPONSE') this.isServerError = true;
      } finally {
        this.isLoadingLocations = false;
      }
    },

    async fetchDetail(warehouse, area, address) {
      this.isLoadingDetail = true;
      this.detail = null;
      try {
        const http = getHttp();
        const { data } = await http.get('/rack-dashboard/detail', {
          params: { warehouse, area, address },
        });
        this.detail = data.Data;
        this.isDetailModalOpen = true;
        return this.detail;
      } catch (err) {
        console.error('Error fetching location detail:', err);
        throw err;
      } finally {
        this.isLoadingDetail = false;
      }
    },

    closeDetailModal() {
      this.isDetailModalOpen = false;
      this.detail = null;
    },

    setWarehouse(code) {
      this.selectedWarehouse = (!code || code === 'ALL') ? '' : code;
      this.selectedArea = '';
      this.selectedItem = '';
      this.allItems = [];
      this.fetchAreas().then(() => this.fetchDashboardData());
    },

    setArea(code) {
      this.selectedArea = (!code || code === 'ALL') ? '' : code;
      this.selectedItem = '';
      this.allItems = [];
      this.fetchDashboardData();
    },

    setStatus(status) {
      this.selectedStatus = (!status || status === 'ALL') ? '' : status;
      this.fetchDashboardData();
    },

    setItem(code) {
      this.selectedItem = (!code || code === 'ALL') ? '' : code;
      this.fetchDashboardData();
    },

    setSearch(query) {
      this.searchQuery = query;
      this.fetchDashboardData();
    },

    setActiveView(view) {
      this.activeView = view;
    },

    setDetailDisplayMode(mode) {
      this.detailDisplayMode = mode;
    },

    async resetFilters() {
      this.selectedWarehouse = '';
      this.selectedArea = '';
      this.selectedStatus = '';
      this.selectedItem = '';
      this.searchQuery = '';
      this.allItems = [];
      await this.fetchAreas();
      await this.fetchDashboardData();
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useRackDashboard, import.meta.hot));
}
