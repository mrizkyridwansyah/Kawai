SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Notifications_UpdateSeen]
	@Id int
as
begin
	update [Notification] set HasSeen = 1, SeenDate = getdate() where Id = @Id
end
GO
