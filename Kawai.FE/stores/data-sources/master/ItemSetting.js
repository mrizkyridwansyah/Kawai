
var app = useNuxtApp();

export const useItemSetting = defineStore('ItemSetting', {
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
      Length: 25,
    },
    filter: {
      Page: 1,
      Length: 25,
      Filters: [

      ],
      Sorts: {},
    },
  }),
  actions: {
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/itemsetting/list`, this.filter)
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
    submititemsetting: function (data) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/itemsetting/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })

    },
    setSort: function (v) {
      this.filter.Sorts = v;
    },
    setFilter: function (v) {
      this.filter.Filters = v;
      this.filter.Page = 1;
    },
    resetList: function () {
      this.data.Items = [];
      this.data.Total = 0;
      this.data.Filtered = 0;
      this.data.Page = 1;
      this.data.Length = 25;
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useItemSetting, import.meta.hot));
}