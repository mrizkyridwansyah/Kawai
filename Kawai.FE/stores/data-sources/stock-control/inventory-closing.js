var app = useNuxtApp();

export const useInventoryClosing = defineStore('InventoryClosing', {
    state: () => ({
        isLoading: false,
        isLoaded: false,
        data: null,
    }),
    actions: {
      load() {
    this.isLoading = true;
    this.isLoaded = false;

    return app.$http.post(`/inventoryclosing/getcurrent`)
        .then(({ data }) => {
            this.data = data?.Data || null;
            this.isLoaded = true;
        })
        .finally(() => this.isLoading = false);
},

        getPeriod() {
            return this.data?.Period ?? null;
        },
  async processClosing() {
  const [year, month] = this.data.Period.split("-");

  try {
    await app.$http.post(`/inventoryclosing/process`, {
      IvtYear: Number(year),
      IvtMonth: Number(month),
    });
  } catch (err) {
    // lempar ke Vue
    throw err;
  }
},
    },
    persist: true
});

if (import.meta.hot) {
    import.meta.hot.accept(
        acceptHMRUpdate(useInventoryClosing, import.meta.hot)
    );
}