
CREATE   procedure [dbo].[sp_Wms_IQCResult_List]
	@SupplierCode varchar(25) = null,
	@Source varchar(25) = null,
	@StatusInspection varchar(20) = null,-- ALL, OK, NG
	@PeriodFrom date	,--= '2025-11-27',
	@PeriodUntil date	,--= '2025-11-30'
	@ReceiptId bigint	 --= '2025-11-30'
as
begin
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

	select 
		iqch.InspectionID InspectionId, prh.SupplierCode, sp.Trade_Name SupplierName, prh.DNNumber, prh.DNDate,
		iqch.ItemCode, iqch.ItemName, mi.Unit_Cls UnitCls, uc.Description UnitClsDescription, iqch.Soruce [Source],
		Qty = iqch.TotalQtySample, 
		QtyReceipt = pdtl.ReceiptQty,
		QtyNG = isnull(iqch.TotalQtyNG, 0), 
		iqch.RegisterDate,  iqch.InspectorID RegisterUser, qcus.FullName RegisterUserName,
		iqch.InspectionDate, iqch.InspectorID, qcus.FullName InspectorName,
		iqch.InspectionResult, iqch.InspectionResultDate ApprovalDate, iqch.InspectionResultApproval ApprovalUser, approver.FullName ApprovalUserName, 
		iqch.InspectionResultSA, iqch.InspectionResultSADate SAApprovalDate, iqch.InspectionResultSAApproval SAApprovalUser, saapprover.FullName SAApprovalUserName, 
		iqch.StatusQC
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	inner join PartReceiptDetail pdtl on pdtl.ReceiptId = prh.Id and isnull(pdtl.PONumber, '') = iqch.PO_Number and pdtl.ItemCode = iqch.ItemCode
	inner join Trade_Master sp on iqch.SupplierCode = sp.Trade_Code
	inner join Item_Master mi on iqch.ItemCode = mi.Item_Code
	inner join Unit_Cls uc on mi.Unit_Cls = uc.Unit_Cls
	left join SS_UserSetup qcus on qcus.UserID = iqch.InspectorID
	left join SS_UserSetup approver on approver.UserID = iqch.InspectionResultApproval
	left join SS_UserSetup saapprover on saapprover.UserID = iqch.InspectionResultSAApproval
	where 1=1
	and 1 = CASE WHEN @SupplierCode = 'ALL' THEN 1 WHEN @SupplierCode = iqch.SupplierCode THEN 1 ELSE 0 END
	and 1 = CASE WHEN @StatusInspection = 'ALL' THEN 1 WHEN @StatusInspection = iqch.InspectionResult THEN 1 ELSE 0 END
	and 1 = CASE WHEN @Source = 'ALL' THEN 1 WHEN @Source = iqch.Soruce THEN 1 ELSE 0 END
	and 1 = CASE WHEN @ReceiptId = 0 THEN 1 WHEN @ReceiptId = prh.Id THEN 1 ELSE 0 END
	and cast(iqch.InspectionDate as date) between @PeriodFrom and @PeriodUntil
	order by iqch.InspectionDate
end
