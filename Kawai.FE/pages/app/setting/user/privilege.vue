<template>
  <header-menu title="User Privileges" :breadcrumbs="this.breadcrumbs" />
  <button class="btn btn-sm btn-primary btn-elevate" @click="submit" :disabled="isLoading">
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
    @click="() => this.$router.push('/app/setting/user')"
  >
    <font-awesome-icon icon="arrow-left" />
    <span class="ml-2">Back</span>
  </button>
  <div class="row mt-4">
    <div class="col-lg-8 col-md-8 col-sm-12 col-12">
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
          class="tab-pane fade active show mt-3"
          id="default-tab-1"
          role="tabpanel"
        >
          <table class="table table-striped table-bordered mb-0 align-middle">
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
                    <input-checkbox @click="(e) => allowAllAccessMenu(e)" />
                  </div>
                </th>
                <th class="text-center" style="vertical-align: middle">
                  <span>Update</span>
                  <div class="mt-1" style="justify-items: center">
                    <input-checkbox @click="(e) => allowAllUpdateMenu(e)" />
                  </div>
                </th>
                <th class="text-center" style="vertical-align: middle">
                  <span>Price</span>
                  <div class="mt-1" style="justify-items: center">
                    <input-checkbox @click="(e) => allowAllPriceMenu(e)" />
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
        <!-- END tab-pane -->
        <!-- BEGIN tab-pane WAREHOUSE -->
        <div class="tab-pane fade mt-3" id="default-tab-2" role="tabpanel">
          <table class="table table-striped table-bordered mb-0 align-middle">
            <thead>
              <tr>
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
              <tr v-for="(item, idx) in allowed.warehouse">
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

<script>
import { TheaterIcon } from "vue-tabler-icons";

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
      { value: "Warehouse", text: "Warehouse", defaultActive: false },
      // { value: "Mobile", text: "Mobile", defaultActive: false },
    ],
    allowed: {
      menu: [],
      warehouse: [],
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
      this.allowed.warehouse = dt.Data.WarehousePrivileges;
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
    allowAccessWarehouse: function (e, item) {
      this.allowed.warehouse.find(
        (p) => p.WarehouseCode === item.WarehouseCode
      ).AllowAccess = e.target.checked;
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
    allowAllAccessWarehouse: function (e) {
      this.allowed.warehouse.map((p) => (p.AllowAccess = e.target.checked));
    },
    submit: function () {
      this.isLoading = true;

      let model = {
        UserID: this.$route.query.id,
        MenuPrivileges: this.allowed.menu,
        WarehousePrivileges: this.allowed.warehouse,
      };

      this.dsMenu
        .submitPrivilege(model)
        .then((datas) => {
          toastSuccess("Anying........ nge-sep!");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (
          setTimeout(() => {
            this.isLoading = false
          }, 500)));
    },
  },
};
</script>

<style scoped>
thead {
  background-color: #2d353c;
}

thead tr th {
  color: white !important;
}
</style>
