var app = useNuxtApp();

export const useShippingInstruction = defineStore("ShippingInstruction", {
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
      Length: 10,
      Filters: [],
      Sorts: {},
    },
  }),
  actions: {
    loadShippingByPO: function (poNumber) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http
          .get(`/shipping-instruction/data-shipping-by-po?poNumber=${poNumber}`)
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
    loadShipping: function (shippingNo) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http
          .get(`/shipping-instruction/data-shipping?shippingNo=${shippingNo}`)
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
    create: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http
          .post(`/shipping-instruction/create`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally((_) => (this.isCreating = false));
      });
    },
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http
          .patch(`/shipping-instruction/update`, data)
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
    acceptHMRUpdate(useShippingInstruction, import.meta.hot),
  );
}
