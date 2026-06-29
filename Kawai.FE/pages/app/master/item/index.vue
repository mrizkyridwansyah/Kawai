<template>
  <v-frame title="Item" icon="database">
    <template #frame-content>
      <table>
        <tr>
          <td><label class="form-label">Item Code</label></td>
          <td style="padding-left: 15px">
            <div class="d-flex">
              <input-text
                v-model="model.ItemCode"
                :disabled="mode !== 'add'"
                :errors="errors?.ItemCode"
                style="width: 200px"
                maxlength="25"
              />
              <button
                class="form-submit bg-primary"
                style="margin-left: 2px; width: 3em; border-radius: 0.5em"
                @click="loadItem"
              >
                <v-icon name="search" width="16px" />
              </button>
            </div>
          </td>
          <td style="padding-left: 15px">
            <label class="form-label">Description</label>
          </td>
          <td style="padding-left: 15px">
            <div class="d-flex">
              <input-text
                v-model="model.ItemName"
                :errors="errors?.ItemName"
                style="width: 460px"
                maxlength="75"
              />
            </div>
          </td>
          <td
            style="padding-left: 15px; min-width: 280px !important"
            colspan="2"
          >
            <v-button
              :action="submit"
              label="Submit"
              cClass="btn-primary mr-1"
              :is-loading="isLoading"
              :disabled="!menuPrivAllowUpdate" 
            />
            <v-button
              :action="remove"
              label="Delete"
              icon="trash"
              cClass="btn-danger mr-1"
              :is-loading="isLoading"
              :disabled="mode === 'add' || !menuPrivAllowUpdate"
            />
            <v-button
              :action="clear"
              label="Clear"
              icon="refresh"
              cClass="btn-danger mr-1"
              :is-loading="isLoading"
            />
          </td>
        </tr>
      </table>

      <ul class="nav nav-pills segmented-tabs mt-4 mb-3">
        <li class="nav-item" v-for="item in list" :key="item.tab">
          <a
            class="nav-link"
            :class="{ active: activeTab === item.tab }"
            href="#"
            @click.prevent="activeTab = item.tab"
          >
            {{ item.title }}
          </a>
        </li>
      </ul>

      <div class="form-content" style="padding-bottom: 5em">
        <div v-if="this.activeTab == 1" class="table-wrapper">
          <table class="fixed-table">
            <tr>
              <td style="padding-right: 5px; vertical-align: top">
                <div class="fieldset mt-2">
                  <div class="fieldset-title">Item Info</div>
                  <div>
                    <table>
                      <tr>
                        <td style="vertical-align: middle" class="label-kiri">
                          <label class="form-label">Finish Good Part</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-cls
                            type-data="ItemFinishGoodCls"
                            v-model="model.FinishGoodPartCls"
                            :errors="errors?.FinishGoodPartCls"
                            style-code="width: 90px"
                            style-desc="width: 190px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Part Number</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-text
                            v-model="model.MakerItemCode"
                            :errors="errors?.MakerItemCode"
                            style="width: 280px"
                            maxlength="30"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Drawing Code</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-text
                            v-model="model.DrawingNumber"
                            :errors="errors?.DrawingNumber"
                            style="width: 280px"
                            maxlength="15"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
              <td rowspan="3" style="vertical-align: top">
                <div class="fieldset mt-2" style="width: max-content">
                  <div class="fieldset-title">Stock</div>
                  <div style="width: max-content">
                    <table>
                      <tr>
                        <td style="vertical-align: middle">
                          <label class="form-label"
                            >Qty / Case <br />(Finish Goods)</label
                          >
                        </td>
                        <td style="padding-left: 15px">
                          <input-money
                            class="text-right"
                            v-model="model.NumberEntering"
                            :errors="errors?.NumberEntering"
                          />
                        </td>
                        <td style="vertical-align: middle; padding-left: 15px">
                          <label class="form-label">Make Or Buy Cls</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-cls
                            type-data="ItemMakeOrBuyCls"
                            v-model="model.MakeBuyCls"
                            :errors="errors?.MakeBuyCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Packing Style</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="PackingStyle_Cls"
                            v-model="model.PackingStyleCls"
                            :errors="errors?.PackingStyleCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Group Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Group_Cls"
                            v-model="model.GroupCls"
                            :errors="errors?.GroupCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>

                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Control</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Control_Cls"
                            v-model="model.ControlCls"
                            :errors="errors?.ControlCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Standard Stock</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.StandardStock"
                            :errors="errors?.StandardStock"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Unit Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Unit_Cls"
                            v-model="model.UnitCls"
                            :errors="errors?.UnitCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Safety Stock</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.SafetyStock"
                            :errors="errors?.SafetyStock"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Order Point Qty</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.OrderPointQty"
                            :errors="errors?.OrderPointQty"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Safety Stock (%)</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.SafetyStockPercentage"
                            :errors="errors?.SafetyStockPercentage"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Min. Order Qty</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.MinOrder"
                            :errors="errors?.MinOrder"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Max Stock</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.MaxStock"
                            :errors="errors?.MaxStock"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: top; padding-top: 5px">
                          <label class="form-label">Min Stock</label>
                        </td>
                        <td
                          style="
                            vertical-align: top;
                            padding-left: 15px;
                            padding-top: 5px;
                          "
                        >
                          <input-money
                            v-model="model.MinStock"
                            :errors="errors?.MinStock"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: top;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label"
                            >Packing Style <br />Part / Material</label
                          >
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="PackingStyle_Cls"
                            v-model="model.PackingStyleMaterialCls"
                            :errors="errors?.PackingStyleMaterialCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label"
                            >Qty/box <br />
                            (Parts/Material)</label
                          >
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.NumberBox"
                            :errors="errors?.NumberBox"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Accounting</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-text
                            v-model="model.AccountingCode"
                            :errors="errors?.AccountingCode"
                            maxlength="7"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Type Accs</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ItemTypeAccs"
                            v-model="model.TypeAccs"
                            :errors="errors?.TypeAccs"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Explosion Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ItemExplosionCls"
                            v-model="model.ExplosionCls"
                            :errors="errors?.ExplosionCls"
                            style-code="width: 90px"
                            style-desc="width: 130px"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Model</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Model_Cls"
                            v-model="model.ModelCls"
                            :errors="errors?.ModelCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: top; padding-top: 5px">
                          <label class="form-label">Purchase Person</label>
                        </td>
                        <td
                          style="
                            vertical-align: top;
                            padding-left: 15px;
                            padding-top: 5px;
                          "
                        >
                          <input-cls
                            type-data="PersonInCharge_Cls"
                            v-model="model.PersonInChargeCls"
                            :errors="errors?.PersonInChargeCls"
                            style-code="width: 90px"
                            style-desc="width: 130px"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">PO Type</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="POType_Cls"
                            v-model="model.POTypeCls"
                            :errors="errors?.POTypeCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: top; padding-top: 5px">
                          <label class="form-label">Stock Control</label>
                        </td>
                        <td
                          style="
                            vertical-align: top;
                            padding-left: 15px;
                            padding-top: 5px;
                          "
                        >
                          <input-cls
                            type-data="ItemStockControlCls"
                            v-model="model.StockControlCls"
                            :errors="errors?.StockControlCls"
                            style-code="width: 90px"
                            style-desc="width: 130px"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Destination Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Destination_Cls"
                            v-model="model.DestinationCls"
                            :errors="errors?.DestinationCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Allowance Day</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.AlowanceDay"
                            :errors="errors?.AlowanceDay"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Color Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Color_Cls"
                            v-model="model.ColorCls"
                            :errors="errors?.ColorCls"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>

                       <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Use End Date</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-date
                            v-model="model.UseEndDay"
                            :errors="errors?.UseEndDay"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Grouping Part Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="Grouping_Class_Part"
                            v-model="model.Grouping_Class_Part_Code"
                            style-code="width: 90px"
                            style-desc="width: 120px"
                          />
                        </td>
                      </tr>


               
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Delivery Leadtime</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.DeliveryReadTime"
                            :errors="errors?.DeliveryReadTime"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
            </tr>
            <tr>
              <td style="padding-right: 5px; vertical-align: top">
                <div class="fieldset">
                  <div class="fieldset-title">Order & Delivery</div>
                  <div>
                    <table>
                      <tr>
                        <td style="vertical-align: middle" class="label-kiri">
                          <label class="form-label">Warehouse Code</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-warehouse-item
                            v-model="model.WarehouseCode"
                            :errors="errors?.WarehouseCode"
                            style-code="width: 125px"
                            style-desc="width: 155px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Address</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-text
                            v-model="model.Address"
                            :errors="errors?.Address"
                            maxlength="15"
                            style="width: 130px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Supplier Code</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-trade
                            :trade-cls="['2', '3']"
                            v-model="model.SupplierCode"
                            :errors="errors?.SupplierCode"
                            style-code="width: 125px"
                            style-desc="width: 155px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Delivery Code</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-delivery-place
                            v-model="model.DeliveryPlaceCode"
                            :errors="errors?.DeliveryPlaceCode"
                            :trade="model.SupplierCode"
                            style-code="width: 125px"
                            style-desc="width: 155px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">HS Code</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-hs
                            v-model="model.HSCode"
                            :errors="errors?.HSCode"
                            style="width: 125px"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
            </tr>
            <tr>
              <td style="padding-right: 5px; vertical-align: top">
                <div class="fieldset">
                  <div class="fieldset-title">Item Classification</div>
                  <div>
                    <table>
                      <tr>
                        <td style="vertical-align: middle" class="label-kiri">
                          <label class="form-label">Part Cls</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-cls
                            type-data="ItemPartCls"
                            v-model="model.PartCls"
                            :errors="errors?.PartCls"
                            style-code="width: 90px"
                            style-desc="width: 190px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Reserve Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ItemReserveCls"
                            v-model="model.ReserveCls"
                            :errors="errors?.ReserveCls"
                            style-code="width: 90px"
                            style-desc="width: 190px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Supply Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ItemSupplyCls"
                            v-model="model.SupplyCls"
                            :errors="errors?.SupplyCls"
                            style-code="width: 90px"
                            style-desc="width: 190px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Provision Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ItemProvisionCls"
                            v-model="model.ProvisionCls"
                            :errors="errors?.ProvisionCls"
                            style-code="width: 90px"
                            style-desc="width: 190px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Production Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ItemProductionCls"
                            v-model="model.ProductionCls"
                            :errors="errors?.ProductionCls"
                            style-code="width: 90px"
                            style-desc="width: 190px"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
            </tr>
          </table>
        </div>
        <div v-if="this.activeTab == 2" class="table-wrapper">
          <table class="fixed-table">
            <tr>
              <td
                style="padding-right: 5px; vertical-align: top; height: 140px"
              >
                <div class="fieldset mt-2">
                  <div class="fieldset-title">Factory Info</div>
                  <div>
                    <table>
                      <tr>
                        <td style="vertical-align: middle" class="label-kiri">
                          <label class="form-label">Factory Code</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-manufacture
                            v-model="model.ManufactureCode"
                            :errors="errors?.ManufactureCode"
                            style-code="width: 100px"
                            style-desc="width: 180px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Line Code</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-line
                            :manufacture="model.ManufactureCode"
                            v-model="model.LineCode"
                            :errors="errors?.LineCode"
                            style-code="width: 100px"
                            style-desc="width: 180px"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
              <td rowspan="3" style="vertical-align: top">
                <div class="fieldset mt-2" style="width: max-content">
                  <div class="fieldset-title">Material Classification</div>
                  <div style="width: max-content">
                    <table>
                      <tr>
                        <td style="vertical-align: middle">
                          <label class="form-label">Sheet/Coil Cls</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-cls
                            type-data="SheetCoil_Cls"
                            v-model="model.SheetCoilCls"
                            :errors="errors?.SheetCoilCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Pitch</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.Pitch"
                            :errors="errors?.Pitch"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Number Producible</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.NumberProducible"
                            :errors="errors?.NumberProducible"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Scrap Weight</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.ScrapWeight"
                            :errors="errors?.ScrapWeight"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Drawing Material Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="DrawingMaterial_Cls"
                            v-model="model.DrawingMaterialCls"
                            :errors="errors?.DrawingMaterialCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label"
                            >Surface Treatment Cls</label
                          >
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="SurfaceTreatment_Cls"
                            v-model="model.SurfaceTreatmentCls"
                            :errors="errors?.SurfaceTreatmentCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label"></label>
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 5px;
                          "
                        >
                          <input-money
                            v-model="model.SurfaceOrderPointQty"
                            :errors="errors?.SurfaceOrderPointQty"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Heat Treatment Cls</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="HeatTreatment_Cls"
                            v-model="model.HeatTreatmentCls"
                            :errors="errors?.HeatTreatmentCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label"></label>
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 5px;
                          "
                        >
                          <input-money
                            v-model="model.HeatOrderPointQty"
                            :errors="errors?.HeatOrderPointQty"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Sample</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.Sample"
                            :errors="errors?.Sample"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Min Lot</label>
                        </td>
                        <td style="padding-left: 5px; padding-top: 5px">
                          <input-money
                            v-model="model.MinLot"
                            :errors="errors?.MinLot"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">SW Qty</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.SWQty"
                            :errors="errors?.SWQty"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Lot Qty</label>
                        </td>
                        <td style="padding-left: 5px; padding-top: 5px">
                          <input-money
                            v-model="model.LotQty"
                            :errors="errors?.LotQty"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">EW Qty</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.EWQty"
                            :errors="errors?.EWQty"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Lot Coefficient</label>
                        </td>
                        <td style="padding-left: 5px; padding-top: 5px">
                          <input-money
                            v-model="model.LotCoefficience"
                            :errors="errors?.LotCoefficience"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Number Of Process</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.NumberProcess"
                            :errors="errors?.NumberProcess"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Product Lead Time</label>
                        </td>
                        <td style="padding-left: 5px; padding-top: 5px">
                          <input-money
                            v-model="model.ProductReadTime"
                            :errors="errors?.ProductReadTime"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Material Coeficient</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.MaterialCoefficient"
                            :errors="errors?.MaterialCoefficient"
                          />
                        </td>
                        <td
                          style="
                            vertical-align: middle;
                            padding-top: 5px;
                            padding-left: 15px;
                          "
                        >
                          <label class="form-label">Yield</label>
                        </td>
                        <td style="padding-left: 5px; padding-top: 5px">
                          <input-money
                            v-model="model.YieldPercentage"
                            :errors="errors?.YieldPercentage"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Process Coeficient</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.ProcessCoefficient"
                            :errors="errors?.ProcessCoefficient"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td style="vertical-align: middle; padding-top: 5px">
                          <label class="form-label">Classification Part</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-cls
                            type-data="ClasificationPart_Cls"
                            v-model="model.ClasificationPartCls"
                            :errors="errors?.ClasificationPartCls"
                            style-code="width: 95px"
                            style-desc="width: 130px"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
            </tr>
            <tr>
              <td style="padding-right: 5px; vertical-align: top">
                <div class="fieldset" style="margin-top: -10px !important">
                  <div class="fieldset-title">Material Dimension</div>
                  <div>
                    <table>
                      <tr>
                        <td style="vertical-align: middle" class="label-kiri">
                          <label class="form-label">Material Cls</label>
                        </td>
                        <td style="padding-left: 15px">
                          <input-cls
                            type-data="Material_Cls"
                            v-model="model.MaterialCls"
                            :errors="errors?.MaterialCls"
                            style-code="width: 100px"
                            style-desc="width: 180px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Thickness</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.Thickness"
                            :errors="errors?.Thickness"
                            style="width: 100px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Width</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.Width"
                            :errors="errors?.Width"
                            style="width: 100px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Length</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.Length"
                            :errors="errors?.Length"
                            style="width: 100px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Nett Weight</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.Weight"
                            :errors="errors?.Weight"
                            style="width: 100px"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td
                          style="vertical-align: middle; padding-top: 5px"
                          class="label-kiri"
                        >
                          <label class="form-label">Gross Weight</label>
                        </td>
                        <td style="padding-left: 15px; padding-top: 5px">
                          <input-money
                            v-model="model.GrossWeight"
                            :errors="errors?.GrossWeight"
                            style="width: 100px"
                          />
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </td>
            </tr>
          </table>
        </div>
      </div>
    </template>
  </v-frame>

  <v-modal id="shared-item" title="List Item" size="lg">
    <shared-item
      :list="this.dsItem"
      :refresh="refreshItemList"
      :actions="[
        {
          href: 'javascript:void(0);',
          icon: 'edit',
          label: 'Select',
          event: (item) => selectItem(item),
        },
      ]"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
    activeTab: 1,
    mode: "",
    list: [
      { title: "General", tab: 1 },
      { title: "Manufacture", tab: 2 },
    ],
    isLoading: false,
    mode: "add",
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
      ReserveCls: "",
      SupplyCls: "",
      ProvisionCls: "",
      ProductionCls: "",
      StockControlCls: "",
      Grouping_Class_Part_Code:"",
    },
    errorResponse: {},
    errors: {},
    debounce: null,
    refreshItemList: 0,
   menuPrivAllowUpdate: false,
  }),
  computed: {
    ds: function () {
      return useItem();
    },
    dsItem: function () {
      return useItem();
    },
     dsMenu: function () {
      return useMenu();
    },
  },
  mounted: function () {
        this.dsMenu.privileges().then((dt) => {
      this.menuPrivAllowUpdate = dt.Data.filter(
        (a) => a.MenuID == "A08",
      )[0].AllowUpdate;
    });

    // document.body.style.overflow = "auto";
  },
  methods: {
    loadItem: function (idx) {
      this.refreshItemList++;
      this.$bvModal.show("shared-item");
    },
    selectItem: function (dt) {
      console.log(dt);
      this.mode = "edit";
      this.ds.loadDetail(dt.ItemCode).then(dtx => {
        let result = dtx.Data;
        this.model = {
          ItemCode: result.ItemCode,
          ItemName: result.ItemName,
          FinishGoodPartCls: result.FinishGoodPartCls,
          DrawingNumber: result.DrawingNumber,
          WarehouseCode: result.WarehouseCode,
          Address: result.Address,
          SupplierCode: result.SupplierCode,
          DeliveryPlaceCode: result.DeliveryPlaceCode,
          ManufactureCode: result.ManufactureCode,
          LineCode: result.LineCode,
          MakerItemCode: result.MakerItemCode,
          PartCls: result.PartCls,
          MaterialCls: result.MaterialCls,
          SheetCoilCls: result.SheetCoilCls,
          DrawingMaterialCls: result.DrawingMaterialCls,
          SurfaceTreatmentCls: result.SurfaceTreatmentCls,
          HeatTreatmentCls: result.HeatTreatmentCls,
          PackingStyleCls: result.PackingStyleCls,
          GroupCls: result.GroupCls,
          MakeBuyCls: result.MakeBuyCls,
          ControlCls: result.ControlCls,
          PackingStyleMaterialCls: result.PackingStyleMaterialCls,
          AccountingCode: result.AccountingCode,
          ExplosionCls: result.ExplosionCls,
          PersonInChargeCls: result.PersonInChargeCls,
          SupplyIssueCls: result.SupplyIssueCls,
          UnitCls: result.UnitCls,
          HSCode: result.HSCode,
          TypeAccs: result.TypeAccs,
          ModelCls: result.ModelCls,
          POTypeCls: result.POTypeCls,
          ClasificationPartCls: result.ClasificationPartCls,
          DestinationCls: result.DestinationCls,
          ColorCls: result.ColorCls,
          Thickness: result.Thickness,
          Width: result.Width,
          Length: result.Length,
          Weight: result.Weight,
          Pitch: result.Pitch,
          NumberProducible: result.NumberProducible,
          ScrapWeight: result.ScrapWeight,
          SurfaceOrderPointQty: result.SurfaceOrderPointQty,
          HeatOrderPointQty: result.HeatOrderPointQty,
          Sample: result.Sample,
          SWQty: result.SWQty,
          EWQty: result.EWQty,
          NumberProcess: result.NumberProcess,
          MaterialCoefficient: result.MaterialCoefficient,
          ProcessCoefficient: result.ProcessCoefficient,
          MinLot: result.MinLot,
          LotQty: result.LotQty,
          LotCoefficience: result.LotCoefficience,
          ProductReadTime: result.ProductReadTime,
          YieldPercentage: result.YieldPercentage,
          NumberEntering: result.NumberEntering,
          StandardStock: result.StandardStock,
          SafetyStock: result.SafetyStock,
          MaxStock: result.MaxStock,
          MinStock: result.MinStock,
          AlowanceDay: result.AlowanceDay,
          DeliveryReadTime: result.DeliveryReadTime,
          OrderPointQty: result.OrderPointQty,
          NumberBox: result.NumberBox,
          UseEndDay: result.UseEndDay,
          MinOrder: result.MinOrder,
          SafetyStockPercentage: result.SafetyStockPercentage,
          ReserveCls: result.ReserveCls,
          SupplyCls: result.SupplyCls,
          ProvisionCls: result.ProvisionCls,
          ProductionCls: result.ProductionCls,
          StockControlCls: result.StockControlCls,
          Grouping_Class_Part_Code : result.Grouping_Class_Part_Code,
        };
        this.$bvModal.hide("shared-item");
      })
    },
    submit: function () {
      this.errors = {};
      if (this.mode === "add") this.create();
      else this.update();
    },
    create: function () {
      this.ds
        .create(this.model)
        .then((datas) => {
          toastSuccess("Data saved successfully!");
          this.clear();
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
          this.clear();
        })
        .catch((err) => {
          this.errors = err?.Errors;
          toastDanger(err?.Message);
        });
    },
    remove: function () {
      confirmRemove(
        () =>
          new Promise((resolve, reject) => {
            this.ds
              .remove(this.model.ItemCode)
              .then((datas) => {
                toastSuccess("Data Deleted successfully!");
                this.clear();
                resolve();
              })
              .catch((err) => {
                this.errors = err?.Errors;
                toastDanger(err?.Message);
              });
          }),
        null,
        this.model.ItemName,
      );
    },
    clear: function () {
      this.mode = "add";
      this.errors = {};
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
        ReserveCls: "",
        SupplyCls: "",
        ProvisionCls: "",
        ProductionCls: "",
        StockControlCls: "",
        Grouping_Class_Part_Code:"",
      };
    },
  },
};
</script>

