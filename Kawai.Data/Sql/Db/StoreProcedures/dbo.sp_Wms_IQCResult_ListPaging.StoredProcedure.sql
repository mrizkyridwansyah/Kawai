/****** Object:  StoredProcedure [dbo].[sp_Wms_IQCResult_ListPaging]    Script Date: 11/28/2025 10:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[sp_Wms_IQCResult_ListPaging]
--declare
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@SupplierCode varchar(25) = null,
	@Source varchar(25) = null,
	@StatusInspection varchar(20) = null,-- ALL, OK, NG
	@PeriodFrom date	,--= '2025-11-27',
	@PeriodUntil date	 --= '2025-11-30'
as
begin
	declare @sqlSort varchar(max) = ''
	declare @offset int

    -- Hitung offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by iqch.InspectionDate'
	end 

	IF ISNULL(@SupplierCode, '') = ''
	BEGIN
		SET @SupplierCode = 'ALL'
	END

	IF ISNULL(@StatusInspection, '') = ''
	BEGIN
		SET @StatusInspection = 'ALL'
	END

	IF ISNULL(@Source, '') = ''
	BEGIN
		SET @Source = 'ALL'
	END

	declare @TotalRow int =
	(
		select count(1)
		from IQC_Inspection_Header iqch
		inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
		inner join Trade_Master sp on iqch.SupplierCode = sp.Trade_Code
		where 1=1
		and 1 = CASE WHEN @SupplierCode = 'ALL' THEN 1 WHEN @SupplierCode = iqch.SupplierCode THEN 1 ELSE 0 END
		and 1 = CASE WHEN @StatusInspection = 'ALL' THEN 1 WHEN @StatusInspection = iqch.InspectionResult THEN 1 ELSE 0 END
		and 1 = CASE WHEN @Source = 'ALL' THEN 1 WHEN @Source = iqch.Soruce THEN 1 ELSE 0 END
		and cast(iqch.InspectionDate as date) between @PeriodFrom and @PeriodUntil
		and (iqch.ItemCode like '%'+@Keyword+'%' or iqch.ItemName like '%'+@Keyword+'%' or sp.Trade_Name like '%'+@Keyword+'%' or prh.DNNumber like '%'+@Keyword+'%')
	)

	declare @sql nvarchar(max) = 
	'
		select 
			iqch.InspectionID InspectionId, prh.SupplierCode, sp.Trade_Name SupplierName, prh.DNNumber, prh.DNDate,
			iqch.ItemCode, iqch.ItemName, mi.Unit_Cls UnitCls, uc.Description UnitClsDescription, iqch.Soruce [Source],
			Qty = iqch.TotalQtySample, 
			QtyNG = isnull(iqch.TotalQtyNG, 0), 
			iqch.RegisterDate,  iqch.InspectorID RegisterUser, qcus.FullName RegisterUserName,
			iqch.InspectionDate, iqch.InspectorID, qcus.FullName InspectorName,
			iqch.InspectionResult, iqch.InspectionResultDate ApprovalDate, iqch.InspectionResultApproval ApprovalUser, approver.FullName ApprovalUserName,
			TotalRow = @TotalRow
		from IQC_Inspection_Header iqch
		inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
		inner join Trade_Master sp on iqch.SupplierCode = sp.Trade_Code
		inner join Item_Master mi on iqch.ItemCode = mi.Item_Code
		inner join Unit_Cls uc on mi.Unit_Cls = uc.Unit_Cls
		left join SS_UserSetup qcus on qcus.UserID = iqch.InspectorID
		left join SS_UserSetup approver on approver.UserID = iqch.InspectionResultApproval
		where 1=1
		and 1 = CASE WHEN @SupplierCode = ''ALL'' THEN 1 WHEN @SupplierCode = iqch.SupplierCode THEN 1 ELSE 0 END
		and 1 = CASE WHEN @StatusInspection = ''ALL'' THEN 1 WHEN @StatusInspection = iqch.InspectionResult THEN 1 ELSE 0 END
		and 1 = CASE WHEN @Source = ''ALL'' THEN 1 WHEN @Source = iqch.Soruce THEN 1 ELSE 0 END
		and cast(iqch.InspectionDate as date) between @PeriodFrom and @PeriodUntil
		and (iqch.ItemCode like ''%''+@Keyword+''%'' or iqch.ItemName like ''%''+@Keyword+''%'' or sp.Trade_Name like ''%''+@Keyword+''%'' or prh.DNNumber like ''%''+@Keyword+''%'')
		'+ @sqlSort +'
		OFFSET @Offset ROWS FETCH NEXT @Length ROWS ONLY
	'
	print(@sql)

	EXEC sp_executesql 
	  @sql,
	  N'@Keyword VARCHAR(MAX), @SupplierCode varchar(25), @StatusInspection varchar(25), @Source varchar(25), @PeriodFrom date, @PeriodUntil date, @Offset INT, @Length INT, @TotalRow INT',
	  @Keyword = @Keyword,
	  @SupplierCode = @SupplierCode,
	  @StatusInspection = @StatusInspection,
	  @Source = @Source,
	  @PeriodFrom = @PeriodFrom,
	  @PeriodUntil = @PeriodUntil,
	  @Offset = @offset,
	  @Length = @Length,
	  @TotalRow = @TotalRow;

end
GO
