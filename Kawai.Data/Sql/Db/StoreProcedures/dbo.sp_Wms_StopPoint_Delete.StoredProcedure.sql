SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_StopPoint_Delete]
	@StopPointCode varchar(25)
as
begin
	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPointCode)
	begin
		raiserror('StopPoint Code didn''t Exists',16,1)
		return;
	end
	if exists (select 1 from WorkStationLineSetting where StopPointCode = @StopPointCode)
	begin
		raiserror('this stopPoint code exists in workstation line setting',16,1)
		return;
	end

	if exists (select 1 from MS_Address where StopPointCode = @StopPointCode)
	begin
		raiserror('this stopPoint code exists in address',16,1)
		return;
	end
	delete from  MS_StopPoint where StopPointCode = @StopPointCode
end
GO
