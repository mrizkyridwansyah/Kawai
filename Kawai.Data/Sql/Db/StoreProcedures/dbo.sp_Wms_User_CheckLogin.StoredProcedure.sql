SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_User_CheckLogin]
	@UserID varchar(25) = '',
	@Password varchar(max) = ''
as
begin
	if not exists (select * From SS_UserSetup where UserID = @UserID and [Password] = @Password)
	begin
		raiserror('Username or Password is invalid!',16,1)
		return
	end

	select 
		us.UserID, us.FullName, cp.Description [JobPosition], us2.UserID [UserGroupID], isnull(us2.FullName, '') [UserGroupName], 
		case when isnull(us.StatusAdmin, 0) = 1 then cast(1 as bit) else cast(0 as bit) end [IsAdmin], UserPhoto = isnull(us.UserPhoto, '')
	From SS_UserSetup us
	left join Cls_Parameter cp on us.JobPosition = cp.Code
	left join SS_UserSetup us2 on us.UserGroup = us2.UserID
	where us.UserID = @UserID and us.[Password] = @Password
end
GO
