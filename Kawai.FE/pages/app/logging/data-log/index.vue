<template>
  <header-menu title="Data Changes Logs" />
  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle"
        style="width: 100%"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center" style="width: 50px">#</th>
            <th class="text-center" style="width: 150px">Date</th>
            <th class="text-center">Username</th>
            <th class="text-center">Document Type</th>
            <th class="text-center">Reference Id</th>
            <th class="text-center">Activity</th>
            <th class="text-center">Remote Addr.</th>
            <th class="text-center" style="width: 20px">&nbsp;</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, i) in ds.data.Items" :key="i">
            <td>{{ i + 1 }}.</td>
            <td>
              {{ $func.formatDateTime(item.Date, "DD MMM YYYY HH:mm:ss") }}
            </td>
            <td>{{ item.FullName }}</td>
            <td>{{ item.DocumentType }}</td>
            <td>{{ item.ReferenceId }}</td>
            <td>
              <span
                v-if="item.Activity == 'Create'"
                class="fw-bold text-success"
                >{{ item.Activity }}</span
              >
              <span
                v-else-if="item.Activity == 'Update'"
                class="fw-bold text-primary"
                >{{ item.Activity }}</span
              >
              <span
                v-else-if="item.Activity == 'Delete'"
                class="fw-bold text-danger"
                >{{ item.Activity }}</span
              >
              <span v-else class="fw-bold text-primary">{{
                item.Activity
              }}</span>
            </td>
            <td>{{ item.RemoteAddr }}</td>
            <td>
              <v-button-dropdown
                :list="[
                  {
                    href: 'javascript:void(0);',
                    icon: 'edit',
                    label: 'Detail',
                    event: () => detailItem(item),
                  },
                  // { separator: true },
                  // { href: 'javascript:void(0);', icon: 'trash', label: 'Remove', event: () => removeItem(item) },
                ]"
              />
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
    breadcrumbs: [
      { title: "Logging", active: false, to: "/logging" },
      { title: "Data Changes Log", active: false, to: "" },
    ],
    selectedItem: {},
    list: [],
    filter: {
      keyword: null,
      keywordKey: "ReferenceId",
      sorts: {
        Date: "desc",
      },
      sortItems: [
        {
          label: "FullName",
          value: "FullName",
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
    ds: function () {
      return useLogData();
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

    this.$http
      .post("/logging/data/list", {
        Page: 1,
        Length: 2000000000,
        Filters: [],
        Sorts: { Date: "desc" },
      })
      .then(({ data }) => {
        this.list = data.Data.Items;
      });
  },
  methods: {
    detailItem: function (data) {
      this.$router.push(`data-log/data-detail?id=${data.Id}`);
    },
  },
};
</script>
