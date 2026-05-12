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
          <v-button-print :print="print" cClass="ml-1" :is-loading="isLoadingPrint" />
      
         <v-button
                :action="printpdf"
                label="Print Label PDF"
                icon="file-pdf"
                cClass="ml-1 btn-green"
                :is-loading="isLoadingPrint"
              />
      </div>
      <hr />
      <v-table
        :filter="filter"
         :use-paging=false
        :ds="ds"
        :data-items="ds.data.Items"
        :frozen-column-left="2"
        :top-content-height="280"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                  <th class="text-center">
                    <div style="justify-items: center">
                      <input-checkbox
                        :modelValue="isAllChecked"
                        @update:modelValue="checkAll"
                      />
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
                <th class="text-center">Print User</th>
                <th class="text-center">Print Date</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
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
                <td>{{ item.PrintUser }}</td>
                <td>{{ $func.formatDateTime(item.PrintDate) }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
</template>

<script>
import { faBullseye } from '@fortawesome/free-solid-svg-icons/faBullseye';

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
      this.selectedPrint.some((p) => p.Key === item.BarcodeNo)
    );
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
    .then((data) => {

      const blob = new Blob([data], {
        type: "application/pdf",
      });

      const url = window.URL.createObjectURL(blob);

      window.open(url);
    })
    .catch((err) => {
      console.log(err);

      toastDanger(
        err?.response?.data?.Message ||
        err?.Message ||
        "Print PDF failed"
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
</style>
