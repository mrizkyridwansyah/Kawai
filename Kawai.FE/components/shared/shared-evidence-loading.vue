<template>
  <v-modal
    :id="id || 'shared-evidence-loading'"
    :title="title || 'Detail Attachment'"
    :size="size || 'xl'"
  >
    <div class="shared-evidence-loading-container p-2">
      <!-- Loading State -->
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <p class="mt-2 text-muted">Memuat gambar evidence...</p>
      </div>

      <!-- Empty State -->
      <div
        v-else-if="!images || images.length === 0"
        class="text-center py-5 text-muted"
      >
        <v-icon
          name="image"
          width="48"
          height="48"
          class="mb-2 text-secondary"
        />
        <p>Tidak ada evidence gambar yang tersedia.</p>
      </div>

      <!-- Main Display -->
      <div v-else>
        <!-- Top Toolbar with Info and Download Button -->
        <div
          class="d-flex justify-content-between align-items-center mb-2 px-1"
        >
          <div class="fw-semibold text-secondary" style="font-size: 14px">
            Gambar {{ selectedIndex + 1 }} dari {{ images.length }}
          </div>
          <button
            class="btn btn-sm btn-outline-primary d-inline-flex align-items-center gap-1"
            :disabled="isDownloading"
            @click="downloadAllAttachments"
          >
            <span
              v-if="isDownloading"
              class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
            ></span>
            <v-icon v-else name="download" width="14" height="14" />
            <span>{{
              isDownloading ? "Downloading..." : "Download All Attachments"
            }}</span>
          </button>
        </div>

        <!-- Main Image Preview Container -->
        <div class="main-image-wrapper text-center position-relative mb-3">
          <img
            :src="getFullImageUrl(images[selectedIndex])"
            alt="Evidence Detail"
            class="img-fluid rounded border shadow-sm main-evidence-img"
            @error="onImageError"
          />
        </div>

        <!-- Thumbnail List -->
        <div
          class="thumbnail-strip d-flex justify-content-center align-items-center gap-2 overflow-auto py-2"
        >
          <div
            v-for="(img, index) in images"
            :key="index"
            class="thumbnail-item position-relative cursor-pointer"
            :class="{ 'selected-thumb': index === selectedIndex }"
            @click="selectedIndex = index"
          >
            <img
              :src="getFullImageUrl(img)"
              alt="Thumbnail"
              class="thumb-img rounded border"
              @error="onImageError"
            />
            <!-- Selected Indicator Checkmark -->
            <div v-if="index === selectedIndex" class="selected-badge">
              <svg
                width="12"
                height="12"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="3"
                stroke-linecap="round"
                stroke-linejoin="round"
              >
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
            </div>
          </div>
        </div>
      </div>
    </div>
  </v-modal>
</template>

