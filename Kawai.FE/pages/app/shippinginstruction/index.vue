<template>
  <v-frame title="Shipping Instruction Input" icon="receipt">
    <template #frame-content>
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Supplier</label>
          <div>
            <filter-trade-cust
              class="form-control"
              v-model="filter.Supplier"
              :placeholder="''"
              style-code="width: 170px;"
              style-desc="width: 250px;"
            />
          </div>
        </div>

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Delivery Date</label>
          <div>
            <input-date v-model="filter.DeliveryFrom" />
          </div>
          <label
            class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-1"
            >To</label
          >
          <div>
            <input-date v-model="filter.DeliveryTo" />
          </div>
        </div>

        <div class="filter-item">
          <label class="form-label">Order Number</label>
          <input-order-entry
            class="form-control"
            v-model="filter.PONumber"
            :supplier-code="filter.Supplier"
            :type-date="'ALL'"
            :period-from="filter.DeliveryFrom"
            :period-until="filter.DeliveryTo"
            :show-option-all="true"
            :si-no="filter.ShippingInstructionNo"
            style="width: 360px"
          />
        </div>

        <div class="filter-item">
          <label class="form-label">SI No</label>
          <input-shipping-instruction
            class="form-control"
            :disabled="isNew"
            source-menu="SI"
            :supplier-code="filter.Supplier"
            :order-entry="filter.PONumber"
            v-model="filter.ShippingInstructionNo"
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

        <div class="filter-item">
          <label class="form-label">SI Date</label>
          <input-date
            v-model="model.ShippingInstructionDate"
            style-date="width: 115px"
            :errors="errors?.ShippingInstructionDate"
          />
        </div>
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <button
            class="btn btn-sm btn-blue btn-elevate mr-1"
            @click="searchDetail"
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

          <v-button-submit
            :submit="submit"
            :disabled="!menuPrivAllowUpdate"
            label="Save"
            icon="save"
            cClass="mr-1"
            :is-loading="isLoading"
          />
          <v-button-print
            label="Print Surat Jalan"
            class="mr-1"
            :print="print"
            :is-loading="isLoading"
          />
        </div>
      </div>
      <hr />
      <v-table-input
        :data-items="listDetail"
        :frozen-column-left="3"
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
                  <th class="text-center"></th>
                  <th class="text-center">Part Number</th>
                  <th class="text-center">Description</th>
                  <th class="text-center">Unit</th>
                  <th class="text-center">Qty Shipping</th>
                  <th class="text-center">Delivery Date</th>
                  <th class="text-center">Qty Stock</th>
                  <th class="text-center">Serial No</th>
                  <th class="text-center">Picking Detail</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in listDetail || []" :key="idx">
                  <td class="text-center">
                    <input-checkbox
                      v-model="item.Selected"
                      @click="
                        (e) => {
                          if (item.Selected) {
                            e.preventDefault(); // Sudah tercentang, tidak boleh di-uncheck
                            return;
                          }
                          check(e, item);
                        }
                      "
                    />
                  </td>
                  <td>{{ item.Item_Code }}</td>
                  <td>{{ item.Item_Name }}</td>
                  <td>{{ item.Unit_Desc }}</td>
                  <td class="text-right">{{ item.Qty }}</td>
                  <td>{{ item.DeliveryDate }}</td>
                  <td class="text-right">{{ item.Qty_Stock }}</td>
                  <td>{{ item.Serial_No }}</td>
                  <td class="text-center">
                    <v-button
                      @click="
                        viewDetail(
                          item.ShippingInstructionNo,
                          item.Item_Code,
                          item.PONumber,
                          item.PO_SeqNo,
                        )
                      "
                      icon="eye"
                      label="View Detail"
                      cClass="ml-1 btn-info"
                      :is-loading="isLoading"
                    />
                  </td>
                </tr>
              </tbody>
            </table>
            <v-data-empty
              class="mt-3"
              v-if="
                !ds.isLoading &&
                !ds.isNetworkError &&
                !ds.isServerError &&
                (!listDetail || listDetail.length === 0)
              "
            />
          </div>
        </template>
      </v-table-input>
    </template>
  </v-frame>
  <v-modal title="Picking Detail" class="modal-lg" id="modal-list-detail">
    <shared-shipping-scan
      :sino="this.selectedSINo"
      :item="this.selectedItem"
      :pono="this.selectedPONo"
      :poseqno="this.selectedPOSeqNo"
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
      Supplier: "ALL",
      DeliveryFrom: null,
      DeliveryTo: null,
      PONumber: null,
      ShippingInstructionNo: null,
    },
    model: {
      ShippingInstructionDate: null,
      Details: [],
    },
    listDetail: [],
    Details: [],
    debounce: null,
    selectedSINo: "",
    selectedItem: "",
    selectedPONo: "",
    selectedPOSeqNo: 0,
    counter: 0,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useShippingInstruction();
    },
    dsMenu: function () {
      return useMenu();
    },
  },
  watch: {
    "filter.Supplier": function () {
      this.listDetail = [];
    },
    "filter.PONumber": function () {
      this.listDetail = [];
    },
    "filter.ShippingInstructionNo": function () {
      if (this.filter.ShippingInstructionNo) this.getSI();
      else {
        this.isNew = true;
        let today = new Date();
        this.filter.PeriodFrom = new Date(
          today.getFullYear(),
          today.getMonth(),
          1,
        );
        this.filter.PeriodUntil = today;
        this.filter.PONumber = null;
        this.listDetail = [];
        this.filter.ReceiptId = null;
        this.model = {
          ShippingInstructionDate: null,
          Details: [],
        };
      }
    },
  },
  mounted: function () {
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "E14",
      )[0].AllowUpdate;
    });
    let today = new Date();
    this.model.ShippingInstructionDate = today;
    this.filter.DeliveryFrom = today;
    this.filter.DeliveryTo = today;
  },
  methods: {
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    reset: function () {
      let today = new Date();
      this.isNew = true;
      this.filter = {
        Supplier: "ALL",
        DeliveryFrom: today,
        DeliveryTo: today,
        PONumber: null,
        ShippingInstructionNo: null,
      };
      this.model = {
        ShippingInstructionDate: today,
        Details: [],
      };
      this.listDetail = [];
    },
    changeNew: function (e) {
      if (e.target.checked) this.reset();
    },

    check: function (e, item) {
      item.Selected = e.target.checked;
    },
    print() {
      debugger;
      this.PrintSuratJalan(this.filter.ShippingInstructionNo)
        .then((dt) => {
          toastSuccess("Download successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          //toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },

    PrintSuratJalan: function (sino) {
      this.isLoading = true;
      return this.$http
        .post(`/shipping-instruction/report-surat-jalan?sino=${sino}`, null, {
          responseType: "blob",
        })
        .then((res) => {
          const blob =
            res.data instanceof Blob
              ? res.data
              : new Blob([res.data], { type: "application/pdf" });

          const url = window.URL.createObjectURL(blob);

          let fileName = "Surat_Jalan.pdf";
          const contentDisposition = res.headers["content-disposition"];
          if (contentDisposition) {
            const match = contentDisposition.match(
              /filename\*?=(?:UTF-8''|")?([^;"\n]+)/i,
            );
            if (match && match[1]) {
              fileName = decodeURIComponent(match[1].trim());
            }
          }

          const link = document.createElement("a");
          link.href = url;
          link.download = fileName;
          document.body.appendChild(link);
          link.click();

          document.body.removeChild(link);
          window.URL.revokeObjectURL(url);
        })
        .catch(async (err) => {
          if (err.response) {
            // server ngirim response, tapi error
            const blob = err.response.data;
            try {
              const text = await blob.text();
              const json = JSON.parse(text);
              throw {
                message: json.Message || "Server returned an error",
                isServerError: true,
              };
            } catch (e) {
              console.log("Server Error, but not JSON", e);
              throw {
                message: e.message || "Server returned an error",
                isServerError: true,
              };
            }
          }

          if (err?.code === "ERR_NETWORK") this.isNetworkError = true;
          if (err?.code === "ERR_BAD_RESPONSE") this.isServerError = true;

          throw err;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    submit: function () {
      this.isLoading = true;
      this.errors = {};

      let details = this.listDetail.filter((x) => x.Selected);
      if (details.length == 0) {
        this.isLoading = false;
        toastDanger("Please Choose Part No!");
        return;
      }

      this.model.ShippingInstructionNo =
        this.filter.ShippingInstructionNo?.toString() || "";
      this.model.ShippingInstructionDate = this.model.ShippingInstructionDate;
      this.model.PONumber = this.filter.PONumber;
      this.model.Supplier = this.filter.Supplier;
      this.model.Details = details.map((p) => {
        return {
          Item_Code: p.Item_Code,
          PO_SeqNo: p.PO_SeqNo,
          Qty: p.Qty?.toString() || "0",
          SerialNo_From: p.SerialNo_From?.toString() || " ",
          SerialNo_To: p.SerialNo_To?.toString() || " ",
        };
      });

      if (this.isNew) {
        this.createSI();
      } else {
        this.updateSI();
      }
    },

    getSI: function () {
      this.ds.loadDetail(this.filter.ShippingInstructionNo).then((dt) => {
        this.model = this.deepClone(dt.Data || {});
        this.filter.Supplier = this.model.Supplier;
        this.filter.PONumber = this.model.PONumber;
        this.filter.DeliveryFrom = this.model.DeliveryDate;
        this.filter.DeliveryTo = this.model.DeliveryDate;
        this.$nextTick(() => setTimeout(() => this.searchDetail(), 500));
      });
    },
    searchDetail: function () {
      if (!this.filter.Supplier) {
        toastDanger("Please Select Supplier!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          Supplier: this.filter.Supplier,
          PONumber: this.filter.PONumber?.toString() || "0",
          ShippingInstructionNo:
            this.filter.ShippingInstructionNo?.toString() || " ",
          DeliveryFrom: this.filter.DeliveryFrom,
          DeliveryTo: this.filter.DeliveryTo,
        },
      ];
      this.ds.listDetail(filters).then((dt) => {
        this.listDetail = dt.Data.Items;
        this.listDetail.map((x) => (x.Selected = x.SIDetailID > 0));
      });
    },
    viewDetail: function (sino, item, pono, poseqno) {
      this.selectedSINo = sino;
      this.selectedItem = item;
      this.selectedPONo = pono;
      this.selectedPOSeqNo = poseqno;
      this.counter++;
      this.$bvModal.show("modal-list-detail");
    },

    remove: function () {
      if (!this.filter.ShippingInstructionNo) {
        toastDanger("Silahkan pilih Shipping Instruction No!");
        return;
      }

      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(this.filter.ShippingInstructionNo)
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
    createSI: function () {
      this.ds
        .create(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          //toastDanger(err?.Message);
        })
        .finally(() => (this.isLoading = false));
    },
    updateSI: function () {
      this.ds
        .update(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.reset();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          // toastDanger(err?.Message);
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
