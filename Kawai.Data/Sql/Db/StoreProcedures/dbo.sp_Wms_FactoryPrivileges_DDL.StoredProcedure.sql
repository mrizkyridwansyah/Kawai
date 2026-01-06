SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_FactoryPrivileges_DDL]
	@UserId varchar(25) = '',
	@Keyword varchar(max)
as
begin
	select Company_Code CompanyCode, Company_Name CompanyName, Company_Code + ' | ' + Company_Name DDLDescription
	from Company_Profile a
	inner join SS_UserFactoryPrivilege b on a.Company_Code = b.FactoryCode
	where 1=1
	and b.UserID = @UserId
	and b.AllowAccess = 1
	and (a.Company_Code like '%'+ @Keyword +'%' or a.Company_Name like '%'+ @Keyword +'%')
end
GO
