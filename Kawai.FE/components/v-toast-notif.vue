<template>
  <div
    class="toast"
    role="alert"
    aria-live="assertive"
    aria-atomic="true"
    data-bs-autohide="true"
    @click="redirectToUrl"
  >
    <div class="toast-header">
      <svg
        class="bd-placeholder-img rounded me-2"
        width="20"
        height="20"
        xmlns="http://www.w3.org/2000/svg"
        aria-hidden="true"
        preserveAspectRatio="xMidYMid slice"
        focusable="false"
      >
        <rect width="100%" height="100%" :fill="color"></rect>
      </svg>
      <strong class="me-auto">
        <span>{{ notif.title }}</span>
      </strong>
      <!-- <small class="text-muted">just now</small> -->
      <button
        type="button"
        class="btn-close"
        data-bs-dismiss="toast"
        aria-label="Close"
      ></button>
    </div>
    <div class="toast-body">
      <!-- <v-icon 
        name="check-circle"
        color="#00e396"
        width="40"
      /> -->
      <span>{{ notif.description }}</span>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    notif: {
      type: Object,
      required: true
    }
  },
  computed: {
    color: function () {
      if (this.notif.notifType.toLowerCase() == "info") return "#00e396";

      if (this.notif.notifType.toLowerCase() == "error") return "#f44336";

      if (this.notif.notifType.toLowerCase() == "warning") return "yellow";

      return "#fff";
    },
  },
  methods: {
    redirectToUrl: function () {
      if (this.notif?.urlRedirect) {
        location.href = "/app" + this.notif.urlRedirect;
      }
    },
  },
};
</script>

<style>
.toast-container {
  z-index: 1500;
}
</style>
