import configs from "@/app.config.json";

var baseUrl = () => process.env.NODE_ENV == "production" ? configs.baseWSUrl : configs.baseWSUrlDev;

export default defineNuxtPlugin(nuxtApp => {
  
  // var key = '';

  // nuxtApp.$http.get(`/auth/v1/ws-key`)
  //   .then(({data}) => {
  //     var ws = new WebSocket(`${baseUrl()}/ws?key=${data}`);
  //     ws.onopen = function() {
  //       console.log('ws connected');
  //     }
  //     ws.onmessage = function(msg) {
  //       console.log(JSON.parse(msg.data));
  //     }
  //   })


  // var taskProcess = useTaskProcess();

  // var ws = new WebSocket(`${baseUrl}/ws`);

  // const handlers = {
  //   TASK: function(message) {
  
  //   },
  //   SYSTEM_TASK: function(message) {
  
  //   }
  // }

  // ws.onopen = function() {
  //   alert('connected')
  // }

  // ws.onmessage = function() {

  // }

  // ws.onclose = function() {

  // }

  // ws.onerror = function() {

  // }
})
