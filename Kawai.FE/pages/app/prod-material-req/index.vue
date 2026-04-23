<template>
  <v-frame title="Production Material Requirement" icon="boxes-stacked">
    <template #frame-content>
      <table style="width: 100%">
        <tr>
          <td style="width: 50%">
            <table>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Schedule (Up To)</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
                  <input-date
                    v-model="model.ScheduleDateTo"
                    :errors="errors?.ScheduleDateTo"
                    style-date="width: 100px !important"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Factory</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
                  <filter-factory-privileges
                    class="form-control"
                    v-model="model.Factory"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Process</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
                  <filter-trade-2
                    class="form-control"
                    :trade-cls="['1']"
                    :show-option-all="true"
                    default-option-all="ALL"
                    v-model="model.Process"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Line</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
                  <filter-line-factory
                    class="form-control"
                    :company="model.Factory"
                    :manufacture="model.Process"
                    v-model="model.Line"
                    :show-option-all="true"
                    default-option-all="ALL"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td style="padding-top: 5px">
                  <label class="form-label">Model</label>
                </td>
                <td style="padding-top: 5px; padding-left: 15px">
                  <filter-cls-2
                    type-data="Model_Cls"
                    class="form-control"
                    v-model="model.Model"
                    :show-option-all="true"
                    default-option-all="ALL"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>
              <tr>
                <td colspan="2" style="padding-top: 5px">
                  <div class="d-flex flex-fill">
                    <button
                      class="btn btn-primary btn-elevate btn-search"
                      :disabled="isLoading"
                      @click="save"
                    >
                      <font-awesome-icon icon="save" />
                      <span class="ml-2">Calculate</span>
                    </button>
                    <button
                      class="btn btn-primary btn-elevate btn-search"
                      :disabled="isLoading"
                      @click="search"
                      style="margin-left: 5px"
                    >
                      <font-awesome-icon icon="search" />
                      <span class="ml-2">Search</span>
                    </button>
                    <button
                      class="btn btn-green btn-elevate btn-search"
                      :disabled="isLoading"
                      @click="exportExcel"
                      style="margin-left: 5px"
                    >
                      <font-awesome-icon icon="file-excel" />
                      <span class="ml-2">Excel</span>
                    </button>
                  </div>
                </td>
              </tr>
            </table>
          </td>
          <td style="width: 40%; padding-left: 50px; vertical-align: top">
            <div style="border: 1px solid black; width: 100%; padding: 1em">
              <table>
                <tr>
                  <td style="padding: 10px">
                    <label class="form-label">Last Calculation Time</label>
                  </td>
                  <td style="padding: 10px; vertical-align: top">:</td>
                  <td style="padding: 10px">
                    <label class="form-label">{{
                      calculation.Header?.LastCalculation ?? ""
                    }}</label>
                  </td>
                </tr>
                <tr>
                  <td style="padding: 10px">
                    <label class="form-label">Factory</label>
                  </td>
                  <td style="padding: 10px; vertical-align: top">:</td>
                  <td style="padding: 10px">
                    <label class="form-label">{{
                      calculation.Header?.FactoryName ?? ""
                    }}</label>
                  </td>
                </tr>
                <tr>
                  <td style="padding: 10px">
                    <label class="form-label">Process</label>
                  </td>
                  <td style="padding: 10px; vertical-align: top">:</td>
                  <td style="padding: 10px">
                    <label class="form-label">{{
                      calculation.Header?.ProcessName ?? ""
                    }}</label>
                  </td>
                </tr>
                <tr>
                  <td style="padding: 10px">
                    <label class="form-label">Line</label>
                  </td>
                  <td style="padding: 10px; vertical-align: top">:</td>
                  <td style="padding: 10px">
                    <label class="form-label">{{
                      calculation.Header?.LineName ?? ""
                    }}</label>
                  </td>
                </tr>
                <tr>
                  <td style="padding: 10px">
                    <label class="form-label">Model</label>
                  </td>
                  <td style="padding: 10px; vertical-align: top">:</td>
                  <td style="padding: 10px">
                    <label class="form-label">{{
                      calculation.Header?.ModelName ?? ""
                    }}</label>
                  </td>
                </tr>
              </table>
            </div>
          </td>
        </tr>
      </table>
      <hr />
      <v-table
        :ds="ds"
        ref="vtable"
        :use-paging="false"
        :use-header="false"
        :data-items="calculation.Materials"
        :top-content-height="405"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            style="max-height: 50px !important"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Material Code</th>
                <th class="text-center">Material Name</th>
                <th class="text-center">Unit</th>
                <th class="text-center">Request Qty</th>
                <th class="text-center">Current Stock</th>
                <th class="text-center">Shortage</th>
                <th class="text-center">Line</th>
                <th class="text-center">Schedule Date</th>
                <th class="text-center">Parent Item</th>
                <th class="text-center">Qty</th>
              </tr>
            </thead>
            <tbody>
              <template
                v-for="(item, idx) in calculation.Materials || []"
                :key="idx"
              >
                <tr>
                  <td>{{ item.ChildItemCode }}</td>
                  <td>{{ item.ChildItemName }}</td>
                  <td>{{ item.UnitClsName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.TotalReqQty) }}
                  </td>
                  <td class="text-right">
                    {{ $func.formatMoney(item.CurrentStock) }}
                  </td>
                  <td
                    class="text-right"
                    style="font-weight: bold"
                    :class="item.Shortage > 0 ? '' : 'text-danger'"
                  >
                    {{ $func.formatMoney(item.Shortage) }}
                  </td>
                  <td>
                    <div style="text-align: center">
                      <span
                        v-if="item.Details.length > 0"
                        :class="[
                          'toggle-button',
                          item.Expanded ? 'collapse' : 'expand',
                        ]"
                        @click="() => (item.Expanded = !item.Expanded)"
                      >
                        {{ item.Expanded ? "-" : "+" }}
                      </span>
                    </div>
                  </td>
                  <td></td>
                  <td></td>
                  <td></td>
                </tr>
                <tr
                  v-if="item.Expanded"
                  v-for="(dtl, idxx) in item.Details || []"
                  :key="dtl.ProductionId"
                >
                  <td colspan="6"></td>
                  <td>{{ dtl.LineName }}</td>
                  <td>
                    {{ $func.formatDate(dtl.ScheduleDate) }}
                  </td>
                  <td>{{ dtl.ParentItemName }}</td>
                  <td class="text-right">
                    {{ $func.formatMoney(dtl.FinalReqQty) }}
                  </td>
                </tr>
              </template>
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
    model: {
      ParamKey: null,
      ScheduleDateTo: null,
      Factory: null,
      Process: null,
      Line: null,
      Model: null,
    },
    debounce: null,
    calculation: {
      Header: {},
      Materials: [],
    },
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return useProdMaterialRequirement();
    },
  },
  mounted: function () {
    let today = new Date();
    this.model.ScheduleDateTo = today;

    this.search();
    this.$nextTick(() =>
      setTimeout(() => {
        this.model.Process = "ALL";
        this.model.Line = "ALL";
        this.model.Model = "ALL";
      }, 500),
    );
  },
  methods: {
    search: function () {
      this.ds.getCalculation(this.model.Factory).then((dt) => {
        this.calculation = dt.Data;
        this.calculation.Materials.map((x) => (x.Expanded = false));
      });
    },
    exportExcel: function () {
      this.isLoading = true;

      return new Promise((resolve, reject) => {
        this.ds
          .exportExcel(this.model.Factory)
          .then((_) => {
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          })
          .finally(() => (this.isLoading = false));
      });
    },
    save: function () {
      this.isLoading = true;
      this.errors = {};
      this.model.ParamKey = `${this.model.Factory}|${this.model.Process || "ALL"}|${this.model.Line || "ALL"}|${this.model.Model || "ALL"}|${this.$func.formatDate(this.model.ScheduleDateTo, "YYYYMMDD")}`;
      this.ds
        .save(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
          this.search();
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
