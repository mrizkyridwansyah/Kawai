<template>
  <v-frame title="User Privileges" icon="users">
    <template #frame-content>
      <button
        class="btn btn-sm btn-primary btn-elevate"
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
        class="ml-2 btn btn-sm btn-danger btn-elevate"
        :disabled="isLoading"
        @click="() => this.$router.push('/app/setting/user')"
      >
        <font-awesome-icon icon="arrow-left" />
        <span class="ml-2">Back</span>
      </button>
      <hr>
      <div class="row mt-2" style="min-height: calc(100vh - 210px)">
        <div class="col-lg-12 col-md-12 col-sm-12 col-12">
          <!-- BEGIN nav-tabs -->
          <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item" role="presentation" v-for="(li, idx) in list">
              <a
                :href="`#default-tab-${idx + 1}`"
                data-bs-toggle="tab"
                class="nav-link"
                :class="{
                  active: li.defaultActive,
                }"
                aria-selected="false"
                role="tab"
                tabindex="-1"
              >
                <span class="d-sm-block d-none">{{ li.text }}</span>
              </a>
            </li>
          </ul>
          <!-- END nav-tabs -->
          <!-- BEGIN tab-content -->
          <div class="tab-content panel rounded-0 p-3 m-0">
            <!-- BEGIN tab-pane MENU -->
            <div
              class="tab-pane fade active show"
              id="default-tab-1"
              role="tabpanel"
            >
              <div class="v-table-wrapper">
                <table
                  class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                >
                  <thead>
                    <tr>
                      <th class="text-center" style="vertical-align: middle">
                        Menu Group
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Menu ID
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Description
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Access</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllAccessMenu(e)"
                          />
                        </div>
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Update</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllUpdateMenu(e)"
                          />
                        </div>
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Price</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllPriceMenu(e)"
                          />
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, idx) in allowed.menu">
                      <td>{{ item.MenuGroup }}</td>
                      <td>{{ item.MenuID }}</td>
                      <td>{{ item.MenuDescription }}</td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowAccess"
                            @click="(e) => allowAccessMenu(e, item)"
                          />
                        </div>
                      </td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowUpdate"
                            @click="(e) => allowUpdateMenu(e, item)"
                          />
                        </div>
                      </td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowPrice"
                            @click="(e) => allowPriceMenu(e, item)"
                          />
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <!-- END tab-pane -->
            <!-- BEGIN tab-pane FACTORY -->
            <div class="tab-pane fade" id="default-tab-2" role="tabpanel">
              <div class="v-table-wrapper">
                <table
                  class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                >
                  <thead>
                    <tr>
                      <th class="text-center" style="vertical-align: middle">
                        Factory Code
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Factory Name
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Show</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllAccessFactory(e)"
                          />
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, idx) in allowed.factory">
                      <td>{{ item.FactoryCode }}</td>
                      <td>{{ item.FactoryName }}</td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowAccess"
                            @click="(e) => allowAccessFactory(e, item)"
                          />
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <!-- END tab-pane -->
            <!-- BEGIN tab-pane WAREHOUSE -->
            <div class="tab-pane fade" id="default-tab-3" role="tabpanel">
              <div class="v-table-wrapper">
                <table
                  class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                >
                  <thead>
                    <tr>
                      <th class="text-center" style="vertical-align: middle">
                        Factory Code
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Factory Name
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Warehouse Code
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Warehouse Name
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Show</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllAccessWarehouse(e)"
                          />
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="(item, idx) in allowed.warehouse.filter(
                        (x) => x.AllowedAccessFactory,
                      )"
                    >
                      <td>{{ item.FactoryCode }}</td>
                      <td>{{ item.FactoryName }}</td>
                      <td>{{ item.WarehouseCode }}</td>
                      <td>{{ item.WarehouseName }}</td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowAccess"
                            @click="(e) => allowAccessWarehouse(e, item)"
                          />
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <!-- END tab-pane -->
            <!-- BEGIN tab-pane AREA -->
            <div class="tab-pane fade" id="default-tab-4" role="tabpanel">
              <div class="v-table-wrapper">
                <table
                  class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                >
                  <thead>
                    <tr>
                      <th class="text-center" style="vertical-align: middle">
                        Warehouse Code
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Warehouse Name
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Area Code
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Area Name
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Show</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllAccessArea(e)"
                          />
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="(item, idx) in allowed.area.filter(
                        (x) =>
                          x.AllowedAccessWarehouse && x.AllowedAccessFactory,
                      )"
                    >
                      <td>{{ item.WarehouseCode }}</td>
                      <td>{{ item.WarehouseName }}</td>
                      <td>{{ item.AreaCode }}</td>
                      <td>{{ item.AreaName }}</td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowAccess"
                            @click="(e) => allowAccessArea(e, item)"
                          />
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <!-- END tab-pane -->

             <!-- BEGIN tab-pane AREA -->
            <div class="tab-pane fade" id="default-tab-5" role="tabpanel">
              <div class="v-table-wrapper">
                <table
                  class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                >
                  <thead>
                    <tr>
                      <th class="text-center" style="vertical-align: middle">
                        Parts Group Code
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                         Parts Group Name
                      </th>
                     
                      <th class="text-center" style="vertical-align: middle">
                        <span>Show</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllAccessGroupingClass(e)"
                          />
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                     <tr v-for="(item, idx) in allowed.groupingclass">
                      <td>{{ item.GroupingClassPartCode }}</td>
                      <td>{{ item.GroupingClassPartDescs }}</td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowAccess"
                            @click="(e) => allowAccessGroupingClassPart(e, item)"
                          />
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <!-- END tab-pane -->
            <!-- BEGIN tab-pane MOBILE -->
            <div class="tab-pane fade" id="default-tab-6" role="tabpanel">
              <div class="v-table-wrapper">
                <table
                  class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
                >
                  <thead>
                    <tr>
                      <th class="text-center" style="vertical-align: middle">
                        Menu ID
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        Description
                      </th>
                      <th class="text-center" style="vertical-align: middle">
                        <span>Access</span>
                        <div class="mt-1" style="justify-items: center">
                          <input-checkbox
                            @click="(e) => allowAllAccessMenuMobile(e)"
                          />
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, idx) in allowed.mobile">
                      <td>{{ item.MenuID }}</td>
                      <td>{{ item.MenuDescription }}</td>
                      <td>
                        <div style="justify-items: center">
                          <input-checkbox
                            v-model="item.AllowAccess"
                            @click="(e) => allowAccessMenuMobile(e, item)"
                          />
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
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
    isLoading: false,
    breadcrumbs: [
      { title: "Setting", active: false, to: "" },
      { title: "User", active: false, to: "/setting/user" },
      { title: "User Privileges", active: true, to: "" },
    ],
    list: [
      { value: "Menu", text: "Menu", defaultActive: true },
      // { value: "Process", text: "Process", defaultActive: false },
      // { value: "Line", text: "Line", defaultActive: false },
      { value: "Factory", text: "Factory", defaultActive: false },
      { value: "Warehouse", text: "Warehouse", defaultActive: false },
      { value: "Area", text: "Area", defaultActive: false },
      { value: "GroupingClassPart", text: "Parts Group", defaultActive: false },
      { value: "Mobile", text: "Mobile", defaultActive: false },
    ],
    allowed: {
      menu: [],
      factory: [],
      warehouse: [],
      area: [],
      groupingclass : [],
      mobile: [],
    },
  }),
  computed: {
    dsMenu: function () {
      return useMenu();
    },
  },
  mounted: function () {
    this.dsMenu.loadprivileges(this.$route.query.id).then((dt) => {
      this.allowed.menu = dt.Data.MenuPrivileges;
      this.allowed.factory = dt.Data.FactoryPrivileges;
      this.allowed.warehouse = dt.Data.WarehousePrivileges;
      this.allowed.warehouse.map((x) => {
        x.AllowedAccessFactory = false;
        if (
          this.allowed.factory.filter(
            (y) => y.AllowAccess && y.FactoryCode == x.FactoryCode,
          ).length > 0
        )
          x.AllowedAccessFactory = true;
      });

      this.allowed.area = dt.Data.AreaPrivileges;
      this.allowed.area.map((x) => {
        x.AllowedAccessFactory = false;
        x.AllowedAccessWarehouse = false;
        if (
          this.allowed.factory.filter(
            (y) => y.AllowAccess && y.FactoryCode == x.FactoryCode,
          ).length > 0
        )
          x.AllowedAccessFactory = true;

        if (
          this.allowed.warehouse.filter(
            (y) => y.AllowAccess && y.WarehouseCode == x.WarehouseCode,
          ).length > 0
        )
          x.AllowedAccessWarehouse = true;
      });

      this.allowed.groupingclass = dt.Data.GroupingClassPrivileges;
      this.allowed.mobile = dt.Data.MenuMobilePrivileges;
    });
  },
  methods: {
    allowAccessMenu: function (e, item) {
      this.allowed.menu.find((p) => p.MenuID === item.MenuID).AllowAccess =
        e.target.checked;
    },
    allowUpdateMenu: function (e, item) {
      this.allowed.menu.find((p) => p.MenuID === item.MenuID).AllowUpdate =
        e.target.checked;
    },
    allowPriceMenu: function (e, item) {
      this.allowed.menu.find((p) => p.MenuID === item.MenuID).AllowPrice =
        e.target.checked;
    },
    allowAccessFactory: function (e, item) {
      this.allowed.factory.find(
        (p) => p.FactoryCode === item.FactoryCode,
      ).AllowAccess = e.target.checked;

      this.allowed.warehouse
        .filter((p) => p.FactoryCode == item.FactoryCode)
        .map((p) => (p.AllowedAccessFactory = e.target.checked));

      this.allowed.area
        .filter((p) => p.FactoryCode == item.FactoryCode)
        .map((p) => (p.AllowedAccessFactory = e.target.checked));
    },
    allowAccessWarehouse: function (e, item) {
      this.allowed.warehouse.find(
        (p) => p.WarehouseCode === item.WarehouseCode,
      ).AllowAccess = e.target.checked;

      this.allowed.area
        .filter((p) => p.WarehouseCode == item.WarehouseCode)
        .map((p) => (p.AllowedAccessWarehouse = e.target.checked));
    },
    allowAccessArea: function (e, item) {
      this.allowed.area.find((p) => p.AreaCode === item.AreaCode).AllowAccess =
        e.target.checked;
    },
    allowAllAccessMenu: function (e) {
      this.allowed.menu.map((p) => (p.AllowAccess = e.target.checked));
    },
    allowAllUpdateMenu: function (e) {
      this.allowed.menu.map((p) => (p.AllowUpdate = e.target.checked));
    },
    allowAllPriceMenu: function (e) {
      this.allowed.menu.map((p) => (p.AllowPrice = e.target.checked));
    },
    allowAllAccessFactory: function (e) {
      this.allowed.factory.map((p) => (p.AllowAccess = e.target.checked));
      this.allowed.warehouse.map(
        (p) => (p.AllowedAccessFactory = e.target.checked),
      );
      this.allowed.area.map((p) => (p.AllowedAccessFactory = e.target.checked));
    },
    allowAllAccessWarehouse: function (e) {
      this.allowed.warehouse.map((p) => (p.AllowAccess = e.target.checked));
      this.allowed.area.map(
        (p) => (p.AllowedAccessWarehouse = e.target.checked),
      );
    },
    allowAllAccessArea: function (e) {
      this.allowed.area.map((p) => (p.AllowAccess = e.target.checked));
    },

    

     allowAllAccessGroupingClass: function (e) {
      this.allowed.groupingclass.map((p) => (p.AllowAccess = e.target.checked));
       
    },

    allowAccessGroupingClassPart: function (e, item) {
      this.allowed.groupingclass.find((p) => p.GroupingClassPartCode === item.GroupingClassPartCode).AllowAccess =
        e.target.checked;
    },


    allowAllAccessMenuMobile: function (e) {
      this.allowed.mobile.map((p) => (p.AllowAccess = e.target.checked));
    },
    allowAccessMenuMobile: function (e, item) {
      this.allowed.mobile.find((p) => p.MenuID === item.MenuID).AllowAccess =
        e.target.checked;
    },
    submit: function () {
      this.isLoading = true;

      let model = {
        UserID: this.$route.query.id,
        MenuPrivileges: this.allowed.menu,
        FactoryPrivileges: this.allowed.factory,
        WarehousePrivileges: this.allowed.warehouse,
        AreaPrivileges: this.allowed.area,
        GroupingClassPrivileges: this.allowed.groupingclass,
        MenuMobilePrivileges: this.allowed.mobile,
      };

      this.dsMenu
        .submitPrivilege(model)
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

.v-table-wrapper {
  overflow: auto;
  max-height: calc(100vh - 270px);
  /* border: 1px solid #ddd; */
  position: relative;
}

/* thead tr th {
  color: white !important;
} */
</style>
