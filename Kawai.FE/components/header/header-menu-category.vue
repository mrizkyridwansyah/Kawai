<template>
  <div @mouseover="onOver" @mouseleave="onLeave">
    <div class="header-menu-backdrop" v-if="showBackdrop" />
    <b-nav-item-dropdown class="header-menu-category show" ref="header-menu-category" right no-caret>
      <template v-slot:button-content>
        <div>Category</div>
      </template>
      <div class="mega-menu-category">
        <div class="container d-flex">
          <div class="category-parent">
            <ul>
              <li v-for="(item, i) in categories" :key="i">{{ i + ' ' +item.Name}}</li>
            </ul>
          </div>
          <div>
            <ul>
              <li></li>
            </ul>
          </div>
        </div>
      </div>
    </b-nav-item-dropdown>
  </div>
</template>

<script>
export default {
  data: () => ({
    showBackdrop: false,
    categories: [],
    subCategory: []
  }),
  mounted: function() {
    this.$root.$on('bv::dropdown::hide', bvEvent => {
      this.showBackdrop = false;
    })

    httpPost('/product/category-list', {})
      .then(p => {
        this.categories = p.data.Data.Items;
      })
      .catch(err=> {

      })
  },
  methods: {
    onOver: function () {
      this.$refs["header-menu-category"].visible = this.showBackdrop = true;
    },
    onLeave: function () {
      this.$refs['header-menu-category'].visible = this.showBackdrop = false;
    },
  },
};
</script>

<style lang="scss">
.header-menu-category {    
  ul.dropdown-menu {
    // width: 1140px;
    // top: 70px;
    // margin: auto;
    position: fixed!important;
    // top: 97px;
    top: 91px;
    left: 0;
    right: 0;
    // background: blue;
    border-radius: unset;
    height: 0;
    transition: height .25s ease-out;
    display: block!important;
    padding: 0;
    margin: 0;
    border: none;
    overflow: hidden;
    border-radius: 0 !important;
    border-bottom-left-radius: 12px !important;
    border-bottom-right-radius: 12px!important;
  }
}

.b-nav-dropdown.header-menu-category:hover {
    ul.dropdown-menu {
      height: 500px;
      transition: height .25s ease-in;
      display: block!important;
      border-top: solid 1px rgba(0, 0, 0, .1);
      box-shadow: 1px 3px 10px rgba(0, 0, 0, .1);
    }
}

.category-parent {
  overflow-y: auto;
  height: 500px;
}
</style>