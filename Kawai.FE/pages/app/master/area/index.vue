<template>
  <v-frame title="Area" icon="database">
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
          <td style="padding-top: 5px;"><label class="form-label">Warehouse</label></td>
          <td style="padding-left: 15px; padding-top: 5px;">
            <filter-warehouse-privileges
              class="form-control"
              v-model="filter.warehouse"
              :factory-code="filter.factory"
              style-code="width: 140px"
              style-desc="width: 240px"
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
                <th class="text-center">Area Code</th>
                <th class="text-center">Area Name</th>
                <th class="text-center">Register Date</th>
                <th class="text-center">Register User</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.AreaCode)"
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
                <td>{{ item.AreaCode }}</td>
                <td>{{ item.AreaName }}</td>
                <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
                <td>{{ item.RegisterUser }}</td>
                <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                <td>{{ item.LastUser }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
  <v-modal
    ref="modalArea"
    id="modal-form-area"
    :title="title"
    size="lg"
    @hidden="
      () => {
        this.$refs.formArea.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-area
      ref="formArea"
      :id="idSelected"
      :mode="modalMode"
      :warehouse="filter.warehouse"
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
      { title: "Area", active: true, to: "/app/master/area" },
    ],
    keywordKeys: [
      {
        Id: "AreaCode",
        Name: "Area Code",
      },
      {
        Id: "AreaName",
        Name: "Area Name",
      },
    ],
    filter: {
      keyword: null,
      factory: 0,
      warehouse: null,
      sorts: {
        AreaName: "asc",
      },
      sortItems: [
        {
          label: "Area Name",
          value: "AreaName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Area Code",
          value: "AreaCode",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
    selectedPrint: [],
    isLoadingPrint: false,
  }),
  computed: {
    ds: function () {
      return useArea();
    },
  },
  watch: {
    "filter.factory": function () {
      this.selectedPrint = [];
      this.ds.data.Items = [];
    },
    "filter.warehouse": function () {
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
          WarehouseCode: this.filter.warehouse || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.warehouse = null;
      this.search();
    },
    add: function () {
      if (this.filter.warehouse) {
        this.title = "Add Area";
        this.modalMode = "add";
        this.idSelected = null; // Reset ID for Add mode
        this.$bvModal.show("modal-form-area");
      } else {
        toastWarning("Please choose warehouse!");
      }
    },
    edit: function (dt) {
      this.title = "Edit Area";
      this.modalMode = "edit";
      this.idSelected = dt.AreaCode;
      this.$bvModal.show("modal-form-area");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.AreaCode)
              .then((_) => {
                this.search();
                toastSuccess("Data deleted successfully!!");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.AreaName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-area");
      this.search();
    },
    exportExcel: function () {
      if (!this.filter.warehouse) {
        toastWarning("Please choose warehouse!");
        return;
      }

      let filters = [{ WarehouseCode: this.filter.warehouse }];
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
        (p) => p.Key === item.AreaCode
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.AreaCode,
          Value: item.AreaName,
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
        toastWarning("Please choose area");
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

<style>
thead {
  white-space: nowrap;
}
</style>
