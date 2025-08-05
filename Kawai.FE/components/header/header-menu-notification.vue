<template>
  <div @mouseover="onOver" @mouseleave="onLeave">
    <div class="header-menu-backdrop" v-if="showBackdrop" />
    <b-nav-item-dropdown
      right
      no-caret
      class="header-menu-notification"
      ref="header-menu-notification"
      menu-class="animate__animated animate__fadeInDown"
    >
      <template v-slot:button-content>
        <b-avatar variant="indigo" size="2em" badge-top badge-variant="danger">
          <!-- <b-icon icon="bell"></b-icon> -->
          <feather-icon
            name="bell"
            :width="16"
          ></feather-icon>
        </b-avatar>
      </template>

      <div class="p-3">
        <div class="d-flex justify-content-between">
          <div style="font-weight: 600;font-size: 16px;">
            Notifications
          </div>
        </div>
        <hr />
        <b-tabs content-class="mt-3" fill>
          <b-tab>
            <template #title>
              <div class="d-flex justify-content-center align-items-center">
                <p class="mb-0 mr-1 mt-1">Inbox</p>
              </div>
            </template>
            <div class="notification-tabs-inbox">
              <div v-if="table.Items.length == 0" class="text-center p-5">
                <span>Opps!</span>
                <div>Nothing todo there.</div>
              </div>
              <div class="notification-items-inbox" v-for="(item, i) in table.Items" :key="i">
                <div class="d-flex align-items-start">
                  <b-avatar
                    variant="info"
                    class="mr-3"
                    size="lg"
                    alt="Kitten"
                    :text="item.title[0]"
                  ></b-avatar>

                  <div>
                    <div class="title">{{ item.title }}</div>
                    <div>
                      {{ item.message }}
                    </div>
                    <div class="d-flex">
                      <div class="insert-stamp mt-2">
                        <!-- {{ $func.formatDateTime(item.insertStamp) }} -->
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </b-tab>
          <b-tab active>
            <template #title>
              <div class="d-flex justify-content-center">
                <p class="mb-0 mr-1 mt-1">Approvals</p>
              </div>
            </template>
            <div class="notification-tabs-approvals">
              <div v-if="table.Items.length == 0" class="text-center p-5">
                <span>Opps!</span>
                <div>Nothing todo there.</div>
              </div>
              <div class="notification-items-approvals" v-for="(item, i) in table.Items" :key="i">
                <div class="d-flex align-items-start">
                  <b-avatar
                    variant="info"
                    class="mr-3"
                    size="lg"
                    alt="Kitten"
                    :text="item.title[0]"
                  ></b-avatar>

                  <div class="w-100">
                    <div class="title">{{ item.title }}</div>
                    <div>
                      {{ item.message }}
                    </div>
                    <div class="d-flex">
                      <div class="insert-stamp mt-2 mb-2">
                        <!-- {{ $func.formatDateTime(item.insertStamp) }} -->
                      </div>
                    </div>
                    <div
                      class="w-100 d-flex justify-content-around align-items-center flex-direction-row"
                    >
                      <b-button
                        class="w-100 mr-1"
                        variant="danger"
                        size="sm"
                        pill
                        >Reject</b-button
                      >
                      <b-button
                        class="w-100 ml-1"
                        variant="success"
                        size="sm"
                        pill
                        >Approve</b-button
                      >
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </b-tab>
        </b-tabs>
        <br />
        <b-button size="sm" pill block variant="primary">Show All</b-button>
      </div>
    </b-nav-item-dropdown>
  </div>
</template>

<script>
export default {
  data: () => ({
    showBackdrop: false,
    table: {
      Items: [
        
      ],
      Total: 0,
      Parameters: {
        page: 1,
        Length: 10,
        keyword: null,
      },
      isLoaded: false,
      isMax: false,
      isRequestError: false,
      isRequestLoading: false,
    },
  }),
  mounted: function() {
    this.$root.$on("bv::dropdown::hide", bvEvent => {
      this.showBackdrop = false;
    });
    this.loadList();
  },
  methods: {
    onOver: function() {
      this.$refs["header-menu-notification"].visible = this.showBackdrop = true;
    },
    onLeave: function() {
      this.$refs["header-menu-notification"].visible = this.showBackdrop = false;
    },
    loadList: function() {
      // this.table.isRequestError = false;
      // this.table.isRequestLoading = true;
      // this.table.Parameters.length = parseInt(this.table.Parameters.length)
      // httpPost('/notifications/notification-list', this.table.Parameters)
      //   .then(p => {
      //     this.table.Items = p.data.Data.Items;
      //     this.table.isMax = this.table.Items.length == 0;
      //     this.table.isLoaded = true;
      //     this.table.isRequestLoading = false;
      //   })
      //   .catch(err => {
      //     this.table.isRequestError = true;
      //     this.table.isRequestLoading = false;
      //   })
    },
  },
};
</script>

<style lang="scss">
.header-menu-notification {
  ul.dropdown-menu {
    width: 350px;
  }
}

.notification-tabs-approvals,
.notification-tabs-inbox {
  max-height: 300px;
  overflow-y: auto;
  margin: -10px -13px;
  padding: 10px;
  scrollbar-width: thin;

  /* width */
  &::-webkit-scrollbar {
    width: 8px;
  }

  /* Track */
  &::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 10px;
  }

  /* Handle */
  &::-webkit-scrollbar-thumb {
    background: rgb(190, 190, 190);
    border-radius: 10px;
  }

  /* Handle on hover */
  &::-webkit-scrollbar-thumb:hover {
    background: rgb(160, 160, 160);
  }

  .notification-items-approvals,
  .notification-items-inbox {
    border-bottom: solid 1px rgba(0, 0, 0, 0.05);
    padding: 5px 5px;

    .title {
      font-size: 14px;
      font-weight: 500;
    }

    .d-flex {
      .insert-stamp {
        font-size: 10px;
      }
    }
  }

  .text-center {
    span {
      font-weight: 500;
      font-size: 20px;
    }

    div {
      font-size: 16px;
    }
  }

  .no-border {
    border: none !important;
  }
}
</style>
