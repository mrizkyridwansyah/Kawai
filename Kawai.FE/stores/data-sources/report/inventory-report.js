var app = useNuxtApp();

export const useInventoryReport = defineStore('InventoryReport', {
  state: () => ({
    isLoading: false,
    isServerError: false,
    isNetworkError: false,

    data: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },

    filter: {
      Page: 1,
      Length: 10,
      Filters: [],
      Sorts: {},
    },
  }),

  actions: {
    /* =============================
       LOAD DATA (LIST)
    ==============================*/
    load() {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;

      return new Promise((resolve, reject) => {
        app.$http
          .post(`/inventory-report/list`, this.filter)
          .then(({ data }) => {
            this.data = data.Data;
            resolve(data);
          })
          .catch((err) => {
            if (err.code === 'ERR_NETWORK') this.isNetworkError = true;
            if (err.code === 'ERR_BAD_RESPONSE') this.isServerError = true;
            reject(err);
          })
          .finally(() => {
            this.isLoading = false;
          });
      });
    },

    /* =============================
       FILTER & SORT
    ==============================*/
    setFilter(filters) {
      this.filter.Filters = filters;
      this.filter.Page = 1;
    },

    setSort(sorts) {
      this.filter.Sorts = sorts;
    },

    /* =============================
       PAGINATION
    ==============================*/
    setPage(page) {
      this.filter.Page = page;
      this.load();
    },

    setLength(length) {
      this.filter.Page = 1;
      this.filter.Length = length;
      this.load();
    },

    /* =============================
       EXPORT EXCEL
    ==============================*/
    exportExcel(filters) {
      return new Promise((resolve, reject) => {
        const filterExport = {
          Page: 1,
          Length: 1000000,
          Filters: filters,
          Sorts: {},
        };

        app.$http
          .post(`/inventory-report/export/excel`, filterExport)
          .then(({ data }) => {
            if (!data.Data) {
              reject(data);
              return;
            }

            const byteCharacters = atob(data.Data);
            const byteNumbers = new Array(byteCharacters.length)
              .fill(0)
              .map((_, i) => byteCharacters.charCodeAt(i));
            const byteArray = new Uint8Array(byteNumbers);

            const blob = new Blob([byteArray], {
              type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            });

            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'Inventory_Report.xlsx');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);

            resolve();
          })
          .catch((err) => {
            reject(err?.response?.data);
          })
          .finally(() => {
            this.isLoading = false;
          });
      });
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(
    acceptHMRUpdate(useInventoryReport, import.meta.hot)
  );
}
