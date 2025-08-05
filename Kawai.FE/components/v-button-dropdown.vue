<template>
  <div class="dropdown dropstart">
    <a class="" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false" style="padding: 5px;">
      <v-icon name="more-horizontal" width="16px" />
    </a>

    <ul class="dropdown-menu">
      <li
        v-for="(item, index) in list.filter(p => p.show !== false && (!p.feature || (profile.allowed(p.feature))))"
        :key="index" :class="item.separator === true ? 'dropdown-divider' : null">
        <a class="dropdown-item" :href="item.href" @click="item.event" v-if="item.separator !== true">
          <font-awesome-icon :icon="item.icon" width="14px" />
          <span class="ms-2">{{ item.label }}</span>
        </a>
      </li>
      <!-- <li>
        <a class="dropdown-item" href="#" @click="showDetail(item)">
          <v-icon 
            name="file"
            width="14px"
          /> 
          <span class="ms-2">View Detail</span>
        </a>
      </li>
      <li>
        <a class="dropdown-item" href="#">
          <v-icon 
            name="edit"
            width="14px"
          /> 
          <span class="ms-2">Edit</span>
        </a>
      </li>
      <li><hr class="dropdown-divider"></li>
      <li>
        <a class="dropdown-item" href="#" @click="removeItem(item)">
          <v-icon 
            name="trash"
            width="14px"
          /> 
          <span class="ms-2">Delete</span>
        </a>
      </li> -->
    </ul>
  </div>
</template>

<script>
export default {
  props: ['list'],
  data: () => ({
    // list: [
    //   { 
    //     separator: false,
    //     href: '#',
    //     icon: 'trash',
    //     label: 'Delete',
    //     event: () => {}
    //   }
    // ]
  }),
  computed: {
    profile: function () {
      return useProfile()
    },
  },
}
</script>