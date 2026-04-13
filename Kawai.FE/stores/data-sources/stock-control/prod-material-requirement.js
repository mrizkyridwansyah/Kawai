var app = useNuxtApp();

export const useProdMaterialRequirement = defineStore('ProdMaterialRequirement', {
  state: () => ({
    isLoading: false,
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
    getCalculation: function (factory) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/prod-material-requirement/last-calculation?factory=${factory}`)
          .then(({ data }) => {
            this.data.Items = data.Data.Materials;
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
    save: function (data) {
      this.isLoading = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/prod-material-requirement/save`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isLoading = false);
      })
    },
    exportExcel: function (factory) {
      return new Promise((resolve, reject) => {
        app.$http.get(`/prod-material-requirement/export/excel-inquiry?factory=${factory}`)
          .then(({ data }) => {
            if (data.Data) {
              const byteCharacters = atob(data.Data); // decode base64
              const byteNumbers = new Array(byteCharacters.length).fill(0).map((_, i) => byteCharacters.charCodeAt(i));
              const byteArray = new Uint8Array(byteNumbers);

              const blob = new Blob([byteArray], {
                type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
              });

              const url = URL.createObjectURL(blob);
              const link = document.createElement('a');
              link.href = url;
              link.setAttribute('download', 'Prod_Material_Requirement.xlsx');
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
      })
    },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useProdMaterialRequirement, import.meta.hot));
}