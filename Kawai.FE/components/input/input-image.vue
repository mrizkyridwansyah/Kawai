<template>
  <div class="image-input-wrapper" @click="openFileInput">
    <input
      type="file"
      ref="fileInput"
      class="d-none"
      accept="image/*"
      @change="handleFileChange"
    />
    <div v-if="previewUrl" class="preview">
      <img :src="previewUrl" alt="Preview" />
    </div>
    <div v-else class="placeholder">
      <span style="color: black; font-size: small; padding: 1em"
        >Select Image</span
      >
    </div>
  </div>
</template>

<script>
export default {
  name: "ImageInput",
  props: {
    modelValue: File,
    initialImage: {
      // new prop untuk base64 atau URL
      type: String,
      default: null,
    },
  },
  emits: ["update:modelValue"],
  data: () => ({
    previewUrl: null,
  }),
  watch: {
    modelValue: function (newFile) {
      if (newFile instanceof File) {
        this.createPreview(newFile);
      } else if (newFile === null) {
        this.previewUrl = this.initialImage || null;
        this.$refs.fileInput.value = null; // clear input value
      }
    },
    initialImage: function (newVal) {
      if (!this.modelValue) {
        this.previewUrl = this.getNormalizedImage(newVal);
      }
    },
  },
  mounted: function () {
    if (this.modelValue instanceof File) {
      this.createPreview(this.modelValue);
    } else if (this.initialImage) {
      this.previewUrl = this.getNormalizedImage(this.initialImage);
    }
  },
  methods: {
    getNormalizedImage(base64) {
      if (!base64) return null;
      if (base64.startsWith("data:image")) return base64;
      return `data:image/png;base64,${base64}`;
    },
    openFileInput: function () {
      this.$refs.fileInput.click();
    },
    handleFileChange: function (e) {
      const file = e.target.files[0];
      if (file) {
        this.$emit("update:modelValue", file);
        this.createPreview(file);
      }
    },
    createPreview: function (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    },
  },
};
</script>

<style scoped>
.image-input-wrapper {
  width: 200px;
  height: 200px;
  border: 2px dashed #ccc;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  overflow: hidden;
  position: relative;
}
.preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.placeholder {
  color: #999;
  font-size: 14px;
  border-radius: 1em;
}
</style>
