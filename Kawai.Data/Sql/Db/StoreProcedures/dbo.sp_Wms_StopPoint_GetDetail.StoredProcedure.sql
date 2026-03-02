SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [sp_Wms_StopPoint_GetDetail]
	@StopPointCode varchar(25)
as
begin
	select 
		wh.StopPointCode, wh.Description, wh.IsActive, wh.PickingSeq, wh.LastUpdate LastUpdate, us.FullName Lastuser 
	From MS_StopPoint wh
	left join vw_User us on wh.Lastuser = us.UserID
	 
	where wh.StopPointCode = @StopPointCode
end
GO
