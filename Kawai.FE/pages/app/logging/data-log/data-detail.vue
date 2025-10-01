<template>
  <div>
    <h5>Data Change Log</h5>
    <div>
      <div>
        <v-icon name="calendar" width="12" color="#555" />
        Date: {{ $func.formatDateTime(data.Date, "DD MMM YYYY HH:mm:ss") }}
      </div>
      <div>
        <v-icon name="user" width="12" color="#555" />
        Username: {{ data.UserName }}
      </div>
      <div>
        <v-icon name="chrome" width="12" color="#555" />
        User Agent: {{ data.UserAgent }}
      </div>
      <div>
        <v-icon name="globe" width="12" color="#555" />
        Remote Addr.: {{ data.RemoteAddr }}
      </div>
      <div class="d-flex mt-3">
        <div>
          Action:
          <span v-if="data.Action == 'Create'" class="fw-bold text-success">{{
            data.Action
          }}</span>
          <span v-if="data.Action == 'Update'" class="fw-bold text-primary">{{
            data.Action
          }}</span>
          <span v-if="data.Action == 'Delete'" class="fw-bold text-danger">{{
            data.Action
          }}</span>
        </div>
        <div class="ms-5">
          Document Type:
          <span class="fw-bold text-primary">{{ data.DocumentType }}</span>
        </div>
        <div class="ms-5">
          Reference Id:
          <span class="fw-bold text-primary">{{ data.ReferenceId }}</span>
        </div>
      </div>
    </div>
    <div class="mt-5 row">
      <div class="col-sm-12">
        <div class="card">
          <div class="card-body">
            <h5 class="mb-4">Before</h5>
            <div
              v-if="
                JSON.stringify(compared.Before) == '{}' &&
                data.Action == 'Create'
              "
            >
              <div class="alert alert-primary" role="alert">
                Data not available!
              </div>
            </div>
            <div
              v-if="
                JSON.stringify(compared.Before) == '{}' &&
                data.Action == 'Update'
              "
            >
              <div class="alert alert-primary" role="alert">
                No data changes!
              </div>
            </div>
            <shared-log-data-viewer
              :data="compared.Before"
              :compare-data="compared.After"
              mode="before"
            />
          </div>
        </div>
      </div>
      <div class="col-sm-12 mt-5">
        <div class="card">
          <div class="card-body">
            <h5 class="mb-4">After</h5>
            <div
              v-if="
                JSON.stringify(compared.After) == '{}' &&
                data.Action == 'Delete'
              "
            >
              <div class="alert alert-primary" role="alert">
                Data not available!
              </div>
            </div>
            <div
              v-if="
                JSON.stringify(compared.After) == '{}' &&
                data.Action == 'Update'
              "
            >
              <div class="alert alert-primary" role="alert">
                No data changes!
              </div>
            </div>
            <shared-log-data-viewer
              :data="compared.After"
              :compare-data="compared.Before"
              mode="after"
            />
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
    ds: function () {
      return useLogData();
    },
  },
  mounted: function () {
    this.ds.loadDetail(this.$route.query.id).then((data) => {
      this.data = data.Data;
      this.compared = JSON.parse(data.Data.Data);
    });
  },
  methods: {
    load: function () {},
  },
};
</script>
