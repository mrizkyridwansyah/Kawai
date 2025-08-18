<template>
  <header-menu title="Part Receipt Material" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />

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
