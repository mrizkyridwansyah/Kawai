<template>
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
  <ul class="nav nav-pills mt-4 mb-3">
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
  <div class="form-content" style="padding-bottom: 5em">
    <div v-if="this.activeTab == 1">
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
                v-model="model.MakerItemCode"
                :errors="errors?.MakerItemCode"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Drawing Code</label>
              <input-text
                placeholder="Drawing Code"
                v-model="model.DrawingNumber"
                :errors="errors?.DrawingNumber"
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
                :trade-cls="['2', '3']"
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
            <div class="mb-3">
              <label class="form-label"></label>
              <input-checkbox
                label="Stock Control Cls"
                v-model="model.StockControlCls"
                :errors="errors?.StockControlCls"
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
              v-model="model.NumberEntering"
              :errors="errors?.NumberEntering"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Packing Style</label>
            <input-cls
              type-data="PackingStyle_Cls"
              placeholder="Packing Style"
              v-model="model.PackingStyleCls"
              :errors="errors?.PackingStyleCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Group Cls</label>
            <input-cls
              type-data="Group_Cls"
              placeholder="Group Cls"
              v-model="model.GroupCls"
              :errors="errors?.GroupCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Standard Stock</label>
            <input-money
              placeholder="Standard Stock"
              v-model="model.StandardStock"
              :errors="errors?.StandardStock"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Safety Stock</label>
            <input-money
              placeholder="Safety Stock"
              v-model="model.SafetyStock"
              :errors="errors?.SafetyStock"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Safety Stock (%)</label>
            <input-money
              placeholder="Safety Stock (%)"
              v-model="model.SafetyStockPercentage"
              :errors="errors?.SafetyStockPercentage"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Max Stock</label>
            <input-money
              placeholder="Max Stock"
              v-model="model.MaxStock"
              :errors="errors?.MaxStock"
            />
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-3">
            <label class="form-label">Qty/Box (Parts/Material)</label>
            <input-money
              placeholder="Qty/Box (Parts/Material)"
              v-model="model.NumberBox"
              :errors="errors?.NumberBox"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Accounting</label>
            <input-text
              placeholder="Accounting"
              v-model="model.AccountingCode"
              :errors="errors?.AccountingCode"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Explosion Cls</label>
            <input-cls
              type-data="ItemExplosionCls"
              placeholder="Explosion Cls"
              v-model="model.ExplosionCls"
              :errors="errors?.ExplosionCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Purchase Person</label>
            <input-cls
              type-data="PersonInCharge_Cls"
              placeholder="Purchase Person"
              v-model="model.PersonInChargeCls"
              :errors="errors?.PersonInChargeCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Allowance Day</label>
            <input-money
              placeholder="Allowance Day"
              v-model="model.AlowanceDay"
              :errors="errors?.AlowanceDay"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Use End Date</label>
            <input-date
              placeholder="Use End Date"
              v-model="model.UseEndDay"
              :errors="errors?.UseEndDay"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Min Stock</label>
            <input-money
              placeholder="Min Stock"
              v-model="model.MinStock"
              :errors="errors?.MinStock"
            />
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-4">
            <label class="form-label">Make Or Buy Cls</label>
            <input-cls
              placeholder="Make Or Buy Cls"
              type-data="ItemMakeOrBuyCls"
              v-model="model.MakeBuyCls"
              :errors="errors?.MakeBuyCls"
            />
          </div>
          <div class="mb-4">
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
            <input-cls
              type-data="PackingStyle_Cls"
              placeholder="Packing Style Part/Material"
              v-model="model.PackingStyleMaterialCls"
              :errors="errors?.PackingStyleMaterialCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Type Accs</label>
            <input-cls
              type-data="ItemTypeAccs"
              placeholder="Type Accs"
              v-model="model.TypeAccs"
              :errors="errors?.TypeAccs"
            />
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="mb-4">
            <label class="form-label">PO Type</label>
            <input-cls
              placeholder="PO Type"
              type-data="POType_Cls"
              v-model="model.POTypeCls"
              :errors="errors?.POTypeCls"
            />
          </div>
          <div class="mb-4">
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
          <div class="mb-3">
            <label class="form-label">Model</label>
            <input-cls
              type-data="Model_Cls"
              placeholder="Model Cls"
              v-model="model.ModelCls"
              :errors="errors?.ModelCls"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Delivery Leadtime</label>
            <input-money
              placeholder="Delivery Leadtime"
              v-model="model.DeliveryReadTime"
              :errors="errors?.DeliveryReadTime"
            />
          </div>
        </div>
      </div>
    </div>
    <div v-else-if="this.activeTab == 3">
      <div class="row mt-4">
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="fieldset">
            <div class="fieldset-title">Factory Info</div>
            <div class="mb-3">
              <label class="form-label">Factory</label>
              <input-manufacture
                placeholder="Factory Code"
                v-model="model.ManufactureCode"
                :errors="errors?.ManufactureCode"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Line</label>
              <input-line
                :manufacture="model.ManufactureCode"
                placeholder="Line Code"
                v-model="model.LineCode"
                :errors="errors?.LineCode"
              />
            </div>
          </div>
        </div>
        <div class="col-sm-12 col-md-3 col-lg-3 col-xl-3">
          <div class="fieldset">
            <div class="fieldset-title">Material Dimension</div>
            <div class="mb-3">
              <label class="form-label">Material Cls</label>
              <input-cls
                type-data="Material_Cls"
                placeholder="Material Cls"
                v-model="model.MaterialCls"
                :errors="errors?.MaterialCls"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Width</label>
              <input-money
                placeholder="Width"
                v-model="model.Width"
                :errors="errors?.Width"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Length</label>
              <input-money
                placeholder="Length"
                v-model="model.Length"
                :errors="errors?.Length"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Nett Weight</label>
              <input-money
                placeholder="Nett Weight"
                v-model="model.Weight"
                :errors="errors?.Weight"
              />
            </div>
            <div class="mb-3">
              <label class="form-label">Gross Weight</label>
              <input-money
                placeholder="Gross Weight"
                v-model="model.GrossWeight"
                :errors="errors?.GrossWeight"
              />
            </div>
          </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-xl-6">
          <div class="fieldset">
            <div class="fieldset-title">Material Classification</div>
            <div class="row">
              <div class="col-3">
                <div class="mb-4">
                  <label class="form-label">Sheet Coil Cls</label>
                  <input-cls
                    type-data="SheetCoil_Cls"
                    placeholder="Sheet Coil Cls"
                    v-model="model.SheetCoilCls"
                    :errors="errors?.SheetCoilCls"
                  />
                </div>
                <div class="mb-4">
                  <label class="form-label">Drawing Material Cls</label>
                  <input-cls
                    type-data="DrawingMaterial_Cls"
                    placeholder="Drawing Material Cls"
                    v-model="model.DrawingMaterialCls"
                    :errors="errors?.DrawingMaterialCls"
                  />
                </div>
                <div class="mb-4">
                  <label class="form-label">Surface Treatment Cls</label>
                  <input-cls
                    type-data="SurfaceTreatment_Cls"
                    placeholder="Surface Treatment Cls"
                    v-model="model.SurfaceTreatmentCls"
                    :errors="errors?.SurfaceTreatmentCls"
                  />
                </div>
                <div class="mb-4">
                  <label class="form-label">Heat Treatment Cls</label>
                  <input-cls
                    type-data="HeatTreatment_Cls"
                    placeholder="Heat Treatment Cls"
                    v-model="model.HeatTreatmentCls"
                    :errors="errors?.HeatTreatmentCls"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Classification Part Cls</label>
                  <input-cls
                    type-data="ClasificationPart_Cls"
                    placeholder="Classification Part Cls"
                    v-model="model.ClasificationPartCls"
                    :errors="errors?.ClasificationPartCls"
                  />
                </div>
              </div>
              <div class="col-3">
                <div class="mb-3">
                  <label class="form-label">Thickness</label>
                  <input-money
                    placeholder="Thickness"
                    v-model="model.Thickness"
                    :errors="errors?.Thickness"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Pitch</label>
                  <input-money
                    placeholder="Pitch"
                    v-model="model.Pitch"
                    :errors="errors?.Pitch"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Number Producible</label>
                  <input-money
                    placeholder="Number Producible"
                    v-model="model.NumberProducible"
                    :errors="errors?.NumberProducible"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Scrap Weight</label>
                  <input-money
                    placeholder="Scrap Weight"
                    v-model="model.ScrapWeight"
                    :errors="errors?.ScrapWeight"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Number of Process</label>
                  <input-money
                    placeholder="Number of Process"
                    v-model="model.NumberProcess"
                    :errors="errors?.NumberProcess"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Min Lot</label>
                  <input-money
                    placeholder="Min Lot"
                    v-model="model.MinLot"
                    :errors="errors?.MinLot"
                  />
                </div>
              </div>
              <div class="col-3">
                <div class="mb-3">
                  <label class="form-label">Material Coefficient</label>
                  <input-money
                    placeholder="Material Coefficient"
                    v-model="model.MaterialCoefficient"
                    :errors="errors?.MaterialCoefficient"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Process Coefficient</label>
                  <input-money
                    placeholder="Process Coefficient"
                    v-model="model.ProcessCoefficient"
                    :errors="errors?.ProcessCoefficient"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Lot Coefficient</label>
                  <input-money
                    placeholder="Lot Coefficient"
                    v-model="model.LotCoefficient"
                    :errors="errors?.LotCoefficient"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Product Leadtime</label>
                  <input-money
                    placeholder="Product Leadtime"
                    v-model="model.ProductReadTime"
                    :errors="errors?.ProductReadTime"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Yield</label>
                  <input-money
                    placeholder="Yield"
                    v-model="model.YieldPercentage"
                    :errors="errors?.YieldPercentage"
                  />
                </div>
              </div>
              <div class="col-3">
                <div class="mb-3">
                  <label class="form-label">Surface Order Point Qty</label>
                  <input-money
                    placeholder="Surface Order Point Qty"
                    v-model="model.SurfaceOrderPointQty"
                    :errors="errors?.SurfaceOrderPointQty"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Heat Order Point Qty</label>
                  <input-money
                    placeholder="Heat Order Point Qty"
                    v-model="model.HeatOrderPointQty"
                    :errors="errors?.HeatOrderPointQty"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">Sample</label>
                  <input-money
                    placeholder="Sample"
                    v-model="model.Sample"
                    :errors="errors?.Sample"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">SW Qty</label>
                  <input-money
                    placeholder="SW Qty"
                    v-model="model.SWQty"
                    :errors="errors?.SWQty"
                  />
                </div>
                <div class="mb-3">
                  <label class="form-label">EW Qty</label>
                  <input-money
                    placeholder="EWQ ty"
                    v-model="model.EWQty"
                    :errors="errors?.EWQty"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div class="submit-wrapper">
    <v-button-submit-modal
      :nomargintop="true"
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
      FinishGoodPartCls: "",
      DrawingNumber: "",
      WarehouseCode: "",
      Address: "",
      SupplierCode: "",
      DeliveryPlaceCode: "",
      ManufactureCode: "",
      LineCode: "",
      MakerItemCode: "",
      PartCls: "",
      MaterialCls: "",
      SheetCoilCls: "",
      DrawingMaterialCls: "",
      SurfaceTreatmentCls: "",
      HeatTreatmentCls: "",
      PackingStyleCls: "",
      GroupCls: "",
      MakeBuyCls: "",
      ControlCls: "",
      PackingStyleMaterialCls: "",
      AccountingCode: "",
      ExplosionCls: "",
      PersonInChargeCls: "",
      SupplyIssueCls: "",
      UnitCls: "",
      HSCode: "",
      TypeAccs: "",
      ModelCls: "",
      POTypeCls: "",
      ClasificationPartCls: "",
      DestinationCls: "",
      ColorCls: "",
      Thickness: null,
      Width: null,
      Length: null,
      Weight: null,
      Pitch: null,
      NumberProducible: null,
      ScrapWeight: null,
      SurfaceOrderPointQty: null,
      HeatOrderPointQty: null,
      Sample: null,
      SWQty: null,
      EWQty: null,
      NumberProcess: null,
      MaterialCoefficient: null,
      ProcessCoefficient: null,
      MinLot: null,
      LotQty: null,
      LotCoefficience: null,
      ProductReadTime: null,
      YieldPercentage: null,
      NumberEntering: null,
      StandardStock: null,
      SafetyStock: null,
      MaxStock: null,
      MinStock: null,
      AlowanceDay: null,
      DeliveryReadTime: null,
      OrderPointQty: null,
      NumberBox: null,
      UseEndDay: null,
      MinOrder: null,
      SafetyStockPercentage: null,
      ReserveCls: false,
      SupplyCls: false,
      ProvisionCls: false,
      ProductionCls: false,
      StockControlCls: false,
      ProductionCls: false,
      ProductionCls: false,
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
        FinishGoodPartCls: "",
        DrawingNumber: "",
        WarehouseCode: "",
        Address: "",
        SupplierCode: "",
        DeliveryPlaceCode: "",
        ManufactureCode: "",
        LineCode: "",
        MakerItemCode: "",
        PartCls: "",
        MaterialCls: "",
        SheetCoilCls: "",
        DrawingMaterialCls: "",
        SurfaceTreatmentCls: "",
        HeatTreatmentCls: "",
        PackingStyleCls: "",
        GroupCls: "",
        MakeBuyCls: "",
        ControlCls: "",
        PackingStyleMaterialCls: "",
        AccountingCode: "",
        ExplosionCls: "",
        PersonInChargeCls: "",
        SupplyIssueCls: "",
        UnitCls: "",
        HSCode: "",
        TypeAccs: "",
        ModelCls: "",
        POTypeCls: "",
        ClasificationPartCls: "",
        DestinationCls: "",
        ColorCls: "",
        Thickness: null,
        Width: null,
        Length: null,
        Weight: null,
        Pitch: null,
        NumberProducible: null,
        ScrapWeight: null,
        SurfaceOrderPointQty: null,
        HeatOrderPointQty: null,
        Sample: null,
        SWQty: null,
        EWQty: null,
        NumberProcess: null,
        MaterialCoefficient: null,
        ProcessCoefficient: null,
        MinLot: null,
        LotQty: null,
        LotCoefficience: null,
        ProductReadTime: null,
        YieldPercentage: null,
        NumberEntering: null,
        StandardStock: null,
        SafetyStock: null,
        MaxStock: null,
        MinStock: null,
        AlowanceDay: null,
        DeliveryReadTime: null,
        OrderPointQty: null,
        NumberBox: null,
        UseEndDay: null,
        MinOrder: null,
        SafetyStockPercentage: null,
        ReserveCls: false,
        SupplyCls: false,
        ProvisionCls: false,
        ProductionCls: false,
        StockControlCls: false,
        ProductionCls: false,
        ProductionCls: false,
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
