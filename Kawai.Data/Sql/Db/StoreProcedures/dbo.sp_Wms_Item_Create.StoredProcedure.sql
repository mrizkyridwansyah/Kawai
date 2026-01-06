SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Item_Create]
	@ItemCode varchar(25),
	@ItemName varchar(75),
	@FinishGoodPartCls varchar(2),
	@DrawingNumber varchar(100),
	@WarehouseCode varchar(15),
	@Address varchar(15),
	@SupplierCode varchar(15),
	@DeliveryPlaceCode varchar(15),
	@ManufactureCode varchar(15),
	@LineCode varchar(15),
	@MakerItemCode varchar(30),
	@PartCls varchar(2),
	@ReserveCls bit,
	@SupplyCls bit,
	@ProvisionCls bit,
	@ProductionCls bit,
	@MaterialCls varchar(2),
	@Thickness numeric(18,5),
	@Width numeric(18,5),
	@Length numeric(18,5),
	@Weight numeric(18,5),
	@GrossWeight numeric(18,5),
	@SheetCoilCls varchar(2),
	@Pitch numeric(9,2),
	@NumberProducible numeric(18,4),
	@ScrapWeight numeric(9,2),
	@DrawingMaterialCls varchar(2),
	@SurfaceTreatmentCls varchar(2),
	@SurfaceOrderPointQty numeric(7,0),
	@HeatTreatmentCls varchar(2),
	@HeatOrderPointQty numeric(7,0),
	@Sample numeric(9,2),
	@SWQty numeric(9,2),
	@EWQty numeric(9,2),
	@NumberProcess numeric(2,0),
	@MaterialCoefficient numeric(9,2),
	@ProcessCoefficient numeric(9,2),
	@MinLot numeric(7,0),
	@LotQty numeric(7,0),
	@LotCoefficience numeric(7,0),
	@ProductReadTime numeric(2,0),
	@YieldPercentage numeric(5,2),
	@NumberEntering numeric(7,2),
	@PackingStyleCls varchar(2),
	@GroupCls varchar(2),
	@StandardStock numeric(10,0),
	@SafetyStock numeric(10,0),
	@MaxStock numeric(10,0),
	@MinStock numeric(10,0),
	@AlowanceDay numeric(2,0),
	@DeliveryReadTime numeric(2,0),
	@MakeBuyCls varchar(2),
	@ControlCls varchar(2),
	@OrderPointQty numeric(10,0),
	@UnitCls varchar(2),
	@NumberBox numeric(10,0),
	@PackingStyleMaterialCls varchar(2),
	@AccountingCode varchar(7),
	@ExplosionCls varchar(2),
	@PersonInChargeCls varchar(2),
	@StockControlCls varchar(2),
	@SupplyIssueCls varchar(2),
	@UseEndDay date,
	@HSCode varchar(15),
	@MinOrder numeric(10, 0),
	@SafetyStockPercentage numeric(5,2),
	@SAPItemCode varchar(18),
	@TypeAccs varchar(2),
	@ModelCls varchar(10),
	@POTypeCls varchar(2),
	@ClasificationPartCls varchar(2),
	@DestinationCls varchar(2),
	@ColorCls varchar(2),
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from item_master where item_Code = @ItemCode)
	begin
		raiserror('Item Code Already Exists',16,1)
		return;
	end

	if not exists (select 1 from Trade_Master where Trade_Code =  @SupplierCode)
	begin
		raiserror('Data Supplier didn''t Exists',16,1)
		return;
	end

	if not exists (select * from Control_Cls where Control_Cls = @ControlCls)
	begin
		raiserror('Invalid Control Cls',16,1)
		return;
	end

	if @UseEndDay is null
	begin
		set @UseEndDay = cast('9999-12-31' as date)
	end

	insert into Item_Master
	(
		Item_Code
		, Item_Name
		, FinishGoodPart_Cls
		, Drawing_Number
		, WH_Code
		, Address
		, Supplier_Code
		, Delivery_Place
		, Manufacture_Code
		, Line_Code
		, Part_Cls
		, MakerItem_Code
		, Reserve_Cls
		, Suply_Cls
		, Provision_Cls
		, Material_Cls
		, Thickness
		, Width
		, Length
		, Weight
		, GrossWeight
		, SheetCoil_Cls
		, Pitch
		, Number_Producible
		, Scrap_Weight
		, DrawingMaterial_Cls
		, SurfaceTreatment_Cls
		, Surface_OrderPointQty
		, HeatTreatment_Cls
		, Heat_OrderPointQty
		, Sample
		, SW_Qty
		, EW_Qty
		, Number_Process
		, Material_Coefficient
		, Process_Coefficient
		, Min_Lot
		, Lot_Qty
		, Lot_Coefficience
		, Product_ReadTime
		, Yield_Percentage
		, Number_Entering
		, PackingStyle_Cls
		, Group_Cls
		, Production_Cls
		, Standard_Stock
		, Safety_Stock
		, Max_Stock
		, Min_Stock
		, Alowance_Day
		, Delivery_ReadTime
		, MakeBuy_Cls
		, Control_Cls
		, OrderPoint_Qty
		, Unit_Cls
		, Number_Box
		, PackingStyleMaterial_Cls
		, Accounting_Code
		, Explosion_Cls
		, PersonInCharge_Cls
		, StockControl_Cls
		, SupplyIssue_Cls
		, Use_EndDay
		, HS_Code
		, Last_Update
		, Last_User
		, Register_Date
		, MinOrder
		, Safety_Stock_Percentage
		, SAP_Item_Code
		, TypeAccs
		, Model_Cls
		, ClasificationPart_Cls
		, Destination_Cls
		, Color_Cls
		, POType_Cls
	)
	values 
	(
		@ItemCode
		, @ItemName
		, @FinishGoodPartCls
		, @DrawingNumber
		, @WarehouseCode
		, @Address
		, @SupplierCode
		, ''
		, @ManufactureCode
		, @LineCode
		, @PartCls
		, @MakerItemCode
		, case when @ReserveCls	  = 1 then '01' else '02' end
		, case when @SupplyCls	  = 1 then '01' else '02' end
		, case when @ProvisionCls = 1 then '01' else '02' end
		, @MaterialCls
		, @Thickness
		, @Width
		, @Length
		, @Weight
		, @GrossWeight
		, @SheetCoilCls
		, @Pitch
		, @NumberProducible
		, @ScrapWeight
		, @DrawingMaterialCls
		, @SurfaceTreatmentCls
		, @SurfaceOrderPointQty
		, @HeatTreatmentCls
		, @HeatOrderPointQty
		, @Sample
		, @SWQty
		, @EWQty
		, @NumberProcess
		, @MaterialCoefficient
		, @ProcessCoefficient
		, @MinLot
		, @LotQty
		, @LotCoefficience
		, @ProductReadTime
		, @YieldPercentage
		, @NumberEntering
		, CASE WHEN @PackingStyleCls = 1 then '01' else '02' end
		, @GroupCls
		, CASE WHEN @ProductionCls = 1 then '01' else '02' end
		, @StandardStock
		, @SafetyStock
		, @MaxStock
		, @MinStock
		, @AlowanceDay
		, @DeliveryReadTime
		, @MakeBuyCls
		, @ControlCls
		, @OrderPointQty
		, @UnitCls
		, @NumberBox
		, @PackingStyleMaterialCls
		, @AccountingCode
		, @ExplosionCls
		, @PersonInChargeCls
		, CASE WHEN @StockControlCls = 1 then '01' else '02' end
		, @SupplyIssueCls
		, @UseEndDay
		, @HSCode
		, GETDATE()
		, @RegisterBy
		, GETDATE()
		, @MinOrder
		, @SafetyStockPercentage
		, @SAPItemCode
		, @TypeAccs
		, @ModelCls
		, @ClasificationPartCls
		, @DestinationCls
		, @ColorCls
		, @POTypeCls
	)
end
GO
