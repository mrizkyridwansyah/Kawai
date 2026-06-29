<template>
  <v-frame title="Physical Inventory Update (New)" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Period</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-month
              v-model="filter.period"
              style="width: 215px"
              disabled
            />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Item</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-item-by-stock
              class="form-control"
              v-model="filter.item"
              :warehouse="filter.warehouse"
              :area="filter.area"
              :address="filter.address"
              category="ALL"
              :show-option-all="true"
              style-code="width: 170px"
              style-desc="width: 250px"
            />
          </td>

          <td width="50px"></td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Scanned</label>
          </td>
          <td class="note-color">
            <div class="bg-scanned note-border">&nbsp;</div>
          </td>
        </tr>

        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Warehouse</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-warehouse-by-stock
              class="form-control"
              v-model="filter.warehouse"
              factory-code="ALL"
              item-code="ALL"
              style-code="width: 170px;"
              style-desc="width: 250px;"
            />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Lot No</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-lot-by-stock
              class="form-control"
              v-model="filter.lotNo"
              :warehouse="filter.warehouse"
              :area="filter.area"
              :address="filter.address"
              :show-option-all="true"
              :item="filter.item"
              category="ALL"
              style="width: 170px"
            />
          </td>

          <td width="50px"></td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Different</label>
          </td>
          <td class="note-color">
            <div class="bg-different note-border">&nbsp;</div>
          </td>
        </tr>

        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Area</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-area-by-stock
              class="form-control"
              v-model="filter.area"
              :warehouse="filter.warehouse"
              item="ALL"
              :include-temp="true"
              style-code="width: 170px"
              style-desc="width: 250px"
            />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Status Scan </label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-scan-status
              class="form-control"
              v-model="filter.scanStatus"
              style="width: 170px"
            />
          </td>

          <td width="50px"></td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Not Scanned</label>
          </td>
          <td class="note-color">
            <div class="note-border bg-notyet">&nbsp;</div>
          </td>
        </tr>

        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Address</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-address-by-stock
              class="form-control"
              v-model="filter.address"
              :warehouse-code="filter.warehouse"
              :area-code="filter.area"
              item-code="ALL"
              :include-temp="true"
              :show-option-all="false"
              style-code="width: 170px"
              style-desc="width: 250px"
            />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Different Qty </label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-checkbox label=" " v-model="filter.diffQty" />
          </td>

          <td style="padding-top: 5px">
            <v-button-search-reset :search="search" :reset="reset" />
            <div class="d-inline-flex">
              <v-button-submit
                class="btn-success ms-1"
                icon="save"
                label="Submit"
                :submit="submit"
                :disabled="isLoading || !menuPrivAllowUpdate"
              />
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <v-table-new
        :frozen-column-left="5"
        :data-items="displayedLists"
        :ds="ds"
        :ds-data="dsDataPagination"
        :use-header="false"
        :use-paging="true"
        :top-content-height="340"
        @page-change="onPageChange"
        @length-change="onLengthChange"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Warehouse</th>
                <th class="text-center">Area</th>
                <th class="text-center">Address</th>
                <th class="text-center">Barcode No.</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Lot No</th>
                <th class="text-center">Current Qty</th>
                <th class="text-center">Inventory</th>
                <th class="text-center">Diffrences</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(item, idx) in displayedLists"
                :key="item.BarcodeNo || idx"
              >
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.WarehouseCode }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.AreaCode }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.AddressCode }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.BarcodeNo }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.ItemCode }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.ItemDesc }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.Unit }}
                </td>
                <td :class="scanStatusClass(item.StatusScan)">
                  {{ item.LotNo }}
                </td>
                <td :class="[scanStatusClass(item.StatusScan), 'text-right']">
                  {{ $func.formatDecimal(item.CurrentQty) }}
                </td>
                <td class="text-right">
                  <input-money v-model.number="item.CurrentInventory" />
                </td>
                <td class="text-right bg-diffrences">
                  {{ $func.formatDecimal(item.CurrentQty - item.Inventory) }}
                </td>
                <td>{{ $func.formatDate(item.LastUpdate) }}</td>
                <td>{{ item.LastUserName }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table-new>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Stock Control", active: false, to: "" },
      {
        title: "Physical Inventory Update (New)",
        active: true,
        to: "/physical-inventory/index-new",
      },
    ],
    filter: {
      keyword: null,
      period: null,
      warehouse: null,
      area: null,
      address: null,
      item: null,
      lotNo: null,
      statusScan: null,
      diffQty: null,
      sorts: {
        ItemCode: "asc",
      },
      sortItems: [
        {
          label: "Item Code",
          value: "Item_Code",
          selected: true,
          direction: "asc",
        },
      ],
    },
    pagination: {
      page: 1,
      length: 10,
    },
    debounce: null,
    lists: [],
    listUpdate: [],
    menuPrivAllowUpdate: false,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return usePhysicalInventory();
    },
    dsMenu: function () {
      return useMenu();
    },
    dsDataPagination: function () {
      return {
        Items: this.lists || [],
        Page: this.pagination.page,
        Length: this.pagination.length,
        Filtered: (this.lists || []).length,
        Total: (this.lists || []).length,
      };
    },
    displayedLists: function () {
      if (!this.lists) return [];
      const start = (this.pagination.page - 1) * this.pagination.length;
      const end = start + this.pagination.length;
      return this.lists.slice(start, end);
    },
  },
  mounted: function () {
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate =
        dt.Data.filter((a) => a.MenuID == "E08")[0]?.AllowUpdate || false;
    });

    this.filter.period = new Date(
      new Date().getFullYear(),
      new Date().getMonth(),
      1,
    ).toISOString();

    this.filter.scanStatus = "ALL";
    this.filter.diffQty = true;
  },
  methods: {
    onPageChange: function (page) {
      this.pagination.page = page;
    },
    onLengthChange: function (length) {
      this.pagination.length = length;
      this.pagination.page = 1;
    },
    resetGrid: function () {
      this.ds.setFilter([]);
      this.ds.setPage(1);
      this.ds.setLength(10);
      this.ds.data.Items = [];
      this.lists = [];
      this.listUpdate = [];
      this.pagination.page = 1;
    },
    search: function () {
      if (!this.filter.warehouse) {
        toastDanger("Silahkan pilih Warehouse!");
        return;
      }

      if (!this.filter.area) {
        toastDanger("Silahkan pilih area!");
        return;
      }

      if (!this.filter.address) {
        toastDanger("Silahkan pilih address!");
        return;
      }

      if (!this.filter.item) {
        toastDanger("Silahkan pilih item!");
        return;
      }

      if (!this.filter.lotNo) {
        toastDanger("Silahkan pilih lotNo!");
        return;
      }

      this.ds.setSort(this.filter.sorts);

      let filters = [
        {
          Keyword: this.filter.keyword || "",
          Period: this.$func.asUtcStringDateOnly(new Date(this.filter.period)),
          WarehouseCode: this.filter.warehouse || "",
          AreaCode: this.filter.area || "",
          AddressCode: this.filter.address || "",
          ItemCode: this.filter.item || "",
          LotNo: this.filter.lotNo || "",
          ScanStatus: this.filter.scanStatus || "",
          DifferentQty: this.filter.diffQty ? "true" : "false",
        },
      ];
      this.ds.setLength(100000);

      this.ds.setFilter(filters);

      this.ds.load().then((dt) => {
        this.lists = dt.Data.Items.map((item) => ({
          ...item,
          CurrentInventory: item.Inventory,
        }));
        this.pagination.page = 1;
      });
    },
    reset: function () {
      this.filter.warehouse = null;
      this.filter.area = null;
      this.filter.address = null;

      this.filter.item = null;
      this.filter.lotNo = null;
      this.filter.scanStatus = "ALL";
      this.filter.diffQty = null;

      this.lists = [];
      this.listUpdate = [];
      this.pagination.page = 1;
    },
    scanStatusClass: function (status) {
      if (status === "NOTYET") return "bg-notyet";
      if (status === "SCANNED") return "bg-scanned";
      if (status === "DIFFERENT") return "bg-different";
      return "";
    },
    submit: async function () {
      let dataChanges = this.lists
        .filter((x) => x.Inventory !== x.CurrentInventory)
        .map((x) => {
          return {
            RefNo: x.RefNo,
            WarehouseCode: x.WarehouseCode,
            AreaCode: x.AreaCode,
            AddressCode: x.AddressCode,
            BarcodeNo: x.BarcodeNo,
            ItemCode: x.ItemCode,
            LotNo: x.LotNo,
            Inventory: x.CurrentInventory,
          };
        });

      if (dataChanges.length === 0) {
        toastInfo("No data changes to submit.");
        return;
      }

      this.isLoading = true;
      this.ds
        .update(dataChanges)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.back();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
  },
};
</script>

<style>
.bg-notyet {
  background-color: lightcoral !important;
}
.bg-scanned {
  background-color: lightgrey !important;
}
.bg-different {
  background-color: yellow !important;
}
.bg-diffrences {
  background-color: lightyellow !important;
}

.note-border {
  border: 1px solid #d1d5db;
}
.note-color {
  padding-top: 5px;
  padding-left: 15px;
  width: 100px;
  text-align: center;
}
</style>
