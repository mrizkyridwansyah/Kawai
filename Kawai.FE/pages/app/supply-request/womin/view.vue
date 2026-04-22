<template>
  <v-frame title="Part Material Supply Request (WOMIN)" icon="cart-flatbed">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Line</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-text v-model="lineName" disabled style="width: 200px" />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Supply Request No.</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-text
              v-model="ds.newRequest[0].RequestNo"
              disabled
              style="width: 200px"
            />
          </td>
          <td style="padding-top: 5px; padding-left: 15px">
            <label class="form-label">Request Date</label>
          </td>
          <td
            style="padding-top: 5px; padding-left: 15px; width: 180px"
            colspan="3"
          >
            <input-date
              v-model="ds.newRequest[0].RequestDate"
              :disabled="true"
              style-date="width: 100px !important"
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
        :default-height="400"
        :max-height="400"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center" style="vertical-align: middle">
                  Schedule Date
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Work Station
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Parent Item
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Parent Item Name
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Picking No.
                </th>
                <th class="text-center" style="vertical-align: middle">No.</th>
                <th class="text-center" style="vertical-align: middle">
                  Status
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Qty Set
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Child Cls
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Child Item Code
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Child Item Name
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Req. Qty
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Picking Qty
                </th>
                <th class="text-center" style="vertical-align: middle">
                  Crn. Qty
                </th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists"
                :key="
                  item.RequestId +
                  '-' +
                  item.ProductionId +
                  '-' +
                  item.SetNumber
                "
              >
                <!-- Parent Row -->
                <tr>
                  <td>{{ $func.formatDate(item.ScheduleDate) }}</td>
                  <td>{{ item.WorkStationName }}</td>
                  <td>{{ item.ParentItemCode }}</td>
                  <td>{{ item.ParentItemName }}</td>
                  <td class="text-right">{{ item.PickingNo }}</td>
                  <td class="text-right">{{ item.SetNumber }}</td>
                  <td>{{ item.Status }}</td>
                  <td class="text-right" style="white-space: nowrap">
                    {{ $func.formatMoney(item.QtySet) }}
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
                    v-for="(details, cls) in item.Classifications"
                    :key="cls"
                  >
                    <!-- Classification Row -->
                    <tr>
                      <td colspan="8"></td>
                      <td>{{ cls }}</td>
                      <td colspan="5"></td>
                    </tr>

                    <!-- Detail Rows -->
                    <tr v-for="dtl in details" :key="dtl.ChildItemCode">
                      <td colspan="9"></td>
                      <td>{{ dtl.ChildItemCode }}</td>
                      <td>{{ dtl.ChildItemName }}</td>
                      <td class="text-right">
                        {{ $func.formatMoney(dtl.RequirementQty) }}
                      </td>
                      <td class="text-right">
                        {{ $func.formatMoney(dtl.TotalScan) }}
                      </td>
                      <td class="text-center">
                        <a
                          href="javascript:void(0)"
                          @click="viewStock(dtl.ChildItemCode)"
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
    model: {
      RequestNo: "#AUTO",
      RequestDate: new Date(),
    },
    debounce: null,
    lists: [],
    groupLists: [],
    isLoading: false,
    selectedItem: null,
    counter: 0,
    lineName: "",
  }),
  computed: {
    ds: function () {
      return useSupplyRequestWomin();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    resetGrid: function () {
      this.lists = [];
    },
    search: function () {
      this.ds.loadDetail().then((dt) => {
        let grouped = {};

        // Isi lineName dari data pertama
        if (dt.Data.length > 0) {
          this.lineName = dt.Data[0].LineName;
        }

        dt.Data.forEach((item) => {
          // Grouping berdasarkan Parent level (tanpa ChildClassificationPartDesc)
          let parentKey = [
            item.RequestId,
            item.ProductionId,
            item.ScheduleDate,
            item.LineCode,
            item.WorkStationCode,
            item.ParentItemCode,
            item.SetNumber,
            item.Status,
          ].join("|");

          if (!grouped[parentKey]) {
            grouped[parentKey] = {
              ...item,
              Expanded: true, // default collapsed
              Classifications: {}, // child groups
            };
          }

          // Buat group classification di dalam parent
          if (
            !grouped[parentKey].Classifications[
              item.ChildClassificationPartDesc
            ]
          ) {
            grouped[parentKey].Classifications[
              item.ChildClassificationPartDesc
            ] = [];
          }

          // Push detail ke classification
          grouped[parentKey].Classifications[
            item.ChildClassificationPartDesc
          ].push({
            ChildItemCode: item.ChildItemCode,
            ChildItemName: item.ChildItemName,
            RequirementQty: item.RequirementQty,
            TotalScan: item.TotalScan,
            RequestId: item.RequestId, // optional untuk key
          });
        });

        // Convert ke array dan sorting parent groups
        this.groupLists = Object.values(grouped).sort((a, b) => {
          const dateA = new Date(a.ScheduleDate);
          const dateB = new Date(b.ScheduleDate);

          if (dateA.getTime() !== dateB.getTime()) {
            return dateA - dateB;
          }

          if (a.ParentItemCode !== b.ParentItemCode) {
            return a.ParentItemCode.localeCompare(b.ParentItemCode);
          }

          if (a.WorkStationName !== b.WorkStationName) {
            return a.WorkStationName.localeCompare(b.WorkStationName);
          }

          return Number(a.SetNumber) - Number(b.SetNumber);
        });
      });
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
        path: "/app/supply-request/womin",
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
    toggleExpand: function (item) {
      item.Expanded = !item.Expanded;
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
}
</style>
