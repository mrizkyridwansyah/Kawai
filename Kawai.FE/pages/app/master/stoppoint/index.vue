<template>
  <v-frame title="Stop Point Master" icon="database">
    <template #frame-content>
      <div class="button-section">
        <div class="d-flex">
          <div class="d-flex flex-fill">
            <v-button-add :add="add" cClass="mr-1" />
            <v-button-print
              :print="print"
              cClass="ml-1"
              :is-loading="isLoadingPrint"
            />
            <v-button-print
              :print="printall"
              label="Print All"
              cClass="ml-1"
              :is-loading="isLoadingPrint"
            />
          </div>
        </div>
      </div>
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
                <th class="text-center">Print</th>
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
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.StopPointCode)"
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
    selectedPrint: [],
    isLoadingPrint: false,
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
    isChecked: function (code) {
      return this.selectedPrint.some((p) => p.Key === code);
    },
    check: function (checked, item) {
      const existingIndex = this.selectedPrint.findIndex(
        (p) => p.Key === item.StopPointCode,
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.StopPointCode,
          Value: item.Description,
        });
      } else if (!checked && existingIndex !== -1) {
        this.selectedPrint.splice(existingIndex, 1);
      }
    },
    print: function () {
      this.isLoadingPrint = true;
      if (this.selectedPrint.length === 0) {
        toastWarning("Please choose stop points");
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

    printall: function () {
      this.isLoadingPrint = true;
      new Promise((resolve, reject) => {
        this.ds
          .exportQRALL()
          .then((_) => {
            resolve();
          })

          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          })
          .finally(() => {
            setTimeout(() => {
              this.isLoadingPrint = false;
            }, 500);
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
