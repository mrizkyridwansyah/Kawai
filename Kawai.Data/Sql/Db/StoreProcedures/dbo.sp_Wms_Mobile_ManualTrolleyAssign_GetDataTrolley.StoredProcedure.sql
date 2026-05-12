
create   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_GetDataTrolley]
	@TrolleyNo varchar(50)
as
begin
	declare @trolleyCls varchar(15), @id bigint, @isActive bit, @description varchar(150)

	select 
		@id = SeNo, @trolleyCls = Trolley_Cls, @isActive = IsActive, @description = [Description]
	From MS_Trolley where TrolleyCode = @TrolleyNo

	if @id is null
	begin
		raiserror('Data Trolley tidak ditemukan!', 16, 1)
		return
	end

	if isnull(@isActive, 0) = 0
	begin
		raiserror('Status Trolley tidak aktif!', 16, 1)
		return
	end

	select TrolleyCode = @TrolleyNo, Trolley_Cls = @trolleyCls, Description = @description
end
