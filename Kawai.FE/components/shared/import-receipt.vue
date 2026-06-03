<template>
  <div>
    <div class="row">
      <div class="col-sm-12 pe-5">
        <div>
          <div>
            <div class="alert alert-danger" role="alert" v-if="isInvalid">
              <div class="d-flex">
                <div class="ms-3">
                  <h6 class="text-gray-900 fw-bolder">Upload Failed!</h6>
                  <div class="fs-7 text-gray-700">{{ invalidMessage }}</div>
                </div>
              </div>
            </div>
            <b-alert v-if="isSuccess" variant="success" show>
              <strong>Import data successful.</strong>
            </b-alert>
            <b-alert v-if="isNotFound" variant="warning" show>
              <strong>Template Not Found.</strong>
            </b-alert>
            <b-alert v-else-if="isError" variant="danger" show>
              <strong>Something went wrong.</strong>
            </b-alert>
          </div>
          <table class="tb-form">
            <tbody>
               <tr>
                <td class="form-label text-left" style="vertical-align: middle">
                  Factory
                </td>
                <td class="form-colon" style="vertical-align: middle">:</td>
                <td>
               <filter-factory-privileges
                    class="form-control"
                    v-model="filter.FactoryCode"
                    disabled="true"
                    style-code="width: 110px"
                    style-desc="width: 250px"
                  />
                </td>
              </tr>

              <tr>
                <td class="form-label text-left" style="vertical-align: middle">
                  Action
                </td>
                <td class="form-colon" style="vertical-align: middle">:</td>
                <td>
                  <input-radio
                    :options="[
                      { value: 'TEST', text: 'Testing' },
                      { value: 'EXECUTE', text: 'Execute' },
                    ]"
                    textField="text"
                    valueField="value"
                    v-model="model.Action"
                    :errors="errors?.Action"
                  />
                </td>
              </tr>
              <tr>
                <td class="form-label text-left" style="vertical-align: middle">
                  File
                </td>
                <td class="form-colon" style="vertical-align: middle">:</td>
                <td>
                  <input
                    type="file"
                    class="form-control"
                    style="width: 250px"
                    ref="fileInput"
                    @change="change"
                    :errors="errors.File"
                  />
                </td>
              </tr>
              <tr>
                <td colspan="2"></td>
                <td>
                  <div class="invalid-feedback d-block" v-if="errors.File">
                    {{ errors.File[0] }}
                  </div>
                  <div>File extension allowed: *.xls. *.xslx</div>

                  <div class="mt-2">
                    <a href="javascript:void(0);" @click="downloadTemplate"
                      ><b>Download Template</b></a
                    >
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="mt-4">
          <v-submit-button-group
            :submit="submit"
            :disabled="isLoadingUpload"
            label="Import"
          />
        </div>
      </div>
    </div>

 <div style="max-width:100%;overflow:auto">
  <div
    class="mt-5 mb-5"
    v-if="importResult && JSON.stringify(importResult) != '{}'"
  >
    <h5>Import Data Preview</h5>

    <!-- STATUS -->
    <div class="mt-1">
      Upload Status :
      <span class="badge text-bg-success" v-if="!isInvalid">
        SUCCESS
      </span>

      <span class="badge text-bg-danger" v-else>
        FAILED
      </span>
    </div>

    <!-- MESSAGE -->
    <div class="mt-1">
      Message :

      <span
        class="text-danger"
        v-if="importResult?.Status?.toUpperCase() != 'SUCCESS'"
      >
        <b>{{ importResult?.Message }}</b>
      </span>

      <span v-else>
        {{ importResult?.Message }}
      </span>
    </div>

    <!-- SUMMARY -->
    <div class="mt-1">
      Total Rows :

      <span class="fw-bold text-primary">
        {{ importResult?.Data?.Details?.length || 0 }}
      </span>

      |

      Valid :

      <span class="fw-bold text-success">
        {{
          importResult?.Data?.Details?.filter(
            x => !x.Errors || x.Errors.length == 0
          ).length || 0
        }}
      </span>

      |

      Invalid :

      <span class="fw-bold text-danger">
        {{
          importResult?.Data?.Details?.filter(
            x => x.Errors && x.Errors.length > 0
          ).length || 0
        }}
      </span>
    </div>

    <!-- HEADER -->

    <div class="card mt-4">
      <div class="card-header">
        <b>Receipt Header</b>
      </div>

      <div class="card-body">

        <div
          v-if="importResult?.Data?.Header?.Errors"
          class="alert alert-danger"
        >
          {{ importResult?.Data?.Header?.Errors }}
        </div>

        <div class="row">

          <div class="col-md-4 mb-3">
            <label class="fw-bold">Supplier Code</label>

            <input
              type="text"
              class="form-control"
              readonly
              :class="{
                'is-invalid':
                importResult?.Data?.Header?.Errors?.includes('Supplier')
              }"
              :value="importResult?.Data?.Header?.SupplierCode"
            >
          </div>

          <div class="col-md-4 mb-3">
            <label class="fw-bold">DN Number</label>

            <input
              type="text"
              class="form-control"
              readonly
              :value="importResult?.Data?.Header?.DNNumber"
            >
          </div>

          <div class="col-md-4 mb-3">
            <label class="fw-bold">Receipt Date</label>

            <input
              type="text"
              class="form-control"
              readonly
              :class="{
                'is-invalid':
                importResult?.Data?.Header?.Errors?.includes('Receipt Date')
              }"
              :value="importResult?.Data?.Header?.ReceiptDate?.substring(0, 10)"
            >
          </div>

          <div class="col-md-4 mb-3">
            <label class="fw-bold">BC Type</label>

            <input
              type="text"
              class="form-control"
              readonly
              :value="importResult?.Data?.Header?.BCType"
            >
          </div>

          <div class="col-md-4 mb-3">
            <label class="fw-bold">BC Number</label>

            <input
              type="text"
              class="form-control"
              readonly
              :value="importResult?.Data?.Header?.BCNumber"
            >
          </div>

          <div class="col-md-4 mb-3">
            <label class="fw-bold">BC Date</label>

            <input
              type="text"
              class="form-control"
              readonly
              :class="{
                'is-invalid':
                importResult?.Data?.Header?.Errors?.includes('BC Date')
              }"
              :value="importResult?.Data?.Header?.BCDate?.substring(0, 10)"
              
            >
          </div>

        </div>

      </div>
    </div>

    <!-- DETAIL -->

    <div class="mt-4">
      <h6>Receipt Detail</h6>

      <table class="x-table w-100">
        <thead>
          <tr>
            <th width="50">#</th>
            <th>PO Number</th>
            <th>Item Code</th>
            <th>Receipt Qty</th>
            <th width="700">Error</th>
          </tr>
        </thead>

        <tbody>

          <tr
            v-for="(item,index) in importResult?.Data?.Details"
            :key="index"
          >
            <td>{{ index + 1 }}</td>

            <td
              :style="
                item.Errors
                ? 'background:pink;border:1px solid red'
                : ''
              "
            >
              {{ item.PONumber }}
            </td>

            <td
              :style="
                item.Errors
                ? 'background:pink;border:1px solid red'
                : ''
              "
            >
              {{ item.ItemCode }}
            </td>

            <td
              :style="
                item.Errors
                ? 'background:pink;border:1px solid red'
                : ''
              "
            >
              {{ item.ReceiptQty }}
            </td>

            <td class="text-danger">
              {{ item.Errors }}
            </td>
          </tr>

          <tr
            v-if="
              !importResult?.Data?.Details ||
              
              importResult?.Data?.Details.length == 0
            "
          >
            <td colspan="5" class="text-center">
              No Data
            </td>
          </tr>

        </tbody>
      </table>
    </div>

  </div>
