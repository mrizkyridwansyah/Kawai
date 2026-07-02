<template>
  <v-frame title="Reprint Barcode" icon="qrcode">
    <template #frame-content>
      <table>
        <tr>
          <td>
            <table>
              <tr>
                <td><label class="form-label">Factory</label></td>
                <td style="padding-left: 15px" colspan="3">
                  <filter-factory-privileges
                    class="form-control"
                    v-model="filter.factory"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Warehouse</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px" colspan="3">
                  <filter-warehouse-privileges
                    class="form-control"
                    v-model="filter.warehouse"
                    :factory-code="filter.factory"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
            </table>
          </td>
          <td style="padding-left: 30px">
            <table>
              <tr>
                <td><label class="form-label">Area</label></td>
                <td style="padding-left: 15px" colspan="3">
                  <filter-area-privileges
                    class="form-control"
                    v-model="filter.area"
                    :warehouse="filter.warehouse"
                    :show-option-all="true"
                    :include-temp="true"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Address</label>
                </td>
                <td style="padding-left: 15px; padding-top: 5px" colspan="3">
                  <filter-address-privileges
                    class="form-control"
                    v-model="filter.address"
                    :warehouse="filter.warehouse"
                    :show-option-all="true"
                    :area="filter.area"
                    :include-temp="true"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
      <div class="d-flex flex-fill mt-1">
        <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
        <v-button-print
          :print="print"
          cClass="ml-1"
          :is-loading="isLoadingPrint"
        />

        <v-button
          :action="printpdf"
          label="Print Label PDF"
          icon="file-pdf"
          cClass="ml-1 btn-green"
          :is-loading="isLoadingPrint"
        />
      </div>
      <hr />
       <div
              class="d-flex align-items-center mb-2 p-2"
              style="gap: 10px; background: #2f2f2f; border-radius: 6px"
            >
              <!-- SORT BUTTON -->

              <!-- SEARCH INPUT -->
              <div style="flex: 1">
                <input
                  type="text"
                  class="form-control form-control-sm"
                  v-model="filter.keyword"
                  placeholder="Search..."
                  style="background: #f1f1f1"
                />
              </div>
            </div>
          <div class="panel-body">
              <div class="v-table-wrapper" @scroll="onScroll($event, 'main')">
                <table
                  class="table mb-0 align-middle w-100 v-fixed-table"
                  ref="table"
                >
                  <thead>
                    <tr>
                      <th class="text-center">
                  <div style="justify-items: center">
                     
                  </div>
                </th>
                <th class="text-center">Barcode No</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Warehouse</th>
                <th class="text-center">Area</th>
                <th class="text-center">Address</th>
                <th class="text-center">Lot No</th>
                <th class="text-center">SubLotNo</th>
                <th class="text-center">Qty</th>
                <th class="text-center">Source</th>
                      
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, idx) in filterGrid" :key="item.BarcodeNo">
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.BarcodeNo)"
                      @update:modelValue="(checked) => check(checked, item)"
                    />
                  </div>
                </td>

                <td>{{ item.BarcodeNo }}</td>
                <td>{{ item.ItemCode }}</td>
                <td>{{ item.ItemName }}</td>
                <td>{{ item.Warehouse }}</td>
                <td>{{ item.Area }}</td>
                <td>{{ item.Address }}</td>
                <td>{{ item.LotNo }}</td>
                <td>{{ item.SubLotNo }}</td>
                <td>{{ item.Qty }}</td>
                <td>{{ item.Source }}</td>
                 
              </tr>
                  </tbody>
                </table>
                <v-data-empty class="mt-3" v-if="!ds.isLoading && filterGrid.length === 0"/>
              </div>
            </div>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    keywordKeys: [
      {
        Id: "BarcodeNo",
        Name: "Barcode No",
      },
      {
        Id: "ItemCode",
        Name: "Item Code",
      },
    ],
    filter: {
      keyword: null,
      factory: 0,
      warehouse: null,
      area: null,
      address: null,
      sorts: {
        ItemName: "asc",
      },
      sortItems: [
        {
          label: "Barcode No",
          value: "BarcodeNo",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "ItemCode",
          selected: true,
          direction: "desc",
        },
        {
          label: "Item Name",
          value: "ItemName",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
    selectedPrint: [],
    isLoadingPrint: false,
  }),
  computed: {
    ds: function () {
      return useReprint();
    },
    isAllChecked() {
      if (!this.ds.data.Items?.length) return false;

      return this.ds.data.Items.every((item) =>
        this.selectedPrint.some((p) => p.Key === item.BarcodeNo),
      );
    },

  filterGrid() {
    const keyword = (this.filter.keyword || "").toLowerCase().trim();
    if (!keyword) {
      return this.ds.data.Items || [];
    }

    return (this.ds.data.Items || []).filter((item) => {
      return (
        String(item.BarcodeNo || "").toLowerCase().includes(keyword) ||
        String(item.ItemCode || "").toLowerCase().includes(keyword) ||
        String(item.ItemName || "").toLowerCase().includes(keyword) ||
        String(item.Warehouse || "").toLowerCase().includes(keyword) ||
        String(item.Area || "").toLowerCase().includes(keyword) ||
        String(item.Address || "").toLowerCase().includes(keyword) ||
        String(item.LotNo || "").toLowerCase().includes(keyword) ||
        String(item.SubLotNo || "").toLowerCase().includes(keyword) ||
        String(item.Source || "").toLowerCase().includes(keyword) ||
        String(item.Qty || "").toLowerCase().includes(keyword)
      );
    });
  },

  },
  watch: {
    "filter.factory": function () {
      this.selectedPrint = [];
      this.ds.data.Items = [];
    },
    "filter.warehouse": function () {
      this.selectedPrint = [];
      this.ds.data.Items = [];
    },
    "filter.area": function () {
      this.selectedPrint = [];
      this.ds.data.Items = [];
    },
    "filter.address": function () {
      this.selectedPrint = [];
      this.ds.data.Items = [];
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    onScroll: function (e, type) {
      const { scrollTop, scrollHeight, clientHeight } = e.target;
      // Jika scroll sudah mendekati bawah (sisa 50px jarak dari bawah)
      if (scrollTop + clientHeight >= scrollHeight - 50) {
        // Cek batasan max item dari masing-masing array
        let maxLen = 0;
        if (type === "main") maxLen = this.ds.data.length;
        
        else if (type === "ng") maxLen = this.ds.data.length;

        if (this.renderLimits[type] < maxLen) {
          this.renderLimits[type] += 50;
        }
      }
    },
    search: function () {
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          WarehouseCode: this.filter.warehouse || "",
          AreaCode: this.filter.area || "",
          AddressCode: this.filter.address || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },

    checkAll: function (checked) {
      if (checked) {
        this.selectedPrint = this.ds.data.Items.map((item) => ({
          Key: item.BarcodeNo,
          Value: item.Source,
        }));
      } else {
        this.selectedPrint = [];
      }
    },
    reset: function () {
      this.filter.warehouse = null;
      this.search();
    },

    check: function (checked, item) {
      const existingIndex = this.selectedPrint.findIndex(
        (p) => p.Key === item.BarcodeNo,
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.BarcodeNo,
          Value: item.Source,
        });
      } else if (!checked && existingIndex !== -1) {
        this.selectedPrint.splice(existingIndex, 1);
      }
    },
    isChecked: function (code) {
      return this.selectedPrint.some((p) => p.Key === code);
    },
    print: function () {
      this.isLoadingPrint = true;
      if (this.selectedPrint.length === 0) {
        toastWarning("Please choose barcode");
        this.isLoadingPrint = false;
        return;
      }

      new Promise((resolve, reject) => {
        debugger;
        this.ds

          .PrintUpdate(this.selectedPrint)
          .then((_) => {
            toastSuccess("Print success");
            this.selectedPrint = [];
            this.search(); //
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message || "Print failed");
            resolve();
          })
          .finally(() => {
            setTimeout(() => {
              this.isLoadingPrint = false;
            }, 1000);
          });
      });
    },

    printpdf: function () {
      if (this.selectedPrint.length === 0) {
        toastWarning("Please choose barcode");
        return;
      }

      this.isLoadingPrint = true;

      this.ds
        .printpdf(this.selectedPrint)
        .then(() => {
          // Download is handled by the store (reprint.js)
        })
        .catch((err) => {
          console.log(err);

          toastDanger(
            err?.response?.data?.Message || err?.Message || "Print PDF failed",
          );
        })
        .finally(() => {
          this.isLoadingPrint = false;
        });
    },
  },
};
</script>

<style scoped>
thead {
  white-space: nowrap;
}

.v-table-wrapper {
  overflow: auto;
  max-height: 500px;
  /* border: 1px solid #ddd; */
  position: relative;
}
/* Bikin table bisa scroll horizontal juga */
.v-fixed-table {
  width: max-content; /* agar scroll horizontal muncul */
  min-width: 100%;
  border: 1px solid gainsboro !important;
  /* border-collapse: separate; */
  /* border-spacing: 0; */
}

.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  border: 1px solid #dee2e6;
  background: #fff;
}

/* Sticky Header (atas) */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20; /* harus lebih tinggi dari sticky kiri */
  background: lightblue !important;
}
</style>
