SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_User_Capture]
	@UserID varchar(25)
as
select * From SS_UserSetup where UserID = @UserID
GO
