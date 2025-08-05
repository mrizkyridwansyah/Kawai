<template>
  <div @mouseover="onOver" @mouseleave="onLeave">
    <div class="header-menu-backdrop" v-if="showBackdrop" />
    <b-nav-item-dropdown
      right
      no-caret
      class="header-menu-chat"
      ref="header-menu-chat" menu-class="animate__animated animate__fadeInDown"
    >
      <template v-slot:button-content>
        <b-avatar
          variant="light"
          size="2.3em"
          badge="3"
          badge-top
          badge-variant="danger"
        >
          <b-icon icon="chat-text" ></b-icon>
          <!-- <feather-icon
            name="message-circle"
            :width="22"
            style="color: #3a413d"
          ></feather-icon> -->
        </b-avatar>
      </template>
      <div class="p-3">
        <div class="d-flex justify-content-between">
          <div style="font-weight: 600;font-size: 16px;">Chat</div>
        </div>
        <hr />
        <div>
          <div class="d-flex mt-2" v-for="(item, i) in [0, 0, 0, 0,]" :key="i">
            <b-avatar text="WH"></b-avatar>
            <div class="pl-2" style="">
              <div style="font-size: 15px;font-weight: 600">
                Momogie Chan
              </div>
              <div style="font-size: 13px;padding-bottom: 5px;">
                Great! Then will meet with them at the party...
              </div>
            </div>
          </div>
        </div>
        <div>
          <hr />
          <!-- <div class="d-flex">
            <div style="flex: 1; background:  #009fda;color: #fff; border-radius: 5px; margin: 5px;text-align: center;font-size: 14px;padding: 10px;">Discussion</div>
            <div style="flex: 1; background:  #009fda;color: #fff; border-radius: 5px; margin: 5px;text-align: center;font-size: 14px;padding: 10px;">Reviews</div>
          </div>
          <div class="d-flex">
            <div style="flex: 1; background:  #009fda;color: #fff; border-radius: 5px; margin: 5px;text-align: center;font-size: 14px;padding: 10px;">Order Complaint</div>
            <div style="flex: 1; background:  #009fda;color: #fff; border-radius: 5px; margin: 5px;text-align: center;font-size: 14px;padding: 10px;">Support Message</div>
          </div> -->
        </div>
      </div>
    </b-nav-item-dropdown>
  </div>
</template>

<script>
export default {
  data: () => ({
    showBackdrop: false
  }),
  mounted: function() {
    this.$root.$on('bv::dropdown::hide', bvEvent => {
      this.showBackdrop = false;
    })
  },
  methods: {
    onOver: function () {
      this.$refs["header-menu-chat"].visible = this.showBackdrop = true;
    },
    onLeave: function () {
      this.$refs['header-menu-chat'].visible = this.showBackdrop = false;
    },
  },
};
</script>

<style lang="scss">
.header-menu-chat {
  ul {
    min-width: 350px;
  }
}
</style>