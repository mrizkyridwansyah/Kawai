<template>
  <v-frame title="BOM Detail Item Setting" icon="database">
    <template #frame-content>
      <div class="row">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Model Cls</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-12">
          <filter-cls-2
            class="form-control"
            type-data="Model_Cls"
            v-model="filter.modelcls"
            style-code="width: 120px"
            style-desc="width: 250px"
            :disabled="true"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Item</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-12">
          <filter-item-by-modelcls
            class="form-control"
            v-model="filter.item"
            :disabled="true"
            :modelCls="filter.modelcls"
            style-code="width: 120px"
            style-desc="width: 250px"
          />
        </div>
      </div>
      <div class="row mt-1 mb-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Workstation</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-12">
          <filter-workstation
            class="form-control"
            :disabled="true"
            v-model="filter.workstation"
            style-code="width: 120px"
            style-desc="width: 250px"
          />
        </div>
      </div>
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
        @click="() => this.$router.push('/app/master/bom-workstation')"
      >
        <font-awesome-icon icon="arrow-left" />
        <span class="ml-2">Back</span>
      </button>
      <div class="row mt-4">
        <div class="col-lg-12 col-md-12 col-sm-12 col-12">
          <!-- END nav-tabs -->
          <!-- BEGIN tab-content -->
          <div class="tab-content panel rounded-0 p-3 m-0">
            <!-- BEGIN tab-pane MENU -->
            <div class="table-scroll">
              <table
                class="table table-striped table-bordered mb-0 align-middle sticky-header-table"
              >
                <thead>
                  <tr>
                    <th class="text-center" style="vertical-align: middle">
                      Setting
                      <!-- <span>Setting</span>
                      <div class="mt-1" style="justify-items: center">
                        <input-checkbox @click="(e) => allowSetting(e)" /> -->
                      <!-- </div> -->
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
                  <tr v-for="(item, idx) in allowed.bomsetting">
                    <td>
                      <div style="justify-items: center">
                        <input-checkbox
                          v-model="item.AllowSetting"
                          @click="(e) => allowSettingBOM(e, item)"
                        />
                      </div>
                    </td>
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
          <!-- BEGIN hljs-wrapper -->
          <div class="hljs-wrapper rounded-0 rounded-bottom mb-4">
            <pre><code class="html hljs language-xml" data-url="../assets/data/ui-tabs-accordions/code-1.json" data-highlighted="yes"></code></pre>
          </div>
          <!-- END hljs-wrapper -->
        </div>
      </div>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    filter: {
      modelcls: null,
      workstation: null,
      item: null,
      keyword: null,
    },
    isLoading: false,
    allowed: {
      bomsetting: [],
    },
  }),
  computed: {
    dsBOMSetting: function () {
      return useBOMWorkstationDetail();
    },
  },
  mounted: function () {
    this.filter.modelcls = this.$route.query.modelcls;
    this.filter.item = this.$route.query.itemcode;
    this.filter.workstation = this.$route.query.workstationcode;

    this.dsBOMSetting
      .load(this.$route.query.itemcode, this.$route.query.workstationcode)
      .then((dt) => {
        this.allowed.bomsetting = dt.Data.BomSetting;
      });
  },
  methods: {
    allowSettingBOM: function (e, item) {
      this.allowed.bomsetting.find(
        (p) => p.ChildItem_Code === item.ChildItem_Code,
      ).AllowSetting = e.target.checked;
    },
    submit: function () {
      // cek apakah ada yg dicentang
      const hasChecked = this.allowed.bomsetting.some(
        (item) => item.AllowSetting === true,
      );

      if (!hasChecked) {
        toastWarning("Please select child item setting (minimal 1 data)!");
        return;
      }

      this.isLoading = true;

      let model = {
        ParentItem_Code: this.$route.query.itemcode,
        WorkStationCode: this.$route.query.workstationcode,
        BomSetting: this.allowed.bomsetting,
      };

      this.dsBOMSetting
        .submit(model)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() =>
          setTimeout(() => {
            this.isLoading = false;
          }, 500),
        );
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
</style>
