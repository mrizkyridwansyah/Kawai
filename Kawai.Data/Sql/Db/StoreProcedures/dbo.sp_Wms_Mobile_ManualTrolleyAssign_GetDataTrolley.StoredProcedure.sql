
CREATE   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_GetDataTrolley]
	@RequestNo varchar(50),
	@TrolleyNo varchar(50)
as
begin
	if not exists (SELECT 1 fROM PartMaterialRequestDetail WHERE RefNumber = @RequestNo)
	begin
		raiserror('Data Request tidak ditemukan!', 16, 1)
		return
	end

	declare @trolleyCls varchar(15), @trolleyClsDesc varchar(100), @id bigint, @isActive bit, @description varchar(150)

	select 
		@id = a.SeNo, @trolleyCls = a.Trolley_Cls, @isActive = a.IsActive, @description = a.[Description], @trolleyClsDesc = b.Description
	From MS_Trolley a
	inner join Trolley_Cls b on a.Trolley_Cls = b.Trolley_Cls
	where TrolleyCode = @TrolleyNo

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

	declare @requestId bigint, @ws varchar(25), @currentTrolley varchar(25)
	select 
		top 1 @requestId = RequestID, @ws = WorkStationCode, @currentTrolley = Trolley_No
	from PartMaterialRequestDetail where RefNumber = @RequestNo

	declare @parentItem varchar(25), @line varchar(25)
	select @parentItem = ParentItem_Code, @line = LineCode From PartMaterialRequestHeader where RequestID = @requestId

	declare @troliClsWorkstation varchar(25) = 
	(
		select Troly_Cls From MS_BOMPerworkstation_Header 
		where Line_Code = @line and ParentItemCode = @parentItem and WorkStationCode = @ws
	)

	if @currentTrolley is not null and @currentTrolley = @TrolleyNo
	begin
		raiserror('Request sudah diassign dengan No. Trolley yang sama!', 16, 1)
		return
	end

	if @troliClsWorkstation <> @trolleyCls
	begin
		raiserror('Trolley Cls berbeda dengan BOM!', 16, 1)
		return
	end

	select TrolleyCode = @TrolleyNo, Trolley_Cls = @trolleyCls, Trolley_ClsDescs = @trolleyClsDesc, Description = @description, IsActive = isnull(@isActive, 0)
end
