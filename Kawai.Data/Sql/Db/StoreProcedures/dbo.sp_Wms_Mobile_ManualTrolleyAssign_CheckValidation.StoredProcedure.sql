
CREATE procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_CheckValidation]
	@RequestNo varchar(100),
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

	if not exists (select 1 from PartMaterialRequestDetail where RefNumber = @RequestNo)
	begin
		raiserror('Data Request tidak ditemukan!', 16, 1)
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

	if exists 
	(
		select 1 from 
		(	
			select RefNo from StockDetail where isnull(Picking_No, '') = @RequestNo and Qty > 0
		) stok
		inner join MS_Trolley troli on stok.RefNo = troli.TrolleyCode
	)
	begin
		SELECT CAST(1 AS BIT) AlreadyHadStock, 'Trolley sebelumnya SUDAH ADA STOK. Anda akan ganti ke trolley ' + @TrolleyNo  MessageConfirmation
		return
	end

	SELECT CAST(0 AS BIT) AlreadyHadStock, 'Request SUDAH MEMILIKI TROLLEY. Anda akan ganti ke trolley ' + @TrolleyNo MessageConfirmation
	return
end
