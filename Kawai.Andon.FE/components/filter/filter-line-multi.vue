 
<template>
  <div class="row">
    <!-- =====================================================
         LINE
         ===================================================== -->
    <div class="col-xl-4 col-lg-4 col-md-6 col-sm-12">

      <div
        class="line-select"
        :class="{
          'is-disabled': isDisabled
        }"
      >

        <!-- =================================================
             SELECTED AREA
             ================================================= -->
        <div
          class="line-select-input"
          @click="toggleDropdown"
        >

          <div
            v-if="selectedItems.length === 0"
            class="line-placeholder"
          >
            {{ placeholder || "Search Line" }}
          </div>

          <div
            v-else
            class="selected-tags"
          >
            <span
              v-for="item in selectedItems"
              :key="item.LineCode"
              class="custom-tag"
            >
              {{ item.LineCode }}

              <span
                class="custom-tag-remove"
                @click.stop="removeItem(item)"
              >
                ×
              </span>
            </span>
          </div>

          <span class="line-arrow">
            {{ isOpen ? "▲" : "▼" }}
          </span>

        </div>

        <!-- =================================================
             DROPDOWN
             ================================================= -->
        <div
          v-if="isOpen"
          class="line-dropdown"
        >

          <!-- SEARCH -->
          <div class="line-search">
            <input
              ref="lineSearch"
              type="text"
              class="form-control"
              placeholder="Search Line..."
              v-model="searchText"
              @input="search(searchText)"
              @click.stop
            />
          </div>

          <!-- LOADING -->
          <div
            v-if="isLoading"
            class="line-loading"
          >
            Loading...
          </div>

          <!-- LIST -->
          <div
            v-else-if="filteredList.length > 0"
            class="line-list"
          >

            <div
              v-for="option in filteredList"
              :key="option.LineCode"
              class="line-option"
              @click.stop="toggleOption(option)"
            >

              <!-- CHECKBOX -->
              <input
                type="checkbox"
                class="line-checkbox"
                :checked="isSelected(option)"
                @click.stop
                @change="toggleOption(option)"
              />

              <!-- LINE INFO -->
              <div class="line-option-text">

                <div class="line-code">
                  {{ option.LineCode }}
                </div>

                <div class="line-name">
                  {{ option.LineName }}
                </div>

              </div>

            </div>

          </div>

          <!-- NO RESULT -->
          <div
            v-else
            class="no-result"
          >
            Line tidak ditemukan
          </div>

        </div>

      </div>

      <!-- ERROR -->
      <div
        class="invalid-feedback d-block"
        v-if="errors"
      >
        {{ errors[0] }}
      </div>

      <!-- DESCRIPTION -->
      <small
        class="form-text text-muted"
        v-if="description"
      >
        {{ description }}
      </small>

    </div>

    <!-- =====================================================
         DESCRIPTION
         ===================================================== -->
    <div class="col-xl-8 col-lg-8 col-md-6 col-sm-12">

      <input
        id="txtDescription"
        type="text"
        disabled
        :value="selectedLineNames"
        class="w-100 form-control"
      />

    </div>
  </div>
</template>

