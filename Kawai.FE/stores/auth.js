
var app = useNuxtApp();

export const useAuth = defineStore('Auth', {
  state: () => ({
    isLoading: false,
    isSigningIn: false,
    isSigningUp: false,
    isServerError: false,
    isNetworkError: false,
    data: {},
  }),
  actions: {
    load: function () {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/user/user-info`)
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
    signIn: function (model) {
      this.isSigningIn = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/auth/sign-in`, model)
          .then(({ data }) => {
            this.data = data.Data;
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => {
            this.isSigningIn = false
          });
      })
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useAuth, import.meta.hot));
}