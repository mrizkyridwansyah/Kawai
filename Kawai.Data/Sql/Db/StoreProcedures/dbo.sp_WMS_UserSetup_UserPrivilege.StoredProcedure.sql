SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_WMS_UserSetup_UserPrivilege]
	@UserID varchar(25)
as 

select 
	a.MenuID, MenuName, MenuDescription, MenuGroup, SubGroup, GroupIndex, MenuIndex, SubGroupIndex, ImageName,
	b.AllowAccess, b.AllowUpdate, b.AllowPrice
From SS_UserMenu a
left join 
(
	select * From SS_UserPrivilege where UserID = @UserID
) b on a.MenuID = b.MenuID
GO
