<template>
  <v-frame title="Manufacture Line" icon="database">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Factory</label>
          <filter-factory-privileges
            class="form-control"
            v-model="filter.factory"
            style-code="width: 140px"
            style-desc="width: 240px"
          />
        </div>
      </div>
      <div class="button-section">
        <div class="d-flex mt-3">
          <div class="d-flex flex-fill">
             
            <v-button-search-reset
              class="ml-1"
              :search="search"
              :reset="reset"
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
        :top-content-height="250"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            style="min-width: 100%; width: max-content; max-height: 10%"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Action</th>
                <th class="text-center">Line Code</th>
                <th class="text-center">Line Name</th>
                <th class="text-center">IP Printer</th>
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
                    @click="menuPrivAllowUpdate && edit(item)"
                  />
                  
                </td>
                <td>{{ item.LineCode }}</td>
                <td>{{ item.LineName }}</td>
                <td>{{ item.IPPrinter }}</td>
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
    ref="modalManufactureLine"
    id="modal-form-manufactureline"
    :title="title"
    size="lg"
    @hidden="
      () => {
        this.$refs.formManufactureLine.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-manufactureline
      ref="formManufactureLine"
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
    keywordKeys: [
      {
        Id: "LineCode",
        Name: "Line Code",
      },
      {
        Id: "LineName",
        Name: "Line Name",
      },
    ],
    filter: {
      keyword: null,
      keywordKey: "LineCode",
      factory: null,
      sorts: {
        LineName: "asc",
      },
      sortItems: [
        {
          label: "Line Name",
          value: "LineName",
          selected: false,
          direction: "asc",
        },
        {
          label: "Line Code",
          value: "LineCode",
          selected: true,
          direction: "asc",
        },
      ],
    },
    idSelected: "",
    title: "",
    modalMode: "",
    debounce: null,
    selectedPrint: [],
    menuPrivAllowUpdate: false,
    isLoadingPrint: false,
  }),
  computed: {
    ds: function () {
      return useManufactureLine();
    },
    dsMenu: function () {
      return useMenu();
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
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "A25",
      )[0].AllowUpdate;
    });
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
  
    edit: function (dt) {
      this.title = "Edit Manufacture Line";
      this.modalMode = "edit";
      this.idSelected = dt.LineCode;
      this.$bvModal.show("modal-form-manufactureline");
    },
 
    close: function () {
      this.$bvModal.hide("modal-form-manufactureline");
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
  width: 100%;
}
</style>
