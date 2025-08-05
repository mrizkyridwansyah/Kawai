var app = useNuxtApp();

export const useSharedLogData = defineStore('SharedLogData', {
  state: () => ({
    isLoading: false,
    isCreating: false,
    isEditing: false,
    isRemoving: false,
    isServerError: false,
    isNetworkError: false,
    isLoadingDetail: false,
    documentType: null,
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
        app.$http.post(`/shared/log/data/list?documentType=${this.documentType}`, this.filter)
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
        app.$http.get(`/shared/log/data/detail?id=${id}&documentType=${this.documentType}`)
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
    setDocumentType: function(v){
      this.documentType = v;
    }
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSharedLogData, import.meta.hot));
}