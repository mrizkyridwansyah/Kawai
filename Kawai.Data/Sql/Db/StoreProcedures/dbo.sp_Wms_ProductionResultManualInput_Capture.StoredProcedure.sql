SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [sp_Wms_ProductionResultManualInput_Capture]
 
	@ProductionId bigint
as
begin
	select 
	 ProdResultID
    ,ProductionID
    ,ProductionDate
    ,ItemCode
    ,Shift
    ,TotalGoodQty
    ,TotalNGQty
	From ProductionResultHeader pmrh 
	where pmrh.ProductionID = @ProductionId


 	select 
		b.ResultDetailID
		,b.ProdResultID
		,b.BarcodeNo
		,b.LotNo
		,b.SerialNo
		,b.Qty
		,b.ResultType

	From 
	(
		select * from ProductionResultHeader where ProductionID = @ProductionId
	) A 
	inner join ProductionResultDetail b on a.ProdResultID = b.ProdResultID

	select 
		b.ResultDetailID
		,b.ProdResultID
		,b.BarcodeNo
		,b.LotNo
		,b.SerialNo
		,b.Qty
		,b.ResultType

	From 
	(
		select * from ProductionResultHeader where ProductionID = @ProductionId
	) A 
	inner join ProductionResultDetail b on a.ProdResultID = b.ProdResultID
end
GO
