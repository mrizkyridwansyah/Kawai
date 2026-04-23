<template>
  <v-frame title="Workstation Line Setting Master" icon="database">
    <template #frame-content>
      <!-- FILTER SECTION -->
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Factory</label>
          <filter-factory-privileges
            class="form-control"
            v-model="filter.factory"
            style-code="width:120px"
            style-desc="width:250px"
          />
        </div>

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Process</label>
          <filter-trade-2
            class="form-control"
            placeholder=" "
            v-model="filter.supplier"
            :trade-cls="['1']"
            style-code="width:120px"
            style-desc="width:250px"
          />
        </div>

        <!-- 3 -->
        <div class="filter-item">
          <label class="form-label">Line</label>
          <filter-line-factory
            class="form-control"
            :company="filter.factory"
            :manufacture="filter.supplier"
            placeholder=" "
            v-model="filter.linecode"
            style-code="width:120px"
            style-desc="width:250px"
          />
        </div>
      </div>

      <div class="button-section">
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
           
            <v-button-search-reset
              class="ms-1"
              :search="search"
              :reset="reset"
            />
             <v-button-print
              :print="print"
              cClass="ml-1"
              :is-loading="isLoadingPrint"
            />
          </div>
        </div>
      </div>
      <hr />
      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :ds="ds"
        :default-height="280"
        :max-height="280"
        :use-paging="false"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle"
            style="width: 100%"
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Setting</th>
                <th class="text-center">Print</th>
                <th class="text-center">WS Code</th>
                <th class="text-center">Description</th>
                <th class="text-center">Stop Point Code #1</th>
                <th class="text-center">Stop Point Code #2</th>
                <th class="text-center">Stop Point Code #3</th>
                <th class="text-center">Register Date</th>
                <th class="text-center">Register User</th>
                <th class="text-center">Last Update</th>
                <th class="text-center">Last User</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in ds.data.Items">
                <td class="text-center">
                  <div style="justify-items: center">
                    <input-checkbox
                      v-model="item.AllowSetting"
                      @click="(e) => allowDataSetting(e, item)"
                    />
                  </div>
                </td>
                <td>
                  <div style="justify-items: center">
                    <input-checkbox
                      :modelValue="isChecked(item.WorkStationCode)"
                      @update:modelValue="(checked) => check(checked, item)"
                    />
                  </div>
                </td>
                <td>{{ item.WorkStationCode }}</td>
                <td>{{ item.WorkStationName }}</td>
                <td style="width: 150px !important">
                  <div style="justify-items: center; display: grid">
                    <input-stoppointbyaddress
                      :line="filter.linecode"
                      :workstation="item.WorkStationCode"
                      v-model="item.StopPointCode"
                      :width="'100%'"
                      :include-temp="true"
                      :errors="item.errors?.StopPointCode"
                      @update:modelValue="
                        (value) => onStopPointChange(value, item)
                      "
                    />
                  </div>
                </td>
                <td style="width: 150px !important">
                  <div style="justify-items: center; display: grid">
                    <input-stoppointbyaddress
                      :line="filter.linecode"
                      :workstation="item.WorkStationCode"
                      v-model="item.StopPointCode2"
                      :width="'100%'"
                      :include-temp="true"
                      :errors="item.errors?.StopPointCode2"
                      @update:modelValue="
                        (value) => onStopPointChange2(value, item)
                      "
                    />
                  </div>
                </td>
                <td style="width: 150px !important">
                  <div style="justify-items: center; display: grid">
                    <input-stoppointbyaddress
                      :line="filter.linecode"
                      :workstation="item.WorkStationCode"
                      v-model="item.StopPointCode3"
                      :width="'100%'"
                      :include-temp="true"
                      :errors="item.errors?.StopPointCode3"
                      @update:modelValue="
                        (value) => onStopPointChange3(value, item)
                      "
                    />
                  </div>
                </td>
                <td>{{ $func.formatDateTime(item.RegisterDate) }}</td>
                <td>{{ item.RegisterUser }}</td>
                <td>{{ $func.formatDateTime(item.LastUpdate) }}</td>
                <td>{{ item.LastUser }}</td>
              </tr>
            </tbody>
          </table>
        </template>
      </v-table>
    </template>
  </v-frame>
