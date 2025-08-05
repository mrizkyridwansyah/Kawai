<template>
  <header-menu title="Area" :breadcrumbs="this.breadcrumbs" />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-6 col-md-6 col-sm-6 col-6 mr-1">
        <div class="mr-1" style="width: 100%">
          <input-warehouse class="form-control" v-model="filter.warehouse" />
        </div>
      </div>
      <div class="col-lg-6 col-md-6 col-sm-6 col-6">
        <div class="" style="width: 100%">
          <input-location
            class="form-control"
            v-model="filter.location"
            :warehouse="filter.warehouse"
          />
        </div>
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <v-button-add :add="add" cClass="mr-1" />
      <v-button-print
        :print="print"
        cClass="mr-1"
        :is-loading="isLoadingPrint"
      />
      <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
    </div>
  </div>
  <v-table
    :filter="filter"
    :keyword-keys="keywordKeys"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle" style="width: max-content;"
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
            <td>{{ item.LocationCode }}</td>
            <td>{{ item.LocationName }}</td>
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

  <v-modal
    ref="modalArea"
    id="modal-form-area"
    :title="title"
    size="800"
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
      :location="filter.location"
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
      warehouse: null,
      location: null,
      keyword: null,
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
    "filter.warehouse": function () {
      this.ds.data.Items = [];
    },
    "filter.location": function () {
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
          LocationCode: this.filter.location || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.warehouse = null;
      this.filter.location = null;
      this.search();
    },
    add: function () {
      if (!this.filter.warehouse) {
        toastWarning("Please choose warehouse!");
        return;
      }

      if (!this.filter.location) {
        toastWarning("Please choose location!");
        return;
      }

      this.title = "Add Location";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-area");
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
                toastSuccess("Delete Data Success!");
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

      if (!this.filter.location) {
        toastWarning("Please choose location!");
        return;
      }

      let filters = [
        {
          WarehouseCode: this.filter.warehouse,
          LocationCode: this.filter.location,
        },
      ];
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
