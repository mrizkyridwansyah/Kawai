<template>
  <div>
    <table
      class="x-table mt-3 w-100"
      v-if="!ds?.isLoading && !ds?.isNetworkError && !ds?.isServerError"
    >
      <thead>
        <tr>
          <th class="text-center">DN Number</th>
          <th class="text-center">Supplier</th>
          <th class="text-center">BC Number</th>
          <th class="text-center">BC Type</th>
          <th class="text-center">DN Date</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in ds?.data?.Items" :key="i">
          <td>
            <span v-for="(x, i) in actions || []">
              <span v-if="x.separator === true"> | </span>
              <nuxt-link @click="() => x?.event(item)" v-else-if="x.to">
                {{ item.DNNumber }}
              </nuxt-link>
              <a
                :href="item.href || 'javascript:void(0)'"
                @click="() => x?.event(item)"
                v-else
              >
                {{ item.DNNumber }}
              </a>
            </span>
          </td>
          <td>{{ item.SupplierName }}</td>
          <td>{{ item.BCNumber }}</td>
          <td>{{ item.BCType }}</td>
          <td>{{ $func.formatDate(item.DNDate) }}</td>
        </tr>
      </tbody>
    </table>

    <v-loading-2 class="m-5 p-5" v-if="ds?.isLoading" />
    <div>
      <v-table-pagination
        v-if="
          !ds?.isLoading &&
          ds?.data?.Items?.length > 0 &&
          !ds?.isNetworkError &&
          !ds?.isServerError
        "
        class="mt-3"
        :table="ds?.data"
        :page-change="ds?.setPage"
        :length-change="ds?.setLength"
      />
      <v-data-empty
        class="mt-3"
        v-if="
          !ds?.isLoading &&
          ds?.data?.Items?.length == 0 &&
          !ds?.isNetworkError &&
          !ds?.isServerError
        "
      />
      <v-error-server
        class="mt-3"
        v-if="!ds?.isLoading && ds?.isServerError"
        :refresh="ds?.load"
      />
      <v-error-network
        class="mt-3"
        v-if="!ds?.isLoading && ds?.isNetworkError"
        :refresh="ds?.load"
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
        DNNumber: "asc",
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
