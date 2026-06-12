CREATE procedure sp_Wms_Mobile_ManualTrolleyAssign_GetListDetailRequestAMR
	@RequestNo varchar(100)
as
begin
	declare @tblRequestDetail table (RequestNo varchar(25), TrolleyNo varchar(25), IsCurrentProcessManual bit)

	insert into @tblRequestDetail
	select top 1 RefNumber, Trolley_No, IsCurrentProcessManual from PartMaterialRequestDetail where RefNumber = @RequestNo

	select 
		RequestNo = dtl.RequestNo,
		TrolleyNo = dtl.TrolleyNo,
		dtl.IsCurrentProcessManual,
		StopPoint = rdtl.Stop_Point,
		StopPointDesc = msp.[Description],
		rdtl.StatusAMR,
		IsCompleteLoading = case when rdtl.Status = 1 then cast(1 as bit) else cast(0 as bit) end,
		us.FullName LastUserRequestAMR, 
		rdtl.LastRequestDateAMR
	From @tblRequestDetail dtl
	left join PartMaterialRequestSendRobotDetail rdtl on dtl.RequestNo = rdtl.RequestSendID
	left join MS_StopPoint msp on rdtl.Stop_Point = msp.StopPointCode
	left join SS_UserSetup us on rdtl.LastUserRequestAMR = us.UserID
end