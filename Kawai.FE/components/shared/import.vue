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
                <td class="form-label text-left" style="vertical-align: middle;">Action</td>
                <td class="form-colon" style="vertical-align: middle;">:</td>
                <td>
                  <input-file @input="change" :errors="errors.File" />
                </td>
              </tr>
              <tr>
                <td class="form-label text-left" style="vertical-align: middle;">File</td>
                <td class="form-colon" style="vertical-align: middle;">:</td>
                <td>
                  <input-file @input="change" :errors="errors.File" />
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
            :disabled="isLoading || isLoadingUpload"
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
      <div class="mt-5 mb-5" v-if="importResult?.Data?.DataList">
        <h5>Import Data Previews</h5>
        <div>
          Upload Status:
          <span class="badge text-bg-success" v-if="!this.isInvalid">
            SUCCESS
          </span>
          <span class="badge text-bg-danger" v-else> FAILED </span>
        </div>
        <div>
          Message:
          <span
            class="text-danger"
            v-if="importResult.Status.toUpperCase() != 'SUCCESS'"
          >
            {{ importResult.Message }}
          </span>
          <span v-else>{{ importResult.Message }}</span>
        </div>
        <div>
          Total Rows:
          <span class="fw-bold text-primary">
            {{ importResult.Data.Rows }}
          </span>
          | Valid:
          <span class="fw-bold text-success">
            {{ importResult.Data.ValidRows }}
          </span>
          | Invalid:
          <span class="fw-bold text-danger">
            {{ importResult.Data.InvalidRows }}
          </span>
        </div>
        <table class="x-table mt-3">
          <thead>
            <tr>
              <th>#</th>
              <th v-for="(item, index) in dataList" :key="index">
                {{ item.Name }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(item, index) in importResult?.Data?.DataList"
              :key="index"
            >
              <td>{{ index + 1 }}.</td>
              <td
                v-for="(header, idx) in dataList"
                :key="idx"
                :style="
                  JSON.stringify(importResult?.Data?.Errors) == '{}'
                    ? ''
                    : importResult?.Data?.Errors?.[`[${index}]`][
                        header.PropertyName
                      ]
                    ? 'background: pink;border:solid 1px red;'
                    : ''
                "
                :title="
                  JSON.stringify(importResult?.Data?.Errors) == '{}'
                    ? ''
                    : importResult?.Data?.Errors?.[`[${index}]`][
                        header.PropertyName
                      ]
                "
              >
                <span>
                  {{ item[header.PropertyName] }}
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
import bootstrap from "bootstrap/dist/js/bootstrap.bundle";
export default {
  props: ["refresh", "template", "query", "title"],
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
      Action: null,
    },
    errorResponse: {},
    errors: {},
    dataList: [],
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
  watch: {
    template: function (after) {
      this.loadSchema();
    },
    // logFilter: {
    //   deep: true,
    //   handler: function(after) {
    //     this.loadLogs();
    //     this.ds.load
    //   }
    // }
  },
  computed: {
    workspaceId: function () {
      return this.$route.params?.id;
    },
    ds: function () {
      return useImportLog();
    },
  },
  mounted: function () {
    // this.ds.setTemplate(this.template);
    // this.ds.load();
    this.loadSchema();
    const tooltipTriggerList = document.querySelectorAll(
      '[data-bs-toggle="tooltip"]'
    );
    const tooltipList = [...tooltipTriggerList].map(
      (tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl)
    );
  },
  methods: {
    loadSchema: function () {
      this.isLoading = true;
      this.$http
        .get(`/import/schema?name=${this.template}`)
        .then(({ data }) => {
          this.dataList = data.Data;
        })
        .catch((err) => {
          if (err.response) {
            this.isNotFound = err.response.status == 404;
          } else {
            this.isError = true;
          }
        })
        .finally(() => (this.isLoading = false));
    },
    // loadLogs: function() {
    //   if(!this.isShowModal)
    //     return;

    //   this.isLoadingLogs = true;
    //   this.$http.post(`/import/histories?workspace=${this.workspaceId}&templateName=${this.template}`, this.logFilter)
    //     .then(({data}) => {
    //       this.logList = data.Data;
    //       console.log('loadlogs',this.logFilter, this.logList)
    //     })
    //     .finally(() => this.isLoadingLogs = false)
    // },
    submit: function (e) {
      e.preventDefault();
      this.isLoadingUpload = true;
      this.invalidMessage = "";
      this.isInvalid = false;
      this.isError = false;
      this.isSuccess = false;

      this.model.TemplateName = this.template;

      var data = new FormData();
      data.append("file", this.model.File);

      if (this.model.Description)
        data.append("description", this.model.Description);

      var queryStrings = "";

      if (this.query)
        queryStrings = Object.keys(this.query)
          .map((r) => `&${r}=${this.query[r]}`)
          .join("");

      this.$http
        .post(`/import/submit?name=${this.template}${queryStrings}`, data)
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
            this.model.File = null;
            this.importResult = data;
            toastSuccess("Import data successful!");
          }

          if (this["refresh"]) this["refresh"]();

          this.ds.load();
        })
        .catch((err) => {
          if (err.response) {
            this.importResult = err.response.data.Data || {};
            this.errors = err.response.data?.Errors ?? {};
            this.isInvalid = err.response.data?.Status == "INVALID";
            this.invalidMessage = err.response.data?.Message;
            this.model.File = null;
          }
          // this.loadLogs();
          this.ds.load();
        })
        .finally((_) => {
          this.model.File = null;
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
    downloadFile: function (id) {
      this.$http.get(`/import/download-key?id=${id}`).then(({ data }) => {
        this.$http.open(`/import-log/download?id=${id}&key=${data.Data}`);
      });
    },
    // setPage: function(i) {
    //   this.logFilter.Page = i;
    //   console.log(this.logFilter, this.logList)
    // },
    // setLength: function(i) {
    //   this.logFilter.Length = i;
    //   console.log(this.logFilter, this.logList)
    // },
    onModalShown: function () {
      this.isShowModal = true;
      this.model.File = null;
      this.model.Description = null;
      this.loadSchema();
      this.ds.setTemplate(this.template);
      this.ds.load();
    },
    onModalHidden: function () {
      this.isShowModal = false;
      this.model.File = null;
      this.model.Description = null;
      this.errors = this.errorResponse = {};
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
