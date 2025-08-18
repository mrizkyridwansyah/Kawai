<template>
  <nav style="--bs-breadcrumb-divider: '>';" aria-label="breadcrumb">
    <ol class="breadcrumb">
      <li class="breadcrumb-item" v-for="(item, i) in list.filter(p => p.to)" :key="i">
        <v-app-link :to="item.to">{{ item.text }}</v-app-link>
      </li>
      <li class="breadcrumb-item active" 
        v-for="(item, i) in list.filter(p => !p.to)" 
        :key="i" 
        aria-current="page"
      >
        {{ item.text }}
      </li>
      <!-- <li class="breadcrumb-item active" aria-current="page">Create</li> -->
    </ol>
  </nav>
</template>

<script>
export default {
  props: ['list']
}
</script>