



create   procedure [dbo].[sp_Wms_StockInquiry_DDLLotNoByStock]
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@AddressCode	varchar(25),
	@ItemCode		varchar(25),
	@Category		varchar(25),
	@StatusReceipt	varchar(25),
	@StatusHoldNG	varchar(50)
as
begin
	select distinct a.LotNo From StockDetail a
	inner join 
	(
		select * from Item_Master where (@Category = 'ALL' or ClasificationPart_Cls = @Category)
	) mi on a.ItemCode = mi.Item_Code
	where 1=1
	and a.LotNo like '%' + @Keyword + '%'
	and (@WarehouseCode = 'ALL' or a.WarehouseCode = @WarehouseCode)
	and (@AreaCode = 'ALL' or a.AreaCode = @AreaCode)
	and (@AddressCode = 'ALL' or a.AddressCode = @AddressCode)
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
	and (@StatusReceipt = 'ALL' or a.StatusReceipt = @StatusReceipt)
	and (@StatusHoldNG = 'ALL' or isnull(a.StatusHoldNG, '') = @StatusHoldNG)
end
