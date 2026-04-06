SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [sp_Wms_TrolleyCls_Create]
	@TrolleyCls varchar(25),
	@Description varchar(200),
	@Qty Int,
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from Trolley_Cls where Trolley_Cls = @TrolleyCls)
	begin
		raiserror('Trolley Cls Already Exists',16,1)
		return;
	end

	 

	insert into Trolley_Cls( Trolley_Cls, Description,Qty,  RegisterDate,RegisterUser, Lastuser,LastUpdate)
	values (@TrolleyCls, @Description,@Qty,    getdate(),@RegisterBy, @RegisterBy, getdate())
end
GO
