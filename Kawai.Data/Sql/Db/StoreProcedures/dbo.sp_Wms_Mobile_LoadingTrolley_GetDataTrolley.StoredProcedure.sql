
CREATE   PROCEDURE [dbo].[sp_Wms_Mobile_LoadingTrolley_GetDataTrolley]
	@TrolleyNo varchar(50)
as
begin
	declare @trolleyCls varchar(15), @trolleyDesc varchar(100)
	select @trolleyCls = Trolley_Cls, @trolleyDesc = Description From MS_Trolley where TrolleyCode = @TrolleyNo

	if @trolleyDesc is null
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Request untuk trolley ini tidak ditemukan!', 16,1)
		return
	end

	declare @pickingNo varchar(100), @requestDetailID bigint
	select top 1 @pickingNo = RefNumber, @requestDetailID = RequestDetailID 
	From PartMaterialRequestDetail where Trolley_No = @TrolleyNo order by RegisterDate desc

	if @pickingNo is null
	begin
		raiserror('Data picking tidak ditemukan!', 16,1)
		return
	end

	declare @trolleyClsWS varchar(15) = 
	(
		select bomh.Troly_Cls From 
		(
			select RequestID, WorkStationCode from PartMaterialRequestDetail where RequestDetailID = @requestDetailID
		) dtl
		inner join PartMaterialRequestHeader hd on dtl.RequestID = hd.RequestID
		inner join MS_BOMPerworkstation_Header bomh on hd.ParentItem_Code = bomh.ParentItemCode and hd.LineCode = bomh.Line_Code and dtl.WorkStationCode = bomh.WorkStationCode	
	)

	if isnull(@trolleyClsWS, '') = ''
	begin
		raiserror('Trolley Cls di BOM Per Workstation belum disetting!', 16,1)
		return
	end

	if isnull(@trolleyClsWS, '') <> isnull(@trolleyCls, '')
	begin
		raiserror('Trolley Cls tidak sesuai!', 16,1)
		return
	end

	if not exists (select 1 From StockDetail where Picking_No = @pickingNo and Qty > 0)
	begin
		raiserror('Data stock picking tidak ditemukan!', 16,1)
		return
	end

	declare @currentStopPoint varchar(25) = 
	(
		select top 1 Stop_Point 
		From PartMaterialRequestSendRobotDetail 
		where RequestSendID = @pickingNo and [Status] = 0 order by Pickup_Seq
	)

	if @currentStopPoint is null
	begin
		declare @msg varchar(max) = 'Loading Request ' + @pickingNo + ' sudah complete di semua stop point'
		raiserror(@msg, 16,1)
		return
	end

	declare @tmpStock table (
		PickingNo		varchar(50),
		RefNo			varchar(50),
		BarcodeNo		varchar(50),
		ItemCode		varchar(25),
		ItemName		varchar(100),
		LotNo			varchar(100),
		Qty				numeric(18,9),
		StopPointCode	varchar(25),
		StopPointName	varchar(100)
	)

	insert into @tmpStock
	select 
		@pickingNo, sd.RefNo, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.Qty, 
		msp.StopPointCode, msp.Description 
	From StockDetail sd 
	inner join Item_Master mi on sd.ItemCode = mi.Item_Code
	left join MS_Address ma on sd.AddressCode = ma.AddressCode
	left join MS_StopPoint msp on ma.StopPointCode = msp.StopPointCode
	where sd.Picking_No = @pickingNo and msp.StopPointCode = @currentStopPoint and sd.Qty > 0

	--declare @errMsg varchar(max) = ''
	--if not exists (select 1 from @tmpStock)
	--begin
	--	declare @spName varchar(100) = (select Description from MS_StopPoint where StopPointCode = @currentStopPoint)
	--	set @errMsg = 'Stock tidak ada di stop point ('+@spName+')!'

	--	raiserror(@errMsg, 16,1)
	--	return
	--end

	select 
		RequestDescription = pmrd.RefNumber + ' | ' + sd.StopPointName,
		pmrh.LineCode, ml.Line_Name LineName,
		pmrd.WorkStationCode, mw.WorkStationName,
		sd.*,
		StatusScan = case when sd.RefNo = @TrolleyNo then cast(1 as bit) else cast(0 as bit) end,
		StatusAMR = ''
	From PartMaterialRequestHeader pmrh
	inner join 
	(
		select distinct RefNumber, WorkStationCode, RequestID From PartMaterialRequestDetail where Trolley_No = @TrolleyNo
	) pmrd on pmrh.RequestID = pmrd.RequestID
	inner join Item_Master mi on pmrh.ParentItem_Code = mi.Item_Code
	inner join Manufacture_Line ml on pmrh.LineCode = ml.Line_Code
	inner join MS_WorkStation mw on pmrd.WorkStationCode = mw.WorkStationCode
	left join @tmpStock sd on sd.PickingNo = pmrd.RefNumber
end
