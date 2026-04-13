
var app = useNuxtApp();

export const useBOMWorkstationDetail = defineStore('BOMSetting', {
    state: () => ({
        isLoading: false,
        isServerError: false,
        isNetworkError: false,
        data: {},
    }),
    actions: {
        load: function (l,i,w) {
            this.isLoading = true;
            this.isNetworkError = this.isServerError = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/bomworkstation/listdetail?linecode=${l}&parentitem_code=${i}&workstationcode=${w}`)
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

        loadQty: function (t) {
            this.isLoading = true;
            this.isNetworkError = this.isServerError = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/bomworkstation/detailqty?id=${t}`)
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
        submit: function (data) {
            this.isLoading = true;
            return new Promise((resolve, reject) => {
                app.$http.post(`/bomworkstation/save`, data)
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
    import.meta.hot.accept(acceptHMRUpdate(useBOMWorkstationDetail, import.meta.hot));
}