SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create   procedure [sp_Wms_Address_DDLByStock]
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@ItemCode		varchar(25),
	@StatusReceipt	varchar(25),
	@StatusHoldNG	varchar(50)
as
begin
	select distinct a.AddressCode, b.AddressName, a.AddressCode +' | '+ b.AddressName DDLDescription 
	From StockDetail a
	inner join MS_Address b on a.AddressCode = b.AddressCode
	where 1=1
	and a.WarehouseCode = @WarehouseCode
	and a.AreaCode = @AreaCode
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
	and (@StatusReceipt = 'ALL' or a.StatusReceipt = @StatusReceipt)
	and (@StatusHoldNG = 'ALL' or isnull(a.StatusHoldNG, '') = @StatusHoldNG)
	and (a.AddressCode like '%'+ @Keyword +'%' or b.AddressName like '%'+ @Keyword +'%')

end
GO
