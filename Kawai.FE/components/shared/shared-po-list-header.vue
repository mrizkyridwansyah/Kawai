<template>
  <div>
    <table
      class="x-table mt-3 w-100"
      v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
    >
      <thead>
        <tr>
          <th class="text-center">PO Number</th>
          <th class="text-center">PO Date</th>
          <th class="text-center">Supplier</th>
          <th class="text-center">Warehouse To</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in ds.data.Items" :key="i">
          <td>
            <span v-for="(x, i) in actions || []">
              <span v-if="x.separator === true"> | </span>
              <nuxt-link @click="() => x?.event(item)" v-else-if="x.to">
                {{ item.PONumber }}
              </nuxt-link>
              <a
                :href="item.href || 'javascript:void(0)'"
                @click="() => x?.event(item)"
                v-else
              >
                {{ item.PONumber }}
              </a>
            </span>
          </td>
          <td>{{ $func.formatDate(item.PODate) }}</td>
          <td>{{ item.SupplierName }}</td>
          <td>{{ item.WarehouseCode }}</td>
        </tr>
      </tbody>
    </table>

    <v-loading-2 class="m-5 p-5" v-if="ds.isLoading" />
    <div>
      <v-table-pagination
        v-if="
          !ds.isLoading &&
          ds.data.Items.length > 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
        class="mt-3"
        :table="ds.data"
        :page-change="ds.setPage"
        :length-change="ds.setLength"
      />
      <v-data-empty
        class="mt-3"
        v-if="
          !ds.isLoading &&
          ds.data.Items.length == 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
      />
      <v-error-server
        class="mt-3"
        v-if="!ds.isLoading && ds.isServerError"
        :refresh="ds.load"
      />
      <v-error-network
        class="mt-3"
        v-if="!ds.isLoading && ds.isNetworkError"
        :refresh="ds.load"
      />
    </div>
  </div>
</template>

<script>
export default {
  props: ["data", "actions", "list"],
  data: () => ({
    filter: {
      sorts: {
        PONumber: "asc",
      },
    },
  }),
  computed: {
    ds: function () {
      return this.list;
    },
  },
  mounted: function () {
    this.ds.setSort(this.filter.sorts);
    this.ds.load();
  },
};
</script>
