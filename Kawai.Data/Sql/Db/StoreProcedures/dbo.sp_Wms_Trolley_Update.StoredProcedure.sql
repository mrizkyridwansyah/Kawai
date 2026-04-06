SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_Trolley_Update]
	@TrolleyCode varchar(25),
	@Description varchar(200),
	@Trolley_Cls varchar(25),
	@IsActive Bit,
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from MS_Trolley where TrolleyCode = @TrolleyCode)
	begin
		raiserror('Trolley Code didn''t Exists',16,1)
		return;
	end

	 

	update MS_Trolley 
	set 
		Description = @Description, 
		IsActive = @IsActive,
		Trolley_Cls = @Trolley_Cls,
		 Lastuser = @UpdateBy, 
		LastUpdate = getdate() 
	where TrolleyCode = @TrolleyCode
end
GO
