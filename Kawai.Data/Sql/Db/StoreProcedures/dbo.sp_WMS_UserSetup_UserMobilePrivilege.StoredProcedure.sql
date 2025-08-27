SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_WMS_UserSetup_UserMobilePrivilege]
	@UserID varchar(25)
as 

select 
	a.MenuID, MenuName, MenuDescription, MenuIndex, ImageName, b.AllowAccess
From SS_UserMenuMobile a
left join 
(
	select * From SS_UserMobilePrivilege where UserID = @UserID
) b on a.MenuID = b.MenuID
GO
