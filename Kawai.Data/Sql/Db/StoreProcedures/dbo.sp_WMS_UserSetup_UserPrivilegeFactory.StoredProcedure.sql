SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create   procedure [sp_WMS_UserSetup_UserPrivilegeFactory]
	@UserID varchar(25)
as 

select 
	a.Company_Code FactoryCode, a.Company_Name FactoryName, b.AllowAccess
From Company_Profile a
left join 
(
	select * From SS_UserFactoryPrivilege where UserID = @UserID
) b on a.Company_Code = b.FactoryCode
GO
