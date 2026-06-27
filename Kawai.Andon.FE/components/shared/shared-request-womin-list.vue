<template>
  <v-table-scan
    :filter="filter"
    :ds="ds"
    :ds-data="ds.dataListScan"
    :ds-page="ds.setPageListScan"
    :ds-length="ds.setLengthListScan"
    :ds-load="ds.loadListScan"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
                <th class="text-center">Child Item Code</th>		
                <th class="text-center">Child Item Name</th>			
                <th class="text-center">Requirement Qty</th>			
                <th class="text-center">Total Scan</th>		
                <th class="text-center">Status</th>	
            
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.dataListScan.Items">
             <td class="text-center">{{ idx + 1 }}.</td>	
                <td>{{ item.ChildItemCode}}</td>		
                <td>{{ item.ChildItemName}}</td>				
                <td class="text-right">{{ item.RequirementQty}}</td>				
                <td class="text-right">{{ item.TotalScan}}</td>		
                <td>{{ item.Status}}</td>	
          </tr>
        </tbody>
      </table>
    </template>
  </v-table-scan>
</template>

<script>
export default {
  props: ["refno","groupclass", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        ChildItemCode: "asc",
        ChildItemName: "asc" 
      },
      sortItems: [
        {
          label: "Child Item Code",
          value: "ChildItemCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Child Item Name",
          value: "ChildItemName",
          selected: true,
          direction: "asc",
        },
         
         
      ],
    },
  }),
  computed: {
    ds: function () {
      return useSupplyRequestWomin();
    },
  },
  watch: {
    refno: function () {
      this.search();
    },
    groupclass: function () {
      this.search();
    },
    counter: function () {
      this.search();
    },
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    search: function () {
      this.ds.setSortListScan(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          RefNo: this.refno || "",
          GroupClass: this.groupclass || "",
        },
      ];

      this.ds.setFilterListScan(filters);
      this.$nextTick(() => this.ds.loadListScan());
    },
    reset: function () {
      this.item = null;
      this.search();
    },
  },
};
</script>
