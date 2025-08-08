<template>
  <header-menu title="Delivery Place" :breadcrumbs="this.breadcrumbs" />
  <button
    class="ml-2 btn btn-sm btn-danger btn-elevate"
    @click="() => this.$router.push('/app/master/trade')"
  >
    <font-awesome-icon icon="arrow-left" />
    <span class="ml-2">Back</span>
  </button>
  <v-button-add
    :add="add"
    cClass="btn btn-sm btn-primary btn-elevate"
    style="margin-left: 10px"
  />

  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle"
        style="min-width: 100%; width: max-content"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Action</th>
            <th class="text-center">Trade Code</th>
            <th class="text-center">Location Code</th>
            <th class="text-center">Location Name</th>
            <th class="text-center">Last Update</th>
            <th class="text-center">Last User</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
            <td class="text-center">
              <font-awesome-icon
                class="mr-2 text-success"
                icon="pencil"
                @click="edit(item)"
              />
              <font-awesome-icon
                class="ml-2 text-danger"
                icon="trash"
                @click="remove(item)"
              />
            </td>
            <td>{{ item.Trade_Code }}</td>
            <td>{{ item.Location_Code }}</td>
            <td>{{ item.Location_Name }}</td>
            <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
            <td>{{ item.Lastuser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
  <v-modal
    ref="modalDeliveryPlace"
    id="modal-form-deliveryplace"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formDeliveryPlace.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-deliveryplace
      ref="formDeliveryPlace"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Master", active: false, to: "" },
      { title: "Trade Master", active: false, to: "/master/trade" },
      { title: "Delivery Place", active: true, to: "" },
    ],
    filter: {
      keyword: null,
      keywordKey: "Location_Code",
      sorts: {
        Location_Name: "asc",
      },
      sortItems: [
        {
          label: "Location Name",
          value: "Location_Name",
          selected: false,
          direction: "asc",
        },
        {
          label: "Location Code",
          value: "Location_Code",
          selected: true,
          direction: "desc",
        },
      ],
    },
    idSelected: "",
    title: "",
    modalMode: "",
    debounce: null,
    selectedPrint: [],
    isLoadingPrint: false,
  }),
  computed: {
    ds: function () {
      return useDeliveryPlace();
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
    alert(this.$route.query.id);
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter([{ itemcode: this.$route.query.id }]);
    this.ds.load();
  },
  methods: {
    add: function () {
      this.title = "Add Delivery Place";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-deliveryplace");
    },
    edit: function (dt) {
      this.title = "Edit Delivery Place";
      this.modalMode = "edit";
      this.idSelected = dt.Location_Code;
      this.$bvModal.show("modal-form-deliveryplace");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.Location_Code)
              .then((_) => {
                this.ds.load();
                toastSuccess("Success");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.WarehouseName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-deliveryplace");
      this.ds.load();
    },
  },
};
</script>
