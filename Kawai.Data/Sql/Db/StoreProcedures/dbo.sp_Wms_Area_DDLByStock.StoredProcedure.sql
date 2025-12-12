SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Area_DDLByStock]
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@ItemCode		varchar(25)
as
begin
	select * From 
	(
		select 'TMP' AreaCode, 'Temporary' AreaName, 'TMP | Temporary' DDLDescription
		where isnull(@WarehouseCode, '') <> ''
		union all
		select distinct a.AreaCode, b.AreaName , a.AreaCode +' | '+ b.AreaName DDLDescription
		From StockDetail a
		inner join MS_Area b on a.AreaCode = b.AreaCode
		where 1=1
		and (@WarehouseCode = 'ALL' or a.WarehouseCode = @WarehouseCode)
		and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
		and Qty > 0
	) res
	where 1=1
	and (AreaCode like '%' + @Keyword + '%' or AreaName like '%' + @Keyword + '%')
end
GO
