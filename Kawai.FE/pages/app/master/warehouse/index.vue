<template>
  <v-frame title="Warehouse" icon="database">
    <template #frame-content>
      <table class="ml-2">
        <tr>
          <td><label class="form-label">Factory</label></td>
          <td style="padding-left: 15px">
            <filter-factory-privileges
              class="form-control"
              v-model="filter.factory"
              style-code="width: 110px"
              style-desc="width: 250px"
            />
          </td>
        </tr>
        <tr>
          <td colspan="2">
            <div class="d-flex flex-fill mt-1">
              <v-button-add :add="add" cClass="mr-1" />
              <v-button-print
                :print="print"
                cClass=""
                :is-loading="isLoadingPrint"
              />
              <v-button-search-reset
                class="ms-1"
                :search="search"
                :reset="reset"
              />
            </div>
          </td>
        </tr>
      </table>
      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :ds="ds"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle"
            style="min-width: 100%; width: max-content"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Print</th>
                <th class="text-center">Action</th>
                <th class="text-center">Warehouse Code</th>
                <th class="text-center">Warehouse Name</th>
                <th class="text-center">Adm Group</th>
                <th class="text-center">Adm Name</th>
                <th class="text-center">Stock Cls</th>
                <th class="text-center">NG Cls</th>
                <th class="text-center">Use End Date</th>
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
                <td>{{ item.AdmGroup }}</td>
                <td>{{ item.AdmGroupName }}</td>
                <td v-if="item.StockControlCls == '01'">YES</td>
                <td v-else-if="item.StockControlCls == '02'">NO</td>
                <td v-if="item.NGCls == '01'">YES</td>
                <td v-else-if="item.NGCls == '02'">NO</td>
                <td>{{ $func.formatDate(item.UseEndDate) }}</td>
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
    ref="modalWarehouse"
    id="modal-form-warehouse"
    :title="title"
    size="lg"
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
      :factory="filter.factory"
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
      factory: null,
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
    "filter.factory": function () {
      this.selectedPrint = [];
      this.ds.data.Items = [];
    },
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          FactoryCode: this.filter.factory || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.factory = null;
      this.search();
    },
    add: function () {
      if (this.filter.factory) {
        this.title = "Add Warehouse";
        this.modalMode = "add";
        this.idSelected = null; // Reset ID for Add mode
        this.$bvModal.show("modal-form-warehouse");
      } else {
        toastWarning("Please choose factory!");
      }
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
                toastSuccess("Data Deleted successfully!");
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
      if (!this.filter.factory) {
        toastWarning("Please choose factory!");
        return;
      }

      let filters = [{ FactoryCode: this.filter.factory }];
      return new Promise((resolve, reject) => {
        this.ds
          .exportExcel(filters)
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
              this.isLoadingPrint = false;
            }, 1000);
          });
      });
    },
  },
};
</script>
