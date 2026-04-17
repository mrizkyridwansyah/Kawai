<template>
  <div class="navbar-item dropdown">
    <a
      href="#"
      data-bs-toggle="dropdown"
      class="navbar-link icon"
      ref="notifToggle"
    >
      <i class="fa fa-bell"></i>
      <span v-if="unreadCount > 0" class="badge">{{ unreadCount }}</span>
    </a>
    <div class="dropdown-menu media-list dropdown-menu-end">
      <div class="dropdown-header">
        NOTIFICATIONS ({{ notifications.length }})
      </div>
      <div style="overflow-y: scroll; max-height: 30em; max-width: max-content">
        <a
          v-for="(notif, index) in notifications"
          :key="index"
          href="javascript:;"
          @click="updateSeen(notif)"
          class="dropdown-item media"
        >
          <div class="media-left">
            <i
              v-if="notif.NotifType == 'INFO' || notif.NotifType == 'SUCCESS'"
              class="fa fa-bell media-object bg-blue-400"
            ></i>
            <i
              v-else-if="notif.NotifType == 'WARNING'"
              class="fa fa-bell media-object bg-yellow-400"
            ></i>
            <i
              v-else-if="notif.NotifType == 'ERROR'"
              class="fa fa-bell media-object bg-red-400"
            ></i>
          </div>
          <div class="media-body">
            <h6 class="media-heading">
              {{ notif.Title }}
              <i v-if="notif.Priority == 'LOW'" class="fa"></i>
              <i v-else-if="notif.Priority == 'MIDDLE'" class="fa"></i>
              <i
                v-else-if="notif.Priority == 'TOP'"
                class="fa"
                :class="{
                  'fa-info-circle':
                    notif.NotifType == 'INFO' || notif.NotifType == 'SUCCESS',
                  'fa-warning':
                    notif.NotifType != 'INFO' && notif.NotifType != 'SUCCESS',
                  'text-info':
                    notif.NotifType == 'INFO' || notif.NotifType == 'SUCCESS',
                  'text-warning': notif.NotifType == 'WARNING',
                  'text-danger': notif.NotifType == 'ERROR',
                }"
              ></i>
            </h6>
            <div style="white-space: break-spaces">{{ notif.Description }}</div>
            <div class="text-muted fs-10px">{{ notif.TimeAgo }}</div>
          </div>
        </a>
      </div>
    </div>
  </div>
  <div class="navbar-item navbar-user dropdown">
    <a
      v-if="image"
      href="#"
      class="navbar-link dropdown-toggle d-flex align-items-center"
      data-bs-toggle="dropdown"
    >
      <img :src="'data:image/png;base64,' + image" alt="" class="user-avatar" />
      <span>
        <span class="d-none d-md-inline fw-bold">{{
          this.userData?.FullName
        }}</span>
      </span>
    </a>
    <a
      v-else
      href="#"
      class="navbar-link dropdown-toggle d-flex align-items-center"
      data-bs-toggle="dropdown"
    >
      <img src="/img/user/user-12.jpg" alt="" />
      <span>
        <span class="d-none d-md-inline fw-bold">{{
          this.userData?.FullName
        }}</span>
      </span>
    </a>
    <div class="dropdown-menu dropdown-menu-end me-1">
      <a
        class="dropdown-item"
        href="javascript:void(0)"
        @click="$bvModal.show('change-password')"
        >Change Password</a
      >
      <a class="dropdown-item" href="javascript:void(0)" @click="changeFactory"
        >Change Factory</a
      >
      <div class="dropdown-divider"></div>
      <li>
        <a class="dropdown-item" href="javascript:void(0)" @click="signOut"
          >Sign Out</a
        >
      </li>
    </div>
  </div>
</template>

