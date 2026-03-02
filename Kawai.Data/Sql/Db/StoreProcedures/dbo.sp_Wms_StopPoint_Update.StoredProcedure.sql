SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_StopPoint_Update]
	@StopPointCode varchar(25),
	@Description varchar(200),
	@IsActive Bit,
	@PickingSeq Numeric(18,0),
	@UpdateBy varchar(25)
as
begin
declare @Msg varchar(max) = ''
	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPointCode)
	begin
		raiserror('StopPoint Code didn''t Exists',16,1)
		return;
	end

	if exists (select 1 from MS_StopPoint where PickingSeq = @PickingSeq and StopPointCode <> @StopPointCode)
	begin
	    Set @Msg = 'Picking Sequence ' + cast(@PickingSeq as varchar) + ' Already Exists'
		raiserror(@Msg,16,1)
		return;
	end
	 

	update MS_StopPoint 
	set 
		Description = @Description, 
		IsActive = @IsActive,
	    PickingSeq = @PickingSeq,
		Lastuser = @UpdateBy, 
		LastUpdate = getdate() 
	where StopPointCode = @StopPointCode
end
GO
