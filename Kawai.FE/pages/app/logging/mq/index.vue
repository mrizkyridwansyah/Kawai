<template>
  <header-menu title="Message Queueing Logs" />
  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table"
        style="width: 100%"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center" style="width: 50px">#</th>
            <th class="text-center" style="width: 150px">Date</th>
            <th class="text-center">Username</th>
            <th class="text-center">Method</th>
            <th class="text-center">Path</th>
            <th class="text-center">Description</th>
            <th class="text-center">Transaction</th>
            <th class="text-center">Elapsed Time (Ms)</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, i) in ds.data.Items" :key="i">
            <td>{{ i + 1 }}.</td>
            <td>
              {{ $func.formatDateTime(item.TimeStamp, "DD MMM YYYY HH:mm:ss") }}
            </td>
            <td>{{ item.FullName }}</td>
            <td>{{ item.Method }}</td>
            <td>{{ item.Path }}</td>
            <td>{{ item.FormatMessage }}</td>
            <td>{{ item.TransactionType }}</td>
            <td class="text-right">
              {{ $func.formatNumber(item.ElapsedtimeMs) }}
            </td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  data: () => ({
    selectedItem: {},
    filter: {
      keyword: null,
      keywordKey: "ReferenceId",
      sorts: {
        Timestamp: "desc",
      },
      sortItems: [
        {
          label: "Timestamp",
          value: "Timestamp",
          selected: true,
          direction: "desc",
        },
        {
          label: "FullName",
          value: "FullName",
          selected: false,
          direction: "asc",
        },
        {
          label: "ElapsedtimeMs",
          value: "ElapsedtimeMs",
          selected: false,
          direction: "desc",
        },
      ],
    },
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useLogMQ();
    },
  },
  watch: {
    filter: {
      deep: true,
      handler: function (after) {
        if (this.debounce) clearTimeout(this.debounce);

        this.debounce = setTimeout(() => {
          var filter = [];

          if (after.keywordKey != "" && after)
            filter.push({ Keyword: after.keyword || "" });

          this.ds.setSort(after.sorts);
          this.ds.setFilter(filter);
          this.ds.load();
        }, 800);
      },
    },
  },
  mounted: function () {
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter([]);
    this.ds.load();
  },
  methods: {
    detailItem: function (data) {
      this.selectedItem = data;
      this.$bvModal.show("shared-log-network-error-detail");
    },
  },
};
</script>
