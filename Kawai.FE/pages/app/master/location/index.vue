<template>
  <header-menu title="Location" :breadcrumbs="this.breadcrumbs" />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-6 col-md-6 col-sm-8 col-8">
        <div class="mr-1" style="width: 100%">
          <input-warehouse class="form-control" v-model="filter.warehouse" />
        </div>
      </div>
      <div class="col-lg-6 col-md-6 col-sm-4 col-4 ml-3">
        <div class="mr-1" style="width: 100%">
          <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
        </div>
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <v-button-add :add="add" cClass="mr-1" />
      <v-button-print :print="print" :is-loading="isLoadingPrint" />
    </div>
  </div>
  <v-table
    :filter="filter"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Print</th>
            <th class="text-center">Action</th>
            <th class="text-center">Warehouse Code</th>
            <th class="text-center">Warehouse Name</th>
            <th class="text-center">Location Code</th>
            <th class="text-center">Location Name</th>
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
                  :modelValue="isChecked(item.LocationCode)"
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
            <td>{{ item.LocationCode }}</td>
            <td>{{ item.LocationName }}</td>
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
    ref="modalLocation"
    id="modal-form-location"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formLocation.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-location
      ref="formLocation"
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
      { title: "Location", active: true, to: "/app/master/location" },
    ],
    keywordKeys: [
      {
        Id: "LocationCode",
        Name: "Location Code",
      },
      {
        Id: "LocationName",
        Name: "Location Name",
      },
    ],
    filter: {
      keyword: null,
      warehouse: null,
      sorts: {
        LocationName: "asc",
      },
      sortItems: [
        {
          label: "Location Name",
          value: "LocationName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Location Code",
          value: "LocationCode",
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
      return useLocation();
    },
  },
  watch: {
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
        this.title = "Add Location";
        this.modalMode = "add";
        this.idSelected = null; // Reset ID for Add mode
        this.$bvModal.show("modal-form-location");
      } else {
        toastWarning("Please choose warehouse!");
      }
    },
    edit: function (dt) {
      this.title = "Edit Location";
      this.modalMode = "edit";
      this.idSelected = dt.LocationCode;
      this.$bvModal.show("modal-form-location");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.LocationCode)
              .then((_) => {
                this.search();
                toastSuccess("Delete Data Success!");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.LocationName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-location");
      this.search();
    },
    exportExcel: function () {
      if (!this.filter.warehouse) {
        toastWarning("Please choose warehouse!");
        return;
      }

      let filters = [{ WarehouseCode: this.filter.warehouse }];
      new Promise((resolve, reject) => {
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
        (p) => p.Key === item.LocationCode
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.LocationCode,
          Value: item.LocationName,
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
        toastWarning("Please choose location");
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
