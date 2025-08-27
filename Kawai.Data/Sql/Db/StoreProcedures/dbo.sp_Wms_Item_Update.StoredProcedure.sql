SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Item_Update]
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
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from item_master where item_Code = @ItemCode)
	begin
		raiserror('Data Item didn''t Exists',16,1)
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

	update Item_Master
	set
		  Item_Name						= @ItemName
		, FinishGoodPart_Cls			= @FinishGoodPartCls
		, Drawing_Number				= @DrawingNumber
		, WH_Code						= @WarehouseCode
		, Address						= @Address
		, Supplier_Code					= @SupplierCode
		, Delivery_Place				= ''
		, Manufacture_Code				= @ManufactureCode
		, Line_Code						= @LineCode
		, Part_Cls						= @PartCls
		, MakerItem_Code				= @MakerItemCode
		, Reserve_Cls					= case when @ReserveCls	  = 1 then '01' else '02' end
		, Suply_Cls						= case when @SupplyCls	  = 1 then '01' else '02' end
		, Provision_Cls					= case when @ProvisionCls = 1 then '01' else '02' end
		, Material_Cls					= @MaterialCls
		, Thickness						= @Thickness
		, Width							= @Width
		, Length						= @Length
		, Weight						= @Weight
		, GrossWeight					= @GrossWeight
		, SheetCoil_Cls					= @SheetCoilCls
		, Pitch							= @Pitch
		, Number_Producible				= @NumberProducible
		, Scrap_Weight					= @ScrapWeight
		, DrawingMaterial_Cls			= @DrawingMaterialCls
		, SurfaceTreatment_Cls			= @SurfaceTreatmentCls
		, Surface_OrderPointQty			= @SurfaceOrderPointQty
		, HeatTreatment_Cls				= @HeatTreatmentCls
		, Heat_OrderPointQty			= @HeatOrderPointQty
		, Sample						= @Sample
		, SW_Qty						= @SWQty
		, EW_Qty						= @EWQty
		, Number_Process				= @NumberProcess
		, Material_Coefficient			= @MaterialCoefficient
		, Process_Coefficient			= @ProcessCoefficient
		, Min_Lot						= @MinLot
		, Lot_Qty						= @LotQty
		, Lot_Coefficience				= @LotCoefficience
		, Product_ReadTime				= @ProductReadTime
		, Yield_Percentage				= @YieldPercentage
		, Number_Entering				= @NumberEntering
		, PackingStyle_Cls				= CASE WHEN @PackingStyleCls = 1 then '01' else '02' end
		, Group_Cls						= @GroupCls
		, Production_Cls				= CASE WHEN @ProductionCls = 1 then '01' else '02' end
		, Standard_Stock				= @StandardStock
		, Safety_Stock					= @SafetyStock
		, Max_Stock						= @MaxStock
		, Min_Stock						= @MinStock
		, Alowance_Day					= @AlowanceDay
		, Delivery_ReadTime				= @DeliveryReadTime
		, MakeBuy_Cls					= @MakeBuyCls
		, Control_Cls					= @ControlCls
		, OrderPoint_Qty				= @OrderPointQty
		, Unit_Cls						= @UnitCls
		, Number_Box					= @NumberBox
		, PackingStyleMaterial_Cls		= @PackingStyleMaterialCls
		, Accounting_Code				= @AccountingCode
		, Explosion_Cls					= @ExplosionCls
		, PersonInCharge_Cls			= @PersonInChargeCls
		, StockControl_Cls				= CASE WHEN @StockControlCls = 1 then '01' else '02' end
		, SupplyIssue_Cls				= @SupplyIssueCls
		, Use_EndDay					= @UseEndDay
		, HS_Code						= @HSCode
		, Last_Update					= GETDATE()
		, Last_User						= @UpdateBy
		, Register_Date					= GETDATE()
		, MinOrder						= @MinOrder
		, Safety_Stock_Percentage		= @SafetyStockPercentage
		, SAP_Item_Code					= @SAPItemCode
		, TypeAccs						= @TypeAccs
		, Model_Cls						= @ModelCls
		, ClasificationPart_Cls			= @ClasificationPartCls
		, Destination_Cls				= @DestinationCls
		, Color_Cls						= @ColorCls
		, POType_Cls					= @POTypeCls
	where item_Code = @ItemCode
end
GO
