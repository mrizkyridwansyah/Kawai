<template>
  <v-frame title="Item Packing Supplier" icon="database">
    <template #frame-content>
      <div class="d-flex">
        <div class="d-flex flex-fill">
          <div class="col-xl-6 col-lg-8 col-md-8 col-sm-8 col-8">
            <div class="mr-1" style="width: 100%">
              <label class="form-label">Trade</label>
              <input-trade class="form-control" v-model="filter.supplier" />
            </div>
          </div>
          <div class="col-xl-6 col-lg-4 col-md-4 col-sm-4 col-4 ml-3">
            <label class="form-label">&nbsp;</label>
            <div class="mr-1" style="width: 100%">
              <v-button-search-reset
                class="ms-1"
                :search="search"
                :reset="reset"
              />
            </div>
          </div>
        </div>
      </div>
      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-add :add="add" cClass="mr-1" />
        </div>
      </div>
      <v-table
        :filter="filter"
        :export-excel="false"
        :export-excel-action="exportExcel"
        :data-items="ds.data.Items"
        :ds="ds"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Action</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Qty Packing</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items" :key="idx">
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
                <td>{{ item.ItemCode }}</td>
                <td>{{ item.ItemName }}</td>
                <td class="text-right">
                  {{ $func.formatMoney(item.QtyPacking) }}
                </td>
                <td>{{ item.UnitClsName }}</td>
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
    ref="modalItemPackingSupplier"
    id="modal-form-item-packing-supplier"
    :title="title"
    size="md"
    @hidden="
      () => {
        this.$refs.formItemPackingSupplier.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-item-packing-supplier
      ref="formItemPackingSupplier"
      :id="idSelected"
      :mode="modalMode"
      :supplier="filter.supplier"
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
      {
        title: "Item Packing Supplier",
        active: true,
        to: "/app/master/item-packing-supplier",
      },
    ],
    filter: {
      keyword: null,
      supplier: null,
      sorts: {
        ItemName: "asc",
      },
      sortItems: [
        {
          label: "Item Name",
          value: "ItemName",
          selected: true,
          direction: "asc",
        },
      ],
    },
    title: "",
    modalMode: "",
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useItemPackingSupplier();
    },
  },
  watch: {
    "filter.supplier": function () {
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
          SupplierCode: this.filter.supplier || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.supplier = null;
      this.search();
    },
    add: function () {
      if (this.filter.supplier) {
        this.title = "Add Item Packing Supplier";
        this.modalMode = "add";
        this.idSelected = null; // Reset ID for Add mode
        this.$bvModal.show("modal-form-item-packing-supplier");
      } else {
        toastWarning("Please choose supplier!");
      }
    },
    edit: function (dt) {
      this.title = "Edit Item Packing Supplier";
      this.modalMode = "edit";
      this.idSelected = dt.ItemCode;
      this.$bvModal.show("modal-form-item-packing-supplier");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.ItemCode)
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
        item.ItemName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-item-packing-supplier");
      this.ds.load();
    },
  },
};
</script>
