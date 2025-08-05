var app = useNuxtApp();

export const useStock = defineStore('Stock', {
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
      this.inquiry();
    },
    setLength: function (v) {
      this.filter.Page = 1;
      this.filter.Length = v;
      this.inquiry();
    },
    setPageDetail: function (v) {
      this.filter.Page = v;
      this.inquiryDetail();
    },
    setLengthDetail: function (v) {
      this.filter.Page = 1;
      this.filter.Length = v;
      this.inquiryDetail();
    },
    inquiry: function () {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/stock/inquiry/location`, this.filter)
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
    inquiryDetail: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/stock/inquiry-detail`, this.filter)
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
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useStock, import.meta.hot));
}