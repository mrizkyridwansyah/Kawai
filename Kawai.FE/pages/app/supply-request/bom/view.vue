<template>
  <v-frame title="Part Material Supply Request (Subcon)" icon="cart-flatbed">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Request No.</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-text
              v-model="ds.newRequest[0].RequestNo"
              disabled
              style="width: 200px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Req. Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
            <input-date
              v-model="ds.newRequest[0].RequestDate"
              :disabled="true"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">PO Number</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 100px">
            <input-text
              v-model="ds.newRequest[0].PONumber"
              disabled
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">PO Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 100px">
            <input-date
              v-model="ds.newRequest[0].PODate"
              disabled
              style-date="width: 100px !important"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">DN Number</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
            <input-text
              v-model="model.DNNumber"
              :errors="errors?.DNNumber"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">DN Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
            <input-date
              v-model="model.DNDate"
              :errors="errors?.DNDate"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Police No.</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <input-text
              v-model="model.VehicleNo"
              :errors="errors?.VehicleNo"
              style="width: 150px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Transport By</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 300px">
            <filter-cls-2
              class="form-control"
              type-data="Transport_Cls"
              v-model="model.Transport"
              :errors="errors?.Transport"
              style-code="width: 150px"
              style-desc="width: 240px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">BC Number</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
            <input-text
              v-model="model.BCNumber"
              :errors="errors?.BCNumber"
              :disabled="disallowEditBC"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">BC Date</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px; width: 180px">
            <input-date
              v-model="model.BCDate"
              :errors="errors?.BCDate"
              style-date="width: 100px !important"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">BC Type</label>
          </td>
          <td
            style="padding-top: 5px; padding-left: 15px; width: 300px"
            colspan="3"
          >
            <filter-cls-2
              class="form-control"
              type-data="BCType_Cls"
              v-model="model.BCType"
              :errors="errors?.BCType"
              style-code="width: 150px"
              style-desc="width: 240px"
            />
          </td>
        </tr>
        <tr>
          <td colspan="4" style="padding-top: 5px">
            <div class="d-flex flex-fill">
              <v-button
                :action="back"
                label="Back"
                icon="arrow-left"
                cClass="ml-1 btn-danger"
                :is-loading="isLoading"
              />

              <v-button
                :action="remove"
                label="Delete"
                icon="trash"
                cClass="ml-1 btn-danger"
                :is-loading="isLoading"
                :disabled="this.groupLists.length == 0 || !menuPrivAllowUpdate"
              />

              <v-button
                :action="print"
                label="Print"
                icon="print"
                cClass="ml-1 btn-blue"
                :is-loading="isLoading"
                :disabled="this.groupLists.length == 0 || !menuPrivAllowUpdate"
              />

              <v-button-submit
                :submit="submit"
                cClass="ml-1"
                :is-loading="isLoading"
                :disabled="this.groupLists.length == 0 || !menuPrivAllowUpdate"
              />
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <v-table
        :ds="ds"
        :ds-data="ds.dataDetails"
        :use-paging="false"
        :use-header="false"
        :top-content-height="330"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">PO Date</th>
                <th class="text-center">Child Cls</th>
                <th class="text-center">Parent Item</th>
                <th class="text-center">Parent Item Name</th>
                <th class="text-center">Qty Set</th>
                <th class="text-center">Child Item Code</th>
                <th class="text-center" style="width: 100px">
                  Child Item Name
                </th>
                <th class="text-center">Req. Qty</th>
                <th class="text-center">Scan Qty</th>
                <th class="text-center">Crt Qty</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists || []"
                :key="item.ProductionId"
              >
                <tr>
                  <td>{{ $func.formatDate(item.PODate) }}</td>
                  <td>{{ item.ClassificationName }}</td>
                  <td>{{ item.ParentItemCode }}</td>
                  <td style="max-width: 5em">{{ item.ParentItemName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.QtySet) }}

                    <span
                      :class="[
                        'toggle-button',
                        item.Expanded ? 'collapse' : 'expand',
                      ]"
                      @click="() => (item.Expanded = !item.Expanded)"
                    >
                      {{ item.Expanded ? "-" : "+" }}
                    </span>
                  </td>
                  <td colspan="7"></td>
                </tr>
                <tr
                  v-if="item.Expanded"
                  v-for="(dtl, idxx) in item.Details || []"
                  :key="dtl.RequestId"
                >
                  <td colspan="5"></td>
                  <td>
                    <a
                      v-if="menuPrivAllowUpdate"
                      href="javascript:void(0)"
                      @click="editStock(dtl.IDSeq)"
                      >{{ dtl.ChildItemCode }}</a
                    >
                    <span v-else>{{ dtl.ChildItemCode }}</span>
                  </td>
                  <td style="white-space: wrap">{{ dtl.ChildItemName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(dtl.RequirementQty) }}
                  </td>
                  <td class="text-right">
                    <a href="javascript:void(0)" @click="viewScan(dtl.IDSeq)">
                      {{ $func.formatMoney(dtl.TotalScan) }}
                    </a>
                  </td>
                  <td>
                    <a
                      href="javascript:void(0)"
                      @click="() => viewStock(dtl.ChildItemCode)"
                      >View Detail</a
                    >
                  </td>
                </tr>
              </template>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>

  <v-modal title="Detail Stock" class="modal-lg" id="modal-list-stock">
    <shared-request-subcon-list-stock
      :item="this.selectedItem"
      :counter="this.counter"
    />
  </v-modal>

  <v-modal title="Detail Scan Supply" class="modal-lg" id="modal-list-scan">
    <shared-request-subcon-list-scan
      :item="this.selectedItem"
      :counter="this.counter"
    />
  </v-modal>
  <v-modal
    ref="modal-edit-reqqty"
    id="shared-request-subcon-edit-reqqty"
    title="Edit Requirement Qty"
    size="md"
    @hidden="
      () => {
        this.$refs.formSubcon.resetForm();
        modalMode = '';
      }
    "
  >
    <shared-request-subcon-edit-reqqty
      ref="formSubcon"
      :item="idSelected"
      :counter="this.counter"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    model: {
      RequestNo: "#AUTO",
      RequestDate: new Date(),
      DNNumber: null,
      DNDate: new Date(),
      BCType: null,
      BCNumber: null,
      BCDate: new Date(),
      VehicleNo: null,
      Transport: null,
    },
    debounce: null,
    lists: [],
    groupLists: [],
    isLoading: false,
    selectedItem: null,
    counter: 0,
    errors: {},
    menuPrivAllowUpdate: false,
    disallowEditBC: false,
  }),
  computed: {
    ds: function () {
      return useSupplyRequestBOM();
    },
    dsMenu: function () {
      return useMenu();
    },
  },
  mounted: function () {
    this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "E12",
      )[0].AllowUpdate;
    });

    this.search();
  },
  methods: {
    resetGrid: function () {
      this.lists = [];
    },
    search: function () {
      this.ds.loadDetail().then((dt) => {
        let grouped = {};

        dt.Data.forEach((item) => {
          let key = [
            item.RequestId,
            item.PONumber,
            item.PODate,
            item.WarehouseCode,
            item.ClassificationCode,
            item.ParentItemCode,
          ].join("|");

          if (!grouped[key]) {
            grouped[key] = {
              ...item,
              Expanded: true,
              Details: [],
            };
          }

          grouped[key].Details.push({
            IDSeq: item.IDSeq,
            ChildItemCode: item.ChildItemCode,
            ChildItemName: item.ChildItemName,
            Qty: item.Qty,
            RequirementQty: item.RequirementQty,
            TotalScan: item.TotalScan,
            RegisterUser: item.RegisterUser,
            RegisterDate: item.RegisterDate,
          });
        });

        this.groupLists = Object.values(grouped);

        this.ds.loadHeader().then((dt) => {
          this.model = dt;
          this.disallowEditBC =
            this.model.BCNumber != null && this.model.BCNumber != "";
        });
      });
    },

    print: function () {
      this.ds
        .PrintSuratJalan(this.model.RequestNo)
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

    submit: function () {
      this.isLoading = true;

      let payload = {
        RequestId: this.model.RequestId,
        RequestNo: this.model.RequestNo,
        DNNumber: this.model.DNNumber,
        DNDate: this.model.DNDate,
        BCNumber: this.model.BCNumber,
        BCDate: this.model.BCDate,
        BCType: this.model.BCType,
        Transport: this.model.Transport,
        VehicleNo: this.model.VehicleNo,
      };

      this.ds
        .update(payload)
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
    remove: function () {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(
                this.ds.newRequest[0].RequestId,
                this.ds.newRequest[0].RequestNo,
              )
              .then((dt) => {
                toastSuccess("Data deleted successfully!");
                resolve();
                this.back();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                resolve();
                toastDanger(err?.Message);
              });
          }),
        null,
        this.ds.newRequest[0].RequestNo,
      );
    },
    back: function () {
      this.$router.push({
        path: "/app/supply-request/bom",
        query: {
          back: 1,
        },
      });
    },
    viewStock: function (item) {
      this.selectedItem = item;
      this.counter++;
      this.$bvModal.show("modal-list-stock");
    },
    close: function () {
      this.$bvModal.hide("shared-request-subcon-edit-reqqty");
      this.search();
    },
    viewScan: function (item) {
      console.log(item);
      this.selectedItem = item;
      this.counter++;
      this.$bvModal.show("modal-list-scan");
    },

    editStock: function (item) {
      this.idSelected = item;
      this.counter++;
      this.$bvModal.show("shared-request-subcon-edit-reqqty");
    },
  },
};
</script>

<style scoped>
.vdatetime {
  max-width: 60% !important;
}

.toggle-button {
  margin-left: 1em;
  display: inline-block;
  width: 14px;
  height: 14px;
  line-height: 12px;
  font-size: 10px;
  font-weight: bold;
  text-align: center;
  border: 1px solid;
  border-radius: 50%; /* full bulat */
  cursor: pointer;
  margin-right: 6px;
  user-select: none;
}

.toggle-button.expand {
  color: #007bff;
  border-color: #007bff;
  background-color: #e6f0ff;
}

.toggle-button.collapse {
  color: #dc3545;
  border-color: #dc3545;
  background-color: #ffe6e6;
}

.toggle-button:hover {
  opacity: 0.85;
}

table thead th {
  position: sticky;
  top: 0;
  z-index: 20; /* harus lebih tinggi dari sticky kiri */
  background: #8ec5fc;
  vertical-align: middle;
}
</style>
