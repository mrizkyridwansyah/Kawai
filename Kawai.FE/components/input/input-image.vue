<template>
  <div
    class="image-input-wrapper"
    :class="{ 'error-border': errors }"
    @click="openFileInput"
  >
    <input
      type="file"
      ref="fileInput"
      class="d-none"
      accept="image/*"
      @change="handleFileChange"
    />
    <div v-if="previewUrl" class="preview">
      <img :src="previewUrl" alt="Preview" />
      <button class="remove-btn" @click.stop="removeImage" v-if="!disabled">
        ✕
      </button>
    </div>
    <div v-else class="placeholder">
      <span
        class="error-text"
        v-if="errors"
        style="font-size: small; padding: 1em"
        >{{ errors[0] }}</span
      >
      <span style="color: black; font-size: small; padding: 1em" v-else
        >Select Image</span
      >
    </div>
  </div>
</template>

<script>
export default {
  name: "ImageInput",
  props: {
    errors: { type: Object },
    modelValue: File,
    initialImage: {
      // new prop untuk base64 atau URL
      type: String,
      default: null,
    },
    disabled: {
      type: Boolean,
      default: false,
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
      if (this.disabled) return; // blok klik
      this.$refs.fileInput.click();
    },
    handleFileChange: function (e) {
      if (this.disabled) return;
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
    removeImage: function () {
      this.previewUrl = null;
      this.$refs.fileInput.value = null;
      this.$emit("update:modelValue", null);
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

.error-border {
  border-color: red !important;
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
  cursor: pointer !important;
}

.error-text {
  color: red;
  /* font-size: 12px;
  margin-top: 4px; */
}

.remove-btn {
  position: absolute;
  top: 6px;
  right: 6px;
  background: rgba(0, 0, 0, 0.6);
  color: white;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 14px;
  cursor: pointer;
  z-index: 5;
  transition: 0.2s;
}

.remove-btn:hover {
  background: rgba(255, 0, 0, 0.8);
}
</style>
