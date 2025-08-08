<template>
  <!-- BEGIN #app -->
  <div id="app" class="app">
    <!-- BEGIN login -->
    <div class="login login-with-news-feed">
      <!-- BEGIN news-feed -->
      <div class="news-feed">
        <div
          class="news-image"
          :style="{
            backgroundImage: `url('${
              useAppConfig().baseURL || ''
            }/img/login-bg/login-bg-11.jpg')`,
          }"
        ></div>
        <div class="news-caption">
          <!-- <h4 class="caption-title"><b>Color</b> Admin App</h4>
                    <p>
                        Download the Color Admin app for iPhone�, iPad�, and Android�. Lorem ipsum dolor sit amet,
                        consectetur adipiscing elit.
                    </p> -->
        </div>
      </div>
      <!-- END news-feed -->

      <!-- BEGIN login-container -->
      <div class="login-container">
        <!-- BEGIN login-header -->
        <div class="login-header mb-30px">
          <div class="brand">
            <div class="d-flex align-items-center">
              <span class="logo"></span>

              <b>TOS</b> Admin
            </div>
            <small>Production control by TOS</small>
          </div>
        </div>
        <!-- END login-header -->

        <!-- BEGIN login-content -->
        <div class="login-content">
          <div class="form-floating">
            <input
              type="text"
              class="form-control h-45px fs-13px"
              placeholder="Username"
              v-model="model.UserName"
            />
            <v-label class="d-flex align-items-center fs-13px text-gray-600"
              >Username</v-label
            >
          </div>
          <div class="invalid-feedback d-block" v-if="errors?.UserName">
            {{ errors?.UserName }}
          </div>
          <div class="form-floating mt-5">
            <input
              type="password"
              class="form-control h-45px fs-13px"
              placeholder="Password"
              v-model="model.Password"
            />
            <label
              for="password"
              class="d-flex align-items-center fs-13px text-gray-600"
              >Password</label
            >
          </div>
          <div class="invalid-feedback d-block" v-if="errors?.Password">
            {{ errors?.Password }}
          </div>
          <div class="text-right mb-30px">
            <!-- <v-app-link to="/auth/forgot-password"
              >Forgot Password ?</v-app-link
            > -->
          </div>
          <div class="mb-15px">
            <button
              class="btn btn-theme d-block h-45px w-100 btn-lg fs-14px"
              @click="signIn"
              :disabled="isLoading"
            >
              <span class="indicator-progress" v-if="isLoading">
                Please wait...
                <span
                  class="spinner-border spinner-border-sm align-middle ms-2"
                ></span>
              </span>
              <span v-else> Sign in</span>
            </button>
          </div>
          <hr class="bg-gray-600 opacity-2" />
          <div class="text-gray-600 text-center mb-0">&copy; TOS 2025</div>
        </div>
        <!-- END login-content -->
      </div>
      <!-- END login-container -->
    </div>
    <!-- END login -->
  </div>
</template>

<script>
export default {
  layout: "auth",
  data: () => ({
    isLoading: false,
    model: {
      UserName: "",
      Password: "",
    },
    errors: {},
    showPassword: false,
    invalidCode: null,
    verificationId: null,
    isResendingEmailVerification: false,
    coolDownResend: 0,
  }),
  computed: {
    auth: function () {
      return useAuth();
    },
  },
  mounted: function () {},
  methods: {
    signIn: function (e) {
      e.preventDefault();
      this.isLoading = true;
      this.invalidCode = null;
      this.auth
        .signIn(this.model)
        .then((p) => {
          setCookie("__SIDX", p.Data.AccessToken, p.Data.ExpiryDate);
          localStorage.setItem("UserPhoto", p.Data.UserPhoto);
          if (this.$router.currentRoute.query?.continue)
            location.href = this.$router.currentRoute.query?.continue;
          else location.href = "/app";
        })
        .catch((err) => {
          this.isLoading = false;
          this.errors = err.Errors;
          toastDanger(err.Message);
        });
    },
    resend: function (e) {},
    timerCooldown: function () {
      var c = setInterval(() => {
        this.coolDownResend--;
        if (this.coolDownResend == 0) clearInterval(c);
      }, 1000);
    },
  },
};
</script>
