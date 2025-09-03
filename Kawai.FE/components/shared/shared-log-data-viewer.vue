<template>
  <div class="data-viewer">
    <div v-if="isObject(data)">
      <div v-for="(value, key) in data" :key="key" class="mb-2">

        <!-- Primitive: langsung tampil tanpa collapse -->
        <div v-if="isPrimitive(value)">
          <strong>{{ key }}:</strong> {{ formatValue(value) }}
        </div>

        <!-- Array atau Object: tampil pakai <details> -->
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
                    />
                    <span v-else>{{ formatValue(row[col]) }}</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Object -->
          <div v-else-if="isObject(value)" class="ms-3 mt-2">
            <DataViewer :data="value" />
          </div>
        </details>
      </div>
    </div>

    <div v-else-if="Array.isArray(data)">
      <p>[Array with {{ data.length }} items]</p>
    </div>

    <div v-else>{{ formatValue(data) }}</div>
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
  },
};
</script>

<style scoped>
summary {
  cursor: pointer;
}
</style>