<style scoped>
.nav-pills.segmented-tabs {
  display: flex;
}

.nav-pills.segmented-tabs .nav-item {
  margin: 0;
}

.nav-pills.segmented-tabs .nav-link {
  border: 1px solid #ced4da;
  border-right: none; /* nyambung */
  background-color: #f8f9fa;
  color: #495057;
  border-radius: 0;
  padding: 8px 18px;
  transition: all 0.2s ease;
}

/* ujung kiri */
.nav-pills.segmented-tabs .nav-item:first-child .nav-link {
  border-top-left-radius: 6px;
  border-bottom-left-radius: 6px;
}

/* ujung kanan */
.nav-pills.segmented-tabs .nav-item:last-child .nav-link {
  border-right: 1px solid #ced4da;
  border-top-right-radius: 6px;
  border-bottom-right-radius: 6px;
}

/* hover */
.nav-pills.segmented-tabs .nav-link:hover {
  background-color: #e9ecef;
}

/* active tab */
.nav-pills.segmented-tabs .nav-link.active {
  background-color: #0d6efd;
  color: #fff;
  border-color: #0d6efd;
  position: relative;
  z-index: 1;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}

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

/*.table-wrapper {
  overflow-x: auto; 
}*/

.fixed-table {
  width: 1170px; /* FIX width */
  border-collapse: collapse;
}

.fixed-table td {
  vertical-align: middle;
}

td {
  vertical-align: middle;
}

.label-kiri {
  width: 100px !important;
}
</style>
