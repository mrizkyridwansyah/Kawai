<template>
  <div>
    <div class="mb-3" style="display: none">
      <input-text v-model="model.Id" />
    </div>

    <div class="mb-3">
      <input-image
        v-model="model.QC_Photo"
        :initial-image="model.ImageBase64"
      />
    </div>
  </div>
  <div></div>
</template>
<script>
export default {
  props: ["id", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    model: {
      QC_Photo: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useQualityCheck();
    },
  },
  mounted: function () {
    if (this.mode === "edit" && this.id) {
      this.loadDetailPhoto(this.id);
    } else if (this.mode === "add") {
      this.resetForm();
    }
  },
  watch: {
    mode: function (val) {
      if (val === "edit") {
        this.loadDetailPhoto(this.id);
      } else {
        this.resetForm();
      }
    },
  },
  methods: {
    loadDetailPhoto: function () {
      this.ds.loadDetailPhoto(this.id).then((dt) => (this.model = dt.Data));
    },
    resetForm: function () {
      this.model = {
        QC_Photo: null,
      };
      this.errors = {}; // Reset errors
    },
  },
};
</script>
