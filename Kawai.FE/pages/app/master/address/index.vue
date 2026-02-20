<template>
  <v-frame title="Address" icon="database">
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
          <td style="padding-top: 5px">
            <label class="form-label">Warehouse</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
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
          <td style="padding-top: 5px">
            <label class="form-label">Area</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <filter-area-privileges
              class="form-control"
              v-model="filter.area"
              :warehouse="filter.warehouse"
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
               <v-button
                :action="setting"
                label="Setting Stop Point"
                icon="file-pdf"
                cClass="ml-1 btn-green"
                :is-loading="isLoading"
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
                <th class="text-center">Setting Stop Point</th>
                <th class="text-center">Warehouse Code</th>
                <th class="text-center">Warehouse Name</th>
                <th class="text-center">Area Code</th>
                <th class="text-center">Area Name</th>
                <th class="text-center">Address Code</th>
                <th class="text-center">Address Name</th>
                <th class="text-center">Stop Point</th>
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
                      :modelValue="isChecked(item.AddressCode)"
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
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isSet(item.AddressCode)"
                      @update:modelValue="(checked) => set(checked, item)"
                    />
                  </div>
                </td>
                <td>{{ item.WarehouseCode }}</td>
                <td>{{ item.WarehouseName }}</td>
                <td>{{ item.AreaCode }}</td>
                <td>{{ item.AreaName }}</td>
                <td>{{ item.AddressCode }}</td>
                <td>{{ item.AddressName }}</td>
                <td>{{ item.StopPointDescs }}</td>
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
    ref="modalSettingStopPoint"
    id="modal-form-settingstoppoint"
    :title="title"
   size="md"
    @hidden="
      () => {
        this.$refs.formSettingStopPoint.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-settingstoppoint
      ref="formSettingStopPoint"
      :id="idSelected"
      :mode="modalMode"
      :data-setting="dataSetting"
      :warehouse="filter.warehouse"
      :area="filter.area"
      @submitted="closestop"
    />
  </v-modal>

    <v-modal
    ref="modalAddress"
    id="modal-form-address"
    :title="title"
    size="lg"
    @hidden="
      () => {
        this.$refs.formAddress.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-address
      ref="formAddress"
      :id="idSelected"
      :mode="modalMode"
      :warehouse="filter.warehouse"
      :area="filter.area"
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
      { title: "Address", active: true, to: "/app/master/address" },
    ],
    keywordKeys: [
      {
        Id: "AddressCode",
        Name: "Address Code",
      },
      {
        Id: "AddressName",
        Name: "Address Name",
      },
    ],
    filter: {
      factory: null,
      warehouse: null,
      area: null,
      address: null,
      keyword: null,
      sorts: {
        AddressName: "asc",
      },
      sortItems: [
        {
          label: "Address Name",
          value: "AddressName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Address Code",
          value: "AddressCode",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
    selectedPrint: [],
    selectedSet: [],
    isLoadingPrint: false,
  }),
  computed: {
    ds: function () {
      return useAddress();
    },
  },
  watch: {
    "filter.factory": function () {
      this.ds.data.Items = [];
    },
    "filter.warehouse": function () {
      this.ds.data.Items = [];
    },
    "filter.area": function () {
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
          AreaCode: this.filter.area || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.warehouse = null;
      this.filter.area = null;
      this.search();
    },
    setting: function () {
      if (!this.filter.warehouse) {
        toastWarning("Please choose warehouse!");
        return;
      }

      if (!this.filter.area) {
        toastWarning("Please choose area!");
        return;
      }
 
      this.title = "Setting Stop Point";
      this.modalMode = "add";
      this.dataSetting = [...this.selectedSet];
      this.$bvModal.show("modal-form-settingstoppoint");
    },

  add: function () {
      if (!this.filter.warehouse) {
        toastWarning("Please choose warehouse!");
        return;
      }

      if (!this.filter.area) {
        toastWarning("Please choose area!");
        return;
      }

      this.title = "Add Address";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-address");
    },
    edit: function (dt) {
      this.title = "Edit Address";
      this.modalMode = "edit";
      this.idSelected = dt.AddressCode;
      this.$bvModal.show("modal-form-address");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.AddressCode)
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
        item.AddressName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-address");
      this.search();
    },

    closestop: function () {
      this.$bvModal.hide("modal-form-settingstoppoint");
      this.selectedSet = [];
       this.search();
     
    },
    exportExcel: function () {
      if (!this.filter.warehouse) {
        toastWarning("Please choose warehouse!");
        return;
      } 

      if (!this.filter.area) {
        toastWarning("Please choose area!");
        return;
      }

      let filters = [
        {
          WarehouseCode: this.filter.warehouse,
          AreaCode: this.filter.area,
        },
      ];

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
        (p) => p.Key === item.AddressCode
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.AddressCode,
          Value: item.AddressName,
        });
      } else if (!checked && existingIndex !== -1) {
        this.selectedPrint.splice(existingIndex, 1);
      }
    },
    set: function (checked, item) {
      const existingIndex = this.selectedSet.findIndex(
        (p) => p.Key === item.AddressCode
      );
      if (checked && existingIndex === -1) {
        this.selectedSet.push({
          Key: item.AddressCode,
        });
      } else if (!checked && existingIndex !== -1) {
        this.selectedSet.splice(existingIndex, 1);
      }
    },
    isChecked: function (code) {
      return this.selectedPrint.some((p) => p.Key === code);
    },
     isSet: function (code) {
      return this.selectedSet.some((p) => p.Key === code);
    },
    print: function () {
      this.isLoadingPrint = true;
      if (this.selectedPrint.length === 0) {
        toastWarning("Please choose address");
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
