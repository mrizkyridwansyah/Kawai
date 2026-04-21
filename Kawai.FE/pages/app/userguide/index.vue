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
              class="form-control"
              v-model="searchText"
              placeholder="Cari kata..."
              style="width: 250px"
              @keyup.enter="findText"
            />
          </td>
        </tr>

        <!-- Button -->
        <tr>
          <td colspan="2">
            <div class="d-flex flex-wrap gap-1 mt-3">
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
                :disabled="totalPages === 0"
                @click="findText"
              />

              <v-button
                label="Prev Result"
                icon="arrow-left"
                cClass="btn btn-info"
                :disabled="searchResults.length === 0"
                @click="prevResult"
              />

              <v-button
                label="Next Result"
                icon="arrow-right"
                cClass="btn btn-info"
                :disabled="searchResults.length === 0"
                @click="nextResult"
              />

              <v-button
                label="-"
                icon="minus"
                cClass="btn btn-warning"
                @click="zoomOut"
              />

              <v-button
                label="+"
                icon="plus"
                cClass="btn btn-warning"
                @click="zoomIn"
              />

              <v-button
                label="Clear"
                icon="rotate-left"
                cClass="btn btn-danger"
                @click="clearPdf"
              />
            </div>
          </td>
        </tr>
      </table>

      <!-- Navigation -->
      <div v-if="totalPages > 0" class="mt-4 text-center">
        <div class="mb-2">
          <v-button
            label="Prev Page"
            icon="arrow-left"
            cClass="btn btn-secondary"
            :disabled="pageNum <= 1"
            @click="prevPage"
          />

          <span class="mx-2">
            Page {{ pageNum }} / {{ totalPages }}
          </span>

          <v-button
            label="Next Page"
            icon="arrow-right"
            cClass="btn btn-secondary"
            :disabled="pageNum >= totalPages"
            @click="nextPage"
          />
        </div>

        <div>
          Zoom {{ (scale * 100).toFixed(0) }}%
          <span v-if="searchResults.length">
            | Result {{ currentResultIndex + 1 }} /
            {{ searchResults.length }}
          </span>
        </div>
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
        menu: null,
      },

      pageNum: 1,
      totalPages: 0,
      isLoading: false,

      scale: 1.5,
      minScale: 0.5,
      maxScale: 3,

      searchText: "",

      searchResults: [],
      currentResultIndex: 0,
    };
  },

  methods: {
    normalizeText(text) {
      return text
        .toLowerCase()
         
    },

    async loadPdf() {
      try {
        if (!this.filter.menu) {
          toastDanger("Silahkan pilih sub menu");
          return;
        }

        this.isLoading = true;

        const url = `/file/${this.filter.menu}.pdf`;

        const loadingTask = pdfjsLib.getDocument(url);
        pdfDocInstance = await loadingTask.promise;

        this.totalPages = pdfDocInstance.numPages;
        this.pageNum = 1;
        this.scale = 1.5;

        this.searchResults = [];
        this.currentResultIndex = 0;

        await this.renderPage();
      } catch (err) {
        toastDanger("File PDF Not Found");
      } finally {
        this.isLoading = false;
      }
    },

async renderPage() {
  if (!pdfDocInstance) return;

  const page = await pdfDocInstance.getPage(this.pageNum);

  const viewport = page.getViewport({
    scale: this.scale,
  });

  const canvas = this.$refs.pdfCanvas;
  const context = canvas.getContext("2d");

  canvas.width = viewport.width;
  canvas.height = viewport.height;

  await page.render({
    canvasContext: context,
    viewport,
  }).promise;

  if (!this.searchText) return;

  const keyword = this.normalizeText(this.searchText);

  const textContent = await page.getTextContent();

  context.save();
  context.globalAlpha = 0.35;
  context.fillStyle = "yellow";
  context.globalCompositeOperation = "multiply";

  textContent.items.forEach((item) => {
    const raw = item.str;

    const words = raw.split(/\s+/);

    if (!words.length) return;

    const tx = pdfjsLib.Util.transform(
      viewport.transform,
      item.transform
    );

    const startX = tx[4];
    const y = tx[5];

    const totalWidth = item.width * this.scale;
    const h = item.height * this.scale;

    const avgWidth = totalWidth / raw.length;

    let currentIndex = 0;

    words.forEach((word) => {
      const cleanWord = this.normalizeText(word);

      const startChar = raw.indexOf(word, currentIndex);

      if (startChar < 0) return;

      const wordWidth = word.length * avgWidth;
      const x = startX + (startChar * avgWidth);

      if (cleanWord === keyword) {
        context.fillRect(x, y - h, wordWidth, h);
      }

      currentIndex = startChar + word.length;
    });
  });

  context.restore();
},

    async findText() {
      if (!pdfDocInstance) return;

      if (!this.searchText.trim()) {
        toastDanger("Masukkan kata pencarian");
        return;
      }

      const keyword = this.normalizeText(this.searchText);

      this.searchResults = [];
      this.currentResultIndex = 0;

      for (let p = 1; p <= this.totalPages; p++) {
        const page = await pdfDocInstance.getPage(p);
        const textContent = await page.getTextContent();

        textContent.items.forEach((item) => {
          const text = this.normalizeText(item.str);

          if (text.includes(keyword)) {
            this.searchResults.push({
              page: p,
              keyword,
            });
          }
        });
      }

      if (!this.searchResults.length) {
        toastDanger("Kata tidak ditemukan");
        return;
      }

      await this.gotoResult(0);

      toastSuccess(
        "Ditemukan " + this.searchResults.length + " hasil"
      );
    },

    async gotoResult(index) {
      this.currentResultIndex = index;

      const result = this.searchResults[index];

      this.pageNum = result.page;

      await this.renderPage();
    },

    prevResult() {
      let i = this.currentResultIndex - 1;

      if (i < 0) i = this.searchResults.length - 1;

      this.gotoResult(i);
    },

    nextResult() {
      let i = this.currentResultIndex + 1;

      if (i >= this.searchResults.length) i = 0;

      this.gotoResult(i);
    },

    nextPage() {
      if (this.pageNum < this.totalPages) {
        this.pageNum++;
        this.renderPage();
      }
    },

    prevPage() {
      if (this.pageNum > 1) {
        this.pageNum--;
        this.renderPage();
      }
    },

    zoomIn() {
      if (this.scale < this.maxScale) {
        this.scale += 0.25;
        this.renderPage();
      }
    },

    zoomOut() {
      if (this.scale > this.minScale) {
        this.scale -= 0.25;
        this.renderPage();
      }
    },

    clearPdf() {
      pdfDocInstance = null;

      this.pageNum = 1;
      this.totalPages = 0;
      this.scale = 1.5;

      this.searchText = "";
      this.searchResults = [];
      this.currentResultIndex = 0;

      const canvas = this.$refs.pdfCanvas;

      if (canvas) {
        canvas.width = 0;
        canvas.height = 0;
      }
    },
  },
};
</script>

<style scoped>
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
}

.pdf-wrapper.active {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}
</style>