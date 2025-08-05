<template>
  <header-menu title="Item Type" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />
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
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Action</th>
            <th class="text-center">Item Type Code</th>
            <th class="text-center">Item Type Name</th>
            <th class="text-center">Active</th>
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
            <td>{{ item.ItemTypeCode }}</td>
            <td>{{ item.ItemTypeName }}</td>
            <td v-if="item.IsActive"><span class="text-green">YES</span></td>
            <td v-else><span class="text-danger">NO</span></td>
            <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
            <td>{{ item.LastUser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    ref="modalItemType"
    id="modal-form-itemtype"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formItemType.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-itemtype
      ref="formItemType"
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
      { title: "Item Type", active: true, to: "/app/master/itemtype" },
    ],
    keywordKeys: [
      {
        Id: "ItemTypeCode",
        Name: "Item Type Code",
      },
      {
        Id: "ItemTypeName",
        Name: "Item Type Name",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "ItemTypeCode",
      sorts: {
        ItemTypeName: "asc",
      },
      sortItems: [
        {
          label: "Item Type Name",
          value: "ItemTypeName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Type Code",
          value: "ItemTypeCode",
          selected: true,
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
      return useItemType();
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
      this.title = "Add Item Type";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-itemtype");
    },
    edit: function (dt) {
      this.title = "Edit Item Type";
      this.modalMode = "edit";
      this.idSelected = dt.ItemTypeCode;
      this.$bvModal.show("modal-form-itemtype");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.ItemTypeCode)
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
        item.ItemTypeName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-itemtype");
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