<script>
export default {
  data: () => ({
    showBackdrop: false,
    openNotif: false,
    inSetupWizard: true,
    workspaceList: [],
    userData: {},
    image: "",
    notifications: [],
    unreadCount: 0,
    hasNewNotification: false,
  }),
  computed: {
    auth: function () {
      return useAuth();
    },
    notif: function () {
      return useNotification();
    },
  },
  mounted: async function () {
    this.auth.load().then((dt) => {
      this.userData = dt.data.Data;
      this.notif
        .loadCountUnread(this.userData.UserId)
        .then((dt3) => (this.unreadCount = dt3.Data));
    });
    this.image = localStorage.getItem("UserPhoto");

    const nuxtApp = useNuxtApp();
    const signalr = await nuxtApp.$createSignalR("/notifapprovalhub");

    if (!signalr) {
      console.warn("SignalR connection not found!");
      return;
    }

    // Register event handler
    signalr.on("NewNotification", (data) => {
      this.hasNewNotification = true;
      this.unreadCount += data?.Count || 1;

      data?.notifications.forEach((notif) => {
        console.log(notif);
        toastNotif(notif);
        this.notif.setNewNotif();
      });
    });

    signalr.on("FileExport", (data) => {
      this.$http
        .get(`/export-file/download?key=${data?.key}`, {
          responseType: "blob",
        })
        .then((res) => {
          const url = URL.createObjectURL(res.data);
          
          console.log(url, data?.fileName);
          const link = document.createElement("a");
          link.href = url;
          link.download = `${data?.fileName}.xlsx`;
          link.click();

          URL.revokeObjectURL(url);
        })
        .catch(async (err) => {
          console.log(err);
        })
    });

    signalr.onreconnected(() => {
      console.log("✅ Reconnected to SignalR");
      this.notif
        .loadCountUnread(this.userData.UserId)
        .then((dt3) => {
          let diff = dt3.Data - this.unreadCount;
          if (diff > 0) {
            toastSuccess(`You have ${diff} new notifications!`);
          }

          this.unreadCount = dt3.Data;
        })
        .catch((err) => {
          console.error("Failed to reload unread count after reconnect:", err);
        });
    });
    const notifToggle = this.$refs.notifToggle;
    if (notifToggle) {
      notifToggle.addEventListener("shown.bs.dropdown", () => {
        this.loadNotifications();
      });

      notifToggle.addEventListener("hidden.bs.dropdown", () => {
        this.openNotif = false;
        console.log(this.openNotif, "openNotif");
      });
    }
  },
  methods: {
    loadNotifications: function () {
      if (this.openNotif) return;

      this.notif
        .load(this.userData.UserId)
        .then((dt) => (this.notifications = dt.Data));
      // Reset badge
      this.unreadCount = 0;
      this.hasNewNotification = false;
      this.openNotif = true;
      console.log(this.openNotif, "openNotif");
    },
    updateSeen: function (notif) {
      this.notif
        .updateSeen(notif.Id)
        .then((datas) => {
          this.$router.push(notif.UrlRedirect);
        })
        .catch((err) => {});
    },
    onOver: function () {
      this.$refs["header-menu-user-info"].visible = this.showBackdrop = true;
    },
    onLeave: function () {
      this.$refs["header-menu-user-info"].visible = this.showBackdrop = false;
    },
    signOut: function () {
      clearCookies();
      localStorage.clear();
      location.href = "/auth/sign-in";
    },
    changeFactory: function () {
      location.href = "/auth/factory";
    },
  },
};
</script>

<style lang="scss">
.dropdown-user-profile {
  .dropdown-item {
    font-size: 13px;
    font-weight: 500;
    // padding: 0.35rem .55rem;
    border-radius: 5px;
  }

  a {
    text-decoration: none !important;
  }
}

.user-info-name {
  font-size: 15px;
  font-weight: 500;
}

.user-avatar {
  border-radius: 50%; /* bulat */
  object-fit: cover; /* ini yang penting biar nggak stretch */
  object-position: center; /* posisi tengah */
}
</style>
