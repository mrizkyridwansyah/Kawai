<template>
    <v-app-link v-if="profile.allowed(featureId)" :to="to">Next
        <v-icon name="arrow-right" width="14px" />
    </v-app-link>
</template>

<script>
export default {
  props: ['to', 'featureId'],
  computed: {
    profile: function () {
      return useProfile()
    },
  },
}
</script>

