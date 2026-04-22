var app = useNuxtApp();

export const useClassification= defineStore('Classification', {
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
      Length: 25,
    },
    filter: {
      Page: 1,
      Length: 25,
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
        app.$http.post(`/classification/listtab`, this.filter)
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
    loadtable: function (t) {
            this.isLoading = true;
            this.isNetworkError = this.isServerError = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/classification/listdetail?tablename=${t}`)
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
  
  loadDetail: function ({ id, tableName }) {
  this.isLoadingDetail = true;
  return new Promise((resolve, reject) => {
    app.$http.get(`/classification/detail`, {
      params: {
        id: id,
        tableName: tableName
      }
    })
      .then(({ data }) => {
        this.detail = data.Data;
        resolve(data);
      })
      .catch(err => {
        if (err.code == 'ERR_NETWORK')
          this.isNetworkError = true;

        if (err.code == 'ERR_BAD_RESPONSE')
          this.isServerError = true;

        reject(err);
      })
      .finally(_ => this.isLoadingDetail = false);
  })
},
 create: function (data) {
      this.isCreating = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/classification/create`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isCreating = false);
      })
    },
    update: function (data) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/classification/update`, data)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
remove: function (payload) {
  this.isRemoving = true;
  return new Promise((resolve, reject) => {
    app.$http.delete(
      `/classification/remove?id=${payload.code}&tablename=${payload.tableName}`
    )
      .then(({ data }) => resolve(data))
      .catch(err => reject(err.response?.data))
      .finally(_ => this.isRemoving = false);
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
   
    
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useClassification, import.meta.hot));
}