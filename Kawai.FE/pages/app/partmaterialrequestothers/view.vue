<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <font-awesome-icon icon="file-lines" style="font-size: 1.25em" />
      <span style="font-size: 1.25em" class="ml-3">
        Part Material Request Others History
      </span>
    </div>
    <!--buatkan div untuk filter area-->
    <div class="filter-area p-3" id="containerfilter">
      <table style="width: 99%">
        <tr>
          <td Style="width:10%">
            <div class="d-flex flex-fill">
              <v-button
                :action="back"
                label="Back"
                icon="arrow-left"
                cClass="ml-1 btn-danger"
                :is-loading="isLoading"
              />
            </div>
          </td>
          <td Style="width:39%"></td>
          <td Style="width:16%"></td>
        </tr>
      </table>
    </div>

    <div class="panel-body" id="panelbody">
      <div class="row">
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #8d56a9"
          >
            <span class="title-summary">Total Request </span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.total)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #888"
          >
            <span class="title-summary">Waiting Warehouse</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.waitingwarehouse)
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #18b2e0"
          >
            <span class="title-summary">Partially Supplied</span>
            <br />
            <span class="qty-summary">{{
              this.summary.partiallysupplied
            }}</span>
          </div>
        </div>
        <div class="col-xl-3 col-lg-3 col-md-6 col-sm-12 col-12">
          <div
            class="p-2 header-summary-content"
            style="background-color: #32a932"
          >
            <span class="title-summary">Completed</span>
            <br />
            <span class="qty-summary">{{
              $func.formatMoney(this.summary.completed)
            }}</span>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <div class="col-xl-12 col-lg-12 col-md-12 col-sm-12 col-12">
          <div class="panel panel-inverse">
            <div class="panel-body">
              <v-table
                :filter="filter"
                :use-paging="false"
                :ds="ds"
                :data-items="ds.data.Items"
                :top-content-height="370"
                ref="vtable"
              >
                <template #table-content>
                  <table
                    class="table table-bordered mb-0 align-middle v-fixed-table"
                    v-if="
                      !ds.isLoading && !ds.isNetworkError && !ds.isServerError
                    "
                    ref="table"
                  >
                    <thead>
                      <tr>
                        <th width="20%">Request</th>
                        <th width="15%">Line</th>
                        <th width="15%">Status</th>
                        <th width="35%">Progress</th>
                        <th width="15%" class="text-center">Action</th>
                      </tr>
                    </thead>

                    <tbody>
                      <tr v-for="item in displayedItems" :key="item.RequestNo">
                        <!-- Request -->
                        <td>
                          <div class="font-weight-bold">
                            {{ item.RequestNo }}
                          </div>

                          <small class="text-muted">
                            {{ item.RequestDate }}
                          </small>
                        </td>
                        <td>
                          <div class="mt-2 font-weight-bold">
                            {{ item.LineName }}
                          </div>

                          <small class="text-muted">
                            {{ item.TotalMaterial }} materials ·
                            {{ $func.formatMoney(item.TotalQty) }} pcs requested
                          </small>
                        </td>

                        <!-- Status -->
                        <td class="align-middle">
                          <span
                            v-if="item.Status == 'Waiting Warehouse'"
                            class="badge text-warning"
                          >
                            ● Not Sent by Warehouse
                          </span>

                          <span
                            v-else-if="item.Status == 'Partial'"
                            class="badge text-info"
                          >
                            ● Partially Supplied
                          </span>

                          <span v-else class="badge text-green">
                            ● Completed
                          </span>
                        </td>

                        <!-- Progress -->
                        <td class="align-middle">
                          <div class="small mb-2">
                            Fulfilled {{ item.FulfilledMaterial }} of
                            {{ item.TotalMaterial }} materials
                          </div>

                          <div class="progress" style="height: 8px">
                            <div
                              class="progress-bar"
                              :class="{
                                'bg-green': item.Status == 'Completed',
                                'bg-blue': item.Status == 'Partial',
                                'bg-secondary':
                                  item.Status == 'Waiting Warehouse',
                              }"
                              :style="{
                                width:
                                  (item.FulfilledMaterial /
                                    item.TotalMaterial) *
                                    100 +
                                  '%',
                              }"
                            ></div>
                          </div>
                        </td>

                        <!-- Button -->
                        <td class="text-center align-middle">
                          <v-button
                            @click="viewDetail(item.RequestNo)"
                            icon="eye"
                            label="View Detail"
                            cClass="ml-1 btn-info"
                            :is-loading="isLoading"
                          />
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </template>
              </v-table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <v-modal
    title="Request Fulfillment Detail"
    class="modal-lg"
    id="modal-list-detailrequest"
  >
    <shared-request-request-list
      :requestno="this.selectedRequestNo"
      :counter="this.counter"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    keywordKeys: [
      {
        Id: "RequestNo",
        Name: "Request No",
      },
      {
        Id: "LineCode",
        Name: "Line Code",
      },
      {
        Id: "LineName",
        Name: "Line Name",
      },
      {
        Id: "Status",
        Name: "Status",
      },
    ],
    filter: {
      keyword: null,
      sorts: {
        RequestNo: "asc",
        LineName: "asc",
      },
      sortItems: [
        {
          label: "Request No",
          value: "RequestNo",
          selected: false,
          direction: "asc",
        },
        {
          label: "Line Code",
          value: "LineCode",
          selected: true,
          direction: "asc",
        },
        {
          label: "Line Name",
          value: "LineName",
          selected: true,
          direction: "asc",
        },
      ],
    },

    isLoading: false,
    selectedRequestNo: "",

    list: [],
    counter: 0,
    summary: {
      total: 0,
      partiallysupplied: 0,
      waitingwarehouse: 0,
      item: 0,
      completed: 0,
    },

    displayedCount: 50,
  }),
  computed: {
    ds: function () {
      return usePartMaterialRequestOthers();
    },
    displayedItems() {
      if (!this.ds.data.Items) return [];
      return this.ds.data.Items.slice(0, this.displayedCount);
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
    this.search();
    this.setupScrollListener();
  },
  beforeUnmount: function () {
    if (this.scrollContainer) {
      this.scrollContainer.removeEventListener("scroll", this.onScroll);
    }
  },

  methods: {
    calculateSummary() {
      const items = this.ds.data.Items || [];

      this.summary = {
        total: items.length,
        waitingwarehouse: items.filter((x) => x.Status === "Waiting Warehouse")
          .length,
        partiallysupplied: items.filter((x) => x.Status === "Partial").length,
        completed: items.filter((x) => x.Status === "Completed").length,
      };
    },
    setupScrollListener() {
      this.$nextTick(() => {
        const tableContainer =
          this.$refs.vtable?.$el?.querySelector(".table-scroll");
        if (tableContainer) {
          this.scrollContainer = tableContainer;
          tableContainer.addEventListener("scroll", this.onScroll);
        }
      });
    },
    onScroll(e) {
      const container = e.target;
      if (
        container.scrollTop + container.clientHeight >=
        container.scrollHeight - 50
      ) {
        if (this.displayedCount < (this.ds.data.Items?.length || 0)) {
          this.displayedCount += 25;
        }
      }
    },
    async search() {
      this.ds.setSort(this.filter.sorts);
      const filters = [
        {
          Keyword: this.filter.keyword || "",
        },
      ];

      this.displayedCount = 50;

      this.ds.setFilter(filters);

      this.isLoading = true;

      try {
        await this.ds.loadhistory();

        // Hitung summary setelah data berhasil di-load
        this.calculateSummary();
      } finally {
        this.isLoading = false;
      }
    },

    reset: function () {
      this.search();
    },

    back: function () {
        this.$router.push("/app/partmaterialrequestothers");
    },

    viewDetail: function (requestno) {
     
      this.selectedRequestNo = requestno;
      this.counter++;
      this.$bvModal.show("modal-list-detailrequest");
    },
  },
};
</script>

