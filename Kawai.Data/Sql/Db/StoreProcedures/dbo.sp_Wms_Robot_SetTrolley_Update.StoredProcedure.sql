SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [sp_Wms_Robot_SetTrolley_Update]
	@RequestID varchar(50),
	@TrolleyNo varchar(25),
	@RobotCode varchar(50)=''
AS
BEGIN
	if not exists (select * From PartMaterialRequestSendRobotHeader where RequestSendID = @RequestID)	
	begin
		raiserror('Data Request tidak ditemukan!',16,1)
		return
	end

	if exists (select * From PartMaterialRequestDetail where RefNumber = @RequestID and isnull(Trolley_No, @TrolleyNo) <> @TrolleyNo)	
	begin
		raiserror('Data Request sudah di set dengan Trolley berbeda!',16,1)
		return
	end

	update PartMaterialRequestDetail set Trolley_No = @TrolleyNo, LastUpdate = GETDATE() --, LastUser = @RobotCode 
	where RefNumber = @RequestID and Trolley_No IS NULL
END
GO
