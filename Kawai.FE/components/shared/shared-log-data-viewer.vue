<template>
  <div class="data-viewer">
    <div v-if="isObject(data)">
      <div v-for="(value, key) in data" :key="key" class="mb-2">

        <!-- Primitive -->
        <div v-if="isPrimitive(value)">
          <strong>{{ key }} : </strong>
          <span :class="getHighlightClass(key, value)">
            {{ formatValue(value) }}
          </span>
        </div>

        <!-- Object or Array -->
        <details v-else open>
          <summary class="fw-bold">{{ key }}</summary>

          <!-- Array -->
          <div v-if="Array.isArray(value)" class="ms-3 mt-2" style="overflow-x: auto;">
            <div v-if="value.length === 0">(Empty)</div>
            <table v-else class="table table-sm table-bordered">
              <thead>
                <tr>
                  <th v-for="col in Object.keys(value[0])" :key="col">{{ col }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(row, idx) in value" :key="idx">
                  <td v-for="col in Object.keys(row)" :key="col">
                    <DataViewer
                      v-if="isComplex(row[col])"
                      :data="{ [col]: row[col] }"
                      :compareData="{ [col]: getCompareValue(key, idx, col) }"
                      :mode="mode"
                    />
                    <span v-else :class="getHighlightClassFromArray(key, idx, col, row[col])">
                      {{ formatValue(row[col]) }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Nested Object -->
          <div v-else-if="isObject(value)" class="ms-3 mt-2">
            <DataViewer
              :data="value"
              :compareData="compareData ? compareData[key] : null"
              :mode="mode"
            />
          </div>
        </details>
      </div>
    </div>

    <!-- Array fallback -->
    <div v-else-if="Array.isArray(data)">
      <p>[Array with {{ data.length }} items]</p>
    </div>

    <!-- Primitive fallback -->
    <div v-else>
      <span :class="getHighlightClass(null, data)">
        {{ formatValue(data) }}
      </span>
    </div>
  </div>
</template>

<script>
export default {
  name: "DataViewer",
  props: {
    data: {
      type: [Object, Array, String, Number, Boolean, null],
      required: true,
    },
    compareData: {
      type: [Object, Array, String, Number, Boolean, null],
      default: null,
    },
    mode: {
      type: String,
      default: "before", // or "after"
    },
  },
  methods: {
    isPrimitive(value) {
      return typeof value !== "object" || value === null;
    },
    isObject(value) {
      return value !== null && !Array.isArray(value) && typeof value === "object";
    },
    isComplex(value) {
      return typeof value === "object" && value !== null;
    },
    formatValue(value) {
      if (typeof value === "string" && value.match(/^\d{4}-\d{2}-\d{2}T/)) {
        return new Date(value).toLocaleString();
      }
      if (typeof value === "number") {
        return value.toLocaleString("en-US");
      }
      return value;
    },
    getHighlightClass(key, value) {
      if (!this.compareData || key == null) return "";

      const compareVal = this.compareData[key];

      if (compareVal !== value) {
        return this.mode === "before" ? "text-danger" : "text-success";
      }
      return "";
    },
    getCompareValue(key, idx, col) {
      if (!this.compareData || !Array.isArray(this.compareData[key])) return null;
      return this.compareData[key][idx] ? this.compareData[key][idx][col] : null;
    },
    getHighlightClassFromArray(key, idx, col, value) {
      const compareVal = this.getCompareValue(key, idx, col);
      if (compareVal !== value) {
        return this.mode === "before" ? "text-danger" : "text-success";
      }
      return "";
    },
  },
};
</script>

<style scoped>
summary {
  cursor: pointer;
}
</style>
