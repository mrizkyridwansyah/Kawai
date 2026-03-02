SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_Trolley_Create]
	@TrolleyCode varchar(25),
	@Description varchar(200),
	@Trolley_Cls varchar(25),
	@IsActive Bit,
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from MS_Trolley where TrolleyCode = @TrolleyCode)
	begin
		raiserror('Trolley Code Already Exists',16,1)
		return;
	end

	 

	insert into MS_Trolley(TrolleyCode, Description,Trolley_Cls, IsActive,  RegisterDate,RegisterUser, Lastuser,LastUpdate)
	values (@TrolleyCode, @Description,@Trolley_Cls, @IsActive,   getdate(),@RegisterBy, @RegisterBy, getdate())
end
GO
