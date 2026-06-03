
create   procedure [dbo].[sp_Wms_Mobile_LoadingTrolley_GetListRouteTrolley]
	@TrolleyNo varchar(50) --= 'SH-SM01-TR001'
as
begin
	DECLARE @RequestSendID varchar(100), @IsCurrentProcessManual bit

	SELECT 
		top 1 @RequestSendID = RefNumber, @IsCurrentProcessManual = IsCurrentProcessManual
	FROM PartMaterialRequestDetail 
	WHERE ISNULL(Trolley_No, '') = @TrolleyNo 
	ORDER BY RegisterDate DESC

	if @RequestSendID  is null and isnull(@TrolleyNo, '') <> ''
	begin
		raiserror('Data Request Trolley tidak ditemukan!', 16, 1)
		return;
	end

	select 
		dtl.RequestSendID, 
		dtl.Stop_Point StopPoint, 
		msp.Description StopPointDesc, 
		@TrolleyNo TrolleyNo, 
		StatusAMR = isnull(dtl.StatusAMR, ''), 
		IsComplete = case when isnull(dtl.StatusAMR, '') <> 'Completed' then cast(0 as bit) else cast(1 as bit) end, 
		IsManual = @IsCurrentProcessManual,
		dtl.LastUserRequestAMR, 
		us.FullName LastUserNameRequestAMR, 
		dtl.LastRequestDateAMR
	From PartMaterialRequestSendRobotDetail dtl
	left join MS_StopPoint msp on dtl.Stop_Point = msp.StopPointCode
	left join SS_UserSetup us on dtl.LastUserRequestAMR = us.UserID
	where dtl.RequestSendID = @RequestSendID --and isnull(dtl.[Status], 0) = 1 

end
