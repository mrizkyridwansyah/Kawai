<template>
  <div>
    <div id="sidebar" class="app-sidebar" data-bs-theme="dark">
      <div class="app-sidebar-content" data-scrollbar="true" data-height="100%">
        <div class="menu">
          <!-- <div class="menu-header"></div> -->
          <div
            v-for="(menuGroup, idx) in menuGroups"
            :key="idx"
            class="menu-item has-sub"
            :class="{ active: isActive(menuGroup) }"
            @click="toggleActive(menuGroup)"
          >
            <template v-if="getLevel(menuGroup) === 1">
              <v-app-link :to="getMenuName(menuGroup)" class="menu-link">
                <div class="menu-icon">
                  <font-awesome-icon :icon="getImageName(menuGroup)" />
                </div>
                <div class="menu-text">
                  {{ menuGroup }}
                </div>
              </v-app-link>
            </template>
            <template v-else-if="getLevel(menuGroup) === 2">
              <a
                href="javascript:;"
                class="menu-link"
                @mouseenter="showFloatingMenu(menuGroup, null, $event)"
                @mouseleave="hideFloatingMenu"
              >
                <div class="menu-icon">
                  <font-awesome-icon :icon="getImageName(menuGroup)" />
                </div>
                <div class="menu-text">{{ menuGroup }}</div>
                <font-awesome-icon class="ml-3" icon="fa-angle-right" />
              </a>
              <div class="menu-submenu">
                <div
                  v-for="(menu, idx) in getMenu(menuGroup, null)"
                  :key="idx"
                  class="menu-item"
                >
                  <v-app-link :to="menu.MenuName" class="menu-link">
                    <div class="menu-text">{{ menu.MenuDescription }}</div>
                  </v-app-link>
                </div>
              </div>
            </template>
            <template v-else>
              <a
                href="javascript:;"
                class="menu-link"
                @mouseenter="showFloatingMenu(menuGroup, null, $event)"
                @mouseleave="hideFloatingMenu"
              >
                <div class="menu-icon">
                  <font-awesome-icon :icon="getImageName(menuGroup)" />
                </div>
                <div class="menu-text">{{ menuGroup }}</div>
                <font-awesome-icon class="ml-3" icon="fa-angle-right" />
              </a>
              <div class="menu-submenu" v-if="getSubMenu(menuGroup).length > 0">
                <div
                  v-for="(subGroup, idx) in getSubMenu(menuGroup)"
                  :key="idx"
                  class="menu-item has-sub"
                  :class="{
                    closed: !isSubMenuActive(menuGroup, subGroup),
                    expand: isSubMenuActive(menuGroup, subGroup),
                  }"
                  @click.stop="toggleSubMenu(menuGroup, subGroup)"
                >
                  <a
                    href="javascript:;"
                    class="menu-link"
                    @mouseenter="showFloatingMenu(menuGroup, subGroup, $event)"
                    @mouseleave="hideFloatingMenu"
                  >
                    <div class="menu-text">{{ subGroup }}</div>
                  </a>
                  <div
                    class="menu-submenu"
                    :style="{
                      'box-sizing': 'border-box',
                      display: isSubMenuActive(menuGroup, subGroup)
                        ? 'block'
                        : 'none',
                    }"
                  >
                    <div
                      v-for="(menu, idx) in getMenu(menuGroup, subGroup)"
                      :key="idx"
                      class="menu-item"
                    >
                      <v-app-link :to="menu.MenuName" class="menu-link">
                        <div class="menu-text">{{ menu.MenuDescription }}</div>
                      </v-app-link>
                    </div>
                  </div>
                </div>
              </div>
            </template>
          </div>
          <!-- BEGIN minify-button -->
          <div class="menu-item d-flex">
            <a
              href="javascript:;"
              class="app-sidebar-minify-btn ms-auto d-flex align-items-center text-decoration-none"
              @click="handleToggle"
            >
              <font-awesome-icon icon="fa-angle-double-left" />
            </a>
          </div>
          <!-- END minify-button -->
        </div>
      </div>
    </div>

    <teleport to="body">
      <div
        data-bs-theme="dark"
        v-if="floatingMenu.show"
        class="app-sidebar-float-submenu-container"
        :style="{ top: floatingMenu.top + 'px', left: '60px' }"
        @mouseenter="clearHideTimer"
        @mouseleave="hideFloatingMenu"
      >
        <div class="app-sidebar-float-submenu-arrow"></div>
        <div class="app-sidebar-float-submenu">
          <div
            v-for="menu in floatingMenu.items"
            :key="menu.MenuName"
            class="menu-item"
          >
            <v-app-link :to="menu.MenuName" class="menu-link">
              <div class="menu-text">{{ menu.MenuDescription }}</div>
            </v-app-link>
          </div>
        </div>
      </div>
    </teleport>
  </div>
</template>

<script>
import { useNuxtApp } from "#app";

