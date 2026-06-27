<template>
  <v-frame title="Model Classification" icon="database">
    <template #frame-content>
    

      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :ds="ds"
        :top-content-height="200"
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
                <th class="text-center">Model Cls</th>
                <th class="text-center">Description</th>
                <th class="text-center">Cycle Time (menit/PCS)</th>
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
                <td>{{ item.Model_Cls }}</td>
                <td>{{ item.Description }}</td>
                <td class="text-right">{{ item.CycleTime }}</td>
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
    ref="modalModelcls"
    id="modal-form-modelcls"
    :title="title"
    size="md"
    @hidden="
      () => {
        this.$refs.formModelcls.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-modelcls
      ref="formModelcls"
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
        Id: "Model_Cls",
        Name: "Model Cls",
      },
      {
        Id: "Description",
        Name: "Description",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "Model_Cls",
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
          label: "Model Cls",
          value: "Model_Cls",
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
      return useModelCls();
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
  
    edit: function (dt) {
      this.title = "Edit Model Classification";
      this.modalMode = "edit";
      this.idSelected = dt.Model_Cls;
      this.$bvModal.show("modal-form-modelcls");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.Model_Cls)
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
      this.$bvModal.hide("modal-form-modelcls");
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
