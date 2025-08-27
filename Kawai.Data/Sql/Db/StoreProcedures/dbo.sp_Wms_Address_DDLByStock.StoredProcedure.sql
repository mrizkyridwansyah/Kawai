SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Address_DDLByStock]
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@ItemCode		varchar(25)
as
begin
	select distinct a.AddressCode, b.AddressName From StockDetail a
	inner join MS_Address b on a.AddressCode = b.AddressCode
	where 1=1
	and a.WarehouseCode = @WarehouseCode
	and a.AreaCode = @AreaCode
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
end
GO
