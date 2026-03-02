SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [sp_Wms_Robot_CompleteStatus_Capture]
	@RequestID varchar(50),
	@TrolleyNo varchar(25),
	@StopPoint varchar(25)=''
AS
BEGIN
	
	SELECT * FROM PartMaterialRequestSendRobotDetail
	WHERE RequestSendID = @RequestID and Stop_Point=@StopPoint 
END
GO
