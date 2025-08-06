<template>
  <header-menu title="Logging" :breadcrumbs="this.breadcrumbs" />
  <div class="mt-4">
    <div class="panel panel-inverse">
      <div class="panel-heading ui-sortable-handle">
        <font-awesome-icon icon="history" />
      </div>
      <div class="panel-body">
        <div class="d-flex">
          <div class="nav-sub">
            <ul class="nav flex-column">
              <li class="nav-item">
                <v-app-link
                  to="/logging/data-log"
                  active-class="active"
                  class="nav-link"
                >
                  Data Changes Log
                </v-app-link>
              </li>
              <li class="nav-item">
                <v-app-link
                  to="/logging/error-log"
                  active-class="active"
                  class="nav-link"
                >
                  Errors Log
                </v-app-link>
              </li>
              <li class="nav-item">
                <v-app-link
                  to="/logging/api-request"
                  active-class="active"
                  class="nav-link"
                >
                  API Request 
                </v-app-link>
              </li>
            </ul>
          </div>
          <div class="ps-5 flex-fill">
            <nuxt-page />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  data: () => ({
    breadcrumbs: [{ title: "Logging", active: false, to: "" }],
  }),
};
</script>
