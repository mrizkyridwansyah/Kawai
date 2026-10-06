var app = useNuxtApp();

export const useInventoryClosing = defineStore('InventoryClosing', {
    state: () => ({
        isLoading: false,
        isServerError: false,
        isNetworkError: false,
        data: null,
    }),
    actions: {
        load: function () {
            this.isLoading = true;
            this.isNetworkError = this.isServerError = false;

            return new Promise((resolve, reject) => {
                app.$http
                    .post(`/inventoryclosing/getcurrent`)
                    .then(({ data }) => {
                        this.data = data.Data;

                        resolve(data);
                    })
                    .catch((err) => {
                        if (err.code == "ERR_NETWORK") this.isNetworkError = true;

                        if (err.code == "ERR_BAD_RESPONSE") this.isServerError = true;

                        reject(err);
                    })
                    .finally((_) => (this.isLoading = false));
            });

        },
        getPeriod: function () {
            return this.data?.Period ?? null;
        },
        processClosing: function () {
            const [year, month] = this.data.Period.split("-");
            this.isLoading = true;
            this.isNetworkError = this.isServerError = false;
            let payload = {
                IvtYear: Number(year),
                IvtMonth: Number(month),
            };

            return new Promise((resolve, reject) => {
                app.$http
                    .post("/inventoryclosing/process", payload)
                    .then(({ data }) => {
                        resolve(data);
                    })
                    .catch((err) => {
                        if (err.code === "ERR_NETWORK") this.isNetworkError = true;

                        if (err.code === "ERR_BAD_RESPONSE") this.isServerError = true;

                        reject(err);
                    })
                    .finally(() => {
                        this.isLoading = false;
                    });
            });
        },
    },
});

if (import.meta.hot) {
    import.meta.hot.accept(
        acceptHMRUpdate(useInventoryClosing, import.meta.hot)
    );
}