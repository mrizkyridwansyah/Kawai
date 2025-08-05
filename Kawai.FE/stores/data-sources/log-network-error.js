var app = useNuxtApp();

export const useLogNetworkError = defineStore('LogNetworkError', {
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
    load: function() {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/logging/network-error/list`, this.filter)
          .then(({data}) => {
            this.data = data.Data;

            resolve(data);
          })
          .catch(err => {
            if(err.code == 'ERR_NETWORK')
              this.isNetworkError = true;
            
            if(err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoading = false);
      })
    },
    loadDetail: function(id) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/logging/network-error/detail?id=${id}`)
          .then(({data}) => {
            this.detail = data.Data;

            resolve(data);
          })
          .catch(err => {
            if(err.code == 'ERR_NETWORK')
              this.isNetworkError = true;
            
            if(err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoadingDetail = false);
      })
    },
    setFilter: function(v) {
      this.filter.Filters = v;
      this.filter.Page = 1;
    },
    setSort: function(v) {
      this.filter.Sorts = v;
    },
    setPage: function(v) {
      this.filter.Page = v;
      this.load();
    },
    setLength: function(v) {
      this.filter.Page = 1;
      this.filter.Length = v;
      this.load();
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useLogNetworkError, import.meta.hot));
}