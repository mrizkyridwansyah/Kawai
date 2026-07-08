<template>
  <v-frame title="Item Setting" icon="database">
    <template #frame-content>
      <!-- FILTER SECTION -->
      <div class="filter-wrapper">
        <!-- 1 -->
        <div class="filter-item">
          <label class="form-label">Model</label>
        <filter-cls-2
            type-data="Model_Cls"
            class="form-control"
            v-model="filter.modelcls"
            style-code="width: 110px"
            style-desc="width: 250px"
          />
        </div>

        <!-- 2 -->
        <div class="filter-item">
          <label class="form-label">Parent Item</label>
        <filter-item-by-modelcls
            class="form-control"
            v-model="filter.item"
            :modelCls="filter.modelcls"
            :show-option-all="true"
            style-code="width: 150px"
            style-desc="width: 210px"
          />
        </div>
      </div>

      <div class="button-section">
        <div class="d-flex mt-3">
          <div class="d-flex flex-fill">
            <button
              class="btn btn-sm btn-primary btn-elevate"
              @click="submit"
              :disabled="isLoading || !menuPrivAllowUpdate"
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
          </div>
        </div>
      </div>
      <hr />
      <v-table
        :filter="filter"
        :keyword-keys="keywordKeys"
        :ds="ds"
        :top-content-height="290"
        :use-paging="false"
        ref="vtable"
      >
        <template #table-content>
          <table
            class="table table-striped table-bordered mb-0 align-middle v-fixed-table"
              style="table-layout:auto; "
            v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
            ref="table"
          >
            <thead>
              <tr>
                <th class="text-center">Setting</th>
                <th class="text-center">Item Code</th>
                <th class="text-center">Item Name</th>
                <th class="text-center">Carton</th>
                <th class="text-center">Pallet</th>
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
                <td>{{ item.Item_Code }}</td>
                <td>{{ item.Item_Name }}</td>
                <td style="width: max-content !important">
                  <div style="justify-items: center; display: grid">
                      <input-clsnondescs
                        type-data="Carton_Cls"
                        v-model="item.Carton_Cls"
                        style-code="width:200px"
                        
                      />

                   
                  </div>
                </td>
                 <td style="width: max-content !important">
                  <div style="justify-items: center; display: grid">
                   <input-clsnondescs
                        type-data="Pallet_Cls"
                        v-model="item.Pallet_Cls"
                        style-code="width:200px"
                         
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
        Id: "Item_Code",
        Name: "Item Code",
      },
      {
        Id: "Item_Name",
        Name: "Item Name",
      },
    ],
    filter: {
      modelcls: null,
      item: null,
       keyword: null,
      sorts: {
        Item_Code: "asc",
      },
      sortItems: [
        {
          label: "Item Name",
          value: "Item_Name",
          selected: false,
          direction: "asc",
        },
        {
          label: "Item Code",
          value: "Item_Code",
          selected: true,
          direction: "desc",
        },
      ],
    },
    debounce: null,
    title: "",
    modalMode: "",
    isLoading: false,
      menuPrivAllowUpdate: false,
  }),
  computed: {
    ds: function () {
      return useItemSetting();
    },
    dsMenu: function () {
      return useMenu();
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
  mounted: function () {
     this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "A24",
      )[0].AllowUpdate;
    });
    this.ds.resetList();
  },
  methods: {
 
    submit: function () {
      if (!this.filter.modelcls) {
        toastWarning("Please select Model!");
        return;
      }

      if (!this.filter.item) {
        toastWarning("Please select Item!");
        return;
      }

      // CEK apakah ada yang dicentang
      const hasChecked = this.ds.data.Items.some((x) => x.AllowSetting);

      if (!hasChecked) {
        toastWarning("Please check at least one Setting!");
        return;
      }

      this.isLoading = true;

      const payload = {
       Model_Cls: this.filter.modelcls || "",
       ParentItem_Code: this.filter.item || "",
       ItemList: this.ds.data.Items.map((item) => ({
          Item_Code: item.Item_Code,
          AllowSetting: item.AllowSetting,
          Carton_Cls: item.Carton_Cls,
          Pallet_Cls: item.Pallet_Cls, 
        })),
      };
      debugger;
      this.ds
        .submititemsetting(payload)
        .then(() => {
          toastSuccess("Item setting save successfully");
          this.search();
        })
        .catch((err) => {
          toastDanger(err?.Message || "Failed Saved data");
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    allowDataSetting: function (e, item) {
      const newValue = item.AllowSetting ? 1 : 0;
    },

    search: function () {
      if (!this.filter.modelcls) {
        toastWarning("Please select model cls!");
        return;
      }

      if (!this.filter.item) {
        toastWarning("Please select item!");
        return;
      }

      this.ds.setSort(this.filter.sorts);

      let filters = [
        {
          Keyword: this.filter.keyword || "",
          Model_Cls: this.filter.modelcls || "",
          ParentItem_Code: this.filter.item || "",
        },
      ];

      this.ds.setFilter(filters);

      this.ds.load().then(() => {
        this.ds.data.Items.forEach((x) => {
          x.Carton_Cls = x.Carton_Cls ?? "";
          x.Pallet_Cls = x.Pallet_Cls ?? "";
        });
      });
    },
    reset: function () {
      this.filter.modelcls = null;
      this.filter.item = null; 
      this.ds.data.Items = [];
      //this.search();
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
