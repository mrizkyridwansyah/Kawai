SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   PROCEDURE [sp_Wms_WorkStation_Capture]
	@WorkStationCode varchar(25)
as
select * From MS_WorkStation where WorkStationCode = @WorkStationCode
GO
