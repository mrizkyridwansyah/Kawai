<template>
  <v-frame title="Production Result Manual Input" icon="dumpster-fire">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Line</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-text v-model="lineName" disabled style="width: 200px" />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Item.</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-text v-model="ItemName" disabled style="width: 200px" />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Production ID</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-text v-model="ProdID" disabled style="width: 200px" />
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
              <v-button-submit
                :submit="submit"
                cClass="ml-1"
                :disabled="this.ds.newRequest[0].ProcessCode=== 'DP' || !menuPrivAllowUpdate "
                :is-loading="isLoading"
                
              />
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <v-table
        :ds="ds"
        :ds-data="ds.dataDetails"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
        :top-content-height="320"
      >
        <template #table-content>
          <table
            class="table table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center" style="vertical-align: middle">
                  Schedule Date
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Parent Item
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Parent Item Name
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Result Qty
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Child Item Code
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Child Item Name
                </th>
                <th class="text-center" style="vertical-align: middle">
                  BOM Qty
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Req. Qty
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Scan Qty
                </th>

                <th class="text-center" style="vertical-align: middle">
                  Barcode Detail
                </th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists"
                :key="item.ProdResultId + '-' + item.ProductionId"
              >
                <!-- Parent Row -->
                <tr>
                  <td>{{ $func.formatDate(item.ScheduleDate) }}</td>
                  <td>{{ item.ParentItemCode }}</td>
                  <td>{{ item.ParentItemName }}</td>
                  <td class="text-right" style="white-space: nowrap">
                    {{ $func.formatMoney(item.ResultQty) }}
                    <span
                      :class="[
                        'toggle-button',
                        item.Expanded ? 'collapse' : 'expand',
                      ]"
                      @click="toggleExpand(item)"
                      style="cursor: pointer"
                    >
                      {{ item.Expanded ? "-" : "+" }}
                    </span>
                  </td>
                  <td colspan="6"></td>
                </tr>

                <!-- Child Classification + Details (hanya tampil saat expanded) -->
                <template v-if="item.Expanded">
                  <template
                    v-for="dtl in item.details"
                    :key="dtl.MaterialItemCode"
                  >
                    <!-- Classification Row -->
                    <tr>
                      <td colspan="4"></td>
                      <td :class="getRowColor(dtl.Status)">
                        {{ dtl.MaterialItemCode }}
                      </td>
                      <td :class="getRowColor(dtl.Status)">
                        {{ dtl.MaterialItemName }}
                      </td>
                      <td class="text-right" :class="getRowColor(dtl.Status)">
                        {{ $func.formatMoney(dtl.BOMQty) }}
                      </td>
                      <td class="text-right" :class="getRowColor(dtl.Status)">
                        {{ $func.formatMoney(dtl.RequirementQty) }}
                      </td>
                      <td class="text-right" :class="getRowColor(dtl.Status)">
                        {{ $func.formatMoney(dtl.ScanQty) }}
                      </td>

                      <td class="text-center">
                        <a
                          href="javascript:void(0)"
                          @click="viewStock(dtl.MaterialItemCode)"
                          >View Detail</a
                        >
                      </td>
                    </tr>
                  </template>
                </template>
              </template>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>

  <v-modal
    title="Detail Supply Scan"
    class="modal-lg"
    id="modal-list-supply-scan"
  >
    <shared-supply-scan
      :item="this.selectedItem"
      :productionid="this.ProdID"
      :counter="this.counter"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    model: {},
    debounce: null,
    lists: [],
    groupLists: [],
    isLoading: false,
    selectedItem: null,
    menuPrivAllowUpdate: false,
    productionid: null,
    counter: 0,
    processCode:"",
    lineName: "",
    ItemName: "",
    ProdID: "",
  }),
  computed: {
    ds: function () {
      
      return useProductionManual();
    },
     dsMenu: function () {
      return useMenu();
    },
  },
  mounted: function () {
     this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "P01",
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
    
        if (dt.Data.length > 0) {
      
          this.lineName = dt.Data[0].LineName;
          this.ItemName = dt.Data[0].ParentItemName;
          this.ProdID = dt.Data[0].ProductionId;
        }

        dt.Data.forEach((item) => {
          let parentKey = [
            item.ProdResultId,
            item.ProductionId,
            item.ScheduleDate,
            item.LineCode,
            item.ParentItemCode,
            item.ResultQty,
          ].join("|");

          if (!grouped[parentKey]) {
            grouped[parentKey] = {
              ...item,
              Expanded: true,
              details: [],
            };
          }

          grouped[parentKey].details.push({
            MaterialItemCode: item.MaterialItemCode,
            MaterialItemName: item.MaterialItemName,
            BOMQty: item.BOMQty,
            RequirementQty: item.RequirementQty,
            ScanQty: item.ScanQty,
            RemainingQty: item.RemainingQty,
            Status: item.Status,
          });
        });

        // WAJIB ADA
        this.groupLists = Object.values(grouped);
      });
    },
    submit: function () {
      /* ===============================
        VALIDASI MATERIAL MERAH
      =============================== */
      let hasRed = this.groupLists.some((parent) =>
        (parent.details || []).some((dtl) => dtl.Status == 0),
      );

      if (hasRed) {
        toastWarning(
          "Material Supply Request tidak mencukupi berdasarkan Result Qty yang di input. Silahkan Scan Material atau ubah Result Qty lebih kecil",
        );
        return;
      }

      /* ===============================
     LOADING
  =============================== */
      this.isLoading = true;

      /* ===============================
     AMBIL GROUP VALID
  =============================== */
      let avaiableGroupList = [
        ...new Set(
          this.groupLists
            .filter((group) =>
              (group.details || []).some((detail) => detail.MaterialItemCode),
            )
            .map((group) => group.ProductionId),
        ),
      ];

      /* ===============================
        PAYLOAD
      =============================== */
      let payload = this.ds.newRequest
        .filter((x) => avaiableGroupList.includes(x.ProductionId))
        .map((x) => {
          return {
            LineCode: x.LineCode,
            ProdResultId: x.ProdResultId,
            ProductionId: x.ProductionId,
            ScheduleDate: x.ScheduleDate,
            ItemCode: x.ItemCode,
            ResultQty: x.ResultQty,
          };
        });

      /* ===============================
        SAVE
      =============================== */
      this.ds
        .save(payload)
        .then(() => {
          toastSuccess("Data saved successfully!");
          this.back();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message || "Failed save data");
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    back: function () {
      this.$router.push({
        path: "/app/productionresultmanual",
        query: {
          back: 1,
        },
      });
    },
    viewStock: function (item) {
      this.selectedItem = item;
      this.productionid = this.ProdID;
      this.counter++;
      this.$bvModal.show("modal-list-supply-scan");
    },
    toggleExpand: function (item) {
      item.Expanded = !item.Expanded;
    },
    getRowColor(status) {
      if (status == 1) return "cell-warning";
      if (status == 0) return "cell-danger";
      if (status == 2) return "cell-success";
      return "";
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
.cell-warning {
  background-color: #ffffff !important;
}

.cell-danger {
  background-color: #ff5365 !important;
}

.cell-success {
  background-color: #ffffff !important;
}
.toggle-button.collapse {
  color: #dc3545;
  border-color: #dc3545;
  background-color: #ffe6e6;
}

.toggle-button:hover {
  opacity: 0.85;
}
</style>
