<!-- components/v-tree.vue -->
<template>
  <div class="mt-4">
    <div class="panel panel-inverse">
      <div class="panel-body">
        <table
          class="tree-table table table-striped mb-0 align-middle"
        >
          <thead>
            <tr>
              <th
                v-for="col in columns"
                :key="col.dataField"
                :style="{
                  width: col.width || 'auto',
                  textAlign: col.align || 'left',
                }"
              >
                {{ col.text }}
              </th>
            </tr>
          </thead>
          <tbody>
            <v-tree-row
              v-for="row in treeData"
              :key="row[childKey]"
              :node="row"
              :level="0"
              :columns="columns"
              :child-key="childKey"
            />
          </tbody>
        </table>
        <v-loading-2 class="m-5 p-5" v-if="isLoading" />
        <div>
          <v-data-empty
            class="mt-3"
            v-if="
              !isLoading &&
              treeData.length == 0 &&
              !isNetworkError &&
              !isServerError
            "
          />
          <v-error-server class="mt-3" v-if="!isLoading && isServerError" />
          <v-error-network class="mt-3" v-if="!isLoading && isNetworkError" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "VTree",
  props: {
    isLoading: { type: Boolean, default: false },
    isServerError: { type: Boolean, default: false },
    isNetworkError: { type: Boolean, default: false },
    treeData: {
      type: Array,
      required: true,
    },
    columns: {
      type: Array,
      required: true,
    },
    childKey: {
      type: String,
      required: true
    },
  },
};
</script>

<style scoped>
.tree-table {
  width: 100%;
  border-collapse: collapse;
}
/* .tree-table th,
.tree-table td {
  border: 1px solid #ccc;
  padding: 4px 8px;
} */
</style>
