<template>
  <v-frame title="Item Packing Supplier" icon="database">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Trade</label>
          <filter-trade-2
            class="form-control"
            v-model="filter.supplier"
            :trade-cls="['2', '3']"
            style-code="width: 120px"
            style-desc="width: 250px"
          />
        </div>
      </div>

      <div class="button-section">
        <div class="d-flex mt-3">
          <div class="d-flex flex-fill">
            <v-button-add :add="add" />
            <v-button-search-reset
              class="ms-1"
              :search="search"
              :reset="reset"
            />
          </div>
        </div>
      </div>
      <hr />
      <v-table
        :filter="filter"
        :export-excel="false"
        :export-excel-action="exportExcel"
        :data-items="ds.data.Items"
        :ds="ds"
        ref="vtable"
        :default-height="265"
        :max-height="265"
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
    size="lg"
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
              .remove(item.SupplierCode, item.ItemCode)
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
        item.ItemName,
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-item-packing-supplier");
      this.ds.load();
    },
  },
};
</script>
<style scoped>
thead {
  white-space: nowrap;
}

.filter-wrapper {
  display: grid;
  grid-template-columns: repeat(2, minmax(320px, 1fr));
  grid-auto-flow: column; /* isi atas ke bawah dulu */
  gap: 4px 20px;
  width: 100%;
  align-items: center;
}

/* jumlah baris otomatis sesuai jumlah item */
.filter-wrapper:has(.filter-item:nth-child(8)) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(7)):not(
    :has(.filter-item:nth-child(8))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(6)):not(
    :has(.filter-item:nth-child(7))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(5)):not(
    :has(.filter-item:nth-child(6))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(4)):not(
    :has(.filter-item:nth-child(5))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(3)):not(
    :has(.filter-item:nth-child(4))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(2)):not(
    :has(.filter-item:nth-child(3))
  ) {
  grid-template-rows: repeat(1, auto);
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
  min-height: 32px;
  width: 100%;
}

.filter-item label {
  width: 70px;
  min-width: 70px;
  white-space: nowrap;
}

/* MOBILE = turun kebawah normal */
@media (max-width: 768px) {
  .filter-wrapper {
    grid-template-columns: 1fr !important;
    grid-template-rows: auto !important;
    grid-auto-flow: row !important;
    gap: 6px;
  }

  .filter-item {
    width: 100%;
  }
}

.button-section {
  /* garis panjang bawah */
  width: 100%;
}
</style>
