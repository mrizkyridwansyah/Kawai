SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Address_DDL]
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = '',
	@AreaCode varchar(25) = ''
as
begin
	select 
		ma.AddressCode, ma.AddressName, ma.AddressCode +' | '+ ma.AddressName DDLDescription
	From MS_Address ma
	where 1=1
	and 1 = case when isnull(@WarehouseCode, '') = 'ALL' or ma.WarehouseCode = @WarehouseCode then 1 else 0 end
	and 1 = case when isnull(@AreaCode, '') = 'ALL' or ma.AreaCode = @AreaCode then 1 else 0 end
	and (ma.AddressCode like '%'+ @Keyword +'%' or ma.AddressName like '%'+ @Keyword +'%')
end

GO
