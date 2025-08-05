var app = useNuxtApp();

export const useMktOrder = defineStore('MktOrder', {
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
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.post(`/marketing/order/list`, this.filter)
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
    loadDetail: function (id) {
      this.isLoadingDetail = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/marketing/order/detail?id=${id}`)
          .then(({ data }) => {
            this.detail = data.Data;
            console.log(data.Data);
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
    loadDetailHierarchy: function (id) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/marketing/order/detail-hierarchy?id=${id}`)
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
    create: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/marketing/order/create`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isCreating = false);
      })
    },
    edit: function (id, data) {
      this.isEditing = true;
      console.log(data);
      return new Promise((resolve, reject) => {
        app.$http.patch(`/marketing/order/edit?id=${id}`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    remove: function (id) {
      this.isRemoving = true;
      return new Promise((resolve, reject) => {
        app.$http.delete(`/marketing/order/remove?id=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isRemoving = false);

      })
    },
    printSP: function (id) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/marketing/print-template/check-valid?id=${id}&docType=MKT_ORDER`).then((result) => {
          if (result.data.Status == "SUCCESS") {
            if (result.data.Data) {
              app.$http.open(`/api/marketing/order/print-sp?id=${id}`)
                .then(resolve)
                .catch((err) => reject(err.response?.data))
                .finally(_ => this.isEditing = false);
            }
            else {
              resolve("Request Approval Reprint Berhasil");
            }
          }
        })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err.response?.data);
          })
          .finally(_ => this.isEditing = false);
      })
    },
    printBAST: function (id) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/marketing/print-template/check-valid?id=${id}&docType=MKT_HANDOVER`).then((result) => {
          if (result.data.Status == "SUCCESS" && result.data.Data) {
            app.$http.open(`/api/marketing/order/print-bast?id=${id}`)
              .then(resolve)
              .catch((err) => reject(err.response?.data))
              .finally(_ => this.isEditing = false);
          }
        })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err.response?.data);
          })
          .finally(_ => this.isEditing = false);
      })
    },
    printIncentiveCashback: function (id) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/marketing/print-template/check-valid?id=${id}&docType=MKT_CASH_BACK_INCENTIVE`).then((result) => {
          if (result.data.Status == "SUCCESS" && result.data.Data) {
            app.$http.open(`/api/marketing/order/print-incentive-cashback?id=${id}`)
              .then(resolve)
              .catch((err) => reject(err.response?.data))
              .finally(_ => this.isEditing = false);
          }
        })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err.response?.data);
          })
          .finally(_ => this.isEditing = false);
      })
    },
    printBillingSchedule: function (id) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.get(`/marketing/print-template/check-valid?id=${id}&docType=MKT_PAYMENT_SCHEDULE`).then((result) => {
          if (result.data.Status == "SUCCESS" && result.data.Data) {
            app.$http.open(`/api/marketing/order/print-payment-schedule?id=${id}`)
              .then(resolve)
              .catch((err) => reject(err.response?.data))
              .finally(_ => this.isEditing = false);
          }
        })
          .catch(err => {
            if (err.code == 'ERR_NETWORK')
              this.isNetworkError = true;

            if (err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err.response?.data);
          })
          .finally(_ => this.isEditing = false);
      })
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useMktOrder, import.meta.hot));
}