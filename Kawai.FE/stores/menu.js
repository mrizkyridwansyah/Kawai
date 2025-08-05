
var app = useNuxtApp();

export const useMenu = defineStore('Menu', {
    state: () => ({
        isLoading: false,
        isServerError: false,
        isNetworkError: false,
        data: {},
    }),
    actions: {
        loadprivileges: function (userId) {
            this.isLoading = true;
            this.isNetworkError = this.isServerError = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/menu/listprivileges?userID=${userId}`)
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
        privileges: function () {
            this.isLoading = true;
            this.isLoaded = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/menu/privileges`)
                    .then(({ data }) => {
                        this.data = data.Data;
                        this.isLoaded = true;
                        resolve(data);
                    })
                    .catch((err) => reject(err.response?.data))
                    .finally(_ => this.isLoading = false);
            })
        },
        submitPrivilege: function (data) {
            this.isLoading = true;
            return new Promise((resolve, reject) => {
                app.$http.post(`/menu/privileges/save`, data)
                    .then(({ data }) => {
                        resolve(data);
                    })
                    .catch((err) => reject(err.response?.data))
                    .finally(_ => this.isLoading = false);
            })

        }
    },
});

if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useMenu, import.meta.hot));
}