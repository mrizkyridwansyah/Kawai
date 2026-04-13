SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Mobile_SupplyScanRequest_Capture]
	@BarcodeNo varchar(50),
	@PickingNo varchar(100),
	@ItemClass varchar(25)
as
begin
	Declare @ItemCode Varchar(100) = (Select ItemCode from StockDetail where BarcodeNo= @BarcodeNo and Qty > 0)
	declare @RequestDetailID bigint = (select RequestDetailID from PartMaterialRequestDetail where RefNumber = @PickingNo and AreaCode = @ItemClass)	
	declare @planQty numeric(18,9) = (select ChildRequirement_Qty From PartMaterialRequestItemDetail where RequestDetailID = @RequestDetailID and ItemCode = @ItemCode)

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
GO