<script>
export default {
  model: {
    prop: "modelValue",
    event: "update",
  },

  emits: [
    "update:modelValue",
    "update:lineName",
  ],

  props: [
    "modelValue",
    "type",
    "label",
    "col",
    "description",
    "placeholder",
    "onSelect",
    "errors",
    "disabled",
    "multiple",
    "class",
    "includeTemp",
  ],

  data: () => ({
    isLoading: false,

    isOpen: false,

    list: [],

    tempValue: [],

    searchText: "",

    debounce: null,

    globalClickHandler: null,
  }),

  computed: {

    // =====================================================
    // CLASS
    // =====================================================
    cClass() {
      return (
        (this["class"] ?? "") +
        (this.errors
          ? " is-invalid"
          : "")
      );
    },

    // =====================================================
    // DISABLED
    // =====================================================
    isDisabled() {
      return (
        this.disabled === true ||
        this.disabled === "true"
      );
    },

    // =====================================================
    // SELECTED ARRAY
    // =====================================================
    selectedItems() {

      if (Array.isArray(this.tempValue)) {
        return this.tempValue;
      }

      if (
        this.tempValue !== null &&
        this.tempValue !== undefined &&
        this.tempValue !== ""
      ) {
        return [this.tempValue];
      }

      return [];
    },

    // =====================================================
    // SELECTED LINE NAME
    // =====================================================
    selectedLineNames() {

      return this.selectedItems
        .map(x => {
          if (
            x &&
            typeof x === "object"
          ) {
            return (
              x.LineName ||
              x.LineCode ||
              ""
            );
          }

          return x || "";
        })
        .filter(x => x)
        .join(", ");
    },

    // =====================================================
    // FILTER LIST
    // =====================================================
    filteredList() {

      if (!this.searchText) {
        return this.list;
      }

      const keyword =
        this.searchText
          .toLowerCase()
          .trim();

      return this.list.filter(x => {

        const code =
          String(
            x.LineCode || ""
          ).toLowerCase();

        const name =
          String(
            x.LineName || ""
          ).toLowerCase();

        return (
          code.includes(keyword) ||
          name.includes(keyword)
        );
      });
    },
  },

  watch: {

    // =====================================================
    // MODEL VALUE
    // =====================================================
    modelValue: {

      immediate: true,

      handler(value) {

        const codes =
          this.normalizeCodes(value);

        /*
         * Kalau sudah sama,
         * tidak perlu load ulang.
         */
        if (
          this.sameCodes(
            this.tempValue,
            codes
          )
        ) {
          return;
        }

        this.load(
          "",
          codes
        );
      },
    },
  },

  mounted() {

    /*
     * LOAD AWAL
     */
    const codes =
      this.normalizeCodes(
        this.modelValue
      );

    this.load(
      "",
      codes
    );

    /*
     * CLICK DI LUAR DROPDOWN
     */
    this.globalClickHandler =
      event => {

        if (!this.$el.contains(event.target)) {
          this.close();
        }
      };

    document.addEventListener(
      "click",
      this.globalClickHandler
    );
  },

  beforeUnmount() {

    if (this.debounce) {
      clearTimeout(
        this.debounce
      );

      this.debounce = null;
    }

    if (this.globalClickHandler) {

      document.removeEventListener(
        "click",
        this.globalClickHandler
      );

      this.globalClickHandler = null;
    }
  },

  methods: {

    // =====================================================
    // NORMALIZE
    // =====================================================
    normalizeCodes(value) {

      if (
        value === null ||
        value === undefined ||
        value === ""
      ) {
        return [];
      }

      const arr =
        Array.isArray(value)
          ? value
          : [value];

      return arr
        .map(x => {

          if (
            x &&
            typeof x === "object"
          ) {
            return x.LineCode;
          }

          return x;
        })
        .filter(
          x =>
            x !== null &&
            x !== undefined &&
            x !== ""
        );
    },

    // =====================================================
    // COMPARE
    // =====================================================
    sameCodes(a, b) {

      const aa =
        this.normalizeCodes(a)
          .map(x => String(x))
          .sort();

      const bb =
        this.normalizeCodes(b)
          .map(x => String(x))
          .sort();

      if (
        aa.length !==
        bb.length
      ) {
        return false;
      }

      return aa.every(
        (x, index) =>
          x === bb[index]
      );
    },

    // =====================================================
    // TOGGLE DROPDOWN
    // =====================================================
    toggleDropdown() {

      if (this.isDisabled) {
        return;
      }

      if (this.isOpen) {
        this.close();
      } else {
        this.open();
      }
    },

    // =====================================================
    // OPEN
    // =====================================================
    open() {

      if (this.isDisabled) {
        return;
      }

      this.isOpen = true;

      this.searchText = "";

      /*
       * Refresh data
       */
      this.load(
        "",
        null
      );

      this.$nextTick(() => {

        if (
          this.$refs.lineSearch
        ) {
          this.$refs.lineSearch.focus();
        }

      });
    },

    // =====================================================
    // CLOSE
    // =====================================================
    close() {

      this.isOpen = false;

      this.searchText = "";
    },

    // =====================================================
    // CHECK SELECTED
    // =====================================================
    isSelected(option) {

      if (!option) {
        return false;
      }

      return this.selectedItems.some(
        x => {

          const code =
            x &&
            typeof x === "object"
              ? x.LineCode
              : x;

          return (
            String(code) ===
            String(
              option.LineCode
            )
          );
        }
      );
    },

    // =====================================================
    // TOGGLE OPTION
    // =====================================================
    toggleOption(option) {

      if (
        !option ||
        this.isDisabled
      ) {
        return;
      }

      /*
       * Pastikan array
       */
      if (
        !Array.isArray(
          this.tempValue
        )
      ) {
        this.tempValue =
          this.selectedItems;
      }

      const index =
        this.tempValue.findIndex(
          x => {

            const code =
              x &&
              typeof x === "object"
                ? x.LineCode
                : x;

            return (
              String(code) ===
              String(
                option.LineCode
              )
            );
          }
        );

      if (index >= 0) {

        /*
         * UNCHECK
         */
        this.tempValue.splice(
          index,
          1
        );

      } else {

        /*
         * CHECK
         */
        this.tempValue.push(
          option
        );
      }

      this.emitValue();
    },

    // =====================================================
    // REMOVE TAG
    // =====================================================
    removeItem(item) {

      if (!item) {
        return;
      }

      const index =
        this.tempValue.findIndex(
          x =>
            String(
              x.LineCode
            ) ===
            String(
              item.LineCode
            )
        );

      if (index >= 0) {

        this.tempValue.splice(
          index,
          1
        );

        this.emitValue();
      }
    },

    // =====================================================
    // EMIT VALUE
    // =====================================================
    emitValue() {

      const selected =
        this.selectedItems;

      const lineCodes =
        selected
          .map(x => {

            if (
              x &&
              typeof x === "object"
            ) {
              return x.LineCode;
            }

            return x;
          })
          .filter(x => x);

      const lineNames =
        selected
          .map(x => {

            if (
              x &&
              typeof x === "object"
            ) {
              return (
                x.LineName ||
                x.LineCode ||
                ""
              );
            }

            const item =
              this.list.find(
                y =>
                  String(
                    y.LineCode
                  ) ===
                  String(x)
              );

            return (
              item?.LineName ||
              x ||
              ""
            );
          })
          .filter(x => x)
          .join(", ");

      /*
       * UPDATE MODEL
       */
      if (
        !this.sameCodes(
          this.modelValue,
          lineCodes
        )
      ) {

        this.$emit(
          "update:modelValue",
          lineCodes
        );
      }

      /*
       * UPDATE LINE NAME
       */
      this.$emit(
        "update:lineName",
        lineNames
      );

      /*
       * CALLBACK
       */
      if (this.onSelect) {
        this.onSelect(
          lineCodes
        );
      }
    },

    // =====================================================
    // SEARCH
    // =====================================================
    search(q) {

      this.searchText =
        q || "";

      this.load(
        this.searchText,
        null
      );
    },

    // =====================================================
    // LOAD
    // =====================================================
    load(
      q = "",
      selectedCodes = null
    ) {

      this.isLoading = true;

      if (
        this.debounce !== null
      ) {

        clearTimeout(
          this.debounce
        );
      }

      this.debounce =
        setTimeout(() => {

          const ids =
            Array.isArray(
              selectedCodes
            )
              ? selectedCodes.join(",")
              : selectedCodes || "";

          this.$http
            .get(
              `/andon/filter/ddlline?keyword=${encodeURIComponent(
                q || ""
              )}&ids=${encodeURIComponent(
                ids
              )}`
            )
            .then(p => {

              const data =
                p?.data?.Data || [];

              this.list = data;

              /*
               * Ambil selected
               * dari parent
               */
              const codes =
                this.normalizeCodes(
                  this.modelValue
                );

              if (
                codes.length > 0
              ) {

                const selected =
                  [];

                codes.forEach(
                  code => {

                    const item =
                      data.find(
                        x =>
                          String(
                            x.LineCode
                          ) ===
                          String(code)
                      );

                    if (item) {
                      selected.push(
                        item
                      );
                    }
                  }
                );

                this.tempValue =
                  selected;

              } else {

                this.tempValue =
                  [];
              }
            })
            .catch(err => {

              console.error(
                "Load Line Error:",
                err
              );

              this.list = [];
            })
            .finally(() => {

              this.isLoading =
                false;
            });

          this.debounce = null;

        }, 200);
    },
  },
};
</script>

