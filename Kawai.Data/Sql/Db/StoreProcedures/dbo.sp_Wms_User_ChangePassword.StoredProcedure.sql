SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_User_ChangePassword]
	@UserID varchar(25),
	@Password varchar(max)
as
begin

	update SS_UserSetup set Password = @Password, UpdateDate = getdate(), UpdateUser = @UserID where UserID = @UserID
end
GO
