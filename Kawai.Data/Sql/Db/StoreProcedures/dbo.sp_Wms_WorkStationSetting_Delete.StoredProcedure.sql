SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_WorkStationSetting_Delete]
	@LineCode varchar(100)
as 

delete from WorkStationLineSetting where LineCode = @LineCode 

GO
