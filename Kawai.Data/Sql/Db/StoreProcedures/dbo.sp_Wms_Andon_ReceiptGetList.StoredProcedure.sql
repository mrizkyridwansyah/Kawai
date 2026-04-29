CREATE   PROCEDURE [dbo].[sp_Wms_Andon_ReceiptGetList]
as
begin
select *
 ,
Case when StatusReceiptName  = 'QC Inprogres' then 'PENDING'
when StatusReceiptName  = 'NG QC' then 'NG'
else 'OK' end StatusReceipt,
 
CASE WHEN DateDiff(Day , ReceiptDate , GetDate()) > 20 Then 'C' 
     WHEN DateDiff(Day , ReceiptDate , GetDate()) < 20 and DateDiff(Day , ReceiptDate , GetDate()) > 15 Then 'B'
	 ELSE 'A' END FlagGrid

from (
	select'' ReceiptNo, ph.ReceiptDate, tm.Trade_Abbr [SupplierName], ph.DNNumber, 
	CASE 
    WHEN LEN(mi.Item_Name) > 20 
        THEN LEFT(mi.Item_Name, 20) + '...'
    ELSE  mi.Item_Name END
	 [ItemName], 
	
	CASE WHEN ISNULL((Select top 1 InspectionResult from IQC_Inspection_Header dd where dd.ReceiptNo = ph.ReceiptNo and dd.ItemCode = pdb.ItemCode and dd.PO_Number = pdb.PONumber  and InspectionResult <> 'Rejected' order by InspectionID DESC),'QC Inprogres') = 'Accepted' then 'Passed QC'
	WHEN ISNULL((Select top 1 InspectionResult from IQC_Inspection_Header dd where dd.ReceiptNo = ph.ReceiptNo and dd.ItemCode = pdb.ItemCode and dd.PO_Number = pdb.PONumber  and InspectionResult <> 'Rejected' order by InspectionID DESC),'QC Inprogres') = 'QC Inprogres' then 'QC Inprogres' 
	WHEN ISNULL((Select top 1 InspectionResult from IQC_Inspection_Header dd where dd.ReceiptNo = ph.ReceiptNo and dd.ItemCode = pdb.ItemCode and dd.PO_Number = pdb.PONumber and InspectionResult = 'Rejected' order by InspectionID DESC),'QC Inprogres') = 'Rejected' then 'NG QC' end StatusReceiptName,
	--Case when ph.StatusReceipt = 'NEW' THEN 'QC Inprogres' else '' end,
	 
	pdb.ReceiptQty ReceiptQtyUnit , (select Count(*) From PartReceiptDetailBarcode cc where ph.Id = cc.ReceiptId) ReceiptQtyPack
	from PartReceiptHeader ph
	inner join PartReceiptDetail pdb on ph.Id = pdb.ReceiptId
	left join Trade_Master tm on ph.SupplierCode = tm.Trade_Code
	left join Item_Master mi on pdb.ItemCode = mi.Item_Code	
	 where ph.StatusReceipt <> 'COMPLETE'

	 ) a
end
