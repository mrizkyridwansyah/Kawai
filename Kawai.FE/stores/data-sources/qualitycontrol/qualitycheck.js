var app = useNuxtApp();

export const useQualityCheck = defineStore('QualityCheck', {
  state: () => ({
    isLoading: false,
    isCreating: false,
    isConfirm: false,
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
        app.$http.post(`/qualitycheck/list`, this.filter)
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
        app.$http.get(`/qualitycheck/detail?id=${id}`)
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
    save: function (data) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/qualitycheck/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    confirm: function (data) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/qualitycheck/confirm`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    cancelConfirm: function (data) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/qualitycheck/cancel-confirm`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    approvalSA: function (data) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/qualitycheck/approval-sa`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    printReportNG: function (receiptId) {
      this.isLoading = true;
      return app.$http.post(
        `/qualitycheck/print/report-ng?receiptId=${receiptId}`,
        null,
        { responseType: 'blob' }
      )
        .then(res => {
          const blob = res.data instanceof Blob
            ? res.data
            : new Blob([res.data], { type: 'application/pdf' });

          const url = window.URL.createObjectURL(blob);

          let fileName = 'default.pdf';
          const contentDisposition = res.headers['content-disposition'];
          if (contentDisposition) {
            const match = contentDisposition.match(/filename\*?=(?:UTF-8''|")?([^;"\n]+)/i);
            if (match && match[1]) {
              fileName = decodeURIComponent(match[1].trim());
            }
          }

          const link = document.createElement('a');
          link.href = url;
          link.download = fileName;
          document.body.appendChild(link);
          link.click();

          document.body.removeChild(link);
          window.URL.revokeObjectURL(url);
        })
        .catch(async err => {
          if (err.response) {
            // server ngirim response, tapi error
            const blob = err.response.data;
            try {
              const text = await blob.text();
              const json = JSON.parse(text);
              throw { message: json.Message || 'Server returned an error', isServerError: true };
            } catch (e) {
              console.log('Server Error, but not JSON', e);
              throw { message: e.message || 'Server returned an error', isServerError: true };
            }
          }

          if (err?.code === 'ERR_NETWORK') this.isNetworkError = true;
          if (err?.code === 'ERR_BAD_RESPONSE') this.isServerError = true;

          throw err;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    printReportNGUsingJob: function (id) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/qualitycheck/print/report-ng-by-job?receiptId=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    export: function (filters) {
      let xPayload = {
        Page: 1,
        Length: 10,
        Filters: filters,
        Sorts: {},
      }
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/qualitycheck/export`, xPayload)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useQualityCheck, import.meta.hot));
}