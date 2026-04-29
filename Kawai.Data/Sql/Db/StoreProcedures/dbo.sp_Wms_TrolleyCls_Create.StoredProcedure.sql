




CREATE PROCEDURE [dbo].[sp_Wms_TrolleyCls_Create]
	@Trolley_Cls varchar(25),
	@Description varchar(200),
	@Qty Int,
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from Trolley_Cls where Trolley_Cls = @Trolley_Cls)
	begin
		raiserror('Trolley Cls Already Exists',16,1)
		return;
	end

	 

	insert into Trolley_Cls( Trolley_Cls, Description,Qty,  RegisterDate,RegisterUser, Lastuser,LastUpdate)
	values (@Trolley_Cls, @Description,@Qty,    getdate(),@RegisterBy, @RegisterBy, getdate())
end
