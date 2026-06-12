

create   procedure [dbo].[sp_Wms_Mobile_LoadingTrolley_CompleteLoading]
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
		select scan.BarcodeNo From PartMaterialRequestItemDetailScan scan 
		inner join PartMaterialRequestItemDetail idtl on scan.IDSeq = idtl.IDSeq
		inner join PartMaterialRequestDetail dtl on idtl.RequestDetailID = dtl.RequestDetailID
		inner join StockDetail sd on scan.BarcodeNo = sd.BarcodeNo and sd.Qty > 0
		where dtl.RefNumber = @PickingNo and sd.RefNo <> @TrolleyNo 
	)
	begin
		declare @stopPointName varchar(100) = (select [Description] from MS_StopPoint where StopPointCode = @currentStopPoint)
		set @msg = 'Silahkan scan semua stok loading (' + @PickingNo + ' | '+@stopPointName+') !'
		raiserror(@msg, 16,1)
		return
	end

	begin try 
		begin transaction completeLoadingTransaction

		declare @isManual bit = (select top 1 IsCurrentProcessManual From PartMaterialRequestDetail where RefNumber = @pickingNo)
		declare @statusAMR varchar(max), @lastUserRequestAMR varchar(25), @lastRequestDateAMR date

		if isnull(@isManual, 0) = 0
		begin
			set @statusAMR = 'Requesting to AMR'
			set @lastUserRequestAMR = @UserId
			set @lastRequestDateAMR = getdate()
		end

		update PartMaterialRequestSendRobotDetail 
		set 
			[Status] = 1, 
			IsManual = @isManual,
			StatusAMR = @statusAMR, 
			LastUserRequestAMR = @lastUserRequestAMR,
			LastRequestDateAMR = @lastRequestDateAMR
		where RequestSendID = @pickingNo and [Status] = 0 and Stop_Point = @currentStopPoint

		commit transaction completeLoadingTransaction

		SELECT 
			RequestSendID = @PickingNo,
			TrolleyNo = @TrolleyNo,
			StopPoint = @currentStopPoint,
			CompleteStatus = 1,
			IsCaseSpecial = 0,
			IsManual = @isManual

	end try
	begin catch
		rollback transaction completeLoadingTransaction
		set @msg = (select ERROR_MESSAGE())

		raiserror(@msg, 16,1)
		return
	end catch
end
