<template>
  <div>
    <table
      class="x-table mt-3 w-100"
      v-if="!ds.isLoadingListDetail && !ds.isNetworkError && !ds.isServerError"
    >
      <thead>
        <tr>
          <th class="text-center">PO Number</th>
          <th class="text-center">PO Date</th>
          <th class="text-center">Supplier</th>
          <th class="text-center">Item</th>
          <th class="text-center">Unit</th>
          <th class="text-center">Qty</th>
          <th class="text-center">Total Packing</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in ds.dataDetails.Items" :key="i">
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
          <td>{{ item.ItemName }}</td>
          <td>{{ item.UnitClsName }}</td>
          <td>{{ $func.formatMoney(item.Qty) }}</td>
          <td>{{ $func.formatMoney(item.TotalPacking) }}</td>
        </tr>
      </tbody>
    </table>

    <v-loading-2 class="m-5 p-5" v-if="ds.isLoadingListDetail" />
    <div>
      <v-table-pagination
        v-if="
          !ds.isLoadingListDetail &&
          ds.dataDetails.Items.length > 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
        class="mt-3"
        :table="ds.dataDetails"
        :page-change="ds.setPageListDetail"
        :length-change="ds.setLengthListDetail"
      />
      <v-data-empty
        class="mt-3"
        v-if="
          !ds.isLoadingListDetail &&
          ds.dataDetails.Items.length == 0 &&
          !ds.isNetworkError &&
          !ds.isServerError
        "
      />
      <v-error-server
        class="mt-3"
        v-if="!ds.isLoadingListDetail && ds.isServerError"
        :refresh="ds.listDetail"
      />
      <v-error-network
        class="mt-3"
        v-if="!ds.isLoadingListDetail && ds.isNetworkError"
        :refresh="ds.listDetail"
      />
    </div>
  </div>
</template>

<script>
export default {
  props: ["data", "actions", "list", "filters"],
  data: () => ({
    filter: {
      sorts: {
        PONumber: "asc",
      },
    },
  }),
  watch: {
    filters: {
      deep: true,
      immediate: true,
      handler(newVal) {
        this.ds.setFilter(newVal);
        this.ds.listDetail();
      },
    },
  },
  computed: {
    ds: function () {
      return this.list;
    },
  },
  mounted: function () {
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter(this.filters);
    this.ds.listDetail();
  },
};
</script>
