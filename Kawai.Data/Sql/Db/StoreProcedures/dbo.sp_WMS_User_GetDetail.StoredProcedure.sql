SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--exec sp_Wms_User_GetDetail @UserID=N'2019001'
CREATE OR ALTER PROCEDURE [sp_WMS_User_GetDetail]
	@UserID varchar(25)
as
begin
	select 
		us.UserID, us.FullName, cp.Code [JobPositionCode], us2.UserID [UserGroupID], isnull(us2.FullName, '') [UserGroupName], 
		case when isnull(us.StatusAdmin, 0) = 1 then cast(1 as bit) else cast(0 as bit) end [IsAdmin] ,us.[Password], isnull(us.UserPhoto, '') ImageName
	From SS_UserSetup us
	left join Cls_Parameter cp on us.JobPosition = cp.Code
	left join SS_UserSetup us2 on us.UserGroup = us2.UserID
	where us.UserID = @UserID


end
GO
