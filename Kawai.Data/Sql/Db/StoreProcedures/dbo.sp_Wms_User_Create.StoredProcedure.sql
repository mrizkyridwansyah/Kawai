SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [sp_Wms_User_Create]
    @UserID Varchar(25),
    @FullName varchar(200),
    @Password Nvarchar(200),
	@AdminStatus Bit,
    @JobPosition Varchar(25),
    @UserGroup Varchar(25),
	@UserPhoto varchar(max) = '',
    @RegisterBy Varchar(25)

as
begin
	if exists (select 1 from SS_UserSetup where UserID = @UserID)
	begin
		raiserror('User ID Already Exists',16,1)
		return;
	end

	insert into SS_UserSetup (UserID,FullName,[Password],StatusAdmin,JobPosition,UserGroup, UserPhoto,RegisterUser, registerdate)
	values (@UserID,@FullName,@Password,@AdminStatus,@JobPosition,@UserGroup, @UserPhoto, @RegisterBy, getdate())
end
GO
