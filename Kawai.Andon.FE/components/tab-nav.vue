<template>
  <ul class="nav nav-tabs nav-tabs-inverse" role="tablist">
    <li class="nav-item" role="presentation" v-for="(header, idx) in headers">
      <a
        :href="`#${header.id}`"
        data-bs-toggle="tab"
        class="nav-link active"
        aria-selected="true"
        role="tab"
      >
        <font-awesome-icon :icon="header.icon" />
        <span class="d-none d-xl-inline">{{ header.title }}</span>
      </a>
    </li>
  </ul>
</template>
<script>
export default {
  props: ["headers"],
};
</script>
