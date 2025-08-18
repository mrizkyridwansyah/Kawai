<template>
  <v-modal 
    id="shared-log-network-error-detail" 
    title="Change Log"
    size="600"
  >
    <div>
      <div>
        <div> 
          <v-icon name="calendar" width="12" color="#555" />
          Date: {{ $func.formatDateTime(data.Date, 'DD MMM YYYY HH:mm:ss') }}</div>
        <div> 
          <v-icon name="user" width="12" color="#555" />
          Username: {{ data.UserName }}</div>
        <div> 
          <v-icon name="chrome" width="12" color="#555" />
          User Agent: {{ data.UserAgent }}</div>
        <div> 
          <v-icon name="globe" width="12" color="#555" />
          Remote Addr.: {{ data.RemoteAddr }}</div>
        <div class="d-flex mt-3">
          <div>
            Method: 
            <span v-if="data.Method == 'POST'" class="fw-bold text-success">{{ data.Method }}</span>
            <span v-if="data.Method == 'PATH'" class="fw-bold text-primary">{{ data.Method }}</span>
            <span v-if="data.Method == 'DELETE'" class="fw-bold text-danger">{{ data.Method }}</span>
          </div>
          <div class="ms-5">Content Type: <span class="fw-bold text-primary">{{ data.ContentType }}</span></div>
          <div class="ms-5">Url: <span class="fw-bold text-primary">{{ data.Url }}</span></div>
        </div>
      </div>
      <div class="mt-5 row">
        <div class="col-sm-12">
          <div class="card">
            <div class="card-body">
              <h6>Request Body</h6>
              <pre>{{ JSON.parse(data.RequestBody) }}</pre>
            </div>
          </div>
        </div>
        <div class="col-sm-12 mt-5">
          <div class="card">
            <div class="card-body">
              <h6>Response Body</h6>
              <pre>{{ JSON.parse(data.ResponseBody) }}</pre>
            </div>
          </div>
        </div>
      </div>
    </div>
  </v-modal>
</template>

<script>
export default {
  props:['data'],
  data: () => ({
    model: {
      Code: null,
      Name: null,
      Description: null,
    },
    errorResponse: {},
    errors:{},
  }),
  computed: {

  },
  methods: {
    submit: function(e) {
      
    },
    shown: function() {
      // this.errors = this.errorResponse = {};;
    },
    hidden: function() {
      // this.errors = this.errorResponse = {};;
    },
  }
}
</script>