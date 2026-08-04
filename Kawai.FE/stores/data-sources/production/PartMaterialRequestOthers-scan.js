var app = useNuxtApp();

export const usePartMaterialRequestOthersScan = defineStore("PartMaterialRequestOthersScan", {
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
 
     loadListScan: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;

      return new Promise((resolve, reject) => {
        app.$http
          .post(`/partmaterialrequestothers/list-scan`, this.filterListScan)
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
 
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(
    acceptHMRUpdate(usePartMaterialRequestOthersScan, import.meta.hot),
  );
}

 