<template>
  <header-menu title="User" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />
  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :data-items="ds.data.Items"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Action</th>
            <th class="text-center">User ID</th>
            <th class="text-center">Full Name</th>
            <th class="text-center">Job Position</th>
            <th class="text-center">Status Admin</th>
            <th class="text-center">User Group</th>
            <th class="text-center">Register Date</th>
            <th class="text-center">Register User</th>
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
                style="cursor: pointer"
                @click="edit(item)"
              />
              <font-awesome-icon
                class="ml-2 mr-2 text-danger"
                icon="trash"
                style="cursor: pointer"
                @click="remove(item)"
              />
              <font-awesome-icon
                class="ml-2 text-grey"
                icon="cog"
                style="cursor: pointer"
                @click="
                  () => this.$router.push(`user/privilege?id=${item.UserID}`)
                "
              />
            </td>
            <td>{{ item.UserID }}</td>
            <td>{{ item.FullName }}</td>
            <td>{{ item.JobPosition }}</td>
            <td v-if="item.IsAdmin">Yes</td>
            <td v-else>No</td>
            <td>{{ item.UserGroupID }}</td>
            <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
            <td>{{ item.RegisterUser }}</td>
            <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
            <td>{{ item.LastUser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    class="modal-lg"
    ref="modalUser"
    id="modal-form-user"
    :title="title"
    @hidden="
      () => {
        this.$refs.formUser.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-user
      ref="formUser"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    title: "",
    breadcrumbs: [
      { title: "Setting", active: false, to: "" },
      { title: "User", active: true, to: "" },
    ],
    keywordKeys: [
      {
        Id: "UserID",
        Name: "User ID",
      },
      {
        Id: "FullName",
        Name: "Full Name",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "FullName",
      sorts: {
        FullName: "asc",
      },
      sortItems: [
        {
          label: "Full Name",
          value: "FullName",
          selected: true,
          direction: "asc",
        },
        {
          label: "User ID",
          value: "UserID",
          selected: false,
          direction: "asc",
        },
      ],
    },
    idSelected: "",
    modalMode: "",
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useUser();
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
      this.title = "Add User";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-user");
    },
    edit: function (dt) {
      this.title = "Edit User";
      this.modalMode = "edit";
      this.idSelected = dt.UserID;
      this.$bvModal.show("modal-form-user");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.UserID)
              .then((_) => {
                this.ds.load();
                toastSuccess("Data Delete successfully");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.FullName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-user");
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
