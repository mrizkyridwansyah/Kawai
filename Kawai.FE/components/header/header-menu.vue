<template>
    <ol class="breadcrumb float-xl-end">
        <li :class="['breadcrumb-item', { 'active': breadcrumb.active }]" v-for="(breadcrumb, idx) in breadcrumbs">
            <v-app-link v-if="breadcrumb.to != null && breadcrumb.to != undefined && breadcrumb.to != ''"
                :to="breadcrumb.to">{{ breadcrumb.title }}</v-app-link>
            <a href="javascript:void(0)" v-else>{{ breadcrumb.title }}</a>
        </li>
    </ol>
    <h1 class="page-header">{{ this.title }}</h1>
</template>

<script>
export default {
    props: ['title', 'breadcrumbs']
};
</script>