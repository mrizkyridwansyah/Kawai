var app = useNuxtApp();

export const useNGClaim = defineStore('NGClaim', {
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
        app.$http.post(`/ngclaim/list`, this.filter)
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
    loadDetail: function (claimid) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/ngclaim/data-header?claimid=${claimid}`)
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
    listDetail: function (claimid) {
      this.isLoadingListDetail = true;
      this.isNetworkListDetailError = this.isServerListDetailError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/ngclaim/list-detail?claimid=${claimid}`)
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
        app.$http.post(`/ngclaim/list-po-detail`, filters)
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
      listNGClaimDetail: function (filters) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/ngclaim/list-ngclaim-detail`, filters)
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
        app.$http.post(`/ngclaim/inquiry`, this.filter)
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
        app.$http.post(`/ngclaim/create`, data)
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
        app.$http.patch(`/ngclaim/update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    approve: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/ngclaim/approve`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    remove: function (claimid) {
      this.isRemoving = true;
      return new Promise((resolve, reject) => {
        app.$http.delete(`/ngclaim/remove?claimid=${claimid}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isRemoving = false);

      })
    },

     PrintSuratJalan1: function (factory,claimid) {
      this.isLoading = true;
      return app.$http.post(
        `/ngclaim/report-surat-jalan?factory=${factory}&claimid=${claimid}`,
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
              throw { message: json.Message || 'Server returned an error', isServerError: true};
            } catch (e) {
              console.log('Server Error, but not JSON', e);
              throw { message: e.message || 'Server returned an error', isServerError: true};
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


   PrintSuratJalan: function (filter) {
      return new Promise((resolve, reject) => {
         

        app.$http.post(`/ngclaim/report-surat-jalan`, filter)
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
              link.setAttribute('download', 'SuratJalan.xlsx');
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

   
    exportExcel: function (filters) {
      return new Promise((resolve, reject) => {
        let filterExport = {
          Page: 1,
          Length: 1000000,
          Filters: filters,
          Sorts: {},
        };

        app.$http.post(`/ngclaim/export/excel`, filterExport)
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
              link.setAttribute('download', 'List_NGClaim.xlsx');
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
        app.$http.get(`/ngclaim/history?warehouse=${warehouse}&item=${item}&lotno=${lotno}&period=${period}`)
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
  import.meta.hot.accept(acceptHMRUpdate(useNGClaim, import.meta.hot));
}