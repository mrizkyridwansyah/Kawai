CREATE PROCEDURE sp_Wms_Mobile_PickingByScan_Instruction_DDL
    @Keyword NVARCHAR(50)
AS
BEGIN
    SELECT DISTINCT 
        SI.SI_NO AS InstructionNo,
        Customer = TRIM(SI.Cust_Code) + ' - ' + TM.Trade_Name,
        SI.SI_Date AS InstructionDate
    FROM ShippingInstruction_Master SI
    LEFT JOIN Trade_Master TM 
        ON TM.Trade_Code = SI.Cust_Code
    WHERE SI.SI_NO LIKE '%' + @Keyword + '%'
    ORDER BY SI.SI_NO
END