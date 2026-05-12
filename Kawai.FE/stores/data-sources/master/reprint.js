var app = useNuxtApp();

export const useReprint = defineStore('Reprint', {
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
      Length: 10000,
    },
    filter: {
      Page: 1,
      Length: 10000,
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
        app.$http.post(`/reprint/list`, this.filter)
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
      PrintUpdate: function (selectedPrint) {
      return new Promise((resolve, reject) => {
        app.$http.post(`/reprint/printupdate`, selectedPrint)
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
printpdf: function (selectedPrint) {

  this.isLoading = true;

  return app.$http.post(
    `/reprint/printpdf`,
    selectedPrint,
    {
      responseType: 'blob',
    }
  )
    .then((res) => {

      const blob = new Blob(
        [res.data],
        {
          type: 'application/pdf',
        }
      );

      const url = window.URL.createObjectURL(blob);

      let fileName = 'Barcode.pdf';

      const contentDisposition =
        res.headers['content-disposition'];

      if (contentDisposition) {

        // support filename*=UTF-8''
        let match = contentDisposition.match(
          /filename\*=UTF-8''([^;]+)/
        );

        if (match && match[1]) {

          fileName = decodeURIComponent(match[1]);

        } else {

          // fallback filename=
          match = contentDisposition.match(
            /filename="?([^"]+)"?/
          );

          if (match && match[1]) {
            fileName = match[1];
          }
        }
      }

      const link = document.createElement('a');

      link.href = url;
      link.download = fileName;

      document.body.appendChild(link);

      link.click();

      document.body.removeChild(link);

      window.URL.revokeObjectURL(url);

      toastSuccess("Download success");
       
    })
    .catch(async (err) => {

      if (err?.code === 'ERR_NETWORK')
        this.isNetworkError = true;

      if (err?.code === 'ERR_BAD_RESPONSE')
        this.isServerError = true;

      let message = "Download failed";

      try {

        if (err?.response?.data instanceof Blob) {

          const text = await err.response.data.text();

          try {

            const json = JSON.parse(text);

            message =
              json?.Message ||
              json?.message ||
              text;

          } catch {

            message = text;
          }

        } else {

          message =
            err?.response?.data?.Message ||
            err?.message ||
            message;
        }

      } catch (e) {

        console.error(e);
      }

      toastDanger(message);

      console.error(err);

      throw err;
    })
    .finally(() => {

      this.isLoading = false;
    });
},
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useReprint, import.meta.hot));
}