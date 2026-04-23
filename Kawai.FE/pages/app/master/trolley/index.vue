<template>
  <v-frame title="Trolley Master" icon="database">
    <template #frame-content>
      <div class="button-section">
        <div class="d-flex">
          <div class="d-flex flex-fill">
            <v-button-add :add="add" cClass="mr-1" />
            <v-button-print
              :print="print"
              cClass=""
              :is-loading="isLoadingPrint"
            />
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
                <th class="text-center">Print</th>
                <th class="text-center">Action</th>
                <th class="text-center">Trolley Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Type</th>
                <th class="text-center">Active Status</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.TrolleyCode)"
                      @update:modelValue="(checked) => check(checked, item)"
                    />
                  </div>
                </td>
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
                <td>{{ item.TrolleyCode }}</td>
                <td>{{ item.Description }}</td>
                <td>{{ item.Trolley_ClsDescs }}</td>
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
    ref="modalTrolley"
    id="modal-form-trolley"
    :title="title"
    size="md"
    @hidden="
      () => {
        this.$refs.formTrolley.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-trolley
      ref="formTrolley"
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
      { title: "Master Trolley", active: true, to: "/master/trolley" },
    ],
    keywordKeys: [
      {
        Id: "TrolleyCode",
        Name: "Trolley Code",
      },
      {
        Id: "Description",
        Name: "Description",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "TrolleyCode",
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
          label: "Trolley Code",
          value: "TrolleyCode",
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
      return useTrolley();
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
      this.title = "Add Trolley";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-trolley");
    },
    edit: function (dt) {
      this.title = "Edit Trolley";
      this.modalMode = "edit";
      this.idSelected = dt.TrolleyCode;
      this.$bvModal.show("modal-form-trolley");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.TrolleyCode)
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
      this.$bvModal.hide("modal-form-trolley");
      this.ds.load();
    },
    check: function (checked, item) {
      const existingIndex = this.selectedPrint.findIndex(
        (p) => p.Key === item.TrolleyCode,
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.TrolleyCode,
          Value: item.Description,
        });
      } else if (!checked && existingIndex !== -1) {
        this.selectedPrint.splice(existingIndex, 1);
      }
    },
    isChecked: function (code) {
      return this.selectedPrint.some((p) => p.Key === code);
    },
    print: function () {
      this.isLoadingPrint = true;
      if (this.selectedPrint.length === 0) {
        toastWarning("Please choose trolley");
        this.isLoadingPrint = false;
        return;
      }

      new Promise((resolve, reject) => {
        this.ds
          .exportQR(this.selectedPrint)
          .then((_) => {
            this.selectedPrint = [];
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          })
          .finally(() => {
            setTimeout(() => {
              this.isLoadingPrint = false;
            }, 1000);
          });
      });
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
