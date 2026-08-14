<template>
  <div class="text-start">
    <v-button
      class="btn-success"
      icon="save"
      label="Submit Picking"
      @click="submit"
      :disabled="isSubmittingDetail"
    />
  </div>
  <v-table
    :filter="filter"
    :ds="ds"
    :ds-data="ds.dataListScan"
    :ds-page="ds.setPageListScan"
    :ds-length="ds.setLengthListScan"
    :ds-load="ds.loadListScan"
  >
    <template #table-content>
      <table
        class="table table-striped mb-0 align-middle v-fixed-table w-100"
        v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError"
      >
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="text-center">Serial No</th>
            <th class="text-center">Picking Date</th>
            <th class="text-center">Time</th>
            <th class="text-center">Picking By</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, idx) in ds.dataListScan.Items">
            <td class="text-center">
              <div style="justify-items: center">
                <input-checkbox
                  :model-value="item.AlreadyPicking"
                  :disabled="isSubmitted"
                  @click="(e) => setPicking(e, item)"
                />
              </div>
            </td>
            <td>{{ item.SerialNo }}</td>
            <td>{{ item.PickingDate }}</td>
            <td>{{ item.PickingTime }}</td>
            <td>{{ item.PickingBy }}</td>
          </tr>
        </tbody>
      </table>
    </template>
  </v-table>
</template>

<script>
export default {
  props: ["sino", "item", "pono", "poseqno", "counter"],
  data: () => ({
    filter: {
      keyword: null,
      sorts: {
        SerialNo: "asc",
      },
      sortItems: [
        {
          label: "Serial No",
          value: "SerialNo",
          selected: true,
          direction: "asc",
        },
      ],
    },
    isSubmittingDetail: false,
    isSubmitted: false,
  }),
  computed: {
    ds: function () {
      return useShippingInstructionScan();
    },
  },
  watch: {
    sino: function () {
      this.search();
    },
    item: function () {
      this.search();
    },
    pono: function () {
      this.search();
    },
    poseqno: function () {
      this.search();
    },
    counter: function () {
      this.search();
    },
    "filter.keyword": function () {
      this.search();
    },
    "filter.sorts": function () {
      this.search();
    },
  },
  mounted: function () {
    this.search();
  },
  methods: {
    setPicking(e, item) {
      item.AlreadyPicking = e.target.checked;
      const index = this.ds.dataListScan.Items.findIndex(
        (x) => x.SerialNo === item.SerialNo,
      );

      if (index !== -1) {
        this.ds.dataListScan.Items[index].AlreadyPicking = item.AlreadyPicking;
      }
    },
    submit() {
      const selected = this.ds.dataListScan.Items.filter(
        (x) => x.AlreadyPicking,
      );

      if (selected.length === 0) {
        toastDanger("Pilih minimal satu Serial No.");
        return;
      }

      // if (details.length === 0) {
      //   toastWarning("Please select child item setting (minimal 1 data)!");
      //   return;
      // }

      this.isSubmittingDetail = true;

      const payload = {
        Header: [
          {
            ShippingInstructionNo: this.sino || "",
            ItemCode: this.item || "",
            PONumber: this.pono || "",
            PO_SeqNo: this.poseqno.toString() || "0",
          },
        ],
        Details: selected.map((x) => ({
          SerialNo: x.SerialNo,
          AlreadyPicking: x.AlreadyPicking,
        })),
      };

      this.ds
        .submit(payload)
        .then(() => {
          toastSuccess("Data saved successfully!");
          return this.search();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => {
          this.isSubmittingDetail = false;
        });
    },

    search: function () {
      this.ds.setSortListScan(this.filter.sorts);
      let filters = [
        {
          Keyword: this.filter.keyword || "",
          ShippingInstructionNo: this.sino || "",
          ItemCode: this.item || "",
          PONumber: this.pono || "",
          PO_SeqNo: this.poseqno.toString() || "0",
        },
      ];

      this.ds.setFilterListScan(filters);
      this.$nextTick(() => this.ds.loadListScan());
    },
    reset: function () {
      this.search();
    },
  },
};
</script>
