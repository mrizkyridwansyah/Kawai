<template>
  <ul class="nav nav-pills mb-3">
    <li class="nav-item" v-for="(item, i) in list">
      <a
        class="nav-link"
        :class="{ active: activeTab == item.tab }"
        href="#"
        @click="() => (this.activeTab = item.tab)"
      >
        {{ item.title }}
      </a>
    </li>
  </ul>
  <div class="form-content">
    <div v-if="this.activeTab == 1">
      <div class="mb-3">
        <label class="form-label">Item Code</label>
        <input-text
          placeholder="Item Code"
          v-model="model.ItemCode"
          :disabled="mode === 'edit'"
          :errors="errors?.ItemCode"
        />
      </div>
      <div class="mb-3">
        <label class="form-label">Item Name</label>
        <input-text
          placeholder="Item Name"
          v-model="model.ItemName"
          :errors="errors?.ItemName"
        />
      </div>
      <div class="row mt-4">
        <div class="col-sm-12 col-md-4 col-lg-4 col-xl-4">
          <div class="fieldset">
            <div class="fieldset-title">Item Info</div>
            <div class="mb-3">
              <label class="form-label">Finish Good Part</label>
              <input-cls
                type-data="ItemFinishGoodCls"
                placeholder="Finish Good Part"
                v-model="model.FinishGoodPartCls"
                :errors="errors?.FinishGoodPartCls"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Part Number</label>
              <input-text
                placeholder="Part Number"
                v-model="model.PartNumber"
                :errors="errors?.PartNumber"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Drawing Code</label>
              <input-text
                placeholder="Drawing Code"
                v-model="model.DrawingCode"
                :errors="errors?.DrawingCode"
              />
            </div>
          </div>
        </div>
        <div class="col-sm-12 col-md-4 col-lg-4 col-xl-4">
          <div class="fieldset">
            <div class="fieldset-title">Order & Delivery</div>
            <div class="mb-3">
              <label class="form-label">Warehouse</label>
              <input-warehouse-item
                placeholder="Warehouse"
                v-model="model.WarehouseCode"
                :errors="errors?.WarehouseCode"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Address</label>
              <input-text
                placeholder="Address"
                v-model="model.Address"
                :errors="errors?.Address"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Supplier</label>
              <input-trade
                placeholder="Supplier"
                v-model="model.SupplierCode"
                :errors="errors?.SupplierCode"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Delivery</label>
              <input-delivery-place
                placeholder="Item Name"
                v-model="model.DeliveryPlaceCode"
                :errors="errors?.DeliveryPlaceCode"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">HS Code</label>
              <input-hs
                placeholder="HS"
                v-model="model.HSCode"
                :errors="errors?.HSCode"
              />
            </div>
          </div>
        </div>
        <div class="col-sm-12 col-md-4 col-lg-4 col-xl-4">
          <div class="fieldset">
            <div class="fieldset-title">Item Classification</div>
            <div class="mb-3">
              <label class="form-label">Part Cls</label>
              <input-cls
                type-data="ItemPartCls"
                placeholder="Part Cls"
                v-model="model.PartCls"
                :errors="errors?.PartCls"
              />
            </div>
            <div class="mb-3">
              <label class="form-label"></label>
              <input-checkbox
                label="Reserve Cls"
                v-model="model.ReserveCls"
                :errors="errors?.ReserveCls"
              />
            </div>
            <div class="mb-3">
              <label class="form-label"></label>
              <input-checkbox
                label="Supply Cls"
                v-model="model.SupplyCls"
                :errors="errors?.SupplyCls"
              />
            </div>
            <div class="mb-3">
              <label class="form-label"></label>
              <input-checkbox
                label="Provision Cls"
                v-model="model.ProvisionCls"
                :errors="errors?.ProvisionCls"
              />
            </div>
            <div class="mb-3">
              <label class="form-label"></label>
              <input-checkbox
                label="Production Cls"
                v-model="model.ProductionCls"
                :errors="errors?.ProductionCls"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-else-if="this.activeTab == 2">
      <div class="row mt-4">
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-3">
            <label class="form-label">Qty / Case (Finish Goods)</label>
            <input-money
              placeholder="Qty / Case (Finish Goods)"
              class="text-right"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Packing Style</label>
            <input-text
              placeholder="Packing Style"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Group Cls</label>
            <input-text
              placeholder="Group Cls"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Standard Stock</label>
            <input-text
              placeholder="Standard Stock"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Safety Stock</label>
            <input-text
              placeholder="Safety Stock"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Safety Stock (%)</label>
            <input-text
              placeholder="Safety Stock (%)"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Max Stock</label>
            <input-text
              placeholder="Max Stock"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Min Stock</label>
            <input-text
              placeholder="Min Stock"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-3">
            <label class="form-label">Qty/Box (Parts/Material)</label>
            <input-text
              placeholder="Qty/Box (Parts/Material)"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Accounting</label>
            <input-text
              placeholder="Accounting"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Explosion Cls</label>
            <input-text
              placeholder="Explosion Cls"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Purchase Person</label>
            <input-text
              placeholder="Purchase Person"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Stock Control</label>
            <input-text
              placeholder="Stock Control"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Allowance Day</label>
            <input-text
              placeholder="Allowance Day"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Use End Date</label>
            <input-text
              placeholder="Use End Date"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Delivery Leadtime</label>
            <input-text
              placeholder="Delivery Leadtime"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-3">
            <label class="form-label">Make Or Buy Cls</label>
            <input-cls
              placeholder="Make Or Buy Cls"
              type-data="ItemMakeOrBuyCls"
              v-model="model.MakeBuyCls"
              :errors="errors?.MakeBuyCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Control</label>
            <input-cls
              type-data="Control_Cls"
              placeholder="Control"
              v-model="model.ControlCls"
              :errors="errors?.ControlCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Unit Cls</label>
            <input-cls
              type-data="Unit_Cls"
              placeholder="Unit Cls"
              v-model="model.UnitCls"
              :errors="errors?.UnitCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Order Point Qty</label>
            <input-money
              placeholder="Order Point Qty"
              v-model="model.OrderPointQty"
              :errors="errors?.OrderPointQty"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Min. Order Qty</label>
            <input-money
              placeholder="Min. Order Qty"
              v-model="model.MinOrder"
              :errors="errors?.MinOrder"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Packing Style Part/Material</label>
            <input-text
              placeholder="Packing Style Part/Material"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Type Accs</label>
            <input-text
              placeholder="Type Accs"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Model</label>
            <input-text
              placeholder="Model"
              v-model="model.QtyFinishGood"
              :errors="errors?.QtyFinishGood"
            />
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-3">
            <label class="form-label">PO Type</label>
            <input-cls
              placeholder="PO Type"
              type-data="POType_Cls"
              v-model="model.POTypeCls"
              :errors="errors?.POTypeCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Destination Cls</label>
            <input-cls
              placeholder="Destination Cls"
              type-data="Destination_Cls"
              v-model="model.DestinationCls"
              :errors="errors?.DestinationCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Color Cls</label>
            <input-cls
              placeholder="Color Cls"
              type-data="Color_Cls"
              v-model="model.ColorCls"
              :errors="errors?.ColorCls"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
  <div class="submit-wrapper">
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
    activeTab: 1,
    list: [
      { title: "General", tab: 1 },
      { title: "Stock", tab: 2 },
      { title: "Manufacture", tab: 3 },
    ],
    isLoading: false,
    model: {
      ItemCode: "",
      ItemName: "",
      ItemGroup: "",
      ItemCls: "",
      ItemTypeCode: "",
      BrandCode: "",
      UnitClassificationCode: "",
      QtyPacking: 0,
      StockControlCls: false,
    },
    errorResponse: {},
    errors: {},
  }),
  computed: {
    ds: function () {
      return useItem();
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
        ItemCode: "",
        ItemName: "",
        ItemGroup: "",
        ItemCls: "",
        ItemTypeCode: "",
        BrandCode: "",
        UnitClassificationCode: "",
        QtyPacking: 0,
        StockControlCls: false,
      };
      this.errors = {}; // Reset errors
      this.activeTab = 1;
    },
    submit: function () {
      if (this.mode === "add") this.create();
      else this.update();
    },
    create: function () {
      this.ds
        .create(this.model)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
          this.$emit("submitted");
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
    update: function () {
      this.ds
        .update(this.model)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
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
.fieldset {
  position: relative;
  border: 2px solid grey;
  border-radius: 0.5em;
  padding: 2em 1em 1em;
}

.fieldset-title {
  position: absolute;
  top: -0.8em;
  left: 1em;
  background: white;
  padding: 0 0.5em;
  font-weight: bold;
}
</style>