<style scoped>
.header-summary-content {
  min-height: 5em;
  text-align: center;
}

.v-table-wrapper {
  overflow: auto;
  max-height: 500px;
  /* border: 1px solid #ddd; */
  position: relative;
}
.title-summary {
  color: white;
  font-size: 1.4em;
}

.qty-summary {
  color: white;
  font-size: 3em;
  font-weight: bold;
}

.icon-title {
  font-size: 1.5em;
  font-weight: bolder;
}

/* Bikin table bisa scroll horizontal juga */
.v-fixed-table {
  width: max-content; /* agar scroll horizontal muncul */
  min-width: 100%;
  border: 1px solid gainsboro !important;
  /* border-collapse: separate; */
  /* border-spacing: 0; */
}

.v-fixed-table th,
.v-fixed-table td {
  white-space: nowrap;
  padding: 8px 16px;
  border: 1px solid #dee2e6;
  background: #fff;
}

/* Sticky Header (atas) */
.v-fixed-table thead th {
  position: sticky;
  top: 0;
  z-index: 20; /* harus lebih tinggi dari sticky kiri */
  background: lightblue !important;
}

.center-filter {
  display: flex;
  justify-content: center;
  align-items: center;
}
.request-table tbody tr td {
  vertical-align: middle;
  padding: 15px;
}

.request-table tbody tr {
  height: 90px;
}

.badge {
  padding: 8px 14px;
  border-radius: 20px;
  font-size: 12px;
}

.progress {
  border-radius: 20px;
}

.progress-bar {
  border-radius: 20px;
}
</style>
