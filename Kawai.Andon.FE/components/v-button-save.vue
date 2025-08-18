<template>
    <button class="form-submit bg-success" v-if="!featureId || profile.allowed(featureId)" @click="submit"
        :disabled="disabled">Save</button>
    <button class="form-submit bg-danger txt-light ms-2" @click="$router.back" :disabled="disabled">Cancel</button>
</template>

<script>
export default {
    props: ['submit', 'disabled', 'featureId'],
    computed: {
        profile: function () {
            return useProfile()
        },
    },
}
</script>