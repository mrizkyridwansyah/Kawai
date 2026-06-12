SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Mobile_AssignToTrolleyByBarcode_GetDataTrolley]
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

	declare @pickingNo varchar(100), @requestID bigint, @requestDetailID bigint
	select top 1 @pickingNo = RefNumber, @requestID = RequestID, @requestDetailID = @requestDetailID 
	From PartMaterialRequestDetail where Trolley_No = @TrolleyNo order by RegisterDate desc

	if @pickingNo is null
	begin
		raiserror('Data picking tidak ditemukan!', 16,1)
		return
	end

	select 
		@pickingNo PickingNo, @TrolleyNo TrolleyNo, @trolleyDesc TrolleyDescription, @trolleyCls TrolleyCls,
		rtrim(pmrh.LineCode) + ' - ' + rtrim(ml.Line_Name) Line, rtrim(pmrh.ParentItem_Code) + ' - ' + rtrim(mi.Item_Name) ParentItem, format(pmrh.ProductionDate, 'dd MMM yyyy') ProductionDate
	from PartMaterialRequestHeader pmrh 
	inner join Manufacture_Line ml on pmrh.LineCode = ml.Line_Code
	inner join Item_Master mi on pmrh.ParentItem_Code = mi.Item_Code
	where pmrh.RequestID = @requestID
end
GO
