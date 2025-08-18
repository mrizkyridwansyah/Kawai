<template>
  <header-menu title="Part Receipt Material" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />

  <v-table
    :filter="filter"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :data-items="ds.data.Items"
    :frozen-column-left="2"
    :ds="ds"
    ref="vtable"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
        ref="table"
      >
        <thead>
          <tr>
            <th class="text-center">Action</th>
            <th class="text-center">Supplier</th>
            <th class="text-center">DN Number</th>
            <th class="text-center">BC Number</th>
            <th class="text-center">DN Date</th>
            <th class="text-center">Complete Receipt</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items" :key="idx">
            <td class="text-center" v-if="item.IsComplete"></td>
            <td class="text-center" v-else>
              <v-app-link
                style="text-decoration: none"
                :to="`/receipt/create?id=${item.Id}`"
                >Next
                <font-awesome-icon class="ml-2" icon="arrow-right" />
              </v-app-link>
            </td>
            <td>{{ item.SupplierName }}</td>
            <td>{{ item.DNNumber }}</td>
            <td>{{ item.BCNumber }}</td>
            <td>{{ $func.formatDate(item.DNDate) }}</td>
            <td
              class="text-success text-center"
              style="font-weight: bold"
              v-if="item.IsComplete"
            >
              <font-awesome-icon class="ml-2" icon="check" />
            </td>
            <td class="text-danger text-center" style="font-weight: bold" v-else>
              <font-awesome-icon class="ml-2" icon="x" />
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
      { title: "Stock Control", active: false, to: "" },
      { title: "Part Receipt Material", active: false, to: "" },
    ],
    filter: {
      keyword: null,
      keywordKey: "DNNumber",
      sorts: {
        DNNumber: "asc",
      },
      sortItems: [
        {
          label: "DN Number",
          value: "DNNumber",
          selected: true,
          direction: "asc",
        },
        {
          label: "Supplier",
          value: "SupplierName",
          selected: false,
          direction: "asc",
        },
      ],
    },
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useDeliveryNote();
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
    exportExcel: function () {
      return new Promise((resolve, reject) => {
        this.ds
          .exportExcel()
          .then((_) => {
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          });
      });
    },
  },
};
</script>
