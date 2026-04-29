




create   procedure [dbo].[sp_Wms_Item_DDLByStock]
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@AddressCode	varchar(25),
	@Category		varchar(25),
	@StatusReceipt	varchar(25),
	@StatusHoldNG	varchar(50)
as
begin
	select distinct a.ItemCode, b.Item_Name ItemName, RTRIM(Item_Code) + ' | ' + Item_Name DDLDescription 
	From StockDetail a
	inner join Item_Master b on a.ItemCode = b.Item_Code
	where 1=1
	and (a.ItemCode like '%'+ @Keyword +'%' or b.Item_Name like '%'+ @Keyword +'%')
	and (@WarehouseCode = 'ALL' or WarehouseCode = @WarehouseCode)
	and (@AreaCode = 'ALL' or AreaCode = @AreaCode)
	and (@AddressCode = 'ALL' or AddressCode = @AddressCode)
	and (@Category = 'ALL' or ClasificationPart_Cls = @Category)
	and Qty > 0
	and (@StatusReceipt = 'ALL' or a.StatusReceipt = @StatusReceipt)
	and (@StatusHoldNG = 'ALL' or isnull(a.StatusHoldNG, '') = @StatusHoldNG)
end
