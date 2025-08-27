SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Notifications_Get]
	@Receiver varchar(25) 
as
begin
	select 
		Id, Title, Description, NotifType, Priority, Receiver, Sender, HasSeen, UrlRedirect, TimeAgo = dbo.GetTimeAgo(RegisterDate)
	from [Notification] where Receiver = @Receiver and isnull(HasSeen, 0) = 0
	order by Id desc
end
GO
