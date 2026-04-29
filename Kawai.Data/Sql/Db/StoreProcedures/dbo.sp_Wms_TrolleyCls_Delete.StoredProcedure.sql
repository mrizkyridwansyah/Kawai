


 

CREATE PROCEDURE [dbo].[sp_Wms_TrolleyCls_Delete]
	@Trolley_Cls varchar(25)
as
begin
	if not exists (select 1 from Trolley_Cls where Trolley_Cls = @Trolley_Cls)
	begin
		raiserror('Trolley Cls didn''t Exists',16,1)
		return;
	end


	if   exists (select 1 from MS_BOMPerworkstation_Header where Troly_Cls = @Trolley_Cls)
	begin
		raiserror('Trolley Cls Already Used in BOM Workstation',16,1)
		return;
	end

	if   exists (select 1 from MS_Trolley where Trolley_Cls = @Trolley_Cls)
	begin
		raiserror('Trolley Cls Already Used in Master Trolley',16,1)
		return;
	end

	delete from  Trolley_Cls where Trolley_Cls = @Trolley_Cls
end