<script>
export default {
  name: "SharedEvidenceLoading",
  props: {
    id: {
      type: String,
      default: "shared-evidence-loading",
    },
    title: {
      type: String,
      default: "Detail Attachment",
    },
    size: {
      type: String,
      default: "xl",
    },
    // Array of image paths/URLs (or objects containing FilePath / FileName / url)
    items: {
      type: Array,
      default: () => [],
    },
    // Direct API Base URL override if needed
    baseUrl: {
      type: String,
      default: "",
    },
    loading: {
      type: Boolean,
      default: false,
    },
    // Optional custom download handler function returning a Promise/Blob or triggering download
    downloadHandler: {
      type: Function,
      default: null,
    },
  },
  data() {
    return {
      selectedIndex: 0,
      isDownloading: false,
    };
  },
  emits: ["download"],
  computed: {
    images() {
      if (!this.items || !Array.isArray(this.items)) return [];
      return this.items;
    },
    apiBase() {
      if (this.baseUrl) return this.baseUrl;
      const config = useRuntimeConfig();
      return config.public?.apiBase || "http://localhost:9100";
    },
  },
  watch: {
    items: {
      handler(val) {
        if (val && val.length > 0) {
          this.selectedIndex = 0;
        }
      },
      immediate: true,
      deep: true,
    },
  },
  methods: {
    getFullImageUrl(item) {
      if (!item) return "";

      let path = "";
      if (typeof item === "string") {
        path = item;
      } else if (typeof item === "object") {
        path =
          item.FilePath ||
          item.filePath ||
          item.url ||
          item.Url ||
          item.FileName ||
          item.fileName ||
          "";
      }

      if (!path) return "";

      // Absolute HTTP/HTTPS URLs
      if (path.startsWith("http://") || path.startsWith("https://")) {
        return path;
      }

      // Ensure leading slash
      const formattedPath = path.startsWith("/") ? path : `/${path}`;
      const base = this.apiBase.endsWith("/")
        ? this.apiBase.slice(0, -1)
        : this.apiBase;

      return `${base}${formattedPath}`;
    },
    onImageError(e) {
      // Fallback placeholder when image fails to load
      e.target.src =
        "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='400' height='300' viewBox='0 0 400 300'><rect width='100%' height='100%' fill='%23f8f9fa'/><text x='50%' y='50%' font-family='sans-serif' font-size='16' fill='%236c757d' text-anchor='middle' dominant-baseline='middle'>Image Not Found</text></svg>";
    },
    async downloadAllAttachments() {
      if (this.isDownloading) return;
      this.isDownloading = true;

      try {
        if (typeof this.downloadHandler === "function") {
          await this.downloadHandler();
          return;
        }

        this.$emit("download");

        if (!this.images || this.images.length === 0) return;

        for (let i = 0; i < this.images.length; i++) {
          const item = this.images[i];
          const fullUrl = this.getFullImageUrl(item);
          if (!fullUrl) continue;

          let filename = `evidence_${i + 1}.jpg`;
          if (typeof item === "string") {
            filename = item.split("/").pop() || filename;
          } else if (typeof item === "object") {
            filename =
              item.FileName ||
              item.fileName ||
              (item.FilePath ? item.FilePath.split("/").pop() : null) ||
              filename;
          }

          await this.downloadSingleFile(fullUrl, filename);
          await new Promise((r) => setTimeout(r, 300));
        }
      } catch (err) {
        console.error("Error downloading attachments:", err);
      } finally {
        this.isDownloading = false;
      }
    },
    async downloadSingleFile(url, filename) {
      return new Promise((resolve) => {
        const img = new Image();
        img.crossOrigin = "anonymous";
        img.onload = () => {
          try {
            const canvas = document.createElement("canvas");
            canvas.width = img.naturalWidth || img.width;
            canvas.height = img.naturalHeight || img.height;
            const ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0);

            canvas.toBlob(
              (blob) => {
                if (blob) {
                  const blobUrl = window.URL.createObjectURL(blob);
                  const link = document.createElement("a");
                  link.href = blobUrl;
                  link.download = filename;
                  document.body.appendChild(link);
                  link.click();
                  document.body.removeChild(link);
                  setTimeout(() => window.URL.revokeObjectURL(blobUrl), 1000);
                }
                resolve();
              },
              "image/jpeg",
              0.95,
            );
          } catch (e) {
            console.error("Canvas export failed:", e);
            resolve();
          }
        };
        img.onerror = () => {
          // If canvas fails (e.g. tainted canvas due to missing CORS header on static files),
          // fallback to hidden iframe download
          this.downloadViaIframe(url, filename);
          resolve();
        };
        img.src = url;
      });
    },
    downloadViaIframe(url, filename) {
      const iframe = document.createElement("iframe");
      iframe.style.display = "none";
      iframe.src = url;
      document.body.appendChild(iframe);
      setTimeout(() => {
        document.body.removeChild(iframe);
      }, 3000);
    },
  },
};
</script>

<style scoped>
.shared-evidence-loading-container {
  min-height: 300px;
}

.main-image-wrapper {
  background-color: #f8f9fa;
  border-radius: 8px;
  padding: 10px;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 380px;
  max-height: 550px;
}

.main-evidence-img {
  max-height: 520px;
  object-fit: contain;
  width: auto;
  max-width: 100%;
}

.thumbnail-strip {
  max-width: 100%;
  padding-bottom: 8px;
}

.thumbnail-item {
  width: 70px;
  height: 70px;
  border-radius: 6px;
  padding: 2px;
  transition: all 0.2s ease-in-out;
}

.thumb-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.thumbnail-item:hover {
  transform: translateY(-2px);
  opacity: 0.9;
}

.selected-thumb {
  border: 2px solid #0d6efd;
  box-shadow: 0 0 0 2px rgba(13, 110, 253, 0.25);
  border-radius: 6px;
}

.selected-badge {
  position: absolute;
  top: -6px;
  right: -6px;
  background-color: #0d6efd;
  color: white;
  border-radius: 50%;
  width: 18px;
  height: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid white;
}
</style>
