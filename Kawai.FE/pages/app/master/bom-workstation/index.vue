<template>
  <v-frame title="BOM Per Workstation" icon="database">
    <template #frame-content>

      <table width="100%">
  <tr  style="height: 38px;">
    <td style="width: 10%;"><label class="form-label">Factory</label></td>
    <td style="width: 34%;">
      <filter-factory-privileges
                  class="form-control"
                  v-model="filter.factory"
                  style-code="width: 110px"
                    style-desc="width: 250px"
                /></td>
     <td style="width: 1%;"></td>
    <td style="width: 10%;"><label class="form-label">Model Cls</label></td>
    <td style="width: 34%;"> <filter-cls-2
                    type-data="Model_Cls"
                     class="form-control"
                    v-model="filter.modelcls"
                    
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  /> </td>
   
     <td style="width: 10%;"></td>
  </tr>
  <tr  style="height: 38px">
    <td style="width: 10%;"><label class="form-label">Process</label></td>
    <td style="width: 34%;">
      <filter-trade-2
                    class="form-control"
                    placeholder=" "
                    v-model="filter.supplier"
                    
                    :trade-cls="['1']"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  /></td>
     <td style="width: 1%;"></td>
    <td style="width: 10%;"><label class="form-label">Item</label></td>
    <td style="width: 34%;"> <filter-item-by-modelcls
       class="form-control"
                      v-model="filter.item"
                    :modelCls="filter.modelcls"
                    
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  /></td>
   
     <td style="width: 10%;"></td>
  </tr>
  <tr  style="height: 38px">
    <td style="width: 10%;"><label class="form-label">Line</label></td>
    <td style="width: 34%;">
      <filter-line-factory
       class="form-control"
                       :company="filter.factory"
                      :manufacture="filter.supplier"
                        
                      v-model="filter.linecode"
                      style-code="width: 110px"
                        style-desc="width: 250px"
                    /></td>
     <td style="width: 1%;"></td>
    <td style="width: 10%;"></td>
    <td style="width: 34%;">   </td>
   
     <td style="width: 10%;"></td>
  </tr>
      </table>
      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
          <v-button
                 :action="copy"
                label="Copy Bom Workstation"
                icon="copy"
                cClass="ml-1 btn-green"
              />
        </div>
      </div>
      <v-table :filter="filter" :keyword-keys="keywordKeys" :ds="ds">
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle"
            style="width: 100%"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Setting</th>
                <th class="text-center">WS Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Trolley Cls</th>
                <th class="text-center">Max Qty Set</th>
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
                            path: '/app/master/bom-workstation/detail',
                            query: {
                              factory: filter.factory,
                              process: filter.supplier,
                              line: filter.linecode,
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
                <td>{{ item.TrolleyCls }}</td>
                <td>{{ item.MaxQtySet }}</td>
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
  <v-modal
    ref="modalCopyBom"
    id="modal-form-copybom"
    :title="title"
    size="xl"
    @hidden="
      () => {
        this.$refs.formCopyBom.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-copybom
      ref="formCopyBom"
      :id="idSelected"
      :mode="modalMode"
      :factory="filter.factory"
      :process="filter.supplier"
      :line="filter.linecode"
      :item="filter.item"
      @submitted="close"
    />
  </v-modal>
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
   mounted() {
  const q = this.$route.query;

   
  if (q && Object.keys(q).length > 0 && q.itemcode) {
    this.filter.factory = q.factory;
    this.filter.supplier = q.process;
    this.filter.linecode = q.line;
    this.filter.modelcls = q.modelcls;
    this.filter.item = q.itemcode;

    this.ds.setSort(this.filter.sorts);

    const filters = [
      {
        Keyword: this.filter.keyword || "",
        Line: this.filter.linecode || "",
        ModelCls: this.filter.modelcls || "",
        ItemCode: this.filter.item || "",
      },
    ];

    this.ds.setFilter(filters);
    this.ds.load();

    return;  
  }
 
},
  methods: {
    search: function () {
      if (!this.filter.supplier) {
        toastWarning("Please select process!");
        return;
      }
      if (!this.filter.linecode) {
        toastWarning("Please select line!");
        return;
      }
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
          Line: this.filter.linecode || "",
          ModelCls: this.filter.modelcls || "",
          ItemCode: this.filter.item || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.supplier = null;
      this.filter.linecode = null;
      this.filter.modelcls = null;
      this.filter.item = null;
      this.ds.data.Items = [];
    },
    copy: function () {
      if (!this.filter.supplier) {
        toastWarning("Please choose process!");
        return;
      }

      if (!this.filter.linecode) {
        toastWarning("Please choose line!");
        return;
      }

       if (!this.filter.item) {
        toastWarning("Please choose item!");
        return;
      }

      this.title = "Copy Bom Workstation";
      this.modalMode = "add";
      this.$bvModal.show("modal-form-copybom");
    },
    close: function () {
      this.$bvModal.hide("modal-form-copybom");
      this.search();
    },

  },
};
</script>

<style>
thead {
  white-space: nowrap;
}
</style>
