var app = useNuxtApp();

export const useProductionNGClaim = defineStore('ProductionNGClaim', {
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
    loadDetail: function (claimid) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/material-productionng/data-header?claimid=${claimid}`)
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
        app.$http.post(`/material-productionng/list-detail?claimid=${claimid}`)
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
    listNGDetail: function (filters) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/material-productionng/list-ng-detail`, filters)
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
        app.$http.post(`/material-productionng/create`, data)
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
        app.$http.patch(`/material-productionng/update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    submit: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/material-productionng/submit`, data)
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
        app.$http.delete(`/material-productionng/remove?claimid=${claimid}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isRemoving = false);

      })
    },
 
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useProductionNGClaim, import.meta.hot));
}