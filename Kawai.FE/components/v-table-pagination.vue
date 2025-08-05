<template>
  <div :class="`row v-pagination-${size || 'xs'}`">
    <div class="col-lg-12 d-flex justify-content-between pt-3">
      <div>
        <div role="status" aria-live="polite" class="c-pagination-info">
          Showing
          {{ model.Page * model.Length - model.Length + 1 }}
          to
          {{
            (model.Page * model.Length - model.Length + 1 < model.Filtered &&
              model.Page == Math.ceil(model.Filtered / model.Length)) ||
            model.Filtered == 1
              ? model.Filtered
              : model.Page * model.Length
          }}
          of {{ model.Filtered }} entries
        </div>
      </div>
      <div class="d-flex">
        <div>
          <select
            class="form-select v-pagination-length"
            v-model="model.Length"
            @input="changeLength"
          >
            <option v-for="i in [5, 10, 25, 50, 100]" :value="i">
              {{ i }}
            </option>
          </select>
        </div>
        <div class="ms-3">
          <ul class="pagination pagination-xs">
            <li class="page-item" :class="model.Page <= 1 ? 'disabled' : null">
              <a class="page-link" href="javascript:void(0)" @click="goFirst"
                >First</a
              >
            </li>
            <li class="page-item" :class="model.Page <= 1 ? 'disabled' : null">
              <a class="page-link" href="javascript:void(0)" @click="goPrev"
                >Prev</a
              >
            </li>
            <li
              class="page-item"
              v-for="(item, index) in pages"
              :key="index"
              :class="item == model.Page ? 'active' : null"
            >
              <a
                class="page-link"
                href="javascript:void(0)"
                @click="goPage(item)"
                :disabled="disabledGoPage(item)"
              >
                <span v-if="model.Page >= 4 && item == model.Page">{{
                  item
                }}</span>
                <span
                  v-else-if="
                    (model.Page >= 4 &&
                      item == model.Page - 2 &&
                      model.Page < totalPages - 2) ||
                    (model.Page > totalPages - 3 && item == totalPages - 4)
                  "
                  >...</span
                >
                <span
                  v-else-if="
                    model.Page >= 4 &&
                    item == model.Page + 2 &&
                    model.Page < totalPages - 2
                  "
                  >...</span
                >
                <span v-else-if="model.Page <= 3 && item == 5">...</span>
                <span v-else>{{ item }}</span>
              </a>
            </li>
            <li
              class="page-item"
              :disabled="model.Page <= 1"
              :class="model.Page == totalPages ? 'disabled' : null"
            >
              <a class="page-link" href="javascript:void(0)" @click="goNext"
                >Next</a
              >
            </li>
            <li
              class="page-item"
              :disabled="model.Page <= 1"
              :class="model.Page == totalPages ? 'disabled' : null"
            >
              <a class="page-link" href="javascript:void(0)" @click="goLast"
                >Last</a
              >
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ["table", "pageChange", "lengthChange", "size"],
  data: () => ({
    model: {
      Page: 1,
      Length: 10,
      Filtered: 0,
      Total: 0,
    },
  }),
  computed: {
    totalPages: function () {
      return Math.ceil(this.model.Filtered / this.model.Length);
    },
    pages: function () {
      const total = this.totalPages;
      const current = this.model.Page;

      // Jika total halaman <= 5, tampilkan semuanya
      if (total <= 5) {
        return Array.from({ length: total }, (_, i) => i + 1);
      }

      // Jika current page di awal (1-3), tampilkan 1–5
      if (current <= 3) {
        return [1, 2, 3, 4, 5];
      }

      // Jika current page di akhir (last 3 pages), tampilkan last 5 pages
      if (current >= total - 2) {
        return [total - 4, total - 3, total - 2, total - 1, total];
      }

      // Default: current di tengah, tampilkan sekitar current page
      return [current - 2, current - 1, current, current + 1, current + 2];
    },
  },
  watch: {
    table: function (after) {
      this.model.Page = after.Page;
      this.model.Length = after.Length;
      this.model.Filtered = after.Filtered;
      this.model.Total = after.Total;
    },
  },
  mounted: function () {
    this.model.Page = this.table.Page;
    this.model.Length = this.table.Length;
    this.model.Filtered = this.table.Filtered;
    this.model.Total = this.table.Total;
  },
  methods: {
    goNext: function () {
      this.model.Page += 1;
      if (this["pageChange"]) this["pageChange"](this.model.Page);
    },
    goPrev: function () {
      this.model.Page -= 1;
      if (this["pageChange"]) this["pageChange"](this.model.Page);
    },
    goFirst: function () {
      this.model.Page = 1;
      if (this["pageChange"]) this["pageChange"](this.model.Page);
    },
    goLast: function () {
      this.model.Page = Math.ceil(this.model.Filtered / this.model.Length);
      if (this["pageChange"]) this["pageChange"](this.model.Page);
    },
    goPage: function (v) {
      if (
        (this.model.Page >= 4 &&
          v == this.model.Page - 2 &&
          this.model.Page < this.totalPages - 2) ||
        (this.model.Page > this.totalPages - 3 && v == this.totalPages - 4)
      )
        return;

      if (
        this.model.Page >= 4 &&
        v == this.model.Page + 2 &&
        this.model.Page < this.totalPages - 2
      )
        return;

      if (this.model.Page <= 3 && v == 5) return;

      this.model.Page = v;
      if (this["pageChange"]) this["pageChange"](parseInt(v));
    },
    disabledGoPage: function (v) {
      if (
        (this.model.Page >= 4 &&
          v == this.model.Page - 2 &&
          this.model.Page < this.totalPages - 2) ||
        (this.model.Page > this.totalPages - 3 && v == this.totalPages - 4)
      )
        return true;

      if (
        this.model.Page >= 4 &&
        v == this.model.Page + 2 &&
        this.model.Page < this.totalPages - 2
      )
        return true;

      if (this.model.Page <= 3 && v == 5) return true;
    },
    changeLength: function (e) {
      if (this["lengthChange"])
        this["lengthChange"](parseInt(e?.target?.value));
    },
  },
};
</script>

