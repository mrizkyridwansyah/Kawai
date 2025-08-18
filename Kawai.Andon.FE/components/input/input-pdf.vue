<template>
  <div class="pdf-input-wrapper" @click="openFileInput">
    <input
      type="file"
      ref="fileInput"
      class="d-none"
      accept="application/pdf"
      @change="handleFileChange"
    />
    <div v-if="previewUrl" class="preview" style="height: 85%; width: 85%;">
      <iframe :src="previewUrl" frameborder="0"></iframe>
    </div>
    <div v-else class="placeholder">
      <span style="color: black; font-size: small; padding: 1em;">Select PDF</span>
    </div>
  </div>
</template>

<script>
export default {
  name: "PdfInput",
  props: {
    modelValue: File,
    initialPdf: {
      type: String, // base64 or URL
      default: null,
    },
  },
  emits: ["update:modelValue"],
  data: () => ({
    previewUrl: null,
  }),
  watch: {
    modelValue(newFile) {
      if (newFile instanceof File) {
        this.createPreview(newFile);
      } else if (newFile === null) {
        this.previewUrl = this.initialPdf || null;
        this.$refs.fileInput.value = null;
      }
    },
    initialPdf(newVal) {
      if (!this.modelValue) {
        this.previewUrl = newVal;
      }
    },
  },
  mounted() {
    if (this.modelValue instanceof File) {
      this.createPreview(this.modelValue);
    } else if (this.initialPdf) {
      this.previewUrl = this.initialPdf;
    }
  },
  methods: {
    openFileInput() {
      this.$refs.fileInput.click();
    },
    handleFileChange(e) {
      const file = e.target.files[0];
      if (file) {
        this.$emit("update:modelValue", file);
        this.createPreview(file);
      }
    },
    createPreview(file) {
      const url = URL.createObjectURL(file);
      this.previewUrl = url;
    },
  },
};
</script>

<style scoped>
.pdf-input-wrapper {
  width: 100%;
  max-width: 400px;
  height: 400px;
  border: 2px dashed #ccc;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  overflow: hidden;
  position: relative;
}

.preview iframe {
  width: 100%;
  height: 100%;
}

.placeholder {
  color: #999;
  font-size: 14px;
  text-align: center;
}
</style>
