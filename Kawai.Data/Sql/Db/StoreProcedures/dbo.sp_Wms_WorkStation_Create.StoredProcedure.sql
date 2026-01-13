SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_WorkStation_Create]
	@WorkStationCode varchar(25),
	@WorkStationName varchar(200),
 
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from MS_WorkStation where WorkStationCode = @WorkStationCode)
	begin
		raiserror('WorkStation Code Already Exists',16,1)
		return;
	end

	 

	insert into MS_WorkStation(WorkStationCode, WorkStationName,  RegisterUser,RegisterDate)
	values (@WorkStationCode, @WorkStationName,  @RegisterBy, getdate())
end
GO
