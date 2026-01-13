SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [sp_Wms_WorkStation_GetDetail]
	@WorkStationCode varchar(25)
as
begin
	select 
		wh.WorkStationCode, wh.WorkStationName,  
		wh.LastUpdate LastUpdate, us.FullName LastUser 
			,wh.RegisterDate, usw.FullName RegisterUser
	From MS_WorkStation wh
	left join vw_User us on wh.Lastuser = us.UserID
	left join vw_User usw on wh.RegisterUser = usw.UserID
	 
	where wh.WorkStationCode = @WorkStationCode
end
GO
