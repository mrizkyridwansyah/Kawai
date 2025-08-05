<template>
  <div
    style="position: fixed; top: 53px; display: flex; left: 0px; right: 0px; background-color: rgb(255, 255, 255); z-index: 1020; padding: 3px 0px; border-bottom: 1px solid #eeeeee;">
    <div class="container">
      <ul class="nav nav-header-sub">
        <li class="nav-item">
          <v-app-link
            to="/" 
            class="nav-link" 
            active-class="active"
          >
            Home
          </v-app-link>
        </li>
        <li class="nav-item">
          <v-app-link
            to="/personnel" 
            class="nav-link" 
            active-class="active"
          >
            Personnel
          </v-app-link>
        </li>
        <li class="nav-item">
          <v-app-link
            to="/payroll" 
            class="nav-link" 
            active-class="active"
          >
            Payroll
          </v-app-link>
        </li>
        <li class="nav-item">
          <v-app-link
            to="/finance" 
            class="nav-link" 
            active-class="active"
          >
            Finance
          </v-app-link>
        </li>
        <li class="nav-item">
          <v-app-link
            to="/recruitment" 
            class="nav-link" 
            active-class="active"
          >
            Recruitment
          </v-app-link>
        </li>
        <li class="nav-item">
          <v-app-link
            to="/configurations" 
            class="nav-link" 
            active-class="active"
          >
            Configurations
          </v-app-link>
        </li>
        <li class="nav-item">
          <v-app-link
            to="/report" 
            class="nav-link" 
            active-class="active"
          >
            Reports
          </v-app-link>
        </li>
      </ul>
    </div>
    <div style="width: 100%;position: fixed;margin-top: 40px;"
      v-if="ws.isLoading"
    >
      <v-loading />
    </div>
  </div>
</template>

<script>

export default {
  computed: {
    ws: function() {
      return useWorkspace();
    }
  }
}
</script>
<style lang="scss">
.nav-header-sub {
  >.nav-item {
    position: relative;
    >.nav-link {
      color: rgb(63, 66, 84);
      font-weight: 600;
      .avatar {
        margin-right: 10px;
      }
      &.active {
        // border-top-left-radius: 15px;
        // border-top-right-radius: 15px;
        background: #f0f8ff;
      }
    }
    >.active::after {
      height: 4px;
      content: " ";
      background: #009ef7;
      position: absolute;
      bottom: -4px;
      left: 0;
      right: 0;
      border-top-left-radius: 5px;
      border-top-right-radius: 5px;
    }
  }
}

</style>