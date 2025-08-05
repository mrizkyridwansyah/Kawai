<template>
  <div>
    <h5>Change Logs</h5>
    <table class="x-table mt-3 w-100"
      v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
    >
      <thead>
        <tr>
          <th style="width: 50px;">#</th>
          <th style="width: 150px;">Date</th>
          <th>Username</th>
          <th>Document Type</th>
          <th>Reference Id</th>
          <th>Action</th>
          <th>Remote Addr.</th>
          <th style="width: 90px;">&nbsp;</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in ds.data.Items" :key="i">
          <td>{{ i+1 }}.</td>
          <td>{{$func.formatDateTime(item.Date, 'DD MMM YYYY HH:mm:ss')}}</td>
          <td>{{item.UserName}}</td>
          <td>{{item.DocumentType}}</td>
          <td>{{item.ReferenceId}}</td>
          <td>
            <span v-if="item.Action == 'Create'" class="fw-bold text-success">{{ item.Action }}</span>
            <span v-if="item.Action == 'Update'" class="fw-bold text-primary">{{ item.Action }}</span>
            <span v-if="item.Action == 'Delete'" class="fw-bold text-danger">{{ item.Action }}</span>
          </td>
          <td>{{item.RemoteAddr}}</td>
          <td>
            <a href="javascript:void(0);"
              @click="() => detailItem(item)"
            >
              View Detail
            </a>
            <!-- <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Detail', event: () => detailItem(item) },
                // { separator: true },
                // { href: 'javascript:void(0);', icon: 'trash', label: 'Remove', event: () => removeItem(item) },
              ]"
            /> -->
          </td>
        </tr>
      </tbody>
    </table>
      
    <v-loading-2 class="m-5 p-5" v-if="ds.isLoading"/>
    <div>
      <v-table-pagination
        v-if="!ds.isLoading && ds.data.Items.length > 0 && !ds.isNetworkError && !ds.isServerError"
        class="mt-3"
        :table="ds.data"
        :page-change="ds.setPage"
        :length-change="ds.setLength"
      />
      <v-data-empty class="mt-3" v-if="!ds.isLoading && ds.data.Items.length == 0 && !ds.isNetworkError && !ds.isServerError" />
      <v-error-server class="mt-3" v-if="!ds.isLoading && ds.isServerError" :refresh="ds.load" />
      <v-error-network class="mt-3" v-if="!ds.isLoading && ds.isNetworkError" :refresh="ds.load" />
    </div>
    <shared-log-detail :data="selectedItem"/>
  </div>
</template>

<script>
export default {
  props: ['documentType'],
  data: () => ({
    selectedItem: {},
    filter: {
      keyword: null,
      keywordKey: 'ReferenceId',
      sorts: {
        Date: "desc",
      },
      sortItems: [
        {
          label: "UserName",
          value: "UserName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Date",
          value: "Date",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
  }),
  computed: {
    ds: function() {
      return useSharedLogData();
    }
  },
  watch: {
    filter: {
      deep: true,
      handler: function(after) {
        if(this.debounce)
          clearTimeout(this.debounce)

        this.debounce = setTimeout(() => {

          var filter = [
            ['EntityId', this.$route.query.id]
          ]

          if(after.keywordKey != '' && after)
            filter.push([after.keywordKey, 'like', `%${after.keyword || ''}%`]);

          this.ds.setDocumentType(this.documentType)
          this.ds.setSort(after.sorts)
          this.ds.setFilter(filter)
          this.ds.load();
        }, 800)
      },
    }
  },
  mounted: function() {
    this.ds.setDocumentType(this.documentType)
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter([['EntityId', this.$route.query.id]]);
    this.ds.load();
  },
  methods: {
    detailItem: function(data) {
      this.selectedItem = data;
      this.$bvModal.show('shared-log-detail')
      // this.$router.push(`data-log/data-detail?id=${data.Id}`)
    },
  }
}
</script>