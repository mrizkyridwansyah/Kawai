<template>
  <header-menu title="Trade Master" :breadcrumbs="this.breadcrumbs" />
  <v-button-add :add="add" cClass="mr-1" />
  <v-table
    :filter="filter"
    :export-excel="true"
    :export-excel-action="exportExcel"
    :data-items="ds.data.Items"
    :ds="ds"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">Action</th>
            <th class="text-center">Trade Code</th>
            <th class="text-center">Trade Cls</th>
            <th class="text-center">Trade Name</th>
            <th class="text-center">Trade abbr</th>
            <th class="text-center">Contact Person</th>
            <th class="text-center">Address1</th>
            <th class="text-center">Address2</th>
            <th class="text-center">City</th>
            <th class="text-center">Country</th>
            <th class="text-center">Country Cls</th>
            <th class="text-center">Epte Cls</th>
            <th class="text-center">Region Cls</th>
            <th class="text-center">Postal Code</th>
            <th class="text-center">Telephone</th>
            <th class="text-center">Fax</th>
            <th class="text-center">Closing Day</th>
            <th class="text-center">Pay Day</th>
            <th class="text-center">Affiliate Cls</th>
            <th class="text-center">NPWP No</th>
            <th class="text-center">NPWP Name</th>
            <th class="text-center">NPWP Address</th>
            <th class="text-center">NPWP City</th>
            <th class="text-center">Delivery Place</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.data.Items">
            <td class="text-center">
              <font-awesome-icon
                class="mr-2 text-success"
                icon="pencil"
                @click="edit(item)"
              />
              <font-awesome-icon
                class="ml-2 mr-2 text-danger"
                icon="trash"
                style="cursor: pointer"
                @click="remove(item)"
              />
            </td>
            <td>{{ item.Trade_Code }}</td>
            <td>{{ item.Trade_Cls_Descs }}</td>
            <td>{{ item.Trade_Name }}</td>
            <td>{{ item.Trade_Abbr }}</td>
            <td>{{ item.Contact_Person }}</td>
            <td>{{ item.Address1 }}</td>
            <td>{{ item.Address2 }}</td>
            <td>{{ item.City }}</td>
            <td>{{ item.Country }}</td>
            <td>{{ item.Country_Cls_Descs }}</td>
            <td>{{ item.Epte_Cls_Descs }}</td>
            <td>{{ item.Region_Cls_Descs }}</td>
            <td>{{ item.Postal_Code }}</td>
            <td>{{ item.Telephone }}</td>
            <td>{{ item.Fax }}</td>
            <td>{{ item.Closing_Day }}</td>
            <td>{{ item.Pay_Day }}</td>
            <td>{{ item.Affiliate_Cls_Descs }}</td>
            <td>{{ item.NPWP_No }}</td>
            <td>{{ item.NPWP_Name }}</td>
            <td>{{ item.NPWP_Address }}</td>
            <td>{{ item.NPWP_City }}</td>
            <td style="font-align: center">
              <font-awesome-icon
                icon="eye"
                class="mr-2 text-info"
                @click="
                  () => this.$router.push(`trade/detail?id=${item.Trade_Code}`)
                "
              />
            </td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>

  <v-modal
    class="modal-xl"
    ref="modalTrade"
    id="modal-form-trade"
    :title="title"
    :fullscreen="true"
    @hidden="
      () => {
        this.$refs.formTrade.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-trade
      ref="formTrade"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    title: "",
    breadcrumbs: [
      { title: "Setting", active: false, to: "" },
      { title: "Trade Master", active: true, to: "" },
    ],
    filter: {
      keyword: null,
      sorts: {
        Trade_Name: "asc",
      },
      sortItems: [
        {
          label: "Trade Name",
          value: "Trade_Name",
          selected: true,
          direction: "asc",
        },
        {
          label: "Trade Code",
          value: "Trade_Code",
          selected: false,
          direction: "asc",
        },
      ],
    },
    idSelected: "",
    modalMode: "",
    debounce: null,
  }),
  computed: {
    ds: function () {
      return useTrade();
    },
  },
  watch: {
    filter: {
      deep: true,
      handler: function (after) {
        if (this.debounce) clearTimeout(this.debounce);

        this.debounce = setTimeout(() => {
          var filter = [];

          if (after.keywordKey != "" && after)
            filter.push({ Keyword: after.keyword || "" });

          this.ds.setSort(after.sorts);
          this.ds.setFilter(filter);
          this.ds.load();
        }, 800);
      },
    },
  },
  mounted: function () {
    this.ds.setSort(this.filter.sorts);
    this.ds.setFilter([]);
    this.ds.load();
  },
  methods: {
    add: function () {
      this.title = "Add Trade Master";
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
      this.$bvModal.show("modal-form-trade");
    },
    edit: function (dt) {
      this.title = "Edit Trade Master";
      this.modalMode = "edit";
      this.idSelected = dt.Trade_Code;
      this.$bvModal.show("modal-form-trade");
    },
    remove: function (item) {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(item.Trade_Code)
              .then((_) => {
                this.ds.load();
                toastSuccess("Data Delete successfully");
                resolve();
              })
              .catch((err) => {
                toastDanger(err?.Message);
                resolve();
              });
          }),
        null,
        item.Trade_Name
      );
    },
    close: function () {
      this.$bvModal.hide("modal-form-trade");
      this.ds.load();
    },
    exportExcel: function () {
      new Promise((resolve, reject) => {
        this.ds
          .exportExcel()
          .then((_) => {
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          });
      });
    },
  },
};
</script>
