
var app = useNuxtApp();

export const useProductionControl = defineStore('ProductionControl', {
  state: () => ({
    isLoading: false,
    isSigningIn: false,
    isSigningUp: false,
    isServerError: false,
    isNetworkError: false,
    headerData: [],
    scheduleData: [],
    trolleyData: [],
    data: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },
  }),
  actions: {
    loadheaderinfo: function ( line= null,model = null,schedule = null) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/andon/production-control/headerinfo?line=${line || ""}&model=${model || ""}&scheduledate=${schedule || ""}`)
          .then(({ data }) => {
            this.headerData = data.Data;
             //console.log(data);  
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
   
   loadlistschedule: function ( line= null,model = null,schedule = null) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/andon/production-control/listschedule?line=${line || ""}&model=${model || ""}&scheduledate=${schedule || ""}`)
          .then(({ data }) => {
           this.scheduleData = data.Data;
            //console.log(data);  
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

     loadlisttrolley: function ( line= null,model = null,schedule = null) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/andon/production-control/listtrolley?line=${line || ""}&model=${model || ""}&scheduledate=${schedule || ""}`)
          .then(({ data }) => {
            this.trolleyData = data.Data;
            //console.log(data);  
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
    },
    // setSort: function (v) {
    //   this.filter.Sorts = v;
    // },
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useProductionControl, import.meta.hot));
}