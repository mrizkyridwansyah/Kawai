SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [sp_Wms_Reprint_Update] 
	 
	@BarcodeNo varchar(max) = '',
	@Source varchar(max),
	@UserID varchar(max) 
as
begin
	 
	 If  @Source = 'Receipt'
	 Begin
	 Update PartReceiptDetailBarcode Set PrintStatus = NULL,PrintDate = NULL,PrintUser = NULL  where BarcodeNo = @BarcodeNo
	 End

	 If @Source = 'BarcodeSplit'
	 Begin
	 Update Barcode_Split Set Print_Cls = NULL Where BarcodeNo = @BarcodeNo
	 End
		      

end
GO
