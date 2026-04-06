SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE  PROCEDURE [sp_Wms_TrolleyCls_Update]
	@Trolley_Cls varchar(25),
	@Description varchar(200),
	@Qty Int,
	 @UpdateBy varchar(25)
as
begin
	if not exists (select 1 from Trolley_Cls where Trolley_Cls = @Trolley_Cls)
	begin
		raiserror('Trolley_Cls didn''t Exists',16,1)
		return;
	end

	 

	update Trolley_Cls 
	set 
		Description = @Description, 
		Qty = @Qty,
		 Lastuser = @UpdateBy, 
		LastUpdate = getdate() 
	where Trolley_Cls = @Trolley_Cls
end
GO
