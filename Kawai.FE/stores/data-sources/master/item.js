var app = useNuxtApp();

export const useItem = defineStore('Item', {
  state: () => ({
    isLoading: false,
    isCreating: false,
    isEditing: false,
    isRemoving: false,
    isServerError: false,
    isNetworkError: false,
    isLoadingDetail: false,
    detail: {},
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
      Filters: [

      ],
      Sorts: {},
    },
  }),
  actions: {
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/item/list`, this.filter)
          .then(({ data }) => {
            this.data = data.Data;

            resolve(data);
          })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoading = false);
      })
    },
    loadDetail: function (id) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/item/detail?id=${id}`)
          .then(({ data }) => {
            this.detail = data.Data;

            resolve(data);
          })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoadingDetail = false);
      })
    },
    setFilter: function (v) {
      this.filter.Filters = v;
      this.filter.Page = 1;
    },
    setSort: function (v) {
      this.filter.Sorts = v;
    },
    setPage: function (v) {
      this.filter.Page = v;
      this.load();
    },
    setLength: function (v) {
      this.filter.Page = 1;
      this.filter.Length = v;
      this.load();
    },
    create: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/item/create`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isCreating = false);
      })
    },
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/item/update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    remove: function (id) {
      this.isRemoving = true;
      return new Promise((resolve, reject) => {
        app.$http.delete(`/item/remove?id=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isRemoving = false);

      })
    },
    exportExcelUsingJob: function() {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        let filterExport = {
          Page: 1,
          Length: 1000000,
          Filters: [],
          Sorts: {},
        };

        app.$http.post(`/item/export/excel-using-job`, filterExport)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    exportExcel: function () {
      return new Promise((resolve, reject) => {
        let filterExport = {
          Page: 1,
          Length: 1000000,
          Filters: [],
          Sorts: {},
        };

        app.$http.post('/item/export/excel', filterExport, {
          responseType: 'blob'
        })
          .then(res => {
            const url = URL.createObjectURL(res.data);

            const link = document.createElement('a');
            link.href = url;
            link.download = 'List_Item.xlsx';
            link.click();

            URL.revokeObjectURL(url);
            resolve();
          })
          .catch(async (err) => {
            reject(err?.response?.data);
          })
          .finally(() => {
            this.isLoading = false;
          });
      })
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useItem, import.meta.hot));
}