<template>
  <v-frame title="Workstation Line Setting Master" icon="database">
    <template #frame-content>
      <div class="row">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-2"
          >Factory</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-10">
          <filter-factory-privileges
            class="form-control"
            v-model="filter.factory"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-2"
          >Process</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-10">
          <filter-trade
            placeholder="Search Process"
            v-model="filter.supplier"
            :trade-cls="['1']"
          />
        </div>
      </div>
      <div class="row mt-1">
        <label
          class="form-label col-form-label col-xl-1 col-lg-1 col-md-2 col-sm-2 col-xs-2"
          >Line</label
        >
        <div class="col-xl-6 col-lg-6 col-md-10 col-sm-10 col-xs-10">
          <filter-line-factory
            :company="filter.factory"
            :manufacture="filter.supplier"
            v-model="filter.linecode"
          />
        </div>
      </div>

      <div class="d-flex mt-3">
        <div class="d-flex flex-fill">
          <button
            class="btn btn-sm btn-primary btn-elevate"
            @click="submit"
            :disabled="isLoading"
          >
            <div
              class="spinner-border spinner-border-sm text-light"
              role="status"
              v-if="isLoading"
            >
              <span class="visually-hidden">Loading...</span>
            </div>
            <font-awesome-icon icon="save" v-else />
            <span class="ml-2">Save </span>
          </button>
          <v-button-print
            :print="print"
            cClass="ml-1"
            :is-loading="isLoadingPrint"
          />
          <v-button-search-reset class="ms-1" :search="search" :reset="reset" />
        </div>
      </div>
      <v-table-full :filter="filter" :keyword-keys="keywordKeys" :ds="ds" >
        <template #table-content>
          <table
            class="table table-striped mb-0 align-middle v-fixed-table"
            style="width: 100%"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
          >
            <thead>
              <tr>
                <th class="text-center">Print</th>
                <th class="text-center"></th>
                <th class="text-center">WS Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Register Date</th>
                <th class="text-center">Register User</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.WorkStationCode)"
                      @update:modelValue="(checked) => check(checked, item)"
                    />
                  </div>
                </td>
                <td class="text-center">
                  <div style="justify-items: center">
                    <input-checkbox
                      v-model="item.AllowSetting"
                      @click="(e) => allowDataSetting(e, item)"
                    />
                  </div>
                </td>
                <td>{{ item.WorkStationCode }}</td>
                <td>{{ item.WorkStationName }}</td>
                <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
                <td>{{ item.RegisterUser }}</td>
                <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                <td>{{ item.LastUser }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table-full>
    </template>
  </v-frame>
</template>
<script>
export default {
  data: () => ({
    keywordKeys: [
      {
        Id: "WorkStationCode",
        Name: "WorkStationCode",
      },
      {
        Id: "WorkStationName",
        Name: "WorkStationName",
      },
    ],
    filter: {
      factory: null,
      supplier: null,
      linecode: null,
      keyword: null,
      sorts: {
        WorkStationCode: "asc",
      },
      sortItems: [
        {
          label: "WorkStation Name",
          value: "WorkStationName",
          selected: false,
          direction: "asc",
        },
        {
          label: "WorkStation Code",
          value: "WorkStationCode",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
    selectedPrint: [],
    isLoading: false,
    isLoadingPrint: false,
  }),
  computed: {
    ds: function () {
      return useWorkStationSetting();
    },
  },
  watch: {
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },

  methods: {
    submit: function () {
      if (!this.filter.supplier) {
        toastWarning("Please select process!");
        return;
      }

      if (!this.filter.linecode) {
        toastWarning("Please select line!");
        return;
      }
      this.isLoading = true;
      debugger;
      const payload = {
        LineCode: this.filter.linecode || "", // << kirim line code di sini
        SettingList: this.ds.data.Items.map((item) => ({
          WorkStationCode: item.WorkStationCode,
          AllowSetting: item.AllowSetting,
        })),
      };

      this.ds
        .submitworkstationsetting(payload)
        .then(() => {
          toastSuccess("Workstation setting berhasil disimpan");
          this.search();
        })
        .catch((err) => {
          toastDanger(err?.Message || "Gagal menyimpan data");
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    allowDataSetting: function (e, item) {
      const newValue = item.AllowSetting ? 1 : 0;
    },

    search: function () {
      if (!this.filter.supplier) {
        toastWarning("Please select process!");
        return;
      }

      if (!this.filter.linecode) {
        toastWarning("Please select line!");
        return;
      }
      this.ds.setSort(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          LineCode: this.filter.linecode || "",
        },
      ];

      this.ds.setFilter(filters);
      this.ds.load();
    },
    reset: function () {
      this.filter.factory = null;
      this.filter.supplier = null;
      this.filter.linecode = null;
      this.ds.data.Items = [];
      //this.search();
    },

    check: function (checked, item) {
      const existingIndex = this.selectedPrint.findIndex(
        (p) => p.Key === item.Barcode
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.Barcode,
          Value: item.WorkStationName,
        });
      } else if (!checked && existingIndex !== -1) {
        this.selectedPrint.splice(existingIndex, 1);
      }
    },
    isChecked: function (code) {
      return this.selectedPrint.some((p) => p.Key === code);
    },
    print: function () {
      if (!this.filter.supplier) {
        toastWarning("Please select process!");
        return;
      }

      if (!this.filter.linecode) {
        toastWarning("Please select line!");
        return;
      }
      this.isLoadingPrint = true;
      if (this.selectedPrint.length === 0) {
        toastWarning("Please choose Work Station");
        this.isLoadingPrint = false;
        return;
      }
      new Promise((resolve, reject) => {
        this.ds
          .exportQR(this.selectedPrint)
          .then((_) => {
            this.selectedPrint = [];
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          })
          .finally(() => {
            setTimeout(() => {
              this.isLoadingPrint = false;
            }, 1000);
          });
      });
    },
  },
};
</script>

<style>
thead {
  white-space: nowrap;
}
</style>
