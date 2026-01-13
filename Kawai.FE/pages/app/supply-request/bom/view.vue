<template>
  <v-frame title="Part Material Supply Request (By BOM)" icon="cart-flatbed">
    <template #frame-content>
      <table>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Supply Request No.</label>
          </td>
          <td style="padding-top: 5px; padding-left: 15px" colspan="3">
            <input-text
              v-model="ds.newRequest[0].RequestNo"
              disabled
              style="width: 200px"
            />
          </td>
        </tr>
        <tr>
          <td style="padding-top: 5px">
            <label class="form-label">Request Date</label>
          </td>
          <td
            style="padding-top: 5px; padding-left: 15px; width: 160px"
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
                :action="print"
                label="Print"
                icon="print"
                cClass="ml-1 btn-blue"
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

      <v-table
        :ds="ds"
        :ds-data="ds.dataDetails"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Schedule Date</th>
                <th class="text-center">Line</th>
                <th class="text-center">Parent Item</th>
                <th class="text-center">Parent Item Name</th>
                <th class="text-center">Qty Set</th>
                <th class="text-center">Child Item Code</th>
                <th class="text-center">Child Item Name</th>
                <th class="text-center">Requirement Qty</th>
                <th class="text-center">Current Stock</th>
                <th class="text-center">Request User</th>
                <th class="text-center">Request Date</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in groupLists || []"
                :key="item.ProductionId"
              >
                <tr>
                  <td>{{ $func.formatDate(item.PODate) }}</td>
                  <td>{{ item.AreaName }}</td>
                  <td>{{ item.ParentItemCode }}</td>
                  <td>{{ item.ParentItemName }}</td>
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
                  <td colspan="6"></td>
                </tr>
                <tr
                  v-if="item.Expanded"
                  v-for="(dtl, idxx) in item.Details || []"
                  :key="dtl.RequestId"
                >
                  <td colspan="5"></td>
                  <td>{{ dtl.ChildItemCode }}</td>
                  <td>{{ dtl.ChildItemName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(dtl.RequirementQty) }}
                  </td>
                  <td><a href="javascript:void(0)" @click="() => viewStock(dtl.ChildItemCode)">View Detail</a></td>
                  <td>{{ dtl.RegisterUser }}</td>
                  <td>{{ $func.formatDate(dtl.RegisterDate) }}</td>
                </tr>
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
  }),
  computed: {
    ds: function () {
      return useSupplyRequestBOM();
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

        dt.Data.forEach((item) => {
          let key = [
            item.RequestId,
            item.PONumber,
            item.PODate,
            item.WarehouseCode,
            item.AreaCode,
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
            ChildItemCode: item.ChildItemCode,
            ChildItemName: item.ChildItemName,
            Qty: item.Qty,
            RequirementQty: item.RequirementQty,
            RegisterUser: item.RegisterUser,
            RegisterDate: item.RegisterDate,
          });
        });

        this.groupLists = Object.values(grouped);
      });
    },
    print: function () {},
    remove: function () {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(
                this.ds.newRequest[0].RequestId,
                this.ds.newRequest[0].RequestNo
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
        this.ds.newRequest[0].RequestNo
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
    viewStock: function(item) {
      this.selectedItem = item;
      this.counter++;
      this.$bvModal.show("modal-list-stock");
    }
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
</style>
