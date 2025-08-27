SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_NG_Update]
	@NGCode varchar(25),
	@Description varchar(200),
	@IsCommon Bit,
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from MS_NG where NGCode = @NGCode)
	begin
		raiserror('NG Code didn''t Exists',16,1)
		return;
	end

	 

	update MS_NG 
	set 
		Description = @Description, 
		IsCommon = @IsCommon,
		Lastuser = @UpdateBy, 
		LastUpdate = getdate() 
	where NGCode = @NGCode
end
GO
