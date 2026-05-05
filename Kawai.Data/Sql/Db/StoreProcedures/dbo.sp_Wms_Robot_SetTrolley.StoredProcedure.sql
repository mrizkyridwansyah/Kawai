CREATE procedure [dbo].[sp_Wms_Robot_SetTrolley]
	@RequestID varchar(50),
	@TrolleyNo varchar(25),
	@Status varchar(max)
AS
BEGIN
	if not exists (select 1 from MS_Trolley where TrolleyCode = @TrolleyNo)
	begin
		raiserror('Trolley belum terdaftar!',16,1)
		return
	end

	if not exists (select 1 From PartMaterialRequestSendRobotHeader where RequestSendID = @RequestID)	
	begin
		raiserror('Data Request tidak ditemukan!',16,1)
		return
	end

	if exists (select 1 From PartMaterialRequestDetail where RefNumber = @RequestID and isnull(Trolley_No, @TrolleyNo) <> @TrolleyNo)	
	begin
		raiserror('Data Request sudah di set dengan Trolley berbeda!',16,1)
		return
	end

	if exists (select 1 from StockDetail where RefNo = @TrolleyNo and Qty > 0 and isnull(Picking_No, '') <> '')
	begin
		declare @pickingNo varchar(100) = (select top 1 isnull(Picking_No, '') from StockDetail where RefNo = @TrolleyNo and Qty > 0 and isnull(Picking_No, '') <> '')
		declare @msg varchar(max) = 'Trolley sedang digunakan Request No. '+@pickingNo+'!'
		raiserror(@msg,16,1)
		return
	end

	update PartMaterialRequestDetail set Trolley_No = @TrolleyNo, LastUpdate = GETDATE(), StatusAMR = @Status --, LastUser = @RobotCode 
	where RefNumber = @RequestID and Trolley_No IS NULL
END
