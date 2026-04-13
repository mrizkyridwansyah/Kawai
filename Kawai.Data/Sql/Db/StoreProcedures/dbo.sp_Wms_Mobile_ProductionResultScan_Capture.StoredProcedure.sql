SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_Mobile_ProductionResultScan_Capture]
	@BarcodeNo varchar(50)
AS
BEGIN
	SELECT  rd.BarcodeNo, Line_Code LineCode, '' RequestNo, '' ProductionDate, LotNo, vc.[Description] UnitDesc,
			rh.ItemCode, mi.Item_Name ItemName,  rd.Qty QtyResult
	FROM ProductionResultDetail rd 
	JOIN ProductionResultHeader rh ON rh.ProdResultID=rd.ProdResultID
	--StockDetail sd
	left join Item_Master mi on rh.ItemCode = mi.Item_Code
	left join vw_Cls vc on TypeData = 'Unit_Cls' and mi.Unit_Cls =  vc.ClsCode
	WHERE rd.BarcodeNo = @BarcodeNo 
END
GO
