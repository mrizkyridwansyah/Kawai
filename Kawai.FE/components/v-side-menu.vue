<template>
  <div class="side-menu-container">
    <div class="d-flex">
      <div>

      </div>
      <div class="d-flex align-items-start flex-column" style="height: calc(100vh - 57px);">
        <ul class="side-menu">
          <li class="side-menu-item">
            <v-app-link 
              to="/"
              class="side-menu-link"
            >
              <v-icon 
                name="home"
                width="20"
                color="#596296"
              />
            </v-app-link>
            <!-- <a href="/" class="side-menu-link">
              <v-icon 
                name="home"
                width="20"
                color="#596296"
              />
            </a> -->
          </li>
          <!-- <li class="side-menu-item">
            <a href="#">
              <span class="side-menu-link">
                <v-icon 
                  name="edit"
                  width="20"
                  color="#596296"
                />
              </span>
            </a>
          </li>
          <li class="side-menu-item">
            <a href="#">
              <span class="side-menu-link">
                <v-icon 
                  name="check-square"
                  width="20"
                  color="#596296"
                />
              </span>
            </a>
          </li> -->
          <li class="side-menu-item">
            <a href="#">
              <span class="side-menu-link">
                <v-icon 
                  name="calendar"
                  width="20"
                  color="#596296"
                />
              </span>
            </a>
          </li>
          <!-- <li class="side-menu-item">
            <a href="#">
              <span class="side-menu-link">
                <v-icon 
                  name="edit"
                  width="20"
                  color="#596296"
                />
              </span>
            </a>
          </li> -->
        </ul>
        <ul class="side-menu mt-auto">
          <li class="side-menu-item" v-if="task.list.length > 0">
            <v-admin-console-link to="/" class="side-menu-link">
              <v-icon 
                name="refresh-cw"
                width="18"
                color="#596296"
                class="rotating"
              />
            </v-admin-console-link>
          </li>
          <li class="side-menu-item">
            <v-admin-console-link to="/" class="side-menu-link">
              <v-icon 
                name="settings"
                width="20"
                color="#596296"
              />
            </v-admin-console-link>
          </li>
          <li class="side-menu-item">
            <a href="#" class="side-menu-link">
              <!-- <v-icon 
                name="info"
                width="20"
                color="#596296"
              /> -->
              <i class="bi bi-headset" style="font-size: 18px;color: #596296;font-weight: 600;margin-top: 5px;"></i>
            </a>
          </li>
          <li class="side-menu-item">
            <a href="#" class="side-menu-link" @click="signOut">
              <v-icon 
                name="log-out"
                width="20"
                color="#596296"
              />
            </a>
          </li>
        </ul>
      </div>
    </div>
    <!-- <modal-system-process /> -->
  </div>
</template>

<script>
export default {
  data: () => ({
  }),
  computed: {
    task: function() {
      return useTaskProcess();
    }
  },
  mounted: function() {
    var socket = useWebSocket();
    socket.connect();
  },
  methods: {
    signOut: function() {
      clearCookies();
      location.href = '/'
    },
  }
}
</script>

<style lang="scss">
.side-menu-container {
  z-index: 1020;
  position: fixed;
  left: 0;
  top: 57px;
  bottom: 0;
  border-right: solid 1px rgba($color: #000000, $alpha: .1);
  background-color: #f8f9fd;
}
.side-menu {
  margin-top: 30px;
  width: 60px;
  list-style: none;
  padding: 0;

  .side-menu-item {
    display: block;
    padding: 15px 0px;
    // margin-top: 10px;
    a {
      text-decoration: none;
    }
    .side-menu-link {
      margin: 10px;
      padding: 10px 10px;
      border-radius: 10px;
      text-align: center;
      // background-color: #fff;
      // border: solid 1px rgba($color: #000000, $alpha: .05);
      &:hover {
        background-color: #d2d6e6;
        color: #fff;
        // .icon {
        //   stroke: #fff;
        // }
      }
    }
  }
}
</style>