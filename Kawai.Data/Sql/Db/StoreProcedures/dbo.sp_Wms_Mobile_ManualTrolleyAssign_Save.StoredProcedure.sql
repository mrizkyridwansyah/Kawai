
create   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_Save]
	@RequestNo varchar(100),
	@TrolleyNo varchar(50),
	@UserId varchar(25)
as
begin
	declare @requestId bigint, @ws varchar(25), @currentTrolley varchar(25)
	select 
		top 1 @requestId = RequestID, @ws = WorkStationCode, @currentTrolley = Trolley_No
	from PartMaterialRequestDetail where RefNumber = @RequestNo

	insert into AMRRequestHistory (RequestNo, FromData, ToData, [Action], SourceAction, StatusAMR, RegisterDate, RegisterUser)
	values (@RequestNo, @currentTrolley, @TrolleyNo, 'CANCEL AMR FROM WMS', 'sp_Wms_Mobile_ManualTrolleyAssign_Save', 'Requesting to AMR', GETDATE(), @UserId)

	update PartMaterialRequestDetail 
	set 
		Trolley_No = @TrolleyNo, 
		IsCurrentProcessManual = 1,
		LastUpdate = getdate(), 
		LastUser = @UserId 
	where RefNumber = @RequestNo

	if exists 
	(
		select 1 from 
		(	
			select RefNo from StockDetail where isnull(Picking_No, '') = @RequestNo and Qty > 0
		) stok
		inner join MS_Trolley troli on stok.RefNo = troli.TrolleyCode
	)
	begin
		DECLARE @toWarehouse varchar(25), @toAreaCode varchar(25), @toAddressCode varchar(25)
		SELECT TOP 1 @toWarehouse = WarehouseCode, @toAreaCode = AreaCode, @toAddressCode = AddressCode 
		FROM StockDetail where isnull(Picking_No, '') = @RequestNo and Qty > 0 order by RegisterDate desc

		EXEC sp_Wms_Stock_MovingRef @currentTrolley, @toWarehouse, @toAreaCode, @toAddressCode, @TrolleyNo, @UserId
	end

end