<style scoped>
/* =====================================================
   MAIN
   ===================================================== */

.input-wrapper {
  min-width: 10em;
  width: 100%;
}

/* =====================================================
   SELECT CONTAINER
   ===================================================== */

.line-select {
  position: relative;
  width: 100%;
}

.line-select.is-disabled {
  opacity: 0.65;
  pointer-events: none;
}

/* =====================================================
   INPUT
   ===================================================== */

.line-select-input {
  min-height: 38px;

  width: 100%;

  border: 1px solid #ced4da;

  border-radius: 4px;

  background: #fff;

  display: flex;

  align-items: center;

  padding: 4px 35px 4px 8px;

  cursor: pointer;

  position: relative;

  box-sizing: border-box;
}

.line-select-input:hover {
  border-color: #adb5bd;
}

/* =====================================================
   PLACEHOLDER
   ===================================================== */

.line-placeholder {
  color: #999;

  font-size: 14px;
}

/* =====================================================
   SELECTED TAGS
   ===================================================== */

.selected-tags {
  display: flex;

  flex-wrap: wrap;

  gap: 3px;

  width: 100%;
}

/* =====================================================
   TAG
   ===================================================== */

.custom-tag {
  display: inline-flex;

  align-items: center;

  gap: 5px;

  background: #e9ecef;

  border-radius: 3px;

  padding: 3px 7px;

  font-size: 12px;

  line-height: 1.3;
}

