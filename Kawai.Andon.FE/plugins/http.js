import axios from "axios";

axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if(error.response.status == 401) {
      location.href = `/auth/sign-in`;
    }
    return Promise.reject(error);
  }
)

export default defineNuxtPlugin(nuxtApp => {
  const config = useRuntimeConfig();
  var baseUrl = () => config.public.apiBase;
  const cookieName = config.public.cookieName || '__SIDX';
  axios.defaults.baseURL = baseUrl();

  var route = useRoute();
  // Doing something with nuxtApp
  nuxtApp.provide('http', {
    post: function (...args) {
      axios.defaults.baseURL = baseUrl() + '/api';
      if (getCookie(cookieName)) {
        axios.defaults.headers["Authorization"] = "Bearer " + getCookie(cookieName);
      }

      if (route.params?.id)
        axios.defaults.headers["workspaceid"] = route.params?.id;

      return new Promise((resolve, reject) => {
        axios
          .post(...args)
          .then(resolve)
          .catch((err) => {
            if (err?.response?.status == 401 && !route.path.startsWith("/auth/")) {
              location.href = '/auth/sign-in';
            }
            reject(err);
          });
      });
    },
    patch: function (...args) {
      axios.defaults.baseURL = baseUrl() + '/api';
      if (getCookie(cookieName)) {
        axios.defaults.headers["Authorization"] = "Bearer " + getCookie(cookieName);
      }

      if (route.params?.id)
        axios.defaults.headers["workspaceid"] = route.params?.id;

      return new Promise((resolve, reject) => {
        axios
          .patch(...args)
          .then(resolve)
          .catch((err) => {
            if (err?.response?.status == 401 && !route.path.startsWith("/auth/")) {
              location.href = '/auth/sign-in';
            }
            reject(err);
          });
      });
    },
    delete: function (...args) {
      axios.defaults.baseURL = baseUrl() + '/api';
      if (getCookie(cookieName)) {
        axios.defaults.headers["Authorization"] = "Bearer " + getCookie(cookieName);
      }

      if (route.params?.id)
        axios.defaults.headers["workspaceid"] = route.params?.id;

      return new Promise((resolve, reject) => {
        axios
          .delete(...args)
          .then(resolve)
          .catch((err) => {
            if (err?.response?.status == 401 && !route.path.startsWith("/auth/")) {
              location.href = '/auth/sign-in';
            }
            reject(err);
          });
      });
    },
    get: function (...args) {
      axios.defaults.baseURL = baseUrl() + '/api';
      if (getCookie(cookieName)) {
        axios.defaults.headers["Authorization"] = "Bearer " + getCookie(cookieName);
      }

      if (route.params?.id)
        axios.defaults.headers["workspaceid"] = route.params?.id;

      return new Promise((resolve, reject) => {
        axios
          .get(...args)
          .then(resolve)
          .catch((err) => {
            if (err?.response?.status == 401 && !route.path.startsWith("/auth/")) {
              location.href = '/auth/sign-in';
            }
            reject(err);
          });
      });
    },
    url: function (path) {
      if (path.includes("?")) {
        path = path + '&workspace=' + route.params?.id;
      }
      else {
        path = path + '?workspace=' + route.params?.id;
      }
      return `${baseUrl()}/api${path}`;
    },
    open: function(path, target = '_blank') {
      return new Promise(async (resolve, reject) => {
        await window.open(baseUrl() + path, target)
        resolve();
      })
    },    
  })
})