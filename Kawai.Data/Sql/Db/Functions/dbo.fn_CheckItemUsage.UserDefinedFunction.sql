create FUNCTION [dbo].[fn_CheckItemUsage]
(
	@ItemCode varchar(25)
)
RETURNS BIT
as
begin

	IF EXISTS 
	(
		select top 1 1 From PartReceiptDetail where ItemCode = @ItemCode
		union all
		select top 1 1 From PartMaterialRequestItemDetail where ItemCode = @ItemCode
		union all
		select top 1 1 From PartMaterialRequestItemDetail_PO where ItemCode = @ItemCode
		union all
		select top 1 1 From PurchaseOrder_Detail where Item_Code = @ItemCode
		union all
		select top 1 1 From MS_BOMPerworkstation_Header where ParentItemCode = @ItemCode
		union all
		select top 1 1 From MS_BOMPerworkstation_Detail where ChildItem_Code = @ItemCode
		union all
		select top 1 1 From StockDetail where ItemCode = @ItemCode
		union all
		select top 1 1 From MaterialConsumptionDetail where MaterialCode = @ItemCode
		union all
		select top 1 1 From Barcode_Split where Item_Code = @ItemCode
		union all
		select top 1 1 From BarcodeNGDetail where ItemCode = @ItemCode
	) 
	BEGIN
		RETURN 0;
	END


	RETURN 1;

end
