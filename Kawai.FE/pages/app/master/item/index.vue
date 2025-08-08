<template>
  <header-menu title="Item" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />
  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
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
              <th class="text-center">Item Code</th>
              <th class="text-center">Item Name</th>
              <th class="text-center">Finish Good Part Cls</th>
              <th class="text-center">Drawing Number</th>
              <th class="text-center">Warehouse</th>
              <th class="text-center">Last Update</th>
              <th class="text-center">Last User</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, idx) in ds.data.Items" :key="idx">
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
              <td>{{ item.ItemCode }}</td>
              <td>{{ item.ItemName }}</td>
              <td>{{ item.FinishGoodPartClsDesc }}</td>
              <td>{{ item.DrawingNumber }}</td>
              <td>{{ item.WarehouseName }}</td>
              <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
              <td>{{ item.LastUser }}</td>
            </tr>
          </tbody>
        </table>
    </template>
  </v-table>

  <v-modal
    ref="modalItem"
    id="modal-form-item"
    :title="title"
    :fullscreen="true"
    size="lg"
    @hidden="
      () => {
        this.$refs.formItem.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-item
      ref="formItem"
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
      { title: "Group 2", active: false, to: "" },
      { title: "Item", active: true, to: "/app/master/item" },
    ],
    keywordKeys: [
      {
        Id: "ItemCode",
        Name: "Item Code",
      },
      {
        Id: "ItemName",
        Name: "Item Name",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "ItemCode",
      sorts: {
        Item_Name: "asc",
      },
      sortItems: [
        {
          label: "Item Name",
          value: "Item_Name",
          selected: true,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "ItemCode",
          selected: false,
          direction: "desc",
        },
      ],
    },
    idSelected: "",
    title: "",
    modalMode: "",
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useItem();
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
    add: function () {
      this.title = "Add Item";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-item");
    },
    edit: function (dt) {
      this.title = "Edit Item";
      this.modalMode = "edit";
      this.idSelected = dt.ItemCode;
      this.$bvModal.show("modal-form-item");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.ItemCode)
              .then((_) => {
                this.ds.load();
                toastSuccess("Anying........ ngapus");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.ItemName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-item");
      this.ds.load();
    },
    exportExcel: function () {
      new Promise((resolve, reject) => {
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
