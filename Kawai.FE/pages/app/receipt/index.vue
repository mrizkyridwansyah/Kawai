<template>
  <header-menu title="Part Receipt Material" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />

  <v-table
    :filter="filter"
    :export-excel="false"
    :export-excel-action="exportExcel"
    :data-items="ds.data.Items"
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
            <th class="text-center">Receipt No</th>
            <th class="text-center">Receipt Date</th>
            <th class="text-center">Supplier</th>
            <th class="text-center">DN Number</th>
            <th class="text-center">BC Number</th>
            <th class="text-center">BC Type</th>
            <th class="text-center">Vehicle No</th>
            <th class="text-center">Last User</th>
            <th class="text-center">Last Update</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items" :key="idx">
            <td class="text-center" v-if="item.IsComplete"></td>
            <td class="text-center" v-else>
              <font-awesome-icon
              style="cursor: pointer;"
                class="ml-2 text-primary"
                icon="eye"
                @click="view(item)"
              />
            </td>
            <td>{{ item.ReceiptNo }}</td>
            <td>{{ $func.formatDate(item.ReceiptDate) }}</td>
            <td>{{ item.SupplierName }}</td>
            <td>{{ item.DNNumber }}</td>
            <td>{{ item.BCNumber }}</td>
            <td>{{ item.BCType }}</td>
            <td>{{ item.VehicleNo }}</td>
            <td>{{ item.LastUser }}</td>
            <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    ref="modalPartReceipt"
    id="modal-form-part-receipt"
    :title="title"
    :fullscreen="true"
    @hidden="
      () => {
        this.$refs.formPartReceipt.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-part-receipt
      ref="formPartReceipt"
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
      { title: "Stock Control", active: false, to: "" },
      { title: "Part Receipt Material", active: false, to: "" },
    ],
    filter: {
      keyword: null,
      keywordKey: "ReceiptNo",
      sorts: {
        ReceiptNo: "asc",
      },
      sortItems: [
        {
          label: "Receipt No.",
          value: "ReceiptNo",
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
    modalVisible: false,
    idSelected: null,
    title: "",
    modalMode: "",
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useReceipt();
    },
    notif: function () {
      return useNotification();
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
    "notif.newNotif": function () {
      this.ds.load()
    }
  },
  mounted: function () {
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter([]);
    this.ds.load();
  },
  methods: {
    add: function () {
      this.title = "Add Part Receipt Material";
      this.modalMode = "add";
      this.idSelected = null;
      this.$bvModal.show("modal-form-part-receipt");
    },
    view: function (dt) {
      this.title = "View Part Receipt Material";
      this.modalMode = "view";
      this.idSelected = dt.Id;
      this.$bvModal.show("modal-form-part-receipt");
    },
    close: function () {
      document.getElementById("close-modal-form-part-receipt").click();
      this.ds.load();
    },
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
