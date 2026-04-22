<template>
  <v-frame title="Stop Point Master" icon="database">
    <template #frame-content>
      <div class="button-section">
        <div class="d-flex">
          <div class="d-flex flex-fill">
            <v-button-add :add="add" cClass="mr-1" />
          </div>
        </div>
      </div>
      <hr>
      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :ds="ds"
        :default-height="300"
        :max-height="300"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle"
            style="min-width: 100%; width: max-content"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Action</th>
                <th class="text-center">Stop Point Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Picking Sequence</th>
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
                <td>{{ item.StopPointCode }}</td>
                <td>{{ item.Description }}</td>
                <td>{{ item.PickingSeq }}</td>
                <td v-if="item.IsActive">Yes</td>
                <td v-else>No</td>
                <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                <td>{{ item.Lastuser }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>

  <v-modal
    ref="modalStopPoint"
    id="modal-form-stoppoint"
    :title="title"
    size="md"
    @hidden="
      () => {
        this.$refs.formStopPoint.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-stoppoint
      ref="formStopPoint"
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
        Id: "StopPointCode",
        Name: "Stop Point Code",
      },
      {
        Id: "Description",
        Name: "Description",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "StopPointCode",
      sorts: {
        Description: "asc",
      },
      sortItems: [
        {
          label: "Description",
          value: "Description",
          selected: false,
          direction: "asc",
        },
        {
          label: "Stop Point Code",
          value: "StopPointCode",
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
      return useStopPoint();
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
      this.title = "Add Stop Point";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-stoppoint");
    },
    edit: function (dt) {
      this.title = "Edit Stop Point";
      this.modalMode = "edit";
      this.idSelected = dt.StopPointCode;
      this.$bvModal.show("modal-form-stoppoint");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.StopPointCode)
              .then((_) => {
                this.ds.load();
                toastSuccess("Data deleted successfully");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.Description,
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-stoppoint");
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
