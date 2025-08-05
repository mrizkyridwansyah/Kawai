<template>
  <header-menu title="Warehouse" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />
  <v-button-print :print="print" :is-loading="isLoadingPrint" />
  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle" style="min-width: 100%; width: max-content;"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Print</th>
            <th class="text-center">Action</th>
            <th class="text-center">Warehouse Code</th>
            <th class="text-center">Warehouse Name</th>
            <th class="text-center">Last Update</th>
            <th class="text-center">Last User</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
            <td>
              <div style="justify-items: center">
                <input-checkbox
                  :modelValue="isChecked(item.WarehouseCode)"
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
            <td>{{ item.WarehouseCode }}</td>
            <td>{{ item.WarehouseName }}</td>
            <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
            <td>{{ item.LastUser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    ref="modalWarehouse"
    id="modal-form-warehouse"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formWarehouse.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-warehouse
      ref="formWarehouse"
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
      { title: "Warehouse", active: true, to: "/app/master/warehouse" },
    ],
    keywordKeys: [
      {
        Id: "WarehouseCode",
        Name: "Warehouse Code",
      },
      {
        Id: "WarehouseName",
        Name: "Warehouse Name",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "WarehouseCode",
      sorts: {
        WarehouseName: "asc",
      },
      sortItems: [
        {
          label: "Warehouse Name",
          value: "WarehouseName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Warehouse Code",
          value: "WarehouseCode",
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
      return useWarehouse();
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
      this.title = "Add Warehouse";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-warehouse");
    },
    edit: function (dt) {
      this.title = "Edit Warehouse";
      this.modalMode = "edit";
      this.idSelected = dt.WarehouseCode;
      this.$bvModal.show("modal-form-warehouse");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.WarehouseCode)
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
        item.WarehouseName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-warehouse");
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
    check: function (checked, item) {
      const existingIndex = this.selectedPrint.findIndex(
        (p) => p.Key === item.WarehouseCode
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.WarehouseCode,
          Value: item.WarehouseName,
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
        toastWarning("Please choose warehouse");
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
              
              (this.isLoadingPrint = false)
            }, 1000);
          });
      });
    },
  },
};
</script>
