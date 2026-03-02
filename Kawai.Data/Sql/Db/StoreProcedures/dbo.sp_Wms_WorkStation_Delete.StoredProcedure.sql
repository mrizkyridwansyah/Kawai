SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_WorkStation_Delete]
	@WorkStationCode varchar(25)
as
begin
	if not exists (select 1 from MS_WorkStation where WorkStationCode = @WorkStationCode)
	begin
		raiserror('WorkStation Code didn''t Exists',16,1)
		return;
	end

	if exists (select 1 from WorkStationLineSetting where WorkStationCode = @WorkStationCode)
	begin
		raiserror('Data Workstation already used as reference data',16,1)
		return;
	end

	if exists (select 1 from MS_BOMPerworkstation_Header where WorkStationCode = @WorkStationCode)
	begin
		raiserror('Data Workstation already used as reference data',16,1)
		return;
	end

	delete from  MS_WorkStation where WorkStationCode = @WorkStationCode
end
GO
