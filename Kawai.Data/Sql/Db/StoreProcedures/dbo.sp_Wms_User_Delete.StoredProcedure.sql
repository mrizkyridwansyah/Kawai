SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [sp_Wms_User_Delete]
	@UserID varchar(25)
as
begin
	if not exists (select 1 from SS_UserSetup where UserID = @UserID)
	begin
		raiserror('User ID didn''t Exists',16,1)
		return;
	end

	delete from SS_UserSetup where UserID = @UserID
end
GO
