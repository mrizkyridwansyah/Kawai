<template>
  <div>
    <h5>Error Log</h5>
    <div>
      <div>Date: {{ $func.formatDateTime(data.Date, 'DD MMM YYYY HH:mm:ss') }}</div>
      <div>Username: {{ data.UserName }}</div>
      <div>User Agent: {{ data.UserAgent }}</div>
      <div>Remote Addr.: {{ data.RemoteAddr }}</div>
      <div class="d-flex mt-3">
        <div>
          Method: 
          <span v-if="data.Method == 'GET'" class="fw-bold text-success">{{ data.Method }}</span>
          <span v-if="data.Method == 'POST'" class="fw-bold text-success">{{ data.Method }}</span>
          <span v-if="data.Method == 'PATCH'" class="fw-bold text-primary">{{ data.Method }}</span>
          <span v-if="data.Method == 'DELETE'" class="fw-bold text-danger">{{ data.Method }}</span>
        </div>
        <div class="ms-5">Request Path: <span class="fw-bold text-primary">{{ data.RequestPath }}</span></div>
        <!-- <div class="ms-5">Reference Id: <span class="fw-bold text-primary">{{ data.ReferenceId }}</span></div> -->
      </div>
    </div>
    <div class="mt-5 row">
      <div class="col-sm-12">
        <div class="card">
          <div class="card-body">
            <h6>Stack Trace</h6>
            <span class="fw-bold" style="font-size: 14px;">{{ data.Message }}</span>
            <div>
              <li v-for="x in data.StackTrace?.split('\n')">
                {{ x }}
              </li>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  data: () => ({
    data: {},
    compared: {},
  }),
  computed: {
    ds: function() {
      return useLogError();
    }
  },
  mounted: function() {
    this.ds.loadDetail(this.$route.query.id)
      .then(data => {
        this.data = data.Data;
      })
  },
  methods: {
    load: function() {

    }
  },
}
</script>