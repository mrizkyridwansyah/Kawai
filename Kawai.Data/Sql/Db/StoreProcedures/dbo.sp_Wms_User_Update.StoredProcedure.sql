SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



create   procedure [sp_Wms_User_Update]
    @UserID Varchar(25),
    @FullName varchar(200),
    @Password Nvarchar(200),
	@AdminStatus Bit,
    @JobPosition Varchar(25),
    @UserGroup Varchar(25),
	@UserPhoto varchar(max) = '',
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from SS_UserSetup where UserID = @UserID)
	begin
		raiserror('User ID didn''t Exists',16,1)
		return;
	end

	update SS_UserSetup set FullName = @FullName ,UpdateUser = @UpdateBy , UpdateDate = Getdate(),
	[Password] = @Password , StatusAdmin = @AdminStatus , JobPosition = @JobPosition , UserGroup = @UserGroup, UserPhoto = @UserPhoto
	where UserID = @UserID
end
GO
