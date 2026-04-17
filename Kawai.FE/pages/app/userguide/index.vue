<template>
  <v-frame title="User Guide" icon="book">
    <template #frame-content>
      <table class="ml-2">
        <!-- Sub Menu -->
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Sub Menu</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <filter-userguide
              class="form-control"
              v-model="filter.menu"
              style="width: 250px"
            />
          </td>
        </tr>

        <!-- Find Kata -->
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Find Kata</label>
          </td>
          <td style="padding-left: 15px; padding-top: 5px">
            <input
              type="text"
              class="form-control"
              v-model="searchText"
              placeholder="Cari kata di PDF..."
              style="width: 250px"
              @keyup.enter="findText"
            />
          </td>
        </tr>

        <!-- Button -->
        <tr>
          <td colspan="2">
            <div class="d-flex flex-fill mt-3 flex-wrap gap-1">
              <v-button
                label="Preview"
                icon="file-pdf"
                cClass="btn btn-success"
                :is-loading="isLoading"
                @click="loadPdf"
              />

              <v-button
                label="Find"
                icon="search"
                cClass="btn btn-primary"
                @click="findText"
                :disabled="totalPages === 0"
              />

              <v-button
                label="Clear"
                icon="rotate-left"
                cClass="btn btn-danger"
                @click="clearPdf"
              />

              <v-button
                label="Zoom Out"
                icon="minus"
                cClass="btn btn-warning"
                @click="zoomOut"
                :disabled="totalPages === 0"
              />

              <v-button
                label="Zoom In"
                icon="plus"
                cClass="btn btn-info"
                @click="zoomIn"
                :disabled="totalPages === 0"
              />
            </div>
          </td>
        </tr>
      </table>

      <!-- Page Navigation -->
      <div v-if="totalPages > 0" class="mt-4 text-center">
        <div class="mb-2">
          <v-button
            label="Prev Page"
            icon="arrow-left"
            cClass="btn btn-secondary"
            @click="prevPage"
            :disabled="pageNum <= 1"
          />

          <span class="mx-2">
            Page {{ pageNum }} / {{ totalPages }}
          </span>

          <v-button
            label="Next Page"
            icon="arrow-right"
            cClass="btn btn-secondary"
            @click="nextPage"
            :disabled="pageNum >= totalPages"
          />
        </div>

        <div>Zoom : {{ (scale * 100).toFixed(0) }}%</div>
      </div>

      <!-- PDF -->
      <div class="pdf-container">
        <div :class="['pdf-wrapper', { active: totalPages > 0 }]">
          <canvas ref="pdfCanvas"></canvas>
        </div>
      </div>
    </template>
  </v-frame>
</template>

<script>
import * as pdfjsLib from "pdfjs-dist/legacy/build/pdf";
import workerSrc from "pdfjs-dist/build/pdf.worker.min.js?url";

pdfjsLib.GlobalWorkerOptions.workerSrc = workerSrc;

let pdfDocInstance = null;

export default {
  data() {
    return {
      filter: {
        keyword: null,
        menu: null,
      },

      pageNum: 1,
      totalPages: 0,
      isLoading: false,

      scale: 1.5,
      minScale: 0.5,
      maxScale: 3,

      searchText: "",
    };
  },

  methods: {
    async loadPdf() {
      try {
        if (!this.filter.menu) {
          toastDanger("Silahkan pilih sub menu");
          return;
        }

        this.isLoading = true;

        const url = `/file/${this.filter.menu}.pdf`;

        const loadingTask = pdfjsLib.getDocument(url);
        const pdf = await loadingTask.promise;

        pdfDocInstance = pdf;

        this.totalPages = pdf.numPages;
        this.pageNum = 1;
        this.scale = 1.5;

        this.$nextTick(() => {
          this.renderPage();
        });
      } catch (err) {
        toastDanger("File PDF Not Found");
      } finally {
        this.isLoading = false;
      }
    },

    async renderPage(highlightText = "") {
      try {
        if (!pdfDocInstance) return;

        const page = await pdfDocInstance.getPage(this.pageNum);

        const viewport = page.getViewport({
          scale: this.scale,
        });

        const canvas = this.$refs.pdfCanvas;
        if (!canvas) return;

        const context = canvas.getContext("2d");

        canvas.height = viewport.height;
        canvas.width = viewport.width;

        await page.render({
          canvasContext: context,
          viewport,
        }).promise;

        // Highlight kata
        if (highlightText) {
          const textContent = await page.getTextContent();

          context.fillStyle = "rgba(255,255,0,0.4)";

          textContent.items.forEach((item) => {
            if (
              item.str
                .toLowerCase()
                .includes(highlightText.toLowerCase())
            ) {
              const tx = pdfjsLib.Util.transform(
                viewport.transform,
                item.transform
              );

              const x = tx[4];
              const y = tx[5];
              const h = item.height * this.scale;
              const w = item.width * this.scale;

              context.fillRect(x, y - h, w, h);
            }
          });
        }
      } catch (err) {}
    },

    async findText() {
      try {
        if (!pdfDocInstance) return;

        if (!this.searchText) {
          toastDanger("Masukkan kata pencarian");
          return;
        }

        for (let i = 1; i <= this.totalPages; i++) {
          const page = await pdfDocInstance.getPage(i);
          const textContent = await page.getTextContent();

          const fullText = textContent.items
            .map((item) => item.str)
            .join(" ");

          if (
            fullText
              .toLowerCase()
              .includes(this.searchText.toLowerCase())
          ) {
            this.pageNum = i;
            await this.renderPage(this.searchText);

            toastSuccess("Kata ditemukan di halaman " + i);
            return;
          }
        }

        toastDanger("Kata tidak ditemukan");
      } catch (err) {}
    },

    nextPage() {
      if (this.pageNum < this.totalPages) {
        this.pageNum++;
        this.renderPage(this.searchText);
      }
    },

    prevPage() {
      if (this.pageNum > 1) {
        this.pageNum--;
        this.renderPage(this.searchText);
      }
    },

    zoomIn() {
      if (this.scale < this.maxScale) {
        this.scale += 0.25;
        this.renderPage(this.searchText);
      }
    },

    zoomOut() {
      if (this.scale > this.minScale) {
        this.scale -= 0.25;
        this.renderPage(this.searchText);
      }
    },

    clearPdf() {
      pdfDocInstance = null;
      this.totalPages = 0;
      this.pageNum = 1;
      this.scale = 1.5;
      this.searchText = "";

      const canvas = this.$refs.pdfCanvas;
      if (canvas) {
        canvas.width = 0;
        canvas.height = 0;
      }
    },
  },
};
</script>

<style>
thead {
  white-space: nowrap;
}

.pdf-container {
  display: flex;
  justify-content: center;
  padding: 20px;
  background: #ffffff;
}

.pdf-wrapper {
  padding: 10px;
  border: none;
  box-shadow: none;
  background: transparent;
}

.pdf-wrapper.active {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}
</style>