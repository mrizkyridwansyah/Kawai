SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_StopPoint_Create]
	@StopPointCode varchar(25),
	@Description varchar(200),
	@IsActive Bit,
	@PickingSeq Numeric(18,0),
	@RegisterBy varchar(25)
as
begin
declare @Msg varchar(max) = ''
	if exists (select 1 from MS_StopPoint where StopPointCode = @StopPointCode)
	begin
		raiserror('StopPoint Code Already Exists',16,1)
		return;
	end

	if exists (select 1 from MS_StopPoint where PickingSeq = @PickingSeq)
	begin
	    Set @Msg = 'Picking Sequence ' + cast(@PickingSeq as varchar) + ' Already Exists'
		raiserror(@Msg,16,1)
		return;
	end

	 

	insert into MS_StopPoint(StopPointCode, Description, IsActive, PickingSeq,  Lastuser,LastUpdate)
	values (LTRIM(RTRIM(@StopPointCode)), @Description, @IsActive, @PickingSeq,  @RegisterBy, getdate())
end
GO
