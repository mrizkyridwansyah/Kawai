
create   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_Save]
	@RequestNo varchar(100),
	@TrolleyNo varchar(50),
	@UserId varchar(25)
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

	if not exists (select 1 from PartMaterialRequestDetail where RefNumber = @RequestNo)
	begin
		raiserror('Data Request tidak ditemukan!', 16, 1)
		return
	end

	declare @requestId bigint, @ws varchar(25)
	select 
		top 1 @requestId = RequestID, @ws = WorkStationCode
	from PartMaterialRequestDetail where RefNumber = @RequestNo

	declare @parentItem varchar(25), @line varchar(25)
	select @parentItem = ParentItem_Code, @line = LineCode From PartMaterialRequestHeader where RequestID = @requestId

	declare @troliClsWorkstation varchar(25) = 
	(
		select Troly_Cls From MS_BOMPerworkstation_Header 
		where Line_Code = @line and ParentItemCode = @parentItem and WorkStationCode = @ws
	)

	if @troliClsWorkstation <> @trolleyCls
	begin
		raiserror('Trolley Cls berbeda dengan BOM!', 16, 1)
		return
	end

	if exists 
	(
		select 1 from 
		(	
			select RefNo from StockDetail where isnull(Picking_No, '') = @RequestNo and Qty > 0
		) stok
		inner join MS_Trolley troli on stok.RefNo = troli.TrolleyCode
	)
	begin
		raiserror('Data Request sudah memiliki troli dan sudah terisi!', 16, 1)
		return
	end

	update PartMaterialRequestDetail 
	set 
		Trolley_No = @TrolleyNo, 
		IsCurrentProcessManual = 1,
		LastUpdate = getdate(), 
		LastUser = @UserId 
	where RefNumber = @RequestNo
end
