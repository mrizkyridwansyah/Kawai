<template>
  <v-modal 
    id="shared-log-detail" 
    title="Change Log"
    size="600"
  >
    <div>
      <div>
        <table>
          <tbody>
            <tr>
              <td style="width: 20%;vertical-align: top;">
                <v-icon name="calendar" width="12" color="#555" /> 
                Date
              </td>
              <td style="width: 1%;vertical-align: top;">:</td>
              <td>
                {{ $func.formatDateTime(data.Date, 'DD MMM YYYY HH:mm:ss') }}
              </td>
            </tr>
            <tr>
              <td style="width: 20%;vertical-align: top;">
                <v-icon name="user" width="12" color="#555" />
                Username
              </td>
              <td style="width: 1%;vertical-align: top;">:</td>
              <td>
                {{ data.UserName }}
              </td>
            </tr>
            <tr>
              <td style="width: 20%;vertical-align: top;">
                <v-icon name="chrome" width="12" color="#555" />
                User Agent
              </td>
              <td style="width: 1%;vertical-align: top;">:</td>
              <td>
                <strong>{{ getBrowserName(data.UserAgent) }}</strong>
                <div>{{ data.UserAgent }}</div>
              </td>
            </tr>
            <tr>
              <td style="width: 20%;vertical-align: top;">
                <v-icon name="globe" width="12" color="#555" />
                Remote Addr.
              </td>
              <td style="width: 1%;vertical-align: top;">:</td>
              <td>
                {{ data.RemoteAddr }}
              </td>
            </tr>
          </tbody>
        </table>
        <!-- <div> 
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
          Remote Addr.: {{ data.RemoteAddr }}</div> -->
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
            <div class="card-body" style="overflow-x: auto">
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
            <div class="card-body" style="overflow-x: auto">
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
    compared: function() {
      return JSON.parse(this.data.Data);
    }
  },
  methods: {
    submit: function(e) {
      
    },
    getBrowserName: function(userAgent) {
      if (/chrome|crios|crmo/i.test(userAgent) && !/edg/i.test(userAgent)) {
        return 'Chrome';
      } else if (/firefox|fxios/i.test(userAgent)) {
        return 'Firefox';
      } else if (/safari/i.test(userAgent) && !/chrome|crios|crmo/i.test(userAgent)) {
        return 'Safari';
      } else if (/edg/i.test(userAgent)) {
        return 'Edge';
      } else if (/opr|opera/i.test(userAgent)) {
        return 'Opera';
      } else if (/msie|trident/i.test(userAgent)) {
        return 'Internet Explorer';
      } else {
        return 'Unknown';
      }
    },

    shown: function() {
      // this.errors = this.errorResponse = {};
    },
    hidden: function() {
      // this.errors = this.errorResponse = {};
    },
  }
}
</script>