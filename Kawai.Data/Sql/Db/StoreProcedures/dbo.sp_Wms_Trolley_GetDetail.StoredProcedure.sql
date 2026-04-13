SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [sp_Wms_Trolley_GetDetail]
	@TrolleyCode varchar(25)
as
begin
	select 
		wh.TrolleyCode, wh.Description, wh.Trolley_Cls, C.Description Trolley_ClsDescs, wh.IsActive, wh.LastUpdate LastUpdate, us.FullName Lastuser 
	From MS_Trolley wh
	left join vw_User us on wh.Lastuser = us.UserID
	Left Join Trolley_Cls c ON c.Trolley_Cls = wh.Trolley_Cls
	 
	where wh.TrolleyCode = @TrolleyCode
end
GO
