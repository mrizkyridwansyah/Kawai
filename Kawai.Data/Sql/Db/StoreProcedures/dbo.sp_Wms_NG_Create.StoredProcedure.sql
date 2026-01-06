SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   procedure [sp_Wms_NG_Create]
	@NGCode varchar(25),
	@Description varchar(200),
	@IsCommon Bit,
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from MS_NG where NGCode = @NGCode)
	begin
		raiserror('NG Code Already Exists',16,1)
		return;
	end

	 

	insert into MS_NG(NGCode, Description, IsCommon,  Lastuser,LastUpdate)
	values (@NGCode, @Description, @IsCommon,  @RegisterBy, getdate())
end
GO
