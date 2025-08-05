<template>
  <div class="row">
    <div class="col-lg-6">
      <div class="mb-3">
        <label class="form-label">User ID</label>
        <input-text
          placeholder="User ID"
          v-model="model.UserID"
          :disabled="mode === 'edit'"
          :errors="errors?.UserID"
        />
      </div>
      <div class="mb-3">
        <label class="form-label">Full Name</label>
        <input-text
          placeholder="Full Name"
          v-model="model.FullName"
          :errors="errors?.FullName"
        />
      </div>
      <div class="mb-3">
        <label class="form-label">Password</label>
        <input-password
          v-if="!showpassword"
          placeholder="Password"
          v-model="model.Password"
          :errors="errors?.Password"
        />
        <input-text
          v-else
          placeholder="Password"
          v-model="model.Password"
          :errors="errors?.Password"
        />
        <input-checkbox
          class="mt-2"
          label="show password"
          v-model="showpassword"
        />
      </div>

      <div class="mb-3">
        <label class="form-label">Status Admin</label>
        <input-dropdown
          :options="[
            { value: true, text: 'Yes' },
            { value: false, text: 'No' },
          ]"
          textField="text"
          valueField="value"
          placeholder="Status Admin"
          v-model="model.IsAdmin"
          :errors="errors?.IsAdmin"
        />
      </div>
      <div class="mb-3">
        <label class="form-label">Job Position</label>
        <input-jobposition
          class="form-control"
          v-model="model.JobPositionCode"
          :errors="errors?.JobPositionCode"
        />
      </div>
      <div class="mb-3">
        <label class="form-label">User Group</label>
        <input-usergroup class="form-control" v-model="model.UserGroupID" />
      </div>
    </div>
    <div class="col-lg-6">
      <!-- <input-pdf v-model="model.Attachment" /> -->
      <input-image
        v-model="model.ImageAttachment"
        :initial-image="model.ImageBase64"
      />
      <!-- <input-image-multiple
        v-model="model.ImageAttachment"
      /> -->
    </div>
  </div>
  <div class="mt-4 mb-3">
    <v-button-submit
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id", "btnDisabled", "mode"],
  data: () => ({
    isLoading: false,
    showpassword: false,
    model: {
      UserID: "",
      FullName: "",
      Password: "",
      IsAdmin: "",
      UserGroupID: "",
      JobPositionCode: "",
      ImageAttachment: null,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useUser();
    },
  },
  mounted: function () {
    if (this.mode === "edit" && this.id) {
      this.loadDetail(this.id);
    } else if (this.mode === "add") {
      this.resetForm();
    }
  },
  watch: {
    mode: function (val) {
      if (val === "edit") {
        this.loadDetail(this.id);
      } else {
        this.resetForm();
      }
    },
  },
  methods: {
    loadDetail: function () {
      this.ds.loadDetail(this.id).then((dt) => (this.model = dt.Data));
    },
    resetForm: function () {
      // Kosongkan form untuk mode Add
      this.model = {
        UserID: "",
        FullName: "",
        Password: "",
        IsAdmin: "",
        UserGroupID: "",
        JobPositionCode: "",
        ImageAttachment: null,
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      let data = new FormData();
      data.append("UserID", this.model.UserID);
      data.append("FullName", this.model.FullName);
      data.append("Password", this.model.Password);
      data.append("JobPositionCode", this.model.JobPositionCode);
      data.append("UserGroupID", this.model.UserGroupID);
      data.append("IsAdmin", this.model.IsAdmin);
      data.append("ImageName", this.model.ImageName);

      if (this.model.ImageAttachment) {
        data.append("ImageAttachment", this.model.ImageAttachment);
      }

      if (this.mode === "add") this.create(data);
      else this.update(data);
    },
    create: function (data) {
      this.ds
        .create(data)
        .then((datas) => {
          toastSuccess("success");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
    update: function (data) {
      this.ds
        .update(data)
        .then((datas) => {
          toastSuccess("Anying........ nge-sep!");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
  },
};
</script>

<style scoped>
.image-input-wrapper {
  width: 100%;
  height: 100%;
  max-height: 400px;
}
</style>
