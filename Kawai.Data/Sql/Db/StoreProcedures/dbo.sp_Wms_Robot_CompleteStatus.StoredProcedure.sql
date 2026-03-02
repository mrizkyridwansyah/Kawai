SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [sp_Wms_Robot_CompleteStatus]
	@RequestID varchar(50),
	@TrolleyNo varchar(25),
	@StopPoint varchar(25)='',
	@Status		int=1
AS
BEGIN
	if not exists (select * From PartMaterialRequestSendRobotHeader where RequestSendID = @RequestID)	
	begin
		raiserror('Data Request tidak ditemukan!',16,1)
		return	
	end

	if not exists(select * from PartMaterialRequestDetail where RefNumber=@RequestID and Trolley_No=@TrolleyNo )
	begin
		raiserror('Trolley No not found',16,1)
		return
	end

	if exists (select * From PartMaterialRequestSendRobotDetail where RequestSendID = @RequestID and coalesce(Status,'0')='1')
	begin
		raiserror('Status sudah completed',16,1)
		return
	end

	UPDATE PartMaterialRequestSendRobotDetail 
	SET Status = '1', 
		LastUpdate = GETDATE() --, LastUser = @RobotCode 
	WHERE RequestSendID = @RequestID and Stop_Point=@StopPoint and coalesce(Status,'0')='0'
END
GO
