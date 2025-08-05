<template>
  <b-nav-item-dropdown
    :text="selected.Name"
    style="padding-right: .6rem;padding-left: .6rem;border-radius:7px;"
    right
  >
    <b-dropdown-item v-for="(item, index) in list" :key="index" href="#" @click="() => selectItem(item)">
      {{item.Name}}
    </b-dropdown-item>
  </b-nav-item-dropdown>
</template>

<script>
export default {
  computed: {
    list: function() {
      return this.$store.state.user.companyList;
    },
    selected: function() {
      return this.$store.state.user.currentCompany;
    },
  },
  methods: {
    selectItem: function(data) {
      this.$store.commit("user/setCurrentCompany", data);
      setCookie('__CMP', data.Id)
    }
  }
}
</script>