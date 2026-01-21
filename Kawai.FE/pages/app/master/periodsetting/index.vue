<template>
  <v-frame title="Period Setting" icon="database">
    <template #frame-content>
      <div class="row">
        <table class="form-table" style="width: 40%">
          <colgroup>
            <col style="width: 20%" />
            <col style="width: 30%" />
            <col style="width: 10%" />
            <col style="width: 20%" />
            <col style="width: 20%" />
          </colgroup>
          <tr>
            <td style="text-align: center"><label>Period Year</label></td>
            <td><input-periodyear v-model="filter.Year" /></td>
            <td>
              <button
                class="btn btn-sm btn-blue btn-elevate mr-1"
                @click="searchPeriodSettingDetail"
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
            </td>
            <td></td>
            <td></td>
          </tr>
        </table>
      </div>
      <hr />
      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <v-button-submit
            :submit="submit"
            cClass="mr-1"
            :is-loading="isLoading"
          />

          <button
            class="btn btn-sm btn-red btn-elevate mr-1"
            @click="reset"
            :disabled="isLoading"
          >
            <div
              class="spinner-border spinner-border-sm text-light"
              role="status"
              v-if="isLoading"
            >
              <span class="visually-hidden">Loading...</span>
            </div>
            <font-awesome-icon v-else icon="rotate-left" />
            <span class="ml-2">Clear</span>
          </button>
        </div>
      </div>

      <v-table-input :data-items="listPeriodSettingDetail" ref="vtable">
         <template #table-content>
          <div class="detail-content">
            <table
              class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
              v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
              ref="table"
            >
         
              <thead>
                <tr>
                  <th class="text-center">Month</th>
                  <th class="text-center">Start Period</th>
                  <th class="text-center">End Period</th>
                  <th class="text-center">Start SO</th>
                  <th class="text-center">Finish SO</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(item, idx) in listPeriodSettingDetail || []"
                  :key="idx"
                >
                  <td>{{ item.MonthName }}</td>
                  <!-- Start Period -->
                  <td>
                    <div class="dt-wrapper">
                      <input
                        type="date"
                        class="form-control form-control-sm"
                        v-model="item.StartDate"
                        :disabled="isRowDisabled(item)"
                      />
                      <input
                        type="time"
                        class="form-control form-control-sm"
                        v-model="item.StartTime"
                        :disabled="isRowDisabled(item)"
                      />
                    </div>
                  </td>

                  <!-- End Period -->
                  <td>
                    <div class="dt-wrapper">
                      <input
                        type="date"
                        class="form-control form-control-sm"
                        v-model="item.EndDate"
                        :disabled="isRowDisabled(item)"
                        @change="onEndDateChange(item)"
                      />
                      <input
                        type="time"
                        class="form-control form-control-sm"
                        v-model="item.EndTime"
                        :disabled="isRowDisabled(item)"
                        @change="onEndTimeChange(item)"
                      />
                    </div>
                  </td>

                  <!-- Start SO -->
                  <td>
                    <div class="dt-wrapper">
                      <input
                        type="date"
                        class="form-control form-control-sm"
                        v-model="item.StartSODate"
                        :disabled="isRowDisabled(item)"
                      />
                      <input
                        type="time"
                        class="form-control form-control-sm"
                        v-model="item.StartSOTime"
                        :disabled="isRowDisabled(item)"
                      />
                    </div>
                  </td>

                  <!-- Finish SO -->
                  <td>
                    <div class="dt-wrapper">
                      <input
                        type="date"
                        class="form-control form-control-sm"
                        v-model="item.EndSODate"
                        :disabled="isRowDisabled(item)"
                      />
                      <input
                        type="time"
                        class="form-control form-control-sm"
                        v-model="item.EndSOTime"
                        :disabled="isRowDisabled(item)"
                      />
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
            <v-data-empty
              class="mt-3"
              v-if="
                !ds.isLoading &&
                !ds.isNetworkError &&
                !ds.isServerError  &&
      (!this.listPeriodSettingDetail ||
        this.listPeriodSettingDetail.length === 0)
              "
            />
          </div>
        </template>
      </v-table-input>
    </template>
  </v-frame>
</template>

<script>
export default {
  data: () => ({
    filter: {
      Year: null,
    },
    model: {
      Year: "",
      Details: [],
    },
    listPeriodSettingDetail: [],
    debounce: null,
    isLoading: false,
    errors: {},
  }),
  computed: {
    ds: function () {
      return usePeriodSetting();
    },
    currentYear() {
      return new Date().getFullYear();
    },
    currentMonth() {
      return new Date().getMonth() + 1; // 1–12
    },
    isCurrentYear() {
      return Number(this.filter.Year) === this.currentYear;
    },
  },

  watch: {
    "filter.Year": function () {
      this.listPeriodSettingDetail = [];
    },
  },
  mounted: function () {},
  methods: {
    isRowDisabled(item) {
      // jika bukan tahun sekarang → disable semua
      if (!this.isCurrentYear) return true;

      // disable bulan Januari s/d (bulan sekarang - 2)
      if (item.Month <= this.currentMonth - 2) return true;

      return false;
    },
    onEndDateChange(item) {
      if (!item.EndDate) return;

      // auto isi Start SO & End SO Date
      item.StartSODate = item.EndDate;
      item.EndSODate = item.EndDate;
    },

    onEndTimeChange(item) {
      if (!item.EndTime) return;

      // auto isi End SO Time
      item.EndSOTime = item.EndTime;
    },
    deepClone: function (obj) {
      return typeof structuredClone === "function"
        ? structuredClone(obj)
        : JSON.parse(JSON.stringify(obj));
    },
    reset: function () {
      this.filter = {
        Year: null,
      };
      this.model = {
        Year: "",
        Details: [],
      };
      this.listPeriodSettingDetail = [];
    },

    submit: function () {
      this.isLoading = true;
      this.errors = {};
      let details = this.listPeriodSettingDetail;

      this.model.Year = this.filter.Year;
      const toDateTime = (d, t) => (!d || !t ? null : `${d} ${t}:00`);
      this.model.Details = details
        .filter((p) => p.StartDate || p.EndDate || p.StartSODate || p.EndSODate)
        .map((p) => ({
          Period: p.Period,
          Year: p.Year,
          Month: p.Month,
          MonthName: p.MonthName,
          StartPeriod: !p.StartDate
            ? null
            : toDateTime(p.StartDate, p.StartTime),
          EndPeriod: !p.EndDate ? null : toDateTime(p.EndDate, p.EndTime),
          StartSO: !p.StartSODate
            ? null
            : toDateTime(p.StartSODate, p.StartSOTime),
          FinishSO: !p.EndSODate ? null : toDateTime(p.EndSODate, p.EndSOTime),
        }));
      this.update();
    },
    searchPeriodSettingDetail: function () {
      if (!this.filter.Year) {
        toastDanger("Silahkan pilih Year!");
        return;
      }

      let filters = { ...this.ds.filter };
      filters.Filters = [
        {
          Year: this.filter.Year,
        },
      ];

      this.ds.listPeriodSettingDetail(filters).then((dt) => {
        console.log(dt.Data.Items[0]);
        console.log(typeof dt.Data.Items[0].StartDate);
        this.listPeriodSettingDetail = dt.Data.Items;
      });
    },
    update: function () {
      this.ds
        .update(this.model)
        .then((dt) => {
          toastSuccess("Data saved successfully!");
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
.dt-wrapper {
  display: flex;
  gap: 4px;
}

.dt-wrapper input[type="date"] {
  width: 60%;
}

.dt-wrapper input[type="time"] {
  width: 40%;
}
</style>
