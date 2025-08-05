<template>
  <header-menu title="BOM" :breadcrumbs="this.breadcrumbs" />
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-1">
        <div class="mr-1" style="width: 100%">
          <input-item-cls class="form-control" v-model="filter.itemCls" />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-1">
        <div class="" style="width: 100%">
          <input-item-type class="form-control" v-model="filter.itemType" />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3 mr-1">
        <div class="" style="width: 100%">
          <input-brand class="form-control" v-model="filter.brand" />
        </div>
      </div>
      <div class="col-lg-3 col-md-3 col-sm-3 col-3">
        <div class="" style="width: 100%">
          <input-item
            class="form-control"
            placeholder="Search Parent Item"
            v-model="filter.parentItem"
            :brand="filter.brand"
            :item-type="filter.itemType"
            :item-cls="filter.itemCls"
          />
        </div>
      </div>
    </div>
  </div>
  <div class="d-flex mt-3">
    <div class="d-flex flex-fill">
      <v-button-add :add="add" cClass="mr-1" />
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
        class="table table-striped mb-0 align-middle"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Action</th>
            <th class="text-center">Item Type</th>
            <th class="text-center">Item Code</th>
            <th class="text-center">Item Name</th>
            <th class="text-center">Qty</th>
            <th class="text-center">Unit</th>
            <th class="text-center">Start Date</th>
            <th class="text-center">End Date</th>
            <th class="text-center">Last Update</th>
            <th class="text-center">Last User</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
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
            <td>{{ item.ItemClsName }}</td>
            <td>{{ item.ChildItemCode }}</td>
            <td>{{ item.ChildItemName }}</td>
            <td class="text-right">{{ $func.formatMoney(item.Qty) }}</td>
            <td>{{ item.ChildUnitClassificationName }}</td>
            <td>{{ $func.formatDate(item.StartDate) }}</td>
            <td>{{ $func.formatDate(item.EndDate) }}</td>
            <td>{{ $func.formatDate(item.LastUpdate) }}</td>
            <td>{{ item.LastUser }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    ref="modalBom"
    id="modal-form-bom"
    :title="title"
    size="800"
    @hidden="
      () => {
        this.$refs.formBOM.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-bom
      ref="formBOM"
      :data-selected="dataSelected"
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
      { title: "BOM", active: true, to: "/app/master/bom" },
    ],
    keywordKeys: [
      {
        Id: "ItemCode",
        Name: "Item Code",
      },
      {
        Id: "ItemName",
        Name: "Item Name",
      },
      {
        Id: "StartDate",
        Name: "Start Date",
      },
      {
        Id: "EndDate",
        Name: "End Date",
      },
    ],
    filter: {
      itemCls: null,
      itemType: null,
      brand: null,
      parentItem: null,
      keyword: null,
      keywordKey: "ItemCode",
      sorts: {
        EndDate: "desc",
        ItemName: "asc",
      },
      sortItems: [
        {
          label: "Item Name",
          value: "ItemName",
          selected: true,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "ItemCode",
          selected: false,
          direction: "desc",
        },
        {
          label: "Start Date",
          value: "StartDate",
          selected: false,
          direction: "asc",
        },
        {
          label: "End Date",
          value: "EndDate",
          selected: true,
          direction: "desc",
        },
      ],
    },
    dataSelected: {
      parentItemCode: "",
      childItemCode: "",
      alternateCode: "",
      startDate: "",
    },
    title: "",
    modalMode: "",
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useBom();
    },
  },
  watch: {
    "filter.itemCls": function () {
      this.ds.data.Items = [];
    },
    "filter.itemType": function () {
      this.ds.data.Items = [];
    },
    "filter.brand": function () {
      this.ds.data.Items = [];
    },
    "filter.parentItem": function () {
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
          ParentItemCode: this.filter.parentItem || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.itemCls = null;
      this.filter.itemType = null;
      this.filter.brand = null;
      this.filter.parentItem = null;
      this.search();
    },
    add: function () {
      if (!this.filter.parentItem) {
        toastWarning("Please choose parent item!");
        return;
      }

      this.title = "Add BOM";
      this.modalMode = "add";
      this.dataSelected = {
        parentItemCode: this.filter.parentItem,
        childItemCode: "",
        startDate: "",
      };
      this.$bvModal.show("modal-form-bom");
    },
    edit: function (dt) {
      this.title = "Edit BOM";
      this.modalMode = "edit";
      this.dataSelected = {
        parentItemCode: this.filter.parentItem,
        childItemCode: dt.ChildItemCode,
        startDate: dt.StartDate,
      };
      this.$bvModal.show("modal-form-bom");
    },
    remove: function (dt) {
      this.dataSelected = {
        parentItemCode: this.filter.parentItem,
        childItemCode: dt.ChildItemCode,
        startDate: dt.StartDate,
      };

      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(this.dataSelected)
              .then((_) => {
                this.dataSelected = {
                  parentItemCode: "",
                  childItemCode: "",
                  startDate: "",
                };
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
        dt.ChildItemName
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-bom");
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
  },
};
</script>
