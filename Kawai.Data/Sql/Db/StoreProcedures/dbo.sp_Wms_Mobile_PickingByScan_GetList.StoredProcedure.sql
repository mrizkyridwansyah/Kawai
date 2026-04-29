CREATE procedure [dbo].[sp_Wms_Mobile_PickingByScan_GetList]
    @InstructionNo varchar(50)
as

select
    [PartNo]        = sm.Item_Code
    ,[PartName]     = sm.Item_Name
    ,[SerialNo]     = sm.SI_NO
    ,[BarcodeNo]    = NULL
    ,[PickingDate]  = CAST(sd.Picking_Date AS DATE)
    ,[PickingTime]  = CAST(sd.Picking_Date AS TIME)
from dbo.ShippingInstruction_Master sm
INNER JOIN ShippingInstruction_Detail sd 
    ON sm.SI_NO = sd.SI_No 
    AND sm.PO_NO = sd.PO_NO 
    AND sm.PO_SeqNo = sd.PO_SeqNo 
    AND sm.Item_Code = sd.Item_Code
INNER JOIN Trade_Master t 
    on sm.Cust_Code = t.Trade_Code
LEFT JOIN SS_UserSetup s 
    on sd.Picking_By = s.UserID
where 
(
    (@InstructionNo <> 'ALL' and sm.SI_NO = @InstructionNo) 
    or 
    (@InstructionNo = 'ALL')
)