<template>
  <table>
    <tr>
      <td><label class="form-label">ID SEQ</label></td>
      <td style="padding-left: 15px">
        <input-text
          v-model="model.IDSeq"
          disabled="true"
          style="width: 80px"
          maxlength="25"
        />
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Child Item </label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <table>
          <tr>
            <td>
              <input-text
                v-model="model.ChilItemCode"
                disabled="true"
                style="width: 100px"
                maxlength="25"
              />
            </td>
            <td>
              <input-text
                v-model="model.ChilItemName"
                disabled="true"
                style="width: 250px; margin-left: 5px"
                maxlength="25"
              />
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr>
      <td style="padding-top: 5px">
        <label class="form-label">Child Item Requirement Qty </label>
      </td>
      <td style="padding-top: 5px; padding-left: 15px">
        <input-money v-model="model.ReqQty" style="width: 100px" />
      </td>
    </tr>
  </table>
  <div style="float: right" class="mt-4 mb-3">
    <v-button-submit
      :submit="submit"
      :disabled="btnDisabled !== undefined && btnDisabled !== false"
      :is-loading="isLoading"
    />
  </div>
</template>
<script>
export default {
  props: ["id", "btnDisabled", "mode", "item", "counter"],
  data: () => ({
    isLoading: false,
    model: {
      IDSeq: 0,
      ChilItemCode: "",
      ChilItemName: "",
      ReqQty: 0,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useSupplyRequestBomDetail();
    },
  },
  mounted: function () {
    this.loadDetail(this.item);
  },
  watch: {
    counter: function () {
      this.loadDetail(this.item);
    },
  },
  methods: {
    loadDetail: function () {
      this.ds.loadDetail(this.item).then((dt) => (this.model = dt.Data));
    },
    resetForm: function () {
      // Kosongkan form untuk mode Add
      this.model = {
        IDSeq: 0,
        ChilItemCode: "",
        ChilItemName: "",
        ReqQty: 0,
      };
      this.errors = {}; // Reset errors
    },
    submit: function () {
      this.update();
    },

    update: function () {
      this.isLoading = true;
      this.ds
        .update(this.model)
        .then((datas) => {
          toastSuccess("Data saved successfully");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
  },
};
</script>
