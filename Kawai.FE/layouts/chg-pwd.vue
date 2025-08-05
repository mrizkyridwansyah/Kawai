<template>
    <input-password 
      label="Current Password"
      v-model="model.Password"
      :errors="errors.Password"
    />
    <input-password 
      label="New Password"
      v-model="model.NewPassword"
      :errors="errors.NewPassword"
    />
    <input-password 
      label="Confirm New Password"
      v-model="model.ConfirmPassword"
      :errors="errors.ConfirmPassword"
    />
    <button type="button" @click="submit" class="mt-3 btn btn-primary rounded-pill" style="color: white; width: 100%">
      Change Password
    </button>
</template>

<script>
export default {
  data: () => ({
    isLoading: false,
    model: {
      Password: null,
      NewPassword: null,
      ConfirmPassword: null,
    },
    errors: {},
  }),
  mounted: function() {
    // setTimeout(() => {
    //   this.model.Password = 'asdasd'
    // }, 10000)
  },
  methods: {
    submit: function(e) {
      // e.preventDefault();
      this.isLoading = true;
      this.$http.post("/user/change-password", this.model)
        .then(r => {
          console.log(r)
          this.isLoading = false;
          this.errors = {};
          this.model.Password = null;
          this.model.NewPassword = null;
          this.model.ConfirmPassword = null;
          toastSuccess("Change password successful!");
          this.$bvModal.hide('change-password')
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
