var app = useNuxtApp();

export const useReceipt = defineStore('Receipt', {
  state: () => ({
    isLoading: false,
    isCreating: false,
    isEditing: false,
    isRemoving: false,
    isServerError: false,
    isNetworkError: false,
    isLoadingDetail: false,

    isLoadingListDetail: false,
    isServerListDetailError: false,
    isNetworListDetailkError: false,

    detail: {},
    data: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 25,
    },
    dataListDetail: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 25,
    },
    dataInquiry: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 25,
    },
    filter: {
      Page: 1,
      Length: 25,
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
        app.$http.post(`/receipt/list`, this.filter)
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
        app.$http.get(`/receipt/data-header?id=${id}`)
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
    listDetail: function (id) {
      this.isLoadingListDetail = true;
      this.isNetworkListDetailError = this.isServerListDetailError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/list-detail?id=${id}`)
          .then(({ data }) => {
            this.dataListDetail = data.Data;

            resolve(data);
          })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkListDetailError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoadingListDetail = false);
      })
    },
    listPODetail: function (filters) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/list-po-detail`, filters)
          .then(({ data }) => {
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

    listClaimDetail: function (filters) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/list-claim-detail`, filters)
          .then(({ data }) => {
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
    loadInquiry: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/inquiry`, this.filter)
          .then(({ data }) => {
            this.dataInquiry = data.Data;
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
    setPageInquiry: function (v) {
      this.filter.Page = v;
      this.loadInquiry();
    },
    setLengthInquiry: function (v) {
      this.filter.Page = 1;
      this.filter.Length = v;
      this.loadInquiry();
    },
    create: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/create`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isCreating = false);
      })
    },
    createclaim: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/create-claim`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isCreating = false);
      })
    },
    checkIsDetailUpdate: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/receipt/check-is-details-update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/receipt/update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    updateclaim: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/receipt/update-claim`, data)
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
        app.$http.delete(`/receipt/remove?id=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isRemoving = false);

      })
    },
    printLabel: function (id) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/print-label?receiptId=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },

    print: function (id) {
      this.isLoading = true;

      return app.$http.post(
        `/receipt/print-barcodes?receiptId=${id}`,
        null,
        { responseType: 'blob' }
      )
        .then(res => {
          const blob = res.data instanceof Blob
            ? res.data
            : new Blob([res.data], { type: 'application/pdf' });

          const url = window.URL.createObjectURL(blob);

          const contentDisposition = res.headers['content-disposition'];
          let fileName = 'default.pdf';
          if (contentDisposition) {
            const match = contentDisposition.match(/filename="(.+)"/);
            if (match.length === 2) fileName = match[1];
          }
          const link = document.createElement('a');
          link.href = url;
          link.download = fileName;
          document.body.appendChild(link);
          link.click();

          document.body.removeChild(link);
          window.URL.revokeObjectURL(url);
        })
        .catch(err => {
          if (err?.code === 'ERR_NETWORK')
            this.isNetworkError = true;

          if (err?.code === 'ERR_BAD_RESPONSE')
            this.isServerError = true;

          throw err;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    printBarcodesUsingJob: function (id) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/receipt/print-barcodes-using-job?receiptId=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    exportExcel: function (filters) {
      return new Promise((resolve, reject) => {
        let filterExport = {
          Page: 1,
          Length: 1000000,
          Filters: filters,
          Sorts: {},
        };

        app.$http.post(`/receipt/export/excel`, filterExport)
          .then(({ data }) => {
            if (data.Data) {
              const byteCharacters = atob(data.Data); // decode base64
              const byteNumbers = new Array(byteCharacters.length).fill(0).map((_, i) => byteCharacters.charCodeAt(i));
              const byteArray = new Uint8Array(byteNumbers);

              const blob = new Blob([byteArray], {
                type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
              });

              const url = URL.createObjectURL(blob);
              const link = document.createElement('a');
              link.href = url;
              link.setAttribute('download', 'List_Receipt.xlsx');
              document.body.appendChild(link);
              link.click();
              document.body.removeChild(link);
              URL.revokeObjectURL(url);

              resolve();
            } else {
              reject(data);
            }
          })
          .catch(async (err) => {
            reject(err?.response?.data);
          })
          .finally(() => {
            this.isLoading = false;
          });
      })
    },
    inquiry: function (warehouse, item, lotno, period) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/receipt/history?warehouse=${warehouse}&item=${item}&lotno=${lotno}&period=${period}`)
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
          .finally(_ => this.isLoadingDetail = false);
      })
    },

  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useReceipt, import.meta.hot));
}