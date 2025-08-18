<template>

    <button class="bg-light" @click="() => $router.push(to)" v-if="!featureId || profile.allowed(featureId)"
        style="display: flex;">
        <v-icon :name="icon" width="14px" />
        <span class="ml-1">{{ this.label }}</span>
    </button>
</template>
<script>
export default {
    props: ['to', 'featureId', 'icon', 'label'],
    computed: {
        profile: function () {
            return useProfile()
        },
    },
}
</script>