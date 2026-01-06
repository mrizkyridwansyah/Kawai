SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Notifications_CountUnread]
	@Receiver varchar(25) 
as
begin
	select count(1) from [Notification] where Receiver = @Receiver and isnull(HasSeen, 0) = 0
end
GO
