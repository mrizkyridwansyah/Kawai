SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Notifications_SaveToUserAll]
	@Title varchar(100), 
	@Description varchar(max), 
	@NotifType varchar(10), 
	@Priority varchar(10), 
	@Receiver varchar(25), 
	@Sender varchar(25), 
	@UrlRedirect varchar(max)
as
begin
	insert into [Notification] (Title, [Description], NotifType, [Priority], Receiver, Sender, HasSeen, UrlRedirect, RegisterDate)
	select @Title, @Description, @NotifType, @Priority, UserID, @Sender, 0, @UrlRedirect, getdate() From ss_usersetup
end
GO
