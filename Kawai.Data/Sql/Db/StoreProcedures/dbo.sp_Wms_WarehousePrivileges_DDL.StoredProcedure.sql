SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create   procedure [sp_Wms_WarehousePrivileges_DDL]
	@FactoryCode varchar(25),
	@UserId varchar(25) = '',
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(a.WH_Code) WarehouseCode, a.WH_Name WarehouseName
	From WareHouse_Master a
	inner join
	(
		select WarehouseCode From SS_UserWarehousePrivilege where UserID = @UserId and AllowAccess = 1	
	) priv on a.WH_Code = priv.WarehouseCode
	where 1=1
	and (@FactoryCode = 'ALL' or Company_Code = @FactoryCode)
	and WH_Name like '%'+ @Keyword +'%'

end

GO
