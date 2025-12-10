<template>
  <button class="logout-btn" @click="signOut">
    <i class="fa fa-sign-out-alt"></i> Logout
  </button>

  <div class="factory-page">
    <div class="header">
      <h2 class="title">Select Factory</h2>
      <p class="subtitle">Please choose a factory to continue</p>
    </div>

    <div class="factory-grid">
      <div
        v-for="f in factories"
        :key="f.FactoryCode"
        class="factory-card"
        :class="{ 'no-access': !f.AllowAccess }"
        @click="selectFactory(f)"
      >
        <!-- icon -->
        <div class="icon">
          <i class="fa fa-building"></i>
        </div>

        <!-- name -->
        <div class="name">
          {{ f.FactoryName }}
        </div>

        <!-- overlay lock (hanya kalau tidak punya akses) -->
        <div v-if="!f.AllowAccess" class="lock-overlay">
          <i class="fa fa-lock fa-2x"></i>
          <div>No Access</div>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  layout: "auth",
  data: () => ({
    isLoading: false,
    factories: [],
    selectedFactory: "",
  }),
  computed: {
    ds: function () {
      return useFactory();
    },
  },
  mounted: function () {
    this.ds.load().then((dt) => (this.factories = dt.Data));
  },
  methods: {
    selectFactory: function (factory) {
      if (!factory.AllowAccess) {
        toastDanger("You do not have permission to access this factory.");
        return;
      }

        localStorage.setItem("SelectedFactory", factory.FactoryCode);
        this.$router.push("/app");
    },
    signOut: function () {
      clearCookies();
      localStorage.clear();
      location.href = "/auth/sign-in";
    },
  },
};
</script>

<style scoped>
.factory-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 30px;
  /* Background gradient modern */
  background: linear-gradient(135deg, #eef2f3, #dfe9f3);
}

.header {
  text-align: center;
  margin-bottom: 40px;
}

.title {
  font-weight: 700;
  margin-bottom: 4px;
}

.subtitle {
  color: #666;
}

.factory-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 25px;
  width: 100%;
  max-width: 800px;
}

.factory-card {
  padding: 35px 20px;
  border-radius: 12px;
  text-align: center;
  cursor: pointer;
  transition: 0.25s;
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.factory-card:hover {
  transform: translateY(-5px);
  border-color: #348fe2;
  box-shadow: 0 8px 22px rgba(0, 0, 0, 0.08);
}

.factory-card.no-access {
  opacity: 0.45;
  cursor: not-allowed;
  filter: grayscale(100%);
}

/* overlay lock */
.lock-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.55);
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border-radius: 12px;
  font-weight: 600;
  gap: 8px;
}

.factory-card.no-access .lock-overlay {
  opacity: 1;
}

/* tetap hover effect untuk card aktif */
.factory-card:not(.no-access):hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 22px rgba(0,0,0,0.08);
}

.icon {
  font-size: 40px;
  color: #348fe2;
  margin-bottom: 15px;
}

.name {
  font-size: 18px;
  font-weight: 600;
}

.logout-btn {
  position: fixed;
  top: 20px;
  right: 25px;
  background: #dc3545;
  border: none;
  color: white;
  padding: 10px 18px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  z-index: 999;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: 0.2s ease;
}

.logout-btn:hover {
  background: #bb2d3b;
  transform: translateY(-2px);
}
</style>
