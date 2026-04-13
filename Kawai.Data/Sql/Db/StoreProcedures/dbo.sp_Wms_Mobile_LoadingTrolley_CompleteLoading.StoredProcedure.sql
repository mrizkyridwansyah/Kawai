SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Mobile_LoadingTrolley_CompleteLoading]
	@TrolleyNo varchar(50),
	@PickingNo varchar(50),
	@UserId varchar(25)
as
begin
	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	if not exists (select top 1 RefNumber From PartMaterialRequestDetail where Trolley_No = @TrolleyNo and RefNumber = @PickingNo)
	begin
		raiserror('Data picking tidak ditemukan!', 16,1)
		return
	end

	declare @currentStopPoint varchar(25) = 
	(
		select top 1 Stop_Point 
		From PartMaterialRequestSendRobotDetail 
		where RequestSendID = @pickingNo and [Status] = 0 order by Pickup_Seq
	)

	declare @msg varchar(max)

	if @currentStopPoint is null
	begin
		set @msg = 'Loading Request ' + @pickingNo + ' sudah complete di semua stop point'
		raiserror(@msg, 16,1)
		return
	end

	if exists 
	(
		select 1 from StockDetail sd
		left join MS_Address ma on sd.AddressCode = ma.AddressCode
		left join MS_StopPoint msp on ma.StopPointCode = msp.StopPointCode
		where sd.RefNo <> @TrolleyNo 
		and sd.Picking_No = @pickingNo 
		and msp.StopPointCode = @currentStopPoint 
		and sd.Qty > 0
	)
	begin
		declare @stopPointName varchar(100) = (select [Description] from MS_StopPoint where StopPointCode = @currentStopPoint)
		set @msg = 'Silahkan scan semua stok loading (' + @pickingNo + ' | '+@stopPointName+') !'
		raiserror(@msg, 16,1)
		return
	end

	update PartMaterialRequestSendRobotDetail set [Status] = 1
	where RequestSendID = @pickingNo and [Status] = 0 and Stop_Point = @currentStopPoint
end
GO
