<template>
  <div class="image-input-wrapper">
    <input
      type="file"
      ref="fileInput"
      class="d-none"
      accept="image/*"
      multiple
      @change="handleFileChange"
    />
    <div class="image-grid">
      <div
        v-for="(src, index) in previewUrls"
        :key="index"
        class="image-preview"
      >
        <img :src="src" alt="Preview" />
        <button class="remove-btn" @click.stop="remove(index)">×</button>
      </div>
      <div class="image-placeholder" @click="openFileInput">
        <span>+ Add</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ImageInputMultiple",
  props: {
    modelValue: {
      type: Array, // [File, File, ...]
      default: () => [],
    },
    initialImages: {
      type: Array, // [base64 or URL strings]
      default: () => [],
    },
  },
  emits: ["update:modelValue"],
  data: () => ({
    previewUrls: [], // All previews: initial + selected
    localFiles: [], // Only selected files (File objects)
  }),
  mounted: function () {
    this.previewUrls = [...this.initialImages];
  },
  watch: {
    modelValue: function (newVal) {
      // Watch external modelValue changes if needed
    },
    initialImages: function (newImages) {
      if (this.localFiles.length === 0) {
        this.previewUrls = [...newImages];
      }
    },
  },
  methods: {
    openFileInput: function () {
      this.$refs.fileInput.click();
    },
    handleFileChange: function (e) {
      const files = Array.from(e.target.files);
      this.localFiles = [...this.localFiles, ...files];

      files.forEach((file) => {
        const reader = new FileReader();
        reader.onload = (e) => {
          this.previewUrls.push(e.target.result);
        };
        reader.readAsDataURL(file);
      });

      this.$emit("update:modelValue", this.localFiles);
      this.$refs.fileInput.value = null;
    },
    remove: function (index) {
      const isFromLocal = index >= this.initialImages.length;
      this.previewUrls.splice(index, 1);

      if (isFromLocal) {
        const fileIndex = index - this.initialImages.length;
        this.localFiles.splice(fileIndex, 1);
        this.$emit("update:modelValue", this.localFiles);
      } else {
        // Optional: handle removal from initialImages if desired
      }
    },
  },
};
</script>

<style scoped>
.image-input-wrapper {
  display: flex;
  flex-direction: column;
}
.image-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
.image-preview {
  position: relative;
  width: 120px;
  height: 120px;
  border: 2px solid #ccc;
  border-radius: 8px;
  overflow: hidden;
}
.image-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.image-placeholder {
  width: 120px;
  height: 120px;
  border: 2px dashed #ccc;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #888;
  cursor: pointer;
}
.remove-btn {
  position: absolute;
  top: 2px;
  right: 4px;
  background: rgba(255, 255, 255, 0.8);
  border: none;
  font-size: 18px;
  cursor: pointer;
  border-radius: 50%;
  width: 24px;
  height: 24px;
}
.remove-btn:hover {
  background: rgba(255, 0, 0, 0.7);
  color: white;
}
</style>
