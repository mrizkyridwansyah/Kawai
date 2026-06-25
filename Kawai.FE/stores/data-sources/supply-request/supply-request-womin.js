var app = useNuxtApp();

export const useSupplyRequestWomin = defineStore("SupplyRequestWomin", {
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

     dataListScan: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },
    filterListScan: {
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
          .post(`/supply-request/womin/list-header`, this.filter)
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
          .post(`/supply-request/womin/list-detail`, this.newRequest)
          .then(({ data }) => {
            this.dataDetails.Items = data.Data;
            resolve(data);
          })
          .catch((err) => {
            // if (err.code == "ERR_NETWORK") this.isNetworkError = true;

            // if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

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
          .post(`/supply-request/womin/list-stock`, this.filterListStock)
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

     loadListScan: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;

      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/womin/list-scan`, this.filterListScan)
          .then(({ data }) => {
            this.dataListScan = data.Data;

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

     setFilterListScan: function (v) {
      this.filterListScan.Filters = v;
      this.filterListScan.Page = 1;
    },
    setSortListScan: function (v) {
      this.filterListScan.Sorts = v;
    },
    setPageListScan: function (v) {
      this.filterListScan.Page = v;
      this.loadListScan();
    },
    setLengthListScan: function (v) {
      this.filterListScan.Page = 1;
      this.filterListScan.Length = v;
      this.loadListScan();
    },

    save: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/womin/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isCreating = false));
      });
    },
    remove: function (id, reqNo) {
      this.isRemoving = true;
      return new Promise((resolve, reject) => {
        app.$http
          .delete(
            `/supply-request/womin/remove?requestId=${id}&requestNo=${reqNo}`,
          )
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isRemoving = false));
      });
    },
    print(data) {
      this.isEditing = true;

      return new Promise((resolve, reject) => {
        app.$http
          .post(`/supply-request/womin/print-barcodes`, data, {
            responseType: "blob",
          })
          .then((res) => {
            const blob = new Blob([res.data], { type: "application/pdf" });
            const url = window.URL.createObjectURL(blob);

            const link = document.createElement("a");
            link.href = url;
            link.download = "labels.pdf"; // nama file
            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);

            resolve();
          })
          .catch((err) => {
            if (err.code === "ERR_NETWORK") this.isNetworkError = true;

            if (err.code === "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err.response?.data);
          })
          .finally(() => {
            this.isEditing = false;
          });
      });
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(
    acceptHMRUpdate(useSupplyRequestWomin, import.meta.hot),
  );
}
