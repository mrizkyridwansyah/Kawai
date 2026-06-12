<template>
  <v-frame title="BOM Detail Item Setting" icon="database">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Factory</label>
          <filter-factory-privileges
            class="form-control"
            v-model="filter.factory"
            style-code="width: 110px"
            style-desc="width: 250px"
          />
        </div>

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Process</label>
          <filter-trade-2
            class="form-control"
            placeholder=" "
            v-model="filter.supplier"
            :trade-cls="['1']"
            style-code="width: 110px"
            style-desc="width: 250px"
            :disabled="true"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Line</label>
          <filter-line-factory
            class="form-control"
            :company="filter.factory"
            :manufacture="filter.supplier"
            v-model="filter.linecode"
            style-code="width: 110px"
            style-desc="width: 250px"
            :disabled="true"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Model Cls</label>
          <filter-cls-2
            type-data="Model_Cls"
            class="form-control"
            v-model="filter.modelcls"
            style-code="width: 110px"
            style-desc="width: 250px"
            :disabled="true"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Item</label>
          <filter-item-by-modelcls
            class="form-control"
            v-model="filter.item"
            :modelCls="filter.modelcls"
            style-code="width: 150px"
            style-desc="width: 210px"
            :disabled="true"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">WorkStation</label>
          <filter-workstation
            class="form-control"
            :disabled="true"
            v-model="filter.workstation"
            style-code="width: 110px"
            style-desc="width: 250px"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Trolley Cls</label>
          <filter-cls-2
            class="form-control"
            type-data="Trolley_Cls"
            v-model="filter.trolley_cls"
            :errors="errors?.trolley_cls"
            style-code="width: 150px"
            style-desc="width: 210px"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Max Qty Set</label>
          <input-money
            placeholder="Qty "
            v-model="model.QtySet"
            style="width: 110px"
          />
        </div>
      </div>

      <div class="button-section">
        <div class="d-flex">
          <div class="d-flex flex-fill">
            <button
              class="btn btn-sm btn-primary btn-elevate mt-2"
              @click="submit"
              :disabled="isLoading"
            >
              <div
                class="spinner-border spinner-border-sm text-light"
                role="status"
                v-if="isLoading"
              >
                <span class="visually-hidden">Loading...</span>
              </div>
              <font-awesome-icon icon="save" v-else />
              <span class="ml-2">Save</span>
            </button>
            <button
              class="mt-2 ml-2 btn btn-sm btn-danger btn-elevate"
              :disabled="isLoading"
              @click="
                () =>
                  this.$router.push({
                    path: '/app/master/bom-workstation',
                    query: {
                      factory: filter.factory,
                      process: filter.supplier,
                      line: filter.linecode,
                      modelcls: filter.modelcls,
                      itemcode: filter.item,
                    },
                  })
              "
            >
              <font-awesome-icon icon="arrow-left" />
              <span class="ml-2">Back</span>
            </button>
          </div>
        </div>
      </div>
      <hr>
      <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 col-12">
          <!-- END nav-tabs -->
          <!-- BEGIN tab-content -->
          <div class="tab-content panel rounded-0 p-3 m-0">
            <!-- BEGIN tab-pane MENU -->
            <div
              class="d-flex align-items-center mb-2 p-2"
              style="gap: 10px; background: #2f2f2f; border-radius: 6px"
            >
              <!-- SORT BUTTON -->

              <!-- SEARCH INPUT -->
              <div style="flex: 1">
                <input
                  type="text"
                  class="form-control form-control-sm"
                  v-model="filter.keyword"
                  placeholder="Search..."
                  style="background: #f1f1f1"
                />
              </div>
            </div>
            <div class="table-scroll" style="max-height: calc(100vh - 435px);">
              <table
                class="table table-striped table-bordered mb-0 align-middle sticky-header-table"
              >
                <thead>
                  <tr>
                    <th class="text-center" style="vertical-align: middle">
                      Setting
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Type Material
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Child Item
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Description
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Qty
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Register Date
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Register User
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Last Update
                    </th>
                    <th class="text-center" style="vertical-align: middle">
                      Last User
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="(item, idx) in filteredBomSetting"
                    :key="item.ChildItem_Code"
                  >
                    <td>
                      <div style="justify-items: center">
                        <input-checkbox
                          v-model="item.AllowSetting"
                          @click="(e) => allowSettingBOM(e, item)"
                        />
                      </div>
                    </td>
                    <td>{{ item.TypeMaterial }}</td>
                    <td>{{ item.ChildItem_Code }}</td>
                    <td>{{ item.ChildItem_Name }}</td>
                    <td>
                      <input-money v-model="item.Qty" />
                    </td>
                    <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
                    <td>{{ item.RegisterUser }}</td>
                    <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                    <td>{{ item.LastUser }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
            <!-- END tab-pane -->
          </div>
          <!-- END tab-content -->
        </div>
      </div>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    errors: {},
    headerTrolley: null,
    filter: {
      factory: null,
      process: null,
      line: null,
      modelcls: null,
      workstation: null,
      trolley_cls: "",
      item: null,
      keyword: null,
    },
    model: {
      QtySet: 0,
    },
    isLoading: false,
    allowed: {
      bomsetting: [],
    },
  }),
  computed: {
    dsBOMSetting() {
      return useBOMWorkstationDetail();
    },

    filteredBomSetting() {
      if (!this.filter.keyword) {
        return this.allowed.bomsetting;
      }

      const keyword = this.filter.keyword.toLowerCase();

      return this.allowed.bomsetting.filter(
        (x) =>
          (x.ChildItem_Code ?? "").toLowerCase().includes(keyword) ||
          (x.ChildItem_Name ?? "").toLowerCase().includes(keyword) ||
          (x.TypeMaterial ?? "").toLowerCase().includes(keyword),
      );
    },
  },
  mounted: function () {
    this.filter.factory = this.$route.query.factory;
    this.filter.supplier = this.$route.query.process;
    this.filter.linecode = this.$route.query.line;
    this.filter.modelcls = this.$route.query.modelcls;
    this.filter.item = this.$route.query.itemcode;
    this.filter.workstation = this.$route.query.workstationcode;

    this.loadData();
  },
  watch: {
    "filter.trolley_cls"(val) {
      if (!val) return;

      // jika masih trolley dari header → jangan load API
      if (val === this.headerTrolley) return;

      this.dsBOMSetting.loadQty(val).then((res) => {
        const qty = res.Data?.MaxQtySet ?? 0;

        this.model.QtySet = qty;
      });
    },
  },
  methods: {
    loadMaxQty(trolleyCls) {
      this.dsBOMSetting
        .loadQty(trolleyCls)
        .then((res) => {
          const qty = res.Data?.MaxQtySet ?? 0;

          // isi ke textbox Max Qty Set
          this.model.QtySet = qty;
        })
        .catch(() => {
          toastDanger("Failed load Max Qty");
        });
    },
    loadData() {
      return this.dsBOMSetting
        .load(this.filter.linecode, this.filter.item, this.filter.workstation)
        .then((dt) => {
          this.allowed.bomsetting = dt.Data.BomSetting ?? [];

          const header = dt.Data.Header?.[0]?.[0];

          if (header) {
            this.model.QtySet = header.QtySet; // 200
            this.filter.trolley_cls = header.Trolley_Cls;

            this.headerTrolley = header.Trolley_Cls; // simpan trolley awal
          }
        });
    },
    // loadData() {
    //   return this.dsBOMSetting
    //     .load(this.filter.linecode, this.filter.item, this.filter.workstation)
    //     .then((dt) => {
    //       // DETAIL
    //       this.allowed.bomsetting = dt.Data.BomSetting ?? [];

    //       // HEADER
    //       const header = dt.Data.Header?.[0]?.[0];
    //       if (header) {
    //         this.model.QtySet = header.QtySet;
    //         this.filter.trolley_cls = header.Trolley_Cls;
    //       }
    //     });
    // },

    allowSettingBOM(e, item) {
      this.allowed.bomsetting.find(
        (p) => p.ChildItem_Code === item.ChildItem_Code,
      ).AllowSetting = e.target.checked;
    },

    submit() {
      if (!this.filter.trolley_cls) {
        toastWarning("Please Select Trolley Cls");
        this.isLoadingPrint = false;
        return;
      }
      if (!this.model.QtySet || this.model.QtySet <= 0) {
        toastWarning("Max Qty Set harus lebih besar dari 0");
        return;
      }
      const details = this.allowed.bomsetting.filter(
        (x) => x.AllowSetting === true,
      );

      // if (details.length === 0) {
      //   toastWarning("Please select child item setting (minimal 1 data)!");
      //   return;
      // }

      this.isLoading = true;

      const payload = {
        Header: [
          {
            FactoryCode: this.filter.factory,
            ProcessCode: this.filter.supplier,
            LineCode: this.filter.linecode,
            ModelCls: this.filter.modelcls,
            ParentItem_Code: this.filter.item,
            WorkStationCode: this.filter.workstation,
            Trolley_Cls: this.filter.trolley_cls,
            QtySet: this.model.QtySet,
          },
        ],
        Details: details.map((x) => ({
          ChildItem_Code: x.ChildItem_Code,
          Qty: x.Qty,
          AllowSetting: x.AllowSetting,
        })),
      };

      this.dsBOMSetting
        .submit(payload)
        .then(() => {
          toastSuccess("Data saved successfully!");
          return this.loadData();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
  },
};
</script>

<style scoped>
thead {
  background-color: #8ec5fc;
}
.nav-tabs .nav-link.active {
  background-color: #007bff !important; /* Ganti dengan warna yang kamu mau */
  color: white !important; /* Warna teks di tab aktif */
  border-color: #007bff #007bff #fff; /* Biar matching */
}

.table-scroll {
  max-height: 450px; /* atur tinggi sesuai kebutuhan */
  overflow-y: auto;
  border: 1px solid #dee2e6;
}

/* penting agar sticky bekerja */
.sticky-header-table thead th {
  position: sticky;
  top: 0;
  background: #8ec5fc; /* warna header kamu */
  z-index: 10; /* supaya tidak tertutup row */
}

/* ini stabil untuk Bootstrap table */
.sticky-header-table {
  border-collapse: separate;
  border-spacing: 0;
}
/* thead tr th {
  color: white !important;
} */

/* GANTI CSS .filter-wrapper lama dengan ini */

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
