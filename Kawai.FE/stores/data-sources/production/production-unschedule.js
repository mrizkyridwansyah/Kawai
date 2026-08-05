var app = useNuxtApp();

export const useProductionUnschedule = defineStore('ProductionUnschedule', {
  persist: {
    paths: ['filter.Filters','filter.FiltersBack','filter.FilterBarcode', 'toResult']
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
    // dataDetails: {
    //   Items: [],
    //   Total: 0,
    //   Filtered: 0,
    //   Page: 1,
    //   Length: 10,
    // },
    filter: {
      Page: 1,
      Length: 10,
      Filters: [

      ],
      FiltersBack:[],
      FilterBarcode:[],
      Sorts: {},
    },
    toResult: []
  }),
  actions: {
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/production-unschedule/list-bom`, this.filter)
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
   loadDetail() {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.post("/production-unschedule/list-result", this.filter)
          .then(({ data }) => {
           
            resolve(data);

          })
          .catch(err => {
            if (err.code == "ERR_NETWORK")
              this.isNetworkError = true;

            if (err.code == "ERR_BAD_RESPONSE")
              this.isServerError = true;

            reject(err);
          })
          .finally(() => this.isLoadingDetail = false);
          

      });
    },
    loadBarcode(id) {
      debugger
      return new Promise((resolve, reject) => {
        app.$http.get(`/production-unschedule/list-detail?id=${id}`)
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
      });
    },
    setRequest: function (req) {
      this.toResult = req;
    },
    setFilter: function (v,x) {
      this.filter.Filters = v;
      this.filter.Page = 1;
      this.filter.FiltersBack = x;
    },
    setFilterDetail: function (v,x) {
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
    save: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/production-unschedule/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isCreating = false);
      })
    },
   
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useProductionUnschedule, import.meta.hot));
}