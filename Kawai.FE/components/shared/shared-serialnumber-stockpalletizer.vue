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
                <th class="text-center">Serial Number</th>		
                <th class="text-center">Status</th>			
                <th class="text-center">Scan Time</th>			
              
            
          </tr>
        </thead>
        <tbody>
     
          <tr v-for="(item, idx) in ds.dataListScan.Items">
             <td class="text-center">{{ idx + 1 }}.</td>	
                <td>{{ item.SerialNumber}}</td>		
                <td>{{ item.Status}}</td>				
                <td>{{ $func.formatDateTime(item.ScanTime) }}</td>			
             
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["productcode","whcode","period", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        SerialNumber: "asc" 
      },
      sortItems: [
        {
          label: "Serial Number",
          value: "SerialNumber",
          selected: true,
          direction: "asc",
        },
      
      ],
    },
  }),
  computed: {
    ds: function () {
      return useInventoryReportScan();
    },
  },
  watch: {
    productcode: function () {
      this.search();
    },
     whcode: function () {
      this.search();
    },
     period: function () {
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
          ProductCode: this.productcode || "",
          WarehouseCode: this.whcode || "",
          Period: this.period || "",
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
