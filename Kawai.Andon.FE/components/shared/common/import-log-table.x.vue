<template>
  <div>
    <table class="x-table" v-if="!ds.isLoading && ds.data.Items.length > 0">
      <thead>
        <tr>
          <th>#</th>
          <th style="min-width: 125px;">User</th>
          <!-- <th>File</th> -->
          <th class="text-center">Status</th>
          <th class="text-center">Rows</th>
          <th class="text-center">Valid</th>
          <th class="text-center">Invalid</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in ds.data.Items" 
          :key="item.Id"
        >
          <td style="vertical-align: top;">{{ ds.data.Page * ds.data.Length - ds.data.Length + (i + 1) }}</td>
          <td>
            {{item.FileName}}
            <small><strong>({{$func.bytesToSize(item.Length)}})</strong></small>
            <div>
              <small>{{ item.Description }}</small>
            </div>
            <div><small>{{$func.formatDateTime(item.Date)}}</small>, {{item.UserName}}</div>
          </td>
          <!-- <td>
            {{item.FileName}} <br />
            <small><strong>({{$func.bytesToSize(item.Length)}})</strong></small>
            <div>
              <small>{{ item.Description }}</small>
            </div>
          </td> -->
          <td class="text-center">
            <span class="badge text-bg-success" v-if="item.Status == 'SUCCESS'">
              {{item.Status}}
            </span>
            <span class="badge text-bg-danger" v-else>
              {{item.Status}}
            </span>
          </td>
          <td class="text-center">{{item.RowsCount}}</td>
          <td class="text-center text-success">
            <span>{{item.ValidRowsCount}}</span>
          </td>
          <td class="text-center text-danger">
            <span>{{item.InvalidRowsCount}}</span>
          </td>
          <td class="text-right">
            <a href="#" @click="() => downloadFile(item.Id)"
              data-bs-toggle="tooltip" 
              data-bs-placement="top"
              data-bs-custom-class="custom-tooltip"
              data-bs-title="Download"  
            >
              <v-icon
                name="download"
                :width="18"
                weight="3"
                style="color: #445575"
              />
            </a>
          </td>
        </tr>
      </tbody>
    </table>     
    <v-loading-2 class="m-5 p-5" v-if="ds.isLoading"/>
    <v-data-empty class="my-4" v-if="!ds.isLoading && ds.data.Items.length == 0" />
    <v-table-pagination
      v-if="!ds.isLoading && ds.data.Items.length > 0"
      class="mt-3"
      size="xs"
      :table="ds.data"
      :page-change="ds.setPage"
      :length-change="ds.setLength"
    />   
  </div>
</template>

<script>
export default {
  props: ['template'],
  data: () => ({

  }),
  computed: {
    workspaceId: function() {
      return this.$route.params?.id;
    },
    ds: function() {
      return useImportLog();
    },
  },
  watch: {
    template: function(after) {
      this.ds.setTemplate(after);
      this.ds.load();
    }
  },
  mounted: function() {
    this.ds.setTemplate(this.template);
    this.ds.load();
  },
  methods: {
    downloadFile: function(id) {
      this.$http.get(`/import/download-key?workspace=${this.workspaceId}&id=${id}`)
        .then(({data}) => {
          this.$http.open(`/import-log/download?id=${id}&key=${data.Data}`)
        })
    },
  }
}
</script>