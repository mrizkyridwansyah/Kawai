var app = useNuxtApp();

export const useSupplyScan = defineStore('SupplyScan', {
  persist: {
    paths: ['filter.Filters', 'newRequest']
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
      Filters: [

      ],
      Sorts: {},
    },

    dataListSupplyScan: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },
    filterListSupplyScan: {
      Page: 1,
      Length: 10,
      Filters: [

      ],
      Sorts: {},
    },

    newRequest: []
  }),
  actions: {
   loadListSupplyScan: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;

      return new Promise((resolve, reject) => {
         debugger;
        app.$http.post(`/production/manual-input/list-stock`, this.filterListSupplyScan)
       
          .then(({ data }) => {
            this.dataListSupplyScan = data.Data;

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

    setFilterListSupplyScan: function (v) {
      this.filterListSupplyScan.Filters = v;
      this.filterListSupplyScan.Page = 1;
    },
    setSortListSupplyScan: function (v) {
      this.filterListSupplyScan.Sorts = v;
    },
    setPageListSupplyScan: function (v) {
      this.filterListSupplyScan.Page = v;
      this.loadListSupplyScan();
    },
    setLengthListSupplyScan: function (v) {
      this.filterListSupplyScan.Page = 1;
      this.filterListSupplyScan.Length = v;
      this.loadListSupplyScan();
    },


  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSupplyScan, import.meta.hot));
}