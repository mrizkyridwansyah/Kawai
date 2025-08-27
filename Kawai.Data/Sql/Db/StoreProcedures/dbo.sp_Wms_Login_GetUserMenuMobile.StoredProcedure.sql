SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Login_GetUserMenuMobile]
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
			MenuIndex  ,
			ImageName,
			cast(1 as bit) AllowAccess
		FROM SS_UserMenuMobile 
		ORDER BY MenuIndex
	end 
	else 
	begin
		SELECT 
			B.MenuID,
			B.MenuName AS MenuName,
			B.MenuDescription,
			B.MenuIndex  ,
			B.ImageName,
			A.AllowAccess
		FROM [dbo].[SS_UserMobilePrivilege] A LEFT JOIN SS_UserMenuMobile B ON A.MenuID = B.MenuID
		WHERE A.UserID = @UserId AND A.AllowAccess = 1
		ORDER BY MenuIndex
	end

END
GO
