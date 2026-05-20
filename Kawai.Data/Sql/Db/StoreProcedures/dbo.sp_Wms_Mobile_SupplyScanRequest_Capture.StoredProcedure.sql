
CREATE   PROCEDURE [dbo].[sp_Wms_Mobile_SupplyScanRequest_Capture]
	@BarcodeNo varchar(50),
	@PickingNo varchar(100),
	@ItemClass varchar(25)
as
begin
	Declare @ItemCode Varchar(100) = (Select ItemCode from StockDetail where BarcodeNo= @BarcodeNo and Qty > 0)
	declare @RequestDetailID bigint = (select RequestDetailID from PartMaterialRequestDetail where RefNumber = @PickingNo and AreaCode = @ItemClass)	
	 declare @planQty numeric(18,9) = (select top 1 ChildRequirement_Qty From PartMaterialRequestItemDetail A Left JOIN  PartMaterialRequestDetail B ON B.RequestDetailID = A.RequestDetailID where A.RequestDetailID = @RequestDetailID and ItemCode = @ItemCode and RefNumber = @PickingNo)

	declare @scanQty numeric(18,9) = 
	(
		select sum(Qty) From PartMaterialRequestItemDetailScan a
		inner join PartMaterialRequestDetail b on a.IDSeq = a.IDSeq
		where b.RequestDetailID = @RequestDetailID and a.ItemCode = @ItemCode
	)

	SELECT  sd.WarehouseCode, sd.BarcodeNo, Line_Code LineCode, '' RequestNo, '' ProductionDate, LotNo, vc.[Description] UnitDesc,
			sd.ItemCode, mi.Item_Name ItemName,  isnull(@planQty, 0) PlanQty,  isnull(@scanQty, 0) QtyScan
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	left join Unit_Cls vc on mi.Unit_Cls =  vc.Unit_Cls
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
