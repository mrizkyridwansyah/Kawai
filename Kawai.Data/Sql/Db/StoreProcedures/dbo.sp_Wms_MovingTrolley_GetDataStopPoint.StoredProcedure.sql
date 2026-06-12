CREATE procedure [dbo].[sp_Wms_MovingTrolley_GetDataStopPoint]
	@StopPoint varchar(25)
as
begin
	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPoint)
	begin
		raiserror('Data Stop Point tidak ditemukan!', 16, 1)
		return
	end

	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPoint and IsActive = 1)
	begin
		raiserror('Data Stop Point tidak aktif!', 16, 1)
		return
	end

	SELECT
		StopPointCode, [Description]
	FROM MS_StopPoint	
	WHERE StopPointCode = @StopPoint
	and IsActive = 1
end


