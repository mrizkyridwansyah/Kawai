<template>
  <v-frame title="Part Material Request Others" icon="receipt">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->

          <div class="filter-item">
          <label class="form-label">Request No</label>
          <input-partmaterialrequestother
            class="form-control"
            :disabled="isNew"
            status="DRAFT"
            v-model="filter.RequestId"
            :errors="errors?.RequestId"
            style="width: 375px"
          />
          <div class="col-xl-1 col-lg-1 col-md-1 col-sm-1 col-xs-1 mt-2">
            <input-checkbox
              label="New"
              v-model="isNew"
              @click="(e) => changeNew(e)"
            />
          </div>
        </div>

         <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Request Date</label>
          <div>
            <input-date v-model="model.RequestDate" />
          </div>
        </div>

        <div class="filter-item">
          <label class="form-label">Factory</label>
          <filter-factory-privileges
            class="form-control"
            v-model="filter.FactoryCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

         <div class="filter-item">
          <label class="form-label">Process</label>
          <filter-trade-2
            class="form-control"
            :trade-cls="['1']"
            v-model="filter.ManufactureCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>
        <div class="filter-item">
          <label class="form-label">Line</label>
          <filter-line-factory
            class="form-control"
            :company="filter.FactoryCode"
            :manufacture="filter.ManufactureCode"
            v-model="filter.LineCode"
            style-code="width: 120px"
            style-desc="width: 300px"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">Status</label>
          <input-text
            v-model="model.Status"
            maxlength="50"
           :errors="errors?.Status"
            disabled="true"
            style="width: 420px"
          />
        </div>

     
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <button
            class="btn btn-sm btn-blue btn-elevate mr-1"
            @click="searchRequestOthersDetail"
            :disabled="isLoading"
          >
            <div
              class="spinner-border spinner-border-sm text-light"
              role="status"
              v-if="isLoading"
            >
              <span class="visually-hidden">Loading...</span>
            </div>
            <font-awesome-icon v-else icon="search" />
            <span class="ml-2">Search</span>
          </button>
          <v-button
                :action="remove"
                label="Delete"
                icon="trash"
                cClass="mr-1 btn-danger"
                :is-loading="isLoading"
                :disabled="!menuPrivAllowUpdate"
          />
         <v-button-submit
            :submit="submit"
            :disabled="!menuPrivAllowUpdate"
             label="Save"
            icon="save"
            cClass="ml-1 mr-2"
            :is-loading="isLoading"
          />
           <button
                class="btn btn-green btn-elevate btn-search"
                
                @click="RequestHistory"
              >
                <font-awesome-icon icon="receipt" />
                <span class="ml-2">Part Material Request Others History</span>
              </button>
     
        </div>
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
      <v-table-input
        :data-items="filteredSetting"
         ref="vtable"
        :top-content-height="450" 
      >
        <template #table-content>
          <div class="detail-content">
            <table
              class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
              v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
              ref="table"
            >
              <thead>
                <tr>
                  <th class="text-center">
                    <input-checkbox 
                 
                     @click="(e) => checkAll(e)" />
                  </th>
                  <th class="text-center">Item Code</th>
                  <th class="text-center">Item Name</th>
                  <th class="text-center">Storage</th>
                  <th class="text-center">Qty Packing</th>
                  <th class="text-center">Available Stock</th>
                  <th class="text-center">Request Qty</th>
                  <th class="text-center">Status</th>
                  <th class="text-center">Last Update</th>
                  <th class="text-center">Last User</th>
                </tr>
              </thead>
              <tbody>
                 <tr v-for="(item, idx) in filteredSetting" :key="item.DetailID || idx">
                 <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                          :disabled="item.Status === 'No Stock'"
                      @click="(e) => check(e, item)"
                    />
                  </td>
               
                  <td>{{ item.ItemCode }}</td>
                  <td>{{ item.ItemName }}</td>
                  <td>
                    <a
                          href="javascript:void(0)"
                          @click="viewStock(item.ItemCode)"
                        >
                          View Stock Detail</a
                        >
                        </td>
                  <td class="text-right">{{ $func.formatMoney(item.QtyPacking) }}</td>
                  <td class="text-right">{{ $func.formatMoney(item.AvailableStock) }}</td>
               
                 
                  <td>
                      <div class="d-flex align-items-center">
                        <input-money-small
                          v-model="item.RequestQty"
                          :errors="errors?.[`Details[${idx}].RequestQty`]"
                          style="width: 100px"
                        />
                      </div>
                    </td>
                     <td
                        :class="{
                          'text-danger': item.Status === 'No Stock',
                          'text-green': item.Status === 'Eligible',
                        }"
                      >
                        {{ item.Status }}
                      </td>
                   
                  <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                  <td>{{ item.LastUser }}</td>
                </tr>
              </tbody>
            </table>
            <v-data-empty
              class="mt-3"
              v-if="
                !ds.isLoading &&
                !ds.isNetworkError &&
                !ds.isServerError &&
               (filteredSetting.length === 0)
              "
            />
          </div>
        </template>
      </v-table-input>
  
    </template>
  </v-frame>
   <v-modal title="Detail Stock" class="modal-lg" id="modal-list-stock">
    <shared-request-womin-list-stock
      :item="this.selectedItem"
      :counter="this.counter"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    isNew: true,
    menuPrivAllowUpdate: false,
    filter: {
        FactoryCode: null,
        ManufactureCode: null,
        LineCode: null,
        RequestId: null, 
        keyword: null,
    },
    model: {
      RequestId: null,
      RequestNo: "",
      LineCode: "",
      Status: "",
      RequestDate: null,
      Details: [],
    },
    listRequestOthersDetail: [],
    Details: [],
    debounce: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return usePartMaterialRequestOthers();
    },
     dsMenu: function () {
      return useMenu();
    },
      filteredSetting() {
    const keyword = (this.filter.keyword || "").toLowerCase().trim();

    if (!keyword) return this.listRequestOthersDetail;

    return this.listRequestOthersDetail.filter((item) => {
      return (
        (item.ItemCode || "").toLowerCase().includes(keyword) ||
        (item.ItemName|| "").toLowerCase().includes(keyword)   
       
      );
    });
  },
  },
  watch: {
   
    "filter.LineCode": function () {
      this.listRequestOthersDetail = [];
    },
    "filter.RequestId": function () {
      this.model = {
        RequestId: null,
        RequestNo: "",
        LineCode: "",
        Status: "",
        RequestDate: null,
        Details: [],
      };
      this.listRequestOthersDetail = [];
      
      if (this.filter.RequestId) this.getRequest();
    },
  },
  mounted: function () {
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "P04",
      )[0].AllowUpdate;
    });
    let today = new Date();
    this.model.RequestDate = today;

  },
  methods: {
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    reset: function () {
      this.isNew = true;
      this.filter = {
        FactoryCode: null,
        ManufactureCode: null,
        LineCode: null,
        RequestId: null,
         keyword: null,
      };
      this.model = {
        RequestId: null,
        RequestNo: "",
        LineCode: "",
        Status: "",
        RequestDate: null,
        Details: [],
      };
      this.listRequestOthersDetail = [];
      let today = new Date();
   
    },
    changeNew: function (e) {
      if (e.target.checked) this.reset();
    },
    checkAll: function (e) {
      this.listRequestOthersDetail.map((x) => (x.Selected = e.target.checked));
    },
    check: function (e, item) {
      item.Selected = e.target.checked;
    },
 RequestHistory: function () {
      this.$router.push("/app/partmaterialrequestothers/view");
    
    },
    viewStock: function (item) {
      this.selectedItem = item;
      this.counter++;
      this.$bvModal.show("modal-list-stock");
    },
    submit: function () {
  this.isLoading = true;
  this.errors = {};

  let details = this.listRequestOthersDetail.filter((x) => x.Selected);

  if (details.length == 0) {
    this.isLoading = false;
    toastDanger("Please Choose Item!");
    return;
  }

  // ===== VALIDATION =====
  for (const item of details) {
    const requestQty = Number(item.RequestQty || 0);
    const packQty = Number(item.QtyPacking || 0);
    const availableStock = Number(item.AvailableStock || 0);

    if (requestQty <= 0) {
      this.isLoading = false;
      toastDanger(`Request Qty for Item ${item.ItemCode} must be greater than 0.`);
      return;
    }

    if (packQty > 0 && requestQty % packQty !== 0) {
      this.isLoading = false;
      toastDanger(
        `Request Qty for Item ${item.ItemCode} must be a multiple of Pack Qty (${packQty}).`
      );
      return;
    }

    if (requestQty > availableStock) {
      this.isLoading = false;
      toastDanger(
        `Request Qty for Item ${item.ItemCode} cannot exceed Available Stock (${availableStock}).`
      );
      return;
    }
  }

  this.model.RequestId = this.filter.RequestId?.toString() || "0";
  this.model.LineCode = this.filter.LineCode;
  this.model.Details = details.map((p) => ({
    ItemCode: p.ItemCode,
    RequestQty: p.RequestQty,
  }));

  if (this.isNew) {
    this.createRequest();
  } else {
    this.updateRequest();
  }
},

   remove: function () {
      if (!this.filter.RequestId) {
        toastDanger("Silahkan pilih Request No!");
        return;
      }

      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(this.filter.RequestId)
              .then((dt) => {
                toastSuccess("Data deleted successfully!");
                resolve();
                this.reset();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                resolve();
                //toastDanger(err?.Message);
              });
          }),
        null,

        "",
      );
    },

    getRequest: function () {
      this.ds.loadDetail(this.filter.RequestId).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.filter.FactoryCode = this.model.FactoryCode;
        this.filter.ManufactureCode = this.model.ManufactureCode;
        this.filter.LineCode = this.model.LineCode;
        this.$nextTick(() => setTimeout(() => this.searchRequestOthersDetail(), 500));
      });
    },
    searchRequestOthersDetail: function () {
      if (!this.filter.LineCode) {
        toastDanger("Please Select Line!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          FactoryCode: this.filter.FactoryCode,
          RequestId: this.filter.RequestId?.toString() || "0",
          LineCode: this.filter.LineCode,
          ManufactureCode: this.filter.ManufactureCode,
          
        },
      ];
      this.ds.listRequestOthersDetail(filters).then((dt) => {
        this.listRequestOthersDetail = dt.Data.Items;
        this.listRequestOthersDetail.map((x) => (x.Selected = x.DetailID > 0));
      });
    },
 

    createRequest: function () {
      this.ds
        .create(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    updateRequest: function () {
      this.ds
        .update(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
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