.custom-tag-remove {
  cursor: pointer;

  font-weight: bold;

  font-size: 14px;

  line-height: 12px;
}

.custom-tag-remove:hover {
  color: #dc3545;
}

/* =====================================================
   ARROW
   ===================================================== */

.line-arrow {
  position: absolute;

  right: 10px;

  top: 50%;

  transform: translateY(-50%);

  color: #777;

  font-size: 10px;

  pointer-events: none;
}

/* =====================================================
   DROPDOWN
   ===================================================== */

.line-dropdown {
  position: absolute;

  z-index: 9999;

  top: calc(100% + 2px);

  left: 0;

  width: 100%;

  background: #fff;

  border: 1px solid #ced4da;

  border-radius: 4px;

  box-shadow:
    0 4px 12px
    rgba(0, 0, 0, 0.15);

  overflow: hidden;
}

/* =====================================================
   SEARCH
   ===================================================== */

.line-search {
  padding: 8px;

  border-bottom: 1px solid #eee;

  background: #fff;
}

.line-search .form-control {
  width: 100%;

  height: 34px;

  font-size: 13px;
}

/* =====================================================
   LIST
   ===================================================== */

.line-list {
  max-height: 280px;

  overflow-y: auto;
}

/* =====================================================
   OPTION
   ===================================================== */

.line-option {
  display: flex;

  align-items: center;

  width: 100%;

  min-height: 42px;

  padding: 7px 10px;

  box-sizing: border-box;

  cursor: pointer;

  user-select: none;
}

.line-option:hover {
  background: #f5f5f5;
}

/* =====================================================
   CHECKBOX
   ===================================================== */

.line-checkbox {
  display: block !important;

  width: 17px !important;

  height: 17px !important;

  min-width: 17px !important;

  margin: 0 10px 0 0 !important;

  padding: 0 !important;

  opacity: 1 !important;

  visibility: visible !important;

  appearance: auto !important;

  cursor: pointer;

  flex-shrink: 0;
}

/* =====================================================
   OPTION TEXT
   ===================================================== */

.line-option-text {
  display: flex;

  flex-direction: column;

  min-width: 0;

  line-height: 1.2;
}

.line-code {
  font-size: 13px;

  font-weight: 600;

  color: #333;
}

.line-name {
  font-size: 12px;

  color: #777;

  margin-top: 2px;
}

/* =====================================================
   LOADING
   ===================================================== */

.line-loading {
  padding: 12px;

  text-align: center;

  font-size: 13px;

  color: #777;
}

/* =====================================================
   NO RESULT
   ===================================================== */

.no-result {
  padding: 12px;

  text-align: center;

  font-size: 13px;

  color: #777;
}
</style>
 
