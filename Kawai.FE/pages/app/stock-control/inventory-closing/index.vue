<template>
  <v-frame title="Inventory Closing" icon="boxes-stacked">
    <template #frame-content>
      <div class="page-center">
        <div class="content-box">
          <!-- Filter -->
          <div class="card p-3 mb-4">
            <div class="row align-items-center justify-content-center">
              <div class="col-auto">
                <label class="fw-bold mb-0">Period</label>
              </div>

              <div class="col-auto">
                <input
                  type="month"
                  class="form-control"
                  v-model="period"
                  :disabled="isProcessing"
                />
              </div>

              <div class="col-auto">
                <button
                  class="btn btn-primary"
                  :disabled="isProcessing"
                  @click="process"
                >
                  Process
                </button>
              </div>
            </div>
          </div>

          <!-- Progress Bar -->
          <div class="d-flex justify-content-center">
            <div class="progress progress-custom">
              <div
                class="progress-bar bg-success"
                :style="{ width: progress + '%' }"
              >
                {{ progress }}%
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </v-frame>
</template>
<script>
export default {
  data() {
    return {
      progress: 0,
      isProcessing: false,
    };
  },

  computed: {
    // inject store (Nuxt auto-import)
    ds: function () {
      return useInventoryClosing();
    },

    // period dari fungsi store
    period: {
      get() {
        return this.ds.getPeriod();
      },
      set(val) {
        // guard WAJIB
        if (!this.ds.data) return;
        this.ds.data.Period = val;
      },
    },
  },

  mounted() {
    // load sekali saja
    if (!this.ds.isLoaded) {
      this.ds.load();
    }
  },
  methods: {
    async process() {
      if (!this.period) return;

      this.isProcessing = true;
      this.progress = 0;

      let timer = null; // ⬅️ penting

      try {
        // UX progress
        timer = setInterval(() => {
          if (this.progress < 90) this.progress += 10;
        }, 300);

        await this.ds.processClosing();

        // SUCCESS
        this.progress = 100;
      } catch (e) {
        // ❌ ERROR → stop progress
        this.progress = 0;

        const msg =
          e?.response?.data?.Message ||
          e?.response?.data?.error ||
          e?.message ||
          "Closing gagal";

        toastDanger(msg);
      } finally {
        // 🔥 PASTIKAN TIMER MATI
        if (timer) {
          clearInterval(timer);
          timer = null;
        }
        this.isProcessing = false;
      }
    },
  },
};
</script>

<style scoped>
/* CENTER OF SCREEN */
.page-center {
  min-height: calc(100vh - 165px);
  display: flex;
  justify-content: center; /* horizontal */
  align-items: center; /* vertical */
}

/* CONTENT */
.content-box {
  width: 100%;
  max-width: 70%;
}

/* PROGRESS */
.progress-custom {
  width: 100%;
  height: 24px;
  background-color: #dcdcdc;
  border-radius: 20px;
}
.progress-bar {
  font-weight: bold;
}
</style>
