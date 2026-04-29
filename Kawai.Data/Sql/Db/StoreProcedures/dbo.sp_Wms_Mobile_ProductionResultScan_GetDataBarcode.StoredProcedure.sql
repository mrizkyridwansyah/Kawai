
CREATE   PROCEDURE [dbo].[sp_Wms_Mobile_ProductionResultScan_GetDataBarcode]
	@BarcodeNo nvarchar(50)
AS
BEGIN

	--DECLARE @BarcodeNo NVARCHAR(50)='101J0170J10000-G099657'
	
	
	DECLARE @SerialNo NVARCHAR(25)
	
	SELECT TOP 1 @SerialNo=PartValue FROM dbo.fn_ProductionResult_GetSerialNo(@BarcodeNo)
	WHERE PartNo=2
		

	IF NOT EXISTS 
	(
		SELECT * FROM Daily_Production 
		WHERE Complete_Cls is null --and Item_code='101J0170'
		and @SerialNo>=SerialNoFrom and @SerialNo<=SerialNoTo
	)
	BEGIN 
		RAISERROR('Serial No not found in daily production!',16,1)
		RETURN
	END

	IF EXISTS(SELECT TOP 1 1 FROM ProductionResultDetail WHERE BarcodeNo=@BarcodeNo)
	BEGIN
		RAISERROR('Barcode has been scanned',16,1)
		RETURN
	END


	SELECT @BarcodeNo AS BarcodeNo,
		   RTRIM(A.Item_code) ItemCode, RTRIM(b.Item_Name) ItemName,
		   RTRIM(a.Lot_No) LotNo,
		   RTRIM(a.Line_Code) LineCode ,RTRIM(C.Line_Name) LineName,
		   Qty = 1
	FROM Daily_Production a
	LEFT JOIN Item_Master B ON B.Item_Code=A.Item_code
	LEFT JOIN Manufacture_Line C ON C.Line_Code=A.Line_Code
	WHERE Complete_Cls is null 
	and @SerialNo>=SerialNoFrom and @SerialNo<=SerialNoTo
	
end
