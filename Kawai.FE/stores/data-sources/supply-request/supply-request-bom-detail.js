var app = useNuxtApp();

export const useSupplyRequestBomDetail = defineStore("SupplyRequestBomDetail", {
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
      Filters: [],
      Sorts: {},
    },
  }),
  actions: {
    loadDetail: function (id) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http
          .get(`/supply-request/bom/getdetail?id=${id}`)
          .then(({ data }) => {
            this.detail = data.Data;

            resolve(data);
          })
          .catch((err) => {
            if (err.code == "ERR_NETWORK") this.isNetworkError = true;

            if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err);
          })
          .finally((_) => (this.isLoadingDetail = false));
      });
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
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http
          .patch(`/supply-request/bom/updatereqqty`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isEditing = false));
      });
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(
    acceptHMRUpdate(useSupplyRequestBomDetail, import.meta.hot),
  );
}
