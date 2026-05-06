<template>
<v-frame title="Traceability" icon="receipt">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Item Cls</label>
          <input-cls type-data="ItemFinishGoodCls" class="form-control" v-model="filter.itemCls"
           style-code="width: 120px"
            style-desc="width: 300px" />
    
        </div>
        <div class="filter-item">
          <label class="form-label">Parent Item</label>
         <input-item
            class="form-control"
            v-model="filter.parentItem"
            brand="ALL"
            item-type="ALL"
            :item-cls="filter.itemCls"
           style-code="width: 200px"
            style-desc="width: 220px" />
        </div>
        

         

     
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
        </div>
      </div>
      <hr />
          <v-tree
          :tree-data="treeData"
          :columns="columns"
          :is-loading="ds.isLoading"
          :is-server-error="ds.isServerError"
          :is-network-error="ds.isNetworkError"
          :top-content-height="280"
        />
    </template>
  </v-frame>

 

</template>

<script>
export default {
  data: () => ({
 
    filter: {
      itemCls: null,
      item: null,
    },
    columns: [
      {
        text: "Item Code",
        dataField: "material_code",
        width: "150px",
        align: "left",
      },
      { text: "Description", dataField: "Material_Name", width: "250px" },
      { text: "Process", dataField: "Child_ProcessName", width: "250px" },
      { text: "Machine", dataField: "Child_LineName", width: "250px" },
      { text: "Date", dataField: "Date", width: "250px" },
      { text: "Qty", dataField: "Qty", width: "80px", align: "right" },
      { text: "Unit", dataField: "UnitDescs", width: "80px", align: "left" },
      {
        text: "Start Date",
        dataField: "Start_Date",
        width: "120px",
        align: "center",
      },
      {
        text: "End Date",
        dataField: "End_Date",
        width: "120px",
        align: "center",
      },
    ],
    rawData: [],
    treeData: [],
  }),
  computed: {
    ds: function () {
      return useTraceability();
    },
  },
  mounted: function () {
    this.search();
  },
  watch: {
    "filter.itemCls": function () {
      this.treeData = [];
    },
    "filter.itemType": function () {
      this.treeData = [];
    },
    "filter.brand": function () {
      this.treeData = [];
    },
    "filter.parentItem": function () {
      this.treeData = [];
    },
  },
  methods: {
    search: function () {
      this.ds.inquiry(true, 'BCRS00E0200E', 'EL.250224.1.0002').then((dt) => {
        this.rawData = dt.Data.Items;
        this.treeData = this.buildTree(this.rawData);
        console.log(this.treeData);
      });
    },
    reset: function () {
      this.filter.itemCls = null;
      this.filter.itemType = null;
      this.filter.brand = null;
      this.filter.parentItem = null;
      this.search();
    },
    buildTree: function (data) {
      const map = {};
      const roots = [];

      data.forEach((item) => {
        map[item.ChildLvl] = { ...item, children: [] };
      });

      data.forEach((item) => {
        if (item.ParentLvl && map[item.ParentLvl]) {
          map[item.ParentLvl].children.push(map[item.ChildLvl]);
        } else {
          roots.push(map[item.ChildLvl]);
        }
      });

      return roots;
    },
  },
};
</script>
<style scoped>
.detail-content {
  max-height: 30em;
  max-height: 60%;
}

.detail-content-view {
  /* padding-bottom: 5em; */
  height: 70%;
  max-height: 70%;
  overflow-y: scroll;
}

thead {
  white-space: nowrap;
}
/* GANTI CSS .filter-wrapper lama dengan ini */

.filter-wrapper {
  display: grid;
  grid-template-columns: repeat(2, minmax(320px, 1fr));
  grid-auto-flow: column; /* isi atas ke bawah dulu */
  gap: 4px 20px;
  width: 100%;
  align-items: center;
}

/* jumlah baris otomatis sesuai jumlah item */

.filter-wrapper:has(.filter-item:nth-child(10)) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(9)):not(
    :has(.filter-item:nth-child(10))
  ) {
  grid-template-rows: repeat(5, auto);
}

.filter-wrapper:has(.filter-item:nth-child(8)):not(
    :has(.filter-item:nth-child(9))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(7)):not(
    :has(.filter-item:nth-child(8))
  ) {
  grid-template-rows: repeat(4, auto);
}

.filter-wrapper:has(.filter-item:nth-child(6)):not(
    :has(.filter-item:nth-child(7))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(5)):not(
    :has(.filter-item:nth-child(6))
  ) {
  grid-template-rows: repeat(3, auto);
}

.filter-wrapper:has(.filter-item:nth-child(4)):not(
    :has(.filter-item:nth-child(5))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(3)):not(
    :has(.filter-item:nth-child(4))
  ) {
  grid-template-rows: repeat(2, auto);
}

.filter-wrapper:has(.filter-item:nth-child(2)):not(
    :has(.filter-item:nth-child(3))
  ) {
  grid-template-rows: repeat(1, auto);
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
  min-height: 32px;
  width: 100%;
}

.filter-item label {
  width: 70px;
  min-width: 70px;
  white-space: nowrap;
}

/* MOBILE = turun kebawah normal */
@media (max-width: 1035px) {
  .filter-wrapper {
    grid-template-columns: 1fr !important;
    grid-template-rows: auto !important;
    grid-auto-flow: row !important;
    gap: 6px;
  }

  .filter-item {
    width: 100%;
  }
}

.button-section {
  /* garis panjang bawah */
  width: 100%;
}
</style>

