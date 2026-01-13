SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_PeriodSetting_Capture]
	@Year int
as

Select 
[Period]
,[Year]
,[Month]
,StartPeriod
,EndPeriod
,StartSO
,FinishSO from MS_PeriodSetting where [Year] = @Year and StartPeriod is NOT NULL

 

 
GO
