var app = useNuxtApp();

export const useSupplyRequestBOM = defineStore("SupplyRequestBOM", {
  persist: {
    paths: ["filter.Filters", "newRequest"],
  },
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
    dataDetails: {
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

    dataListStock: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },
    filterListStock: {
      Page: 1,
      Length: 10,
      Filters: [],
      Sorts: {},
    },

    newRequest: [],
  }),
  actions: {
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/bom/list-header`, this.filter)
          .then(({ data }) => {
            this.data = data.Data;
            resolve(data);
          })
          .catch((err) => {
            if (err.code == "ERR_NETWORK") this.isNetworkError = true;

            if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err);
          })
          .finally((_) => (this.isLoading = false));
      });
    },
    loadDetail: function () {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/bom/list-detail`, this.newRequest)
          .then(({ data }) => {
            this.dataDetails.Items = data.Data;
            resolve(data);
          })
          .catch((err) => {
            if (err.code == "ERR_NETWORK") this.isNetworkError = true;

            if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err);
          })
          .finally((_) => (this.isLoadingDetail = false));
      });
    },
    loadListStock: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;

      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/bom/list-stock`, this.filterListStock)
          .then(({ data }) => {
            this.dataListStock = data.Data;

            resolve(data);
          })
          .catch((err) => {
            if (err.code == "ERR_NETWORK") this.isNetworkError = true;

            if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err);
          })
          .finally((_) => (this.isLoading = false));
      });
    },
    loadHeader: function (requestId, itemCode) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;

      return new Promise((resolve, reject) => {
        app.$http
          .get(
            `/supply-request/bom/data-header?requestId=${this.newRequest[0].RequestId}&itemCode=${this.newRequest[0].ItemCode}`,
          )
          .then(({ data }) => {
            resolve(data.Data);
          })
          .catch((err) => {
            if (err.code == "ERR_NETWORK") this.isNetworkError = true;

            if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err);
          })
          .finally((_) => (this.isLoading = false));
      });
    },
    setRequest: function (req) {
      this.newRequest = req;
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
    },
    setLength: function (v) {
      this.filter.Page = 1;
      this.filter.Length = v;
    },

    setFilterListStock: function (v) {
      this.filterListStock.Filters = v;
      this.filterListStock.Page = 1;
    },
    setSortListStock: function (v) {
      this.filterListStock.Sorts = v;
    },
    setPageListStock: function (v) {
      this.filterListStock.Page = v;
      this.loadListStock();
    },
    setLengthListStock: function (v) {
      this.filterListStock.Page = 1;
      this.filterListStock.Length = v;
      this.loadListStock();
    },
    save: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/bom/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isCreating = false));
      });
    },
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http
          .patch(`/supply-request/bom/update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isEditing = false));
      });
    },
    remove: function (id, reqNo) {
      this.isRemoving = true;
      return new Promise((resolve, reject) => {
        app.$http
          .delete(
            `/supply-request/bom/remove?requestId=${id}&requestNo=${reqNo}`,
          )
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isRemoving = false));
      });
    },

     PrintSuratJalan: function (requestno) {
      this.isLoading = true;
      return app.$http.post(
        `/supply-request/bom/report-surat-jalan?requestno=${requestno}`,
        null,
        { responseType: 'blob' }
      )
        .then(res => {
          const blob = res.data instanceof Blob
            ? res.data
            : new Blob([res.data], { type: 'application/pdf' });

          const url = window.URL.createObjectURL(blob);

          let fileName = 'Surat_Jalan.pdf';
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

  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSupplyRequestBOM, import.meta.hot));
}
