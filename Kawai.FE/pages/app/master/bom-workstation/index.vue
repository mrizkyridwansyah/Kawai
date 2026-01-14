<template>
  <v-frame title="BOM Per Workstation" icon="database">
    <template #frame-content>
      <div class="row">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Model Cls</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-12">
           <filter-cls-2
                    class="form-control"
                    type-data="Model_Cls"
                    placeholder="Model Cls"
                    v-model="filter.modelcls"
                      style-code="width: 120px"
                    style-desc="width: 250px"
                  />
 
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
          >Item</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-12">
          <filter-item-by-modelcls
            class="form-control"
            placeholder="Search Item"
            v-model="filter.item"
            :modelCls="filter.modelcls"
             style-code="width: 120px"
                    style-desc="width: 250px"
          />
        </div>
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
        </div>
      </div>
      <v-table :filter="filter" :keyword-keys="keywordKeys" :ds="ds">
        <template #table-content>
          <table
            class="table table-striped mb-0 align-middle"
            style="width: 100%"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Setting</th>
                <th class="text-center">WS Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Register Date</th>
                <th class="text-center">Register User</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td class="text-center">
                  <div style="justify-items: center">
                    <span
                      class="mr-2 text-primary cursor-pointer"
                      @click="
                        () =>
                          this.$router.push({
                            path: 'bom-workstation/detail',
                            query: {
                              modelcls: filter.modelcls,
                              itemcode: filter.item,
                              workstationcode: item.WorkStationCode,
                            },
                          })
                      "
                    >
                      Setting & View
                    </span>
                  </div>
                </td>
                <td>{{ item.WorkStationCode }}</td>
                <td>{{ item.WorkStationName }}</td>
                <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
                <td>{{ item.RegisterUser }}</td>
                <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                <td>{{ item.LastUser }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Master", active: false, to: "" },
      { title: "Group 2", active: false, to: "" },
      {
        title: "BOM Per Workstation",
        active: true,
        to: "/app/master/bom-workstation",
      },
    ],
    keywordKeys: [
      {
        Id: "WorkStationCode",
        Name: "WorkStationCode",
      },
      {
        Id: "WorkStationName",
        Name: "WorkStationName",
      },
    ],
    filter: {
      modelcls: null,
      item: null,
      keyword: null,
      sorts: {
        WorkStationCode: "asc",
      },
      sortItems: [
        {
          label: "WorkStation Name",
          value: "WorkStationName",
          selected: false,
          direction: "asc",
        },
        {
          label: "WorkStation Code",
          value: "WorkStationCode",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
    isLoading: false,
  }),
  computed: {
    ds: function () {
      return useBOMWorkstation();
    },
  },
  watch: {
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },

  methods: {
    search: function () {
      if (!this.filter.modelcls) {
        toastWarning("Please select model!");
        return;
      }

      if (!this.filter.item) {
        toastWarning("Please select item!");
        return;
      }
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ModelCls: this.filter.modelcls || "",
          ItemCode: this.filter.item || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.modelcls = null;
      this.filter.item = null;
      this.ds.data.Items = [];
    },
  },
};
</script>

<style>
thead {
  white-space: nowrap;
}
</style>
