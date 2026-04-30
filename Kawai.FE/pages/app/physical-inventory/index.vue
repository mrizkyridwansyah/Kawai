<template>
  <v-frame title="Physical Inventory Update" icon="receipt">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Period</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-month v-model="filter.period" style="width: 215px" />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Item</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
             <filter-item-by-stock
              class="form-control"
              v-model="filter.item"
              :warehouse= "filter.warehouse"
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
            <filter-warehouse-privileges
              class="form-control"
              v-model="filter.warehouse"
              factory-code="ALL"
              style-code="width: 170px;"
              style-desc="width: 250px;"
            />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Lot No</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <filter-lot-no
              class="form-control"
              v-model="filter.lotNo"
              :warehouse="filter.warehouse"
              :area="filter.area"
              :address="filter.address"
              :show-option-all="true"
              :item="filter.item"
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
            <filter-area-privileges
              class="form-control"
              v-model="filter.area"
              :warehouse="filter.warehouse"
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
            <filter-address-privileges
              class="form-control"
              v-model="filter.address"
              :warehouse="filter.warehouse"
              :area="filter.area"
              :include-temp="true"
              style-code="width: 170px"
              style-desc="width: 250px"
            />
          </td>

          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Different Qty </label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-checkbox
              label=" "
              v-model="filter.diffQty"
              @input="changeDiffQty"
            />
          </td>

          <td style="padding-top: 5px">
            <v-button-search-reset :search="search" :reset="reset" />
            <div class="d-inline-flex">
              <v-button
                class="btn-success ms-1"
                icon="save"
                label="Submit"
                @click="submit"
              ></v-button>
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <v-table
        :filter="filter"
        :export-excel="true"
        :export-excel-action="exportExcel"
        :frozen-column-left="5"
        :data-items="ds.data.Items"
        :ds="ds"
        :use-header="true"
        :use-paging="true"
        :top-content-height="340"
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
              <tr v-for="(item, idx) in lists || []" :key="idx">
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
                  <input
                    type="number"
                    step="0.01"
                    class="form-control form-control-sm text-right"
                    v-model.number="item.Inventory"
                    :ref="`inv-${idx}`"
                    @focus="item.Inventory = Number(item.Inventory)"
                    @blur="onChangeInventory(item)"
                    @keypress="onNextRow(idx)"
                  />
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
      </v-table>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    breadcrumbs: [
      { title: "Stock Control", active: false, to: "" },
      {
        title: "Physical Inventory Update",
        active: true,
        to: "/physical-inventory",
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
        // Warehouse: "asc",
        // Area: "asc",
        // Address: "asc",
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
    debounce: null,
    lists: [],
    listUpdate: [],
  }),
  computed: {
    ds: function () {
      return usePhysicalInventory();
    },
  },
  watch: {},
  mounted: function () {
    this.filter.period = new Date(
      new Date().getFullYear(),
      new Date().getMonth(),
      1,
    ).toISOString();

    this.filter.scanStatus = "ALL";
    this.filter.diffQty = true;
  },
  methods: {
    resetGrid: function () {
      this.ds.setFilter([]);
      this.ds.setPage(1);
      this.ds.setLength(10);
      this.ds.data.Items = [];
      this.lists = [];
      this.listUpdate = [];
    },
    changeDiffQty: function (e) {
      this.$emit("update:modelValue", e.target.checked);
    },
    initInventory(item) {
      item._oldInventory = item.Inventory;
    },
    async search() {
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
      this.ds.setFilter(filters);

      const dt = await this.ds.load();
      this.lists = dt.Data.Items;

      //menambahkan _oldInventory untuk tracking perubahan
      this.lists.forEach((item) => {
        this.initInventory(item);
      });

      this.listUpdate = [];
    },
    reset: function () {
      this.filter.period = null;
      this.filter.warehouse = null;
      this.filter.area = null;
      this.filter.address = null;

      this.filter.item = null;
      this.filter.lotNo = null;
      this.filter.scanStatus = "ALL";
      this.filter.diffQty = null;

      this.lists = [];
      this.listUpdate = [];
    },
    scanStatusClass(status) {
      if (status === "NOTYET") return "bg-notyet";
      if (status === "SCANNED") return "bg-scanned";
      if (status === "DIFFERENT") return "bg-different";
      return "";
    },
    onNextRow(idx) {
      this.$nextTick(() => {
        const nextRef = this.$refs[`inv-${idx + 1}`];
        if (nextRef && nextRef.length > 0) {
          const el = nextRef[0];
          el.focus();
          el.select();
        }
      });
    },
    onChangeInventory(item) {
      const keyMatch = (u) =>
        u.RefNo === item.RefNo &&
        u.WarehouseCode === item.WarehouseCode &&
        u.AreaCode === item.AreaCode &&
        u.AddressCode === item.AddressCode &&
        u.ItemCode === item.ItemCode &&
        u.LotNo === item.LotNo &&
        u.BarcodeNo === item.BarcodeNo;

      //console.log(item._oldInventory, item.Inventory);
      // kalau nilai BALIK ke semula → hapus dari update[]
      if (item._oldInventory === item.Inventory) {
        this.listUpdate = this.listUpdate.filter((u) => !keyMatch(u));
        return;
      }

      //cari index di update[]
      const idxUpdate = this.listUpdate.findIndex(keyMatch);
      //console.log('idxUpdate', idxUpdate);

      if (idxUpdate !== -1) {
        // sudah ada → update Inventory saja
        this.listUpdate[idxUpdate].Inventory = item.Inventory;
      } else {
        // belum ada → push baru
        this.listUpdate.push({
          RefNo: item.RefNo,
          WarehouseCode: item.WarehouseCode,
          AreaCode: item.AreaCode,
          AddressCode: item.AddressCode,
          BarcodeNo: item.BarcodeNo,
          ItemCode: item.ItemCode,
          LotNo: item.LotNo,
          Inventory: item.Inventory,
        });
      }
    },
    async submit() {
      try {
        if (this.listUpdate.length === 0) {
          toastInfo("No data to submit.");
          return;
        }

        //console.log('submit', this.listUpdate);
        await this.ds.update(this.listUpdate);
        toastSuccess("Data saved successfully!");

        this.listUpdate = [];
        await this.search();
      } catch (e) {
        toastDanger(e.Message);
        console.error(e);
      }
    },
    exportExcel() {
      const filters = [
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

      return this.ds.exportExcel(filters);
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
