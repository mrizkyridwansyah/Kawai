SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [sp_Wms_TrolleyCls_GetDetail]
	@Trolley_Cls varchar(25)
as
begin
	select 
		wh.Trolley_Cls , wh.Description,   wh.Qty, wh.LastUpdate LastUpdate, us.FullName Lastuser 
	From Trolley_Cls wh
	left join vw_User us on wh.Lastuser = us.UserID
 	 
	where wh.Trolley_Cls = @Trolley_Cls
end
GO
