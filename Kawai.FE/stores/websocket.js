var app = useNuxtApp();

import configs from "@/app.config.json";
const baseUrl = () => process.env.NODE_ENV == "production" ? configs.baseWSUrl : configs.baseWSUrlDev;

export const useWebSocket = defineStore('WebSocket', {
  state: () => ({
    data: [],
    socket: null,
    key: null,
    isConnected: false,
    eventHandlers: {},
  }),
  actions: {
    authenticate: function() {
      
    },
    connect: function(onConnected) {
      var _this = this;
      app.$http.get('/auth/ws-key')
        .then(({data}) => {
          _this.key = data.Data;
          _this.socket = new WebSocket(`${baseUrl()}/ws?key=${data.Data}`);
          _this.socket.onopen = function() {
            _this.isConnected = true;
            // _this.socket.send(JSON.stringify({
            //   // Key: 1,
            //   // Command: 'ACTION_LOG_LIST'
            // }))

            if(onConnected) {
              onConnected();
            }
          }
          _this.socket.onmessage = function(msg) {
            var d = JSON.parse(msg.data)
            if(_this.eventHandlers[d?.EventId])
              _this.eventHandlers[d?.EventId](d)
          }
          _this.socket.onclose = function() {
            _this.isConnected = false;
            setTimeout(_this.connect, 2000)
          }
        })
    },
    subscribe: function(eventId, eventHandler) {
      app.$http.post(`/ws/event/subscribe`,{ EventId: eventId, ConnectionId: this.key })
        .then(v => {
          this.eventHandlers[eventId] = eventHandler;
        })
    }
    // reconnect: function() {

    // },
    // loadActionLogList: function() {
    //   this.socket.send(JSON.stringify({
    //     Key: 1,
    //     Command: 'ACTION_LOG_LIST'
    //   }))
    // }
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useWebSocket, import.meta.hot));
}