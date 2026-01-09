var app = useNuxtApp();

export const useImportLog = defineStore('ImportLog', {
  state: () => ({
    isLoading: false,
    isServerError: false,
    isNetworkError: false,
    template: null,
    data: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 5,
    },
    filter: {
      Page: 1,
      Length: 5,
      Filters: [],
      Sorts: {},
    },
  }),
  actions: {
    load: function() {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/import/histories?templateName=${this.template}`, this.filter)
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
    setTemplate: function(v) {
      this.template = v;
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
  import.meta.hot.accept(acceptHMRUpdate(useImportLog, import.meta.hot));
}