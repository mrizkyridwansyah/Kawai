SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_StockInquiry_DDLLotNoByStock]
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@AddressCode	varchar(25),
	@ItemCode		varchar(25)
as
begin
	select distinct a.LotNo From StockDetail a
	where 1=1
	and a.LotNo like '%' + @Keyword + '%'
	and (@WarehouseCode = 'ALL' or a.WarehouseCode = @WarehouseCode)
	and (@AreaCode = 'ALL' or a.AreaCode = @AreaCode)
	and (@AddressCode = 'ALL' or a.AddressCode = @AddressCode)
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
end
GO