</template>
<script>
import { width } from "@fortawesome/free-solid-svg-icons/fa0";

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
    onStopPointChange: function (value, item) {
      //jika belum centang allowsetting maka error
      if (!item.AllowSetting) {
        toastWarning("Please allow setting before set Stop Point Code");
        return;
      }

      item.StopPointCode = value;
      this.validateDuplicateStopPoint(item);
    },
    validateDuplicateStopPoint: function (item) {
      const duplicate = this.ds.data.Items.find(
        (x) =>
          x.StopPointCode === item.StopPointCode &&
          x.WorkStationCode !== item.WorkStationCode,
      );
      if (duplicate) {
        toastWarning("Duplicate Stop Point Code!  " + item.StopPointCode);
        //clear value

        // item.errors = {
        //   ...item.errors,
        //   // StopPointCode: "Duplicate Stop Point Code",

        // };
      } else if (item.errors) {
        delete item.errors.StopPointCode;
        if (Object.keys(item.errors).length === 0) {
          delete item.errors;
        }
      }
    },

    onStopPointChange2: function (value, item) {
      //jika belum centang allowsetting maka error
      if (!item.AllowSetting) {
        toastWarning("Please allow setting before set Stop Point Code");
        return;
      }

      item.StopPointCode2 = value;
      this.validateDuplicateStopPoint2(item);
    },
    validateDuplicateStopPoint2: function (item) {
      const duplicate = this.ds.data.Items.find(
        (x) =>
          x.StopPointCode2 === item.StopPointCode2 &&
          x.WorkStationCode !== item.WorkStationCode,
      );
      if (duplicate) {
        toastWarning("Duplicate Stop Point Code!  " + item.StopPointCode2);
      } else if (item.errors) {
        delete item.errors.StopPointCode2;
        if (Object.keys(item.errors).length === 0) {
          delete item.errors;
        }
      }
    },

    onStopPointChange3: function (value, item) {
      //jika belum centang allowsetting maka error
      if (!item.AllowSetting) {
        toastWarning("Please allow setting before set Stop Point Code");
        return;
      }

      item.StopPointCode3 = value;
      this.validateDuplicateStopPoint3(item);
    },
    validateDuplicateStopPoint3: function (item) {
      const duplicate = this.ds.data.Items.find(
        (x) =>
          x.StopPointCode3 === item.StopPointCode3 &&
          x.WorkStationCode !== item.WorkStationCode,
      );
      if (duplicate) {
        toastWarning("Duplicate Stop Point Code!  " + item.StopPointCode3);
      } else if (item.errors) {
        delete item.errors.StopPointCode3;
        if (Object.keys(item.errors).length === 0) {
          delete item.errors;
        }
      }
    },

    submit: function () {
      if (!this.filter.supplier) {
        toastWarning("Please select process!");
        return;
      }

      if (!this.filter.linecode) {
        toastWarning("Please select line!");
        return;
      }

      // CEK apakah ada yang dicentang
      const hasChecked = this.ds.data.Items.some((x) => x.AllowSetting);

      if (!hasChecked) {
        toastWarning("Please check at least one Setting!");
        return;
      }

      const usedStopPoints = new Set();
      const usedStopPoints2 = new Set();
      const usedStopPoints3 = new Set();

      for (let item of this.ds.data.Items) {
        if (item.AllowSetting && item.StopPointCode) {
          console.log("Checking Stop Point Code: ", usedStopPoints);
          if (usedStopPoints.has(item.StopPointCode)) {
            toastWarning("Duplicate Stop Point Code : " + item.StopPointCode);
            return;
          }

          usedStopPoints.add(item.StopPointCode);
        }

        if (item.AllowSetting && item.StopPointCode2) {
          console.log("Checking Stop Point Code: ", usedStopPoints2);
          if (usedStopPoints2.has(item.StopPointCode2)) {
            toastWarning("Duplicate Stop Point Code : " + item.StopPointCode2);
            return;
          }

          usedStopPoints2.add(item.StopPointCode2);
        }

        if (item.AllowSetting && item.StopPointCode3) {
          console.log("Checking Stop Point Code: ", usedStopPoints3);
          if (usedStopPoints3.has(item.StopPointCode3)) {
            toastWarning("Duplicate Stop Point Code : " + item.StopPointCode3);
            return;
          }

          usedStopPoints3.add(item.StopPointCode3);
        }
      }
      this.isLoading = true;

      const payload = {
        LineCode: this.filter.linecode || "", // << kirim line code di sini
        SettingList: this.ds.data.Items.map((item) => ({
          WorkStationCode: item.WorkStationCode,
          AllowSetting: item.AllowSetting,
          StopPointCode: item.StopPointCode,
          StopPointCode2: item.StopPointCode2,
          StopPointCode3: item.StopPointCode3,
        })),
      };
      debugger;
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

      this.ds.load().then(() => {
        this.ds.data.Items.forEach((x) => {
          x.StopPointCode = x.StopPointCode ?? "";
          x.StopPointCode2 = x.StopPointCode2 ?? "";
          x.StopPointCode3 = x.StopPointCode3 ?? "";
        });
      });
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
        (p) => p.Key === item.Barcode,
      );
      if (checked && existingIndex === -1) {
        this.selectedPrint.push({
          Key: item.Barcode,
          Value: item.WorkStationName,
          Value1: item.StopPointCode,
          Value2: item.StopPointCode2,
          Value3: item.StopPointCode3,
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

<style scoped>
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
.filter-wrapper:has(.filter-item:nth-child(8)) {
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
@media (max-width: 768px) {
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
  width: 100%;
}
</style>
