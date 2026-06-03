
CREATE procedure [dbo].[sp_Wms_Receipt_Delete]
	@Id				bigint 
	 
as
begin
        DELETE FROM Part_Receipt WHERE RefWMSReceiptId = @Id
	    DELETE FROM PartReceiptDetailBarcode where ReceiptId = @Id
		DELETE FROM PartReceiptDetail WHERE ReceiptId = @Id
		DELETE FROM PartReceiptHeader WHERE Id = @Id
	 
		
	 
	

	 
end