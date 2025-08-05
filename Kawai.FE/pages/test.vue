<template>
  <div>
    Connection: {{ list.length }}
    Closed: {{ closed }}

    <button @click="dod">Connect</button>
  </div>
</template>

<script>
const url = 'http://localhost:5000/api/auth/v1/ws-key';
const wsUrl = 'https://localhost:7115/api/auth/v1/ws-key';
import axios from 'axios'
export default {
  data: () => ({
    list: [],
    closed: 0,
  }),
  mounted: function() {
    this.connect(0);
  },
  methods: {
    dod: function() {
      var idx = 0;
      setInterval(() => {
        this.connect(idx++)
      }, 500)
    },
    connect: async function(i) {

      var _this = this;

      var c = await axios.get(url)
      // console.log(c)
      let socket = new WebSocket(`ws://localhost:5000/ws?key=${c.data}`);

      socket.onopen = function(e) {
        // console.log("[open] Connection established");
        // console.log("Sending to server");
        // socket.send("My name is John");
        // setInterval(() => {
        //   socket.send(JSON.stringify({
        //       "Command": "TEST3",
        //       "Payload": {
        //           "Title": "test 1",
        //           "Description": "Description 1"
        //       }
        //   }))
        // }, 1000)
        _this.list.push(socket)
      };

      socket.onmessage = function(event) {
        // console.log(`[message] Data received from server: ${event.data}`);
      };

      socket.onclose = function(event) {
        _this.closed--;
      };

      socket.onerror = function(error) {
        _this.closed--;
      };
    }
  }
}
</script>