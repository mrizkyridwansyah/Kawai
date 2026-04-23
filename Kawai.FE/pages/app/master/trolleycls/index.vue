<template>
  <v-frame title="Trolley Classification" icon="database">
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
                <th class="text-center">Print</th>
                <th class="text-center">Action</th>
                <th class="text-center">Trolley Cls</th>
                <th class="text-center">Description</th>
                <th class="text-center">Qty</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.Trolley_Cls)"
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
                <td>{{ item.Trolley_Cls }}</td>
                <td>{{ item.Description }}</td>
                <td class="text-right">{{ item.Qty }}</td>
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
    ref="modalTrolleycls"
    id="modal-form-trolleycls"
    :title="title"
    size="md"
    @hidden="
      () => {
        this.$refs.formTrolleycls.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-trolleycls
      ref="formTrolleycls"
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
        Id: "Trolley_Cls",
        Name: "Trolley Cls",
      },
      {
        Id: "Description",
        Name: "Description",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "Trolley_Cls",
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
          label: "Trolley Cls",
          value: "Trolley_Cls",
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
      return useTrolleyCls();
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
      this.title = "Add Trolley Classification";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-trolleycls");
    },
    edit: function (dt) {
      this.title = "Edit Trolley Classification";
      this.modalMode = "edit";
      this.idSelected = dt.Trolley_Cls;
      this.$bvModal.show("modal-form-trolleycls");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.Trolley_Cls)
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
      this.$bvModal.hide("modal-form-trolleycls");
      this.ds.load();
    },
    check: function (checked, item) {
      const existingIndex = this.selectedPrint.findIndex(
        (p) => p.Key === item.Trolley_Cls,
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.Trolley_Cls,
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
