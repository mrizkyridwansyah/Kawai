SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create procedure [sp_Wms_NG_GetDetail]
	@NGCode varchar(25)
as
begin
	select 
		wh.NGCode, wh.Description, wh.IsCommon, wh.LastUpdate LastUpdate, us.FullName Lastuser 
	From MS_NG wh
	left join vw_User us on wh.Lastuser = us.UserID
	 
	where wh.NGCode = @NGCode
end
GO