<style lang="scss">
.c-pagination-info {
  text-align: right;
  flex: 1;
  margin-top: 5px;
}
.page-item {
  margin-right: 0.5rem;
}
.page-link {
  position: relative;
  display: block;
  color: #5e6278;
  background-color: transparent;
  border: 0 solid transparent;
  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out,
    border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}
.page-link {
  padding: 0.375rem 0.75rem;
}
.page-item .page-link {
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 0.475rem;
  height: 2.5rem;
  min-width: 2.5rem;
  font-weight: 500;
  font-size: 1.075rem;
}
.page-item:first-child .page-link {
  border-top-left-radius: 0.475rem;
  border-bottom-left-radius: 0.475rem;
}
.page-item.disabled .page-link {
  color: #b5b5c3;
  pointer-events: none;
  background-color: transparent;
  border-color: transparent;
}
.v-pagination-length {
  min-width: 80px !important;
}
.v-pagination-xs {
  position: relative;

  .v-pagination-length {
    min-width: 65px !important;
  }

  .v-pagination-length {
    padding: 5px 10px;
    font-size: 12px;
  }

  .pagination-sm {
    --bs-pagination-padding-x: 0.5rem;
    --bs-pagination-padding-y: 0.25rem;
    --bs-pagination-font-size: 0.575rem !important;
    --bs-pagination-border-radius: 0.25rem;
  }

  .page-item .page-link {
    font-size: 14px;
    height: 30px;
    // padding: 0.2rem 0.25rem;
    padding: 2px 2px !important;
    min-width: 30px !important;
  }
  .c-pagination-info {
    font-size: 13px !important;
  }
}
</style>
