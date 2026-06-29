var app = useNuxtApp();

export const usePhysicalInventory = defineStore("PhysicalInventory", {
  state: () => ({
    isLoading: false,
    isEditing: false,
    isServerError: false,
    isNetworkError: false,
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
    load: function () {
      //console.log(this.filter);

      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http
          .post(`/physical-inventory/list`, this.filter)
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
    update(payload) {
      this.isEditing = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http
          .patch("/physical-inventory/update", payload)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => {
            if (err.code === "ERR_NETWORK") this.isNetworkError = true;

            if (err.code === "ERR_BAD_RESPONSE") this.isServerError = true;

            reject(err);
          })
          .finally(() => {
            this.isEditing = false;
          });
      });
    },
    exportExcel: function (filters) {
      return new Promise((resolve, reject) => {
        let filterExport = {
          Page: 1,
          Length: 1000000,
          Filters: filters,
          Sorts: {},
        };

        app.$http
          .post(`/physical-inventory/export/excel`, filterExport)
          .then(({ data }) => {
            if (data.Data) {
              const byteCharacters = atob(data.Data); // decode base64
              const byteNumbers = new Array(byteCharacters.length)
                .fill(0)
                .map((_, i) => byteCharacters.charCodeAt(i));
              const byteArray = new Uint8Array(byteNumbers);

              const blob = new Blob([byteArray], {
                type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
              });

              const url = URL.createObjectURL(blob);
              const link = document.createElement("a");
              link.href = url;
              link.setAttribute("download", "List_Physical_Inventory.xlsx");
              document.body.appendChild(link);
              link.click();
              document.body.removeChild(link);
              URL.revokeObjectURL(url);

              resolve();
            } else {
              reject(data);
            }
          })
          .catch(async (err) => {
            reject(err?.response?.data);
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
    acceptHMRUpdate(usePhysicalInventory, import.meta.hot),
  );
}
