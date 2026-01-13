SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_WorkStation_Update]
	@WorkStationCode varchar(15),
	@WorkStationName varchar(200),
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from MS_WorkStation where WorkStationCode = @WorkStationCode)
	begin
		raiserror('WorkStation Code didn''t Exists',16,1)
		return;
	end

	 

	update MS_WorkStation 
	set 
		WorkStationName = @WorkStationName, 
	 	Lastuser = @UpdateBy, 
		LastUpdate = getdate() 
	where WorkStationCode = @WorkStationCode
end
GO
