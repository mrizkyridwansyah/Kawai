
create   procedure [dbo].[sp_Wms_Robot_GetRequestData] 
	@RequestSendID varchar(100) ,--= '87WS0011',
	@StopPoint varchar(25)		 --= 'ST002'
as
begin
	select
		dtl.RequestSendID, 
		dtl.Stop_Point StopPoint, 
		msp.Description StopPointDesc, 
		dtlx.Trolley_No TrolleyNo, 
		dtl.StatusAMR, 
		IsComplete = case when isnull(dtl.StatusAMR, '') <> 'Completed' then cast(1 as bit) else cast(0 as bit) end, 
		dtl.LastUserRequestAMR, 
		us.FullName LastUserNameRequestAMR, 
		dtl.LastRequestDateAMR
	From PartMaterialRequestSendRobotDetail dtl
	inner join 
	(
		select distinct RefNumber, Trolley_No From PartMaterialRequestDetail
	) dtlx on dtl.RequestSendID = dtlx.RefNumber
	left join MS_StopPoint msp on dtl.Stop_Point = msp.StopPointCode
	left join SS_UserSetup us on dtl.LastUserRequestAMR = us.UserID
	where dtl.RequestSendID = @RequestSendID and dtl.Stop_Point = @StopPoint and isnull(dtl.[Status], 0) = 1 
end
