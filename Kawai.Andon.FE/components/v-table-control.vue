<template>
  <div>
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
  </div>
</template>

<script>
export default {
  props: ['dataSource'],
  computed: {
    ds: function() {
      return this.dataSource ?? {};
    }
  }
}
</script>