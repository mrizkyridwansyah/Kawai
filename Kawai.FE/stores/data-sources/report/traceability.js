var app = useNuxtApp();

export const useTraceability = defineStore('Traceability', {
  state: () => ({
    isLoading: false,
    isServerError: false,
    isNetworkError: false,
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
    inquiry: function (isExplosion, itemCode, lotNo) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/traceability/inquiry?isExplosion=${isExplosion}&itemCode=${itemCode}&lotNo=${lotNo}`)
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
  import.meta.hot.accept(acceptHMRUpdate(useTraceability, import.meta.hot));
}