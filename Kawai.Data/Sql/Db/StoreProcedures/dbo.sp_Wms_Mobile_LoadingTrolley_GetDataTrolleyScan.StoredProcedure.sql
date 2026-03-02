SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [sp_Wms_Mobile_LoadingTrolley_GetDataTrolleyScan]
	@TrolleyNo varchar(50),
	@StopPoint varchar(25)
as
begin
	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	declare @pickingNo varchar(100) = (select top 1 RefNumber From PartMaterialRequestDetail where Trolley_No = @TrolleyNo order by RegisterDate desc)

	if @pickingNo is null
	begin
		raiserror('Data picking tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 From StockDetail where Picking_No = @pickingNo and Qty > 0)
	begin
		raiserror('Data stock picking tidak ditemukan!', 16,1)
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
	where sd.Picking_No = @pickingNo and msp.StopPointCode = @StopPoint and sd.Qty > 0

	select 
		RequestDescription = pmrd.RefNumber + ' | ' + sd.StopPointName,
		pmrh.LineCode, ml.Line_Name LineName,
		pmrd.WorkStationCode, mw.WorkStationName,
		sd.*,
		StatusScan = case when sd.RefNo = @TrolleyNo then cast(1 as bit) else cast(0 as bit) end
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
GO
