
var app = useNuxtApp();

export const useReceiptAndon = defineStore('ReceiptAndon', {
  state: () => ({
    isLoading: false,
    isSigningIn: false,
    isSigningUp: false,
    isServerError: false,
    isNetworkError: false,
    data: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },
  }),
  actions: {
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/andon/receipt/list`)
          .then(v => {
            this.data = v?.Data;
            resolve(v);
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
  import.meta.hot.accept(acceptHMRUpdate(useReceiptAndon, import.meta.hot));
}