export default {
  data: () => ({
    hideTimer: null,
    isSmallScreen: false, // State untuk mengecek apakah ukuran layar kecil
    privileges: [],
    activeGroups: new Set(), // Menyimpan menu yang aktif
    activeSubMenus: {}, // Menyimpan status submenu, { menuGroup: { subGroup: boolean } }
    floatingMenu: {
      show: false,
      top: 0,
      items: [],
    },
  }),
  computed: {
    menu: function () {
      return useMenu();
    },
    menuGroups: function () {
      return [...new Set(this.privileges.map((item) => item.MenuGroup))];
    },
    getLevel: function () {
      return (menuGroup) => {
        let subGroup = this.privileges.filter((x) => x.MenuGroup === menuGroup);
        if (
          subGroup.length === 1 &&
          subGroup[0].SubGroup === null &&
          subGroup[0].MenuDescription === menuGroup
        )
          return 1;
        if (
          subGroup.length === 1 &&
          subGroup[0].SubGroup === null &&
          subGroup[0].MenuDescription !== menuGroup
        )
          return 2;

        if (
          subGroup.length > 1 &&
          subGroup[0].SubGroup === null &&
          subGroup[0].MenuDescription !== menuGroup
        )
          return 2;

        return 3;
      };
    },
    getMenuName: function () {
      return (menuGroup) => {
        return this.privileges.filter((item) => item.MenuGroup === menuGroup)[0]
          .MenuName;
      };
    },
    getImageName: function () {
      return (menuGroup) => {
        return this.privileges.filter((item) => item.MenuGroup === menuGroup)[0]
          .ImageName;
      };
    },
    getSubMenu: function () {
      return (menuGroup) => {
        return [
          ...new Set(
            this.privileges
              .filter(
                (item) => item.MenuGroup === menuGroup && item.SubGroup != null,
                //  &&
                // item.MenuGroup != item.SubGroup
              )
              .map((x) => x.SubGroup),
          ),
        ];
      };
    },
    getMenu: function () {
      return (menuGroup, subGroup) => {
        if (subGroup == null) {
          return this.privileges.filter((item) => item.MenuGroup === menuGroup);
        } else {
          return this.privileges.filter(
            (item) => item.MenuGroup === menuGroup && item.SubGroup == subGroup,
          );
        }
      };
    },
  },
  mounted: function () {
    this.checkScreenSize();
    this.menu.privileges().then((dt) => {
      this.privileges = dt.Data;
    });
    window.addEventListener("resize", this.checkScreenSize);
  },
  methods: {
    toggleActive: function (menuGroup) {
      if (this.activeGroups.has(menuGroup)) {
        this.activeGroups.delete(menuGroup); // Nonaktifkan
      } else {
        this.activeGroups.add(menuGroup); // Aktifkan
      }
    },
    isActive(menuGroup) {
      return this.activeGroups.has(menuGroup); // Periksa apakah menuGroup aktif
    },
    toggleSubMenu(menuGroup, subGroup) {
      // Toggle the status of the submenu (active or not)
      if (!this.activeSubMenus[menuGroup]) {
        this.activeSubMenus[menuGroup] = {}; // Initialize if doesn't exist
      }
      this.activeSubMenus[menuGroup][subGroup] =
        !this.activeSubMenus[menuGroup][subGroup]; // Toggle submenu
    },
    isSubMenuActive(menuGroup, subGroup) {
      // Check if the submenu is active
      return (
        this.activeSubMenus[menuGroup] &&
        this.activeSubMenus[menuGroup][subGroup]
      );
    },
    handleToggle: function () {
      const { $sidebarToggle } = useNuxtApp();
      $sidebarToggle.toggleSidebarMinified();
    },
    checkScreenSize: function () {
      this.isSmallScreen = window.innerWidth <= 768; // Sesuaikan breakpoint sesuai kebutuhan
    },
    showFloatingMenu(menuGroup, subGroup, event) {
      const { $sidebarToggle } = useNuxtApp();

      // Hanya tampilkan floating jika sidebar minified
      if (!$sidebarToggle.isSidebarMinified.value) return;

      clearTimeout(this.hideTimer);
      const rect = event.currentTarget.getBoundingClientRect();
      this.floatingMenu.top = rect.top;
      this.floatingMenu.items = this.getMenu(menuGroup, subGroup);
      this.floatingMenu.show = true;
    },
    hideFloatingMenu() {
      this.hideTimer = setTimeout(() => (this.floatingMenu.show = false), 200);
    },
    clearHideTimer() {
      clearTimeout(this.hideTimer);
    },
  },
  beforeUnmount() {
    window.removeEventListener("resize", this.checkScreenSize);
  },
};
</script>

<style>
.menu-caret::before {
  content: "\f105";
  /* Unicode untuk panah kanan */
  font-family: "Font Awesome 6 Pro" !important;
  /* atau Font Awesome 6 Pro jika Pro */
  font-weight: 900;
  /* 900 untuk solid icons, 400 untuk regular */
  /* display: inline-block; */
}
</style>
