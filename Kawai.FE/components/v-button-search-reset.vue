<template>
    <button class="btn btn-sm btn-primary btn-elevate" v-if="!featureId || profile.allowed(featureId)" @click="search"
        :disabled="disabled">Search</button>
    <button class="btn btn-sm btn-danger btn-elevate txt-light ms-1" @click="reset" :disabled="disabled">Reset</button>
</template>

<script>
export default {
    props: ['search', 'reset', 'disabled'],
    computed: {
        profile: function () {
            return useProfile()
        },
    },
}
</script>