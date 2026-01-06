var app = useNuxtApp();

export const usePeriodSetting = defineStore('PeriodSetting', {
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
      Length: 10,
    },
    dataListDetail: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
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
    listPeriodSettingDetail: function (filters) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/periodsetting/listdetail`, filters)
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
    
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/periodsetting/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(usePeriodSetting, import.meta.hot));
}