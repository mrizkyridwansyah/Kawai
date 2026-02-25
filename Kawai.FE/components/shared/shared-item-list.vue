<template>
  <div class="panel">
    <div
      class="panel-heading ui-sortable-handle"
      style="padding-left: 0; padding-right: 0"
    >
      <v-button-sort
        class="mr-1"
        v-model="this.filter.sorts"
        :items="this.filter.sortItems"
      />
      <div class="input-group ml-2 mr-2">
        <input
          type="text"
          placeholder="Search..."
          class="form-control"
          style="padding: 4px 10px"
          v-model="filter.keyword"
        />
      </div>
    </div>
  </div>
  <div>
    <table
      class="x-table mt-3 w-100"
      v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
    >
      <thead>
        <tr>
          <th class="text-center">Item Code</th>
          <th class="text-center">Item Name</th>
          <th class="text-center">Unit</th>
          <th class="text-center">Qty Packing</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in ds.dataItem.Items || []" :key="i">
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
          <td>{{ item.UnitClsName }}</td>
          <td class="text-right">{{ $func.formatMoney(item.QtyPacking) }}</td>
        </tr>
      </tbody>
    </table>

    <v-loading-2 class="m-5 p-5" v-if="ds.isLoading" />
    <div>
      <v-table-pagination
        v-if="
          !ds.isLoading &&
          ds.dataItem.Items.length > 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
        class="mt-3"
        :table="ds.dataItem"
        :page-change="ds.setPageItem"
        :length-change="ds.setLengthItem"
      />
      <v-data-empty
        class="mt-3"
        v-if="
          !ds.isLoading &&
          ds.dataItem.Items.length == 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
      />
      <v-error-server
        class="mt-3"
        v-if="!ds.isLoading && ds.isServerError"
        :refresh="ds.loadListItem"
      />
      <v-error-network
        class="mt-3"
        v-if="!ds.isLoading && ds.isNetworkError"
        :refresh="ds.loadListItem"
      />
    </div>
  </div>
</template>

<script>
export default {
  props: ["data", "actions", "list", "filters", "refresh"],
  data: () => ({
    lists: [],
    filter: {
      keyword: "",
      sorts: {
        Item_Code: "asc",
      },
      sortItems: [
        {
          label: "Item Code",
          value: "Item_Code",
          selected: true,
          direction: "asc",
        },
        {
          label: "Item Name",
          value: "Item_Name",
          selected: false,
          direction: "asc",
        },
      ],
    },
  }),
  watch: {
    filters: {
      deep: true,
      immediate: true,
      handler(newVal) {
        this.load();
      },
    },
    filter: {
      deep: true,
      immediate: true,
      handler(newVal) {
        this.load();
      },
    },
    refresh: function () {
      this.filter.keyword = "";
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
      this.ds.setSortItem(this.filter.sorts);

      let xFilter = [...this.filters];
      xFilter.push({ Keyword: this.filter.keyword || "" });
      this.ds.setFilterItem(xFilter);

      this.$nextTick(() => this.ds.loadListItem());
    },
  },
};
</script>
