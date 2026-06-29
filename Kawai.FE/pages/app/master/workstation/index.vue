<template>
  <v-frame title="Work Station Master" icon="database">
    <template #frame-content>
      <v-button-add :add="add" cClass="mr-1"  :disabled="!menuPrivAllowUpdate" />
      <hr />
      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :ds="ds"
        :top-content-height="210"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            style="min-width: 100%; width: max-content"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Action</th>
                <th class="text-center">WS Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Register User</th>
                <th class="text-center">Register Date</th>
                <th class="text-center">Last User</th>
                <th class="text-center">Last Update</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td class="text-center">
                  <font-awesome-icon
                    class="mr-2 text-success"
                    icon="pencil"
                    @click="menuPrivAllowUpdate && edit(item)"
                  />
                  <font-awesome-icon
                    class="ml-2 text-danger"
                    icon="trash"
                    @click="menuPrivAllowUpdate && remove(item)"
                  />
                </td>
                <td>{{ item.WorkStationCode }}</td>
                <td>{{ item.WorkStationName }}</td>
                <td>{{ item.RegisterUser }}</td>
                <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
                <td>{{ item.LastUser }}</td>
                <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>

  <v-modal
    ref="modalWorkStation"
    id="modal-form-workstation"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formWorkStation.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-workstation
      ref="formWorkStation"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    keywordKeys: [
      {
        Id: "WorkStationCode",
        Name: "WS Code",
      },
      {
        Id: "WorkStationName",
        Name: "Description",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "WorkStationCode",
      sorts: {
        WorkStationName: "asc",
      },
      sortItems: [
        {
          label: "Description",
          value: "WorkStationName",
          selected: false,
          direction: "asc",
        },
        {
          label: "WS Code",
          value: "WorkStationCode",
          selected: true,
          direction: "desc",
        },
      ],
    },
    idSelected: "",
    title: "",
    modalMode: "",
    debounce: null,
    menuPrivAllowUpdate: false,
  }),
  computed: {
    ds: function () {
      return useWorkStation();
    },
    dsMenu: function () {
      return useMenu();
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
     this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "A13",
      )[0].AllowUpdate;
    });
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter([]);
    this.ds.load();
  },
  methods: {
    add: function () {
      this.title = "Add Work Station";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-workstation");
    },
    edit: function (dt) {
      this.title = "Edit Work Station";
      this.modalMode = "edit";
      this.idSelected = dt.WorkStationCode;
      this.$bvModal.show("modal-form-workstation");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.WorkStationCode)
              .then((_) => {
                this.ds.load();
                toastSuccess("success");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.WorkStationName,
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-workstation");
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
<style scoped>
.button-section {
  /* garis panjang bawah */
  width: 100%;
}
</style>
