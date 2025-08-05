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
                  <div>Reset password successful. you will redirected to the login page in {{ s }} seconds</div>
                </span>
              </div>
              <div class="alert alert-danger mt-3" role="alert"
                v-if="invalidCode"
              >
                <span v-if="invalidCode == 'invalid_token'">
                  <strong>Failed!</strong> 
                  <div>Invalid token.</div>
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
                  <v-label class="font-weight-bold mb-1">New Password</v-label>
                  <v-text-field variant="outlined" type="password" hide-details color="primary"
                    v-model="model.NewPassword"></v-text-field>
                  <label v-if="errors['NewPassword']" class="text-danger">{{ errors['NewPassword'][0] }}</label>
                </v-col>
                <v-col cols="12">
                  <v-label class="font-weight-bold mb-1">Re-Type New Password</v-label>
                  <v-text-field variant="outlined" type="password" hide-details color="primary"
                    v-model="model.ConfirmNewPassword"></v-text-field>
                    <label v-if="errors['ConfirmNewPassword']" class="text-danger">{{ errors['ConfirmNewPassword'][0] }}</label>
                  </v-col>
                <v-col cols="12" class="pt-0">
                  <div class="d-flex flex-wrap align-center ml-n2">
                    <!-- <v-checkbox v-model="checkbox" color="primary" hide-details>
                      <template v-slot:label class="text-body-1">Remember this Device</template>
                    </v-checkbox> -->
                    <div class="ml-sm-auto">
                      <!-- <NuxtLink to="/"
                        class="text-primary text-decoration-none text-body-1 opacity-1 font-weight-medium">Forgot
                        Password ?</NuxtLink> -->
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
                    <span v-else> Change Password</span>
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
      NewPassword: "",
      ConfirmNewPassword: "",
    },
    invalidCode: null,
    showPassword: false,
    s: null,
    errors: {},
  }),
  mounted: function () {
  },
  methods: {
    signIn: function (e) {
      e.preventDefault();
      this.isLoading = true;
      this.invalidCode = null;
      this.$http.post(`/auth/password-reset?code=${this.$route.query.code}`, this.model)
        .then((p) => {
          this.isSuccess = p.data.Status == 'SUCCESS';
          this.s = 5;
          setInterval(() => {
            this.s--;
            if(this.s == 0) location.href = '/auth/sign-in'
          }, 1000)
        })
        .catch((err) => {
          this.errors = err.response?.data?.Errors ?? {};
          this.invalidCode = err.response?.data?.Code;
        })
        .finally(_ => this.isLoading = false)
    },
  },
};
</script>