</div>
  </div>
</template>

<script>
export default {
  props: ["refresh", "template", "query", "title", "urlSubmit"],
 
  data: () => ({
    isShowModal: false,
    isLoading: false,
    isLoadingLogs: false,
    isLoadingUpload: false,
    isSuccess: false,
    isError: false,
    isNotFound: false,
    isInvalid: false,
    invalidMessage: "",
    model: {
      TemplateName: null,
      File: null,
      Action: "TEST",
    },
    filter:{
      FactoryCode:""
    },
    errorResponse: {},
    errors: {},
    dataList: [],
    headers: [],
    importResult: {},
    logFilter: {
      Page: 1,
      Length: 5,
    },
    logList: {
      Items: [],
      Total: 0,
      Filtered: 0,
      Page: 1,
      Length: 10,
    },
  }),
  computed: {
    ds: function () {
      return useImportLog();
    },
  },
  mounted: function () {
    this.ds.setFilter([
      {
        TemplateName: this.template,
      },
    ]);
  },
  methods: {
   submit: function (e) {
  e.preventDefault();

  this.isLoadingUpload = true;
  this.invalidMessage = "";
  this.isInvalid = false;
  this.isError = false;
  this.isSuccess = false;

  this.model.TemplateName = this.template;

  let formData = new FormData();

  formData.append("File", this.model.File);
  formData.append("Action", this.model.Action);
  formData.append("TemplateName", this.template);
  formData.append("FactoryCode", this.filter.FactoryCode);

  this.$http
    .post(this.urlSubmit, formData)
    .then(({ data }) => {
      console.log(data);

      this.importResult = data || {};

      // Receipt Import Structure
      const details = data?.Data?.Details || [];

      this.headers =
        details.length > 0
          ? Object.keys(details[0]).filter(
              x => x !== "RowNumber"
            )
          : [];

      const hasHeaderError =
        !!data?.Data?.Header?.Errors;

      const hasDetailError =
        details.some(x => x.Errors);

      this.isInvalid =
        hasHeaderError ||
        hasDetailError ||
        data.Status?.toUpperCase() === "INVALID";

      if (this.isInvalid) {
        this.errors = data.Data?.Errors ?? {};
        this.invalidMessage = data.Message ?? "";
      }
      else {
        this.isSuccess = true;

        let message =
          this.model.Action === "TEST"
            ? "Test Import successful"
            : "Import data successful!";

        toastSuccess(message);
      }

      if (this.refresh)
        this.refresh();

      this.ds.load();
    })
    .catch((err) => {

      if (err.response) {

        this.importResult =
          err.response.data || {};

        const details =
          err.response.data?.Data?.Details || [];

        this.headers =
          details.length > 0
            ? Object.keys(details[0]).filter(
                x => x !== "RowNumber"
              )
            : [];

        this.errors =
          err.response.data?.Errors ?? [];

        this.invalidMessage =
          err.response.data?.Message ?? "";

        this.isInvalid = true;
      }

      this.ds.load();
    })
    .finally(() => {

      this.model.File = null;

      if (this.$refs.fileInput) {
        this.$refs.fileInput.value = null;
      }

      this.isLoadingUpload = false;
    });
},
    downloadTemplate: function () {
      const link = document.createElement("a");
      link.href = `/file/${this.template}.xlsx`;
      link.target = "_blank";
      link.setAttribute("download", `Template ${this.template}.xlsx`);
      link.click();
    },
    change: function (e) {
      this.invalidMessage = null;
       this.importResult = {};
      // this.invalidMessage = '';
      this.isInvalid = false;
      this.isError = false;
      this.isSuccess = false;
      if (e.target.files.length == 0) return;

      // if(!isNaN(this.maxLength)) {
      //   if(e.target.files[0].size > this.maxLength)
      //     this.invalidMessage = `The file is too large. Allowed maximum size is ${this.$func.bytesToSize(this.maxLength)}`;
      // }
      this.model.File = e.target.files[0];
      // this.$emit("update:modelValue", e.target.files[0]);

      // if(this['onSelect'])
      //   this['onSelect'](e.target.files[0]);
    },
  },
};
</script>
