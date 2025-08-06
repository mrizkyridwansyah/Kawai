<template>
  <div>
    <h5>Data Change Log</h5>
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
          Action: 
          <span v-if="data.Action == 'Create'" class="fw-bold text-success">{{ data.Action }}</span>
          <span v-if="data.Action == 'Update'" class="fw-bold text-primary">{{ data.Action }}</span>
          <span v-if="data.Action == 'Delete'" class="fw-bold text-danger">{{ data.Action }}</span>
        </div>
        <div class="ms-5">Document Type: <span class="fw-bold text-primary">{{ data.DocumentType }}</span></div>
        <div class="ms-5">Reference Id: <span class="fw-bold text-primary">{{ data.ReferenceId }}</span></div>
      </div>
    </div>
    <div class="mt-5 row">
      <div class="col-sm-12">
        <div class="card">
          <div class="card-body">
            <h6>Before</h6>
            <div v-for="(item, i) in Object.keys(compared.Before || {})">
              <div v-if="Array.isArray(compared.Before[item])">

              </div>
              <div v-else>
                {{item}} : {{ compared.Before[item] }}
              </div>
            </div>
            <div v-if="JSON.stringify(compared.Before) == '{}' && data.Action == 'Create'">
              <div class="alert alert-primary" role="alert">
                Data not available!
              </div>
            </div>
            <div v-if="JSON.stringify(compared.Before) == '{}' && data.Action == 'Update'">
              <div class="alert alert-primary" role="alert">
                No data changes!
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-sm-12 mt-5">
        <div class="card">
          <div class="card-body">
            <h6>After</h6>
            <div v-for="(item, i) in Object.keys(compared.After || {})">
              <div v-if="Array.isArray(compared.After[item])">
                {{ item }} :
                <table class="x-table">
                  <thead v-if="compared.After[item].length > 0">
                    <tr>
                      <th v-for="xx in Object.keys(compared.After[item][0] || {})">
                        {{xx}}
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(x, i) in compared.After[item]">
                      <td v-for="xx in Object.keys(x || {})">
                        {{ x[xx] }}
                      </td>
                    </tr>
                  </tbody>
                </table>
                <!-- <div v-for="(x) in compared.After[item]">
                  <div v-for="(xx, i) in Object.keys(x || {})">
                    {{xx}} : {{ x[xx] }}
                  </div>
                </div> -->
              </div>
              <div v-else>
                {{item}} : {{ compared.After[item] }}
              </div>
            </div>
            <div v-if="JSON.stringify(compared.After) == '{}' && data.Action == 'Delete'">
              <div class="alert alert-danger" role="alert">
                Data Deleted!
              </div>
            </div>
            <div v-if="JSON.stringify(compared.After) == '{}' && data.Action == 'Update'">
              <div class="alert alert-primary" role="alert">
                No data changes!
              </div>
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
      return useLogData();
    }
  },
  mounted: function() {
    this.ds.loadDetail(this.$route.query.id)
      .then(data => {
        this.data = data.Data;
        this.compared = JSON.parse(data.Data.Data)
      })
  },
  methods: {
    load: function() {

    }
  },
}
</script>