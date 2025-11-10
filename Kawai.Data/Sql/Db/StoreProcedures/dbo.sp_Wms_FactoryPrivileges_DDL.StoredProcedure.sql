SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_FactoryPrivileges_DDL]
	@UserId varchar(25) = '',
	@Keyword varchar(max)
as
begin
	select Company_Code CompanyCode, Company_Name CompanyName
	from Company_Profile a
	inner join SS_UserFactoryPrivilege b on a.Company_Code = b.FactoryCode
	where 1=1
	and b.UserID = @UserId
	and b.AllowAccess = 1
end
GO
