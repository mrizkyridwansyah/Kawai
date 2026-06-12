

CREATE   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_SendRequestCancelAMR]
	@RequestNo varchar(100),
	@TrolleyNo varchar(50),
	@UserId varchar(25)	
as
begin
	insert into AMRRequestHistory (RequestNo, FromData, ToData, [Action], SourceAction, StatusAMR, RegisterDate, RegisterUser)
	values (@RequestNo, @TrolleyNo, @TrolleyNo, 'CANCEL AMR FROM WMS', 'sp_Wms_Mobile_ManualTrolleyAssign_SendRequestCancelAMR', 'Requesting to AMR', GETDATE(), @UserId)
end
