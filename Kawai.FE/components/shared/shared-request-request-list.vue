<template>
  <v-table
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
                <th class="text-center">Item Code</th>		
                <th class="text-center">Item Name</th>			
                <th class="text-center">Request Qty</th>			
                <th class="text-center">Send Qty</th>		
                <th class="text-center">Status</th>	
            
          </tr>
        </thead>
        <tbody>
     
          <tr v-for="(item, idx) in ds.dataListScan.Items">
             <td class="text-center">{{ idx + 1 }}.</td>	
                <td>{{ item.ItemCode}}</td>		
                <td>{{ item.ItemName}}</td>				
                <td class="text-right">{{ item.RequestQty}}</td>				
                <td class="text-right">{{ item.SendQty}}</td>		
                <td>{{ item.Status}}</td>	
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["requestno", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        ItemCode: "asc",
        ItemName: "asc" 
      },
      sortItems: [
        {
          label: "Item Code",
          value: "ItemCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Item Name",
          value: "ItemName",
          selected: true,
          direction: "asc",
        },
         
         
      ],
    },
  }),
  computed: {
    ds: function () {
      return usePartMaterialRequestOthersScan();
    },
  },
  watch: {
    requestno: function () {
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
       debugger;
      this.ds.setSortListScan(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          RequestNo: this.requestno || "",
        },
      ];

      this.ds.setFilterListScan(filters);
      this.$nextTick(() => this.ds.loadListScan());
    },
    reset: function () {
       this.search();
    },
  },
};
</script>
