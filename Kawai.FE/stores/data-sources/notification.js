var app = useNuxtApp();

export const useNotification = defineStore('Notification', {
  state: () => ({
    isLoading: false,
    isCreating: false,
    isEditing: false,
    isRemoving: false,
    isServerError: false,
    isNetworkError: false,
    isLoadingDetail: false,
    detail: {},
    data: [],
    newNotif: 0
  }),
  actions: {
    loadCountUnread: function(receiver) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/notification/unread-notif-by-receiver?receiver=${receiver}`)
          .then(({data}) => {
            this.data = data.Data;

            resolve(data);
          })
          .catch(err => {
            if(err.code == 'ERR_NETWORK')
              this.isNetworkError = true;
            
            if(err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoading = false);
      })
    },
    load: function(receiver) {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/notification/list-by-receiver?receiver=${receiver}`)
          .then(({data}) => {
            this.data = data.Data;

            resolve(data);
          })
          .catch(err => {
            if(err.code == 'ERR_NETWORK')
              this.isNetworkError = true;
            
            if(err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoading = false);
      })
    },
    updateSeen: function (id) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.patch(`/notification/update-seen?id=${id}`)
          .then(({ data }) => {
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    },
    setNewNotif:function() {
      this.newNotif++;
    }
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useNotification, import.meta.hot));
}