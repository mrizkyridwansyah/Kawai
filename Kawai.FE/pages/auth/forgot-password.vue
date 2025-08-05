<template>
  <div class="authentication">
    <v-container fluid class="pa-3">
      <v-row class="h-100vh d-flex justify-center align-center">
        <v-col cols="12" lg="4" xl="3" class="d-flex align-center">
          <v-card rounded="md" elevation="10" class="px-sm-1 px-0 withbg mx-auto" max-width="500">
            <v-card-item class="pa-sm-8">
              <div class="d-flex justify-center py-4">
                <img class="mb-4" src="/panasonic-login.png" alt="" width="202" />
              </div>
              <div class="alert alert-success mt-3" role="alert"
                v-if="isSuccess"
              >
                <span>
                  <strong>Success!</strong> 
                  <div>We've send email to your email. please check email and follow the instruction to reset your password!</div>
                </span>
              </div>
              <div class="alert alert-danger mt-3" role="alert"
                v-if="invalidCode"
              >
                <span v-if="invalidCode == 'email_invalid'">
                  <strong>Invalid!</strong> 
                  <div>Email is invalid.</div>
                </span>
                <span v-if="invalidCode == 'email_not_found'">
                  <strong>Invalid!</strong> 
                  <div>Email not found.</div>
                </span>
                <span v-if="invalidCode == 'USER_INACTIVE'">
                  <strong>Authentication Failed!</strong> 
                  <div>User inactivated.</div>
                </span>
                <span v-if="invalidCode == 'email_not_confirmed'">
                  Your email is not verified, please verify your email.
                  <div>
                    <small>
                      Not receiving the email? 
                      <a href="#" @click="resend" v-if="coolDownResend == 0 && !isResendingEmailVerification">
                        Resend email verification.
                      </a>
                      <span v-if="isResendingEmailVerification">
                        Sending email verification...
                      </span>
                      <span v-if="coolDownResend > 0">Resend email verification in {{ coolDownResend }}s</span>  
                    </small>
                  </div>
                </span>
              </div>
              <v-row class="d-flex mb-3" v-if="!isSuccess">
                <v-col cols="12">
                  <v-label class="font-weight-bold mb-1">Email</v-label>
                  <v-text-field variant="outlined" hide-details color="primary" v-model="model.Email"></v-text-field>
                </v-col>
                <v-col cols="12" class="pt-0">
                  <div class="d-flex flex-wrap align-center ml-n2">
                    <!-- <v-checkbox v-model="checkbox" color="primary" hide-details>
                      <template v-slot:label class="text-body-1">Remember this Device</template>
                    </v-checkbox> -->
                    <div class="ml-sm-auto">
                      <NuxtLink to="/auth/sign-in"
                        class="text-primary text-decoration-none text-body-1 opacity-1 font-weight-medium">Back to login page</NuxtLink>
                    </div>
                  </div>
                </v-col>
                <v-col cols="12" class="pt-0">
                  <v-btn @click="signIn" color="primary" size="large" block flat
                    :disabled="isLoading"
                  >
                    <span class="indicator-progress" v-if="isLoading">
                      Please wait...
                      <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
                    </span>
                    <span v-else> Reset Password</span>
                  </v-btn>
                </v-col>
              </v-row>
            </v-card-item>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script>
export default {
  layout: "auth",
  data: () => ({
    isLoading: false,
    isSuccess: false,
    model: {
      Email: "",
      Password: "",
    },
    showPassword: false,
    invalidCode: null,
    verificationId: null,
    isResendingEmailVerification: false,
    coolDownResend: 0,
  }),
  mounted: function () {
  },
  methods: {
    signIn: function (e) {
      e.preventDefault();
      this.isLoading = true;
      this.invalidCode = null;
      this.$http.post(`/auth/password-forgot?email=${this.model.Email}`)
        .then((p) => {
          this.isSuccess = p.data.Status == 'SUCCESS';
        })
        .catch((err) => {
          this.isLoading = false
          this.invalidCode = err.response?.data?.Code;
          if (this.invalidCode == 'email_not_confirmed')
            this.verificationId = err.response?.data?.Data;
        })
    },
    resend: function (e) {
      // this.isResendingEmailVerification = true
      // this.$http.post(`/auth/v1/resend-email-verification?id=${this.verificationId}`, {})
      //   .then(v => {
      //     this.coolDownResend = 120;
      //     this.timerCooldown()
      //   })
      //   .finally(_ => this.isResendingEmailVerification = false)
    },
    timerCooldown: function () {
      var c = setInterval(() => {
        this.coolDownResend--;
        if (this.coolDownResend == 0)
          clearInterval(c)
      }, 1000)
    },
  },
};
</script>
