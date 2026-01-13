<template>
  <div>
    <div class="row">
      <div class="col-sm-6 pe-5">
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
      <div class="col-sm-6">
        <h6>Import History</h6>
        <table-log :template="template" />
      </div>
    </div>

    <div style="max-width: 100%; overflow: auto">
      <div class="mt-5 mb-5" v-if="this.importResult && JSON.stringify(this.importResult) != '{}'">
        <h5>Import Data Previews</h5>
        <div class="mt-1">
          Upload Status:
          <span class="badge text-bg-success" v-if="!this.isInvalid">
            SUCCESS
          </span>
          <span class="badge text-bg-danger" v-else> FAILED </span>
        </div>
        <div class="mt-1">
          Message:
          <span
            class="text-danger"
            v-if="importResult?.Status?.toUpperCase() != 'SUCCESS'"
          >
            <b>{{ importResult?.Message }}</b>
          </span>
          <span v-else>{{ importResult?.Message }}</span>
        </div>
        <div class="mt-1">
          Total Rows:
          <span class="fw-bold text-primary">
            {{ importResult?.Data?.length }}
          </span>
          | Valid:
          <span class="fw-bold text-success">
            {{ importResult?.Data?.filter((x) => x.Errors.length == 0).length }}
          </span>
          | Invalid:
          <span class="fw-bold text-danger">
            {{ importResult?.Data?.filter((x) => x.Errors.length > 0).length }}
          </span>
        </div>
        <table class="x-table mt-3 w-100">
          <thead>
            <tr>
              <th>#</th>
              <th
                class="text-center"
                v-for="(header, index) in headers.filter(
                  (x) => x != 'RowNumber'
                ) || []"
                :key="index"
              >
                {{ header }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in importResult?.Data" :key="index">
              <td>{{ index + 1 }}.</td>
              <td
                v-for="(header, idx) in headers.filter((x) => x != 'RowNumber')"
                :key="idx"
                :style="
                  item.Errors ? 'background: pink;border:solid 1px red;' : ''
                "
                :title="item.Errors ? item.Errors : ''"
              >
                <span>
                  {{ item[header] }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import TableLog from "./common/import-log-table.x.vue";
export default {
  props: ["refresh", "template", "query", "title", "urlSubmit"],
  components: { TableLog },
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

      var data = new FormData();
      data.append("File", this.model.File);
      data.append("Action", this.model.Action);
      data.append("TemplateName", this.template);

      this.$http
        .post(this.urlSubmit, data)
        .then(({ data }) => {
          console.log(data);
          this.isInvalid =
            data.Data?.InvalidRows > 0 || data.Status == "Invalid"; // == "Invalid";
          if (this.isInvalid) {
            this.importResult = data || {};
            this.errors = data.Data?.Errors ?? {};
            this.invalidMessage = data.Data?.Message;
          } else {
            this.isSuccess = true;
            this.isLoadingUpload = false;
            this.importResult = data;
            this.headers = Object.keys(data.Data[0]);
            let message = this.model.Action == "TEST" ? "Test Import successfull" : "Import data successful!";
            toastSuccess(message);
          }

          if (this["refresh"]) this["refresh"]();

          this.ds.load();
        })
        .catch((err) => {
          if (err.response) {
            this.importResult = err.response.data || {};
            this.headers = Object.keys(err.response.data.Data[0]);
            this.errors = err.response.data?.Errors ?? [];
            this.isInvalid = err.response.data?.Status?.toUpperCase() == "INVALID";
            this.invalidMessage = err.response.data?.Message;
          }
          // this.loadLogs();
          this.ds.load();
        })
        .finally((_) => {
          this.model.File = null;
          if (this.$refs.fileInput) {
            this.$refs.fileInput.value = null;
          }
          this.isLoadingUpload = false;
        });
    },
    downloadTemplate: function () {
      const link = document.createElement("a");
      link.href = `${axios.defaults.baseURL}/import/template?name=${this.template}`;
      link.target = "_blank";
      link.setAttribute("download", `Template ${this.template}.xlsx`);
      link.click();
    },
    change: function (e) {
      this.invalidMessage = null;
      this.importResult = null;
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
