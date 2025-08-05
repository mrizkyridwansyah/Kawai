<template>
  <header-menu title="Unit Classification" :breadcrumbs="this.breadcrumbs" />
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
            <th class="text-center">Unit Classification Code</th>
            <th class="text-center">Unit Classification Name</th>
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
            <td>{{ item.UnitClassificationCode }}</td>
            <td>{{ item.UnitClassificationName }}</td>
            <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
            <td>{{ item.LastUser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    ref="modalUnitClassification"
    id="modal-form-unitclassification"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formUnitClassification.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-unitclassification
      ref="formUnitClassification"
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
      { title: "Unit Classification", active: true, to: "/app/master/unitclassification" },
    ],
    keywordKeys: [
      {
        Id: "UnitClassificationCode",
        Name: "Unit Classification Code",
      },
      {
        Id: "UnitClassificationName",
        Name: "Unit Classification Name",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "UnitClassificationCode",
      sorts: {
        UnitClassificationName: "asc",
      },
      sortItems: [
        {
          label: "Unit Classification Name",
          value: "UnitClassificationName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Unit Classification Code",
          value: "UnitClassificationCode",
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
      return useUnitClassification();
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
      this.title = "Add Unit Classification";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-unitclassification");
    },
    edit: function (dt) {
      this.title = "Edit Unit Classification";
      this.modalMode = "edit";
      this.idSelected = dt.UnitClassificationCode;
      this.$bvModal.show("modal-form-unitclassification");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.UnitClassificationCode)
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
        item.UnitClassificationName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-unitclassification");
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
