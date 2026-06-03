

CREATE   PROCEDURE [sp_Wms_Robot_CompleteStatus]
	@RequestID varchar(50),
	@TrolleyNo varchar(25),
	@StopPoint varchar(25)='',
	@Status		int=1
AS
BEGIN
	if not exists (select 1 From PartMaterialRequestSendRobotHeader where RequestSendID = @RequestID)	
	begin
		raiserror('Data Request tidak ditemukan!',16,1)
		return	
	end

	if not exists(select 1 from PartMaterialRequestDetail where RefNumber=@RequestID and Trolley_No=@TrolleyNo )
	begin
		raiserror('Trolley No not found',16,1)
		return
	end

	if exists (select 1 From PartMaterialRequestSendRobotDetail where RequestSendID = @RequestID and coalesce(Status,'0')='1')
	begin
		raiserror('Status sudah completed',16,1)
		return
	end

	UPDATE PartMaterialRequestSendRobotDetail 
	SET Status = '1', 
		LastUpdate = GETDATE() --, LastUser = @RobotCode 
	WHERE RequestSendID = @RequestID and Stop_Point=@StopPoint and coalesce(Status,'0')='0'
END
