
var app = useNuxtApp();

export const useMenu = defineStore('Menu', {
    state: () => ({
        isLoading: false,
        isServerError: false,
        isNetworkError: false,
        data: {},
    }),
    actions: {
        menu: function () {
            this.isLoading = true;
            this.isLoaded = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/andon/menu`)
                    .then(({ data }) => {
                        this.data = data.Data;
                        this.isLoaded = true;
                        resolve(data);
                    })
                    .catch((err) => reject(err.response?.data))
                    .finally(_ => this.isLoading = false);
            })
        },
    },
});

if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useMenu, import.meta.hot));
}