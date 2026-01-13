SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_WorkStationSetting_Capture]
	@LineCode varchar(100)
as 

select LineCode, WorkStationCode From WorkStationLineSetting where LineCode = @LineCode
GO
