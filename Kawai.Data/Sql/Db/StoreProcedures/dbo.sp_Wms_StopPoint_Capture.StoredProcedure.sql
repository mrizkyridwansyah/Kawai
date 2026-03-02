SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   PROCEDURE [sp_Wms_StopPoint_Capture]
	@StopPointCode varchar(25)
as
select * From MS_StopPoint where StopPointCode = @StopPointCode
GO
