SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Login_GetUserMenu]
--declare
    @UserId VARCHAR(20) = 'ridwansyah2'
AS
BEGIN
	DECLARE @StatusAdmin bit = (SELECT StatusAdmin fROM SS_UserSetup where UserID = @UserId)

	if isnull(@StatusAdmin, 0) = 1
	begin
		SELECT 
			MenuID,
			MenuName AS MenuName,
			MenuDescription,
			MenuGroup,
			SubGroup,
			GroupIndex  ,
			MenuIndex  ,
			SubGroupIndex  ,
			ImageName,
			cast(1 as bit) AllowAccess,
			cast(1 as bit) AllowUpdate,
			cast(1 as bit) AllowPrice
		FROM SS_UserMenu 
		ORDER BY GroupIndex, SubGroupIndex, MenuIndex
	end 
	else 
	begin
		SELECT 
			B.MenuID,
			B.MenuName AS MenuName,
			B.MenuDescription,
			B.MenuGroup,
			B.SubGroup,
			B.GroupIndex  ,
			B.MenuIndex  ,
			B.SubGroupIndex  ,
			B.ImageName,
			A.AllowAccess,
			A.AllowUpdate,
			A.AllowPrice
		FROM [dbo].[SS_UserPrivilege] A LEFT JOIN SS_UserMenu B ON A.MenuID = B.MenuID
		WHERE A.UserID = @UserId AND A.AllowAccess = 1
		ORDER BY GroupIndex, SubGroupIndex, MenuIndex
	end

END
GO
