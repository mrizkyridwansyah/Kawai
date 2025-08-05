<template>
  <v-modal
    id="change-password"
    title="Change Password"
    :loading="isLoading"
    :errors="errors"
    size="400"
  >
    <input-password 
      label="Current Password"
      v-model="model.CurrentPassword"
      :errors="errors.CurrentPassword"
    />
    <input-password 
      label="New Password"
      v-model="model.NewPassword"
      :errors="errors.NewPassword"
    />
    <input-password 
      label="Confirm New Password"
      v-model="model.ReTypeNewPassword"
      :errors="errors.ReTypeNewPassword"
    />
    <b-button class="mt-3" pill 
      variant="primary" block @click="submit"
    >
      Change Password
    </b-button>
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    isLoading: false,
    model: {
      CurrentPassword: null,
      NewPassword: null,
      ReTypeNewPassword: null,
    },
    errors: {},
  }),
  mounted: function() {
    setTimeout(() => {
      this.model.CurrentPassword = 'asdasd'
    }, 10000)
  },
  methods: {
    submit: function(e) {
      // e.preventDefault();
      this.isLoading = true;
      this.$http.post("/profile/change-password", this.model)
        .then(r => {
          console.log(r)
          this.isLoading = false;
          // toastSuccess("Change password successful!");
        })
        .catch(err => {
          this.isLoading = false;
          if (err.response) this.errors = err.response.data.Errors;
        });
    },
    onModalShown: function() {
    },
    onModalHidden: function() {
      this.errors = {};
    },
  },
};
</script>
