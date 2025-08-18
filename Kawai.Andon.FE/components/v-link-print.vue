<template>
  <div class="dropdown dropstart">
    <a v-if="uselabel" class="form-submit ms-2" href="#" data-bs-toggle="dropdown" aria-expanded="false"
      style="padding: 0px;border-radius: 0;">
      <v-icon name="printer" width="16px" />
      <span style="font-size: 12px;" class="ms-1">Cetak</span>
    </a>

    <a v-else class="form-submit ms-2" href="#" data-bs-toggle="dropdown" aria-expanded="false"
      style="padding: 0px;border-radius: 0;">
      <v-icon name="printer" width="16px" />
    </a>

    <ul class="dropdown-menu">
      <li v-for="(item, index) in list.filter(p => p.show !== false)" :key="index"
        :class="item.separator === true ? 'dropdown-divider' : null">
        <a class="dropdown-item" :href="item.href" @click="item.event" v-if="item.separator !== true">
          <v-icon :name="item.icon" width="14px" />
          <span class="ms-2">{{ item.label }}</span>
          <span class="badge text-bg-primary ms-2">{{ item.printCount }}</span>
        </a>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  props: ['list', "uselabel"],
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
}
</script>