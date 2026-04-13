<template>
  <v-frame title="User Guide" icon="book">
    <template #frame-content>
      <table class="ml-2">
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

        <tr>
          <td colspan="2">
            <div class="d-flex flex-fill mt-3">
              <v-button
                label="Preview"
                icon="file-pdf"
                cClass="ml-1 btn-green"
                :is-loading="isLoading"
                @click="loadPdf"
              />

              <v-button
                label="Clear"
                icon="rotate-left"
                cClass="btn btn-danger btn-elevate txt-light ms-1 btn-search"
                @click="clearPdf"
              />
            </div>
          </td>
        </tr>
      </table>

      <div>
        <div v-if="totalPages > 0" class="mt-4 text-center">
          <div class="mb-2">
            <v-button
              label="Prev Page"
              icon="arrow-left"
              cClass="ml-1 btn-info"
              @click="prevPage"
              :disabled="pageNum <= 1"
            />

            <span class="mx-2"> Page {{ pageNum }} / {{ totalPages }} </span>

            <v-button
              label="Next Page"
              icon="arrow-right"
              cClass="ml-1 btn-info"
              @click="nextPage"
              :disabled="pageNum >= totalPages"
            />
          </div>
        </div>
      </div>
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
  data: () => ({
    filter: {
      keyword: null,
      menu: null,
    },
    pageNum: 1,
    totalPages: 0,
    isLoading: false,
  }),

  methods: {
    async loadPdf() {
      try {
        if ((this.filter.menu || "") == "") {
          toastDanger("Silahkan pilih sub menu");
          return false;
        }

        this.isLoading = true;

        const url = this.filter.menu ? `/file/${this.filter.menu}.pdf` : "";

        console.log("Loading PDF:", url);

        const loadingTask = pdfjsLib.getDocument(url);
        const pdf = await loadingTask.promise;

        pdfDocInstance = pdf;

        this.totalPages = pdf.numPages;
        this.pageNum = 1;

        this.$nextTick(() => {
          this.renderPage();
        });
      } catch (err) {
        toastDanger("File PDF Not Found");
      } finally {
        this.isLoading = false;
      }
    },

    async renderPage() {
      try {
        if (!pdfDocInstance) return;

        const page = await pdfDocInstance.getPage(this.pageNum);

        const viewport = page.getViewport({ scale: 1.5 });

        const canvas = this.$refs.pdfCanvas;
        if (!canvas) return;

        const context = canvas.getContext("2d");

        canvas.height = viewport.height;
        canvas.width = viewport.width;

        await page.render({
          canvasContext: context,
          viewport: viewport,
        }).promise;
      } catch (err) {}
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

    clearPdf() {
      pdfDocInstance = null;
      this.totalPages = 0;
      this.pageNum = 1;

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
}

.pdf-wrapper {
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
