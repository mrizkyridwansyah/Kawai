<template>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-page="ds.setPageDetail"
    :ds-length="ds.setLengthDetail"
    :ds-load="ds.inquiryDetail"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Item Code</th>
            <th class="text-center">Item Name</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, i) in ds.data.Items || []" :key="i">
            <td>
              <span v-for="(x, i) in actions || []">
                <span v-if="x.separator === true"> | </span>
                <nuxt-link @click="() => x?.event(item)" v-else-if="x.to">
                  {{ item.ItemCode }}
                </nuxt-link>
                <a
                  :href="item.href || 'javascript:void(0)'"
                  @click="() => x?.event(item)"
                  v-else
                >
                  {{ item.ItemCode }}
                </a>
              </span>
            </td>
            <td>{{ item.ItemName }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["data", "actions", "list", "refresh"],
  data: () => ({
    lists: [],
    filter: {
      keyword: null,
      sorts: {
        Item_Code: "asc",
      },
      sortItems: [
        {
          label: "Item Name",
          value: "Item_Name",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "Item_Code",
          selected: true,
          direction: "asc",
        },
      ],
    },
  }),
  watch: {
    "filter.keyword": function () {
      this.load();
    },
    "filter.sorts": function () {
      this.load();
    },
    refresh: function () {
      this.filter.keyword = null;
      this.load();
    },
  },
  computed: {
    ds: function () {
      return this.list;
    },
  },
  mounted: function () {
    this.load();
  },
  methods: {
    load: function () {
      this.ds.setSort(this.filter.sorts);

      let filters = [];
      if (this.filter.keyword)
        filters.push({ Keyword: this.filter.keyword || "" });

      this.ds.setFilter(filters);
      this.$nextTick(() => this.ds.load());
    },
  },
};
</script>
