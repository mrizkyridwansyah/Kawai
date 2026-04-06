SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_QualityCheck_PrintReportNG]
	@ReceiptId bigint
as
begin
	--if not exists 
	--(
	--	select 1 From IQC_Inspection_Header iqch
	--	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	--	where prh.Id = @ReceiptId
	--	and iqch.Soruce = 'Incoming Material'
	--	and iqch.TotalQtyNG > 0
	--)
	--begin
	--	raiserror('No Data NG!' ,16,1)
	--	return
	--end

	declare @docNo varchar(25) = (select ReportNGDocNo from PartReceiptHeader where Id = @ReceiptId)

	declare @msgError varchar(max)
	if isnull(@docNo, '') = ''
	begin
		begin try
			begin tran
				set @docNo = cast((ISNULL((select max(cast(ReportNGDocNo as bigint)) from PartReceiptHeader WITH (HOLDLOCK, UPDLOCK) ), 0) + 1) as varchar)
				update PartReceiptHeader set ReportNGDocNo = @docNo where Id = @ReceiptId
			commit tran
		end try
		begin catch
			rollback tran
			set @msgError = ERROR_MESSAGE()
			raiserror(@msgError, 16, 1)
			return
		end catch
	end

	select 
	 	@docNo DocumentNo, cp.Company_Name FactoryName, @ReceiptId ReceiptId, iqch.InspectionID InspectionId, iqch.ItemCode, iqch.ItemName, '' Model, '' NoPPR, iqch.TotalQtyNG, iqch.Remarks,
		prh.SupplierCode, sup.Trade_Name SupplierName, iqch.InspectionDate, iqch.InspectionResultDate, prh.DNNumber, prh.ReceiptDate
	From IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	inner join Trade_Master sup on prh.SupplierCode = sup.Trade_Code
	inner join Company_Profile cp on prh.CompanyCode = cp.Company_Code
	where prh.Id = @ReceiptId
	and iqch.Soruce = 'Incoming Material'
	and iqch.TotalQtyNG > 0
	order by iqch.ItemCode
end
GO
