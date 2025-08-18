import axios from 'axios'


export default defineNuxtPlugin(nuxtApp => {
  
  axios.interceptors.response.use(
    response => response,
    error => {
      if(error.Code == 'ERR_NETWORK') {
        Promise.reject(error);
        return;
      }
      if(error.request.status == 401) {
        Promise.reject(error);
        return;
      }
      if(error.request.status == 404) {
        Promise.reject(error);
        return;
      }
      // Do something with response error
      var payload = {
        Url: error.request.responseURL,
        Method: error.config.method,
        ContentType: error.config.headers['Content-Type'],
        RequestBody: error.config.data,
        ResponseBody: error.request.responseText
      }
      nuxtApp.$http.post(`/logging/network-error/create`, payload)
      // console.log('hoho', nuxtApp)
      return Promise.reject(error);
    }
  );
})