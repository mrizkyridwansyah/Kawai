

create   procedure [dbo].[sp_Wms_Receipt_DataHeader]
	@ReceiptId bigint
as
begin
	declare @minDeliveryDate date, @maxDeliveryDate date, @countPO int, @firstPO varchar(100)
	select @minDeliveryDate = min(Delivery_Date), @maxDeliveryDate = max(Delivery_Date), @countPO = count(distinct a.PONumber), @firstPO = max(PONumber) From PartReceiptDetail a 
	inner join PurchaseOrder_Detail b on a.ItemCode = b.Item_Code and a.PONumber = b.PO_No
	where ReceiptId = @ReceiptId

	SELECT
		a.Id,
		a.ReceiptNo, 
		a.ReceiptDate,
		a.DNNumber,
		a.CompanyCode FactoryCode,
		fak.Company_Name FactoryName,
		a.SupplierCode,
		b.Trade_Name AS SupplierName,
		a.DNDate,
		a.BCNumber,
		a.BCType,
		a.BCDate,
		a.VehicleNo,
		a.Transport,
		a.ReferenceNo,
		a.Remarks,
		@minDeliveryDate [DeliveryDatePOFrom],
		@maxDeliveryDate [DeliveryDatePOUntil],
		case when @countPO > 1 then 'ALL' else @firstPO end [PONumber],
		StatusReceipt,
		a.LastUpdate,
		c.FullName LastUser,
		a.RegisterNo
	FROM PartReceiptHeader a
	LEFT JOIN trade_master b ON a.SupplierCode = b.Trade_Code
	left join ss_usersetup c on isnull(a.LastUser, a.RegisterUser) = c.UserID	
	left join Company_Profile fak on a.CompanyCode = fak.Company_Code
	WHERE a.Id = @ReceiptId
end
