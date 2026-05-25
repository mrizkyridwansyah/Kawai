CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_Instruction_DDL]
    @Keyword    NVARCHAR(50) = NULL
AS
BEGIN
    SELECT DISTINCT 
        SI.SI_NO AS InstructionNo,
        Customer = TRIM(SI.Cust_Code) + ' - ' + TM.Trade_Name,
        InstructionDate = CASE WHEN SI.SI_Date IS NULL THEN NULL ELSE SI.SI_Date END
    FROM dbo.ShippingInstruction_Master SI
    LEFT JOIN Trade_Master TM ON TM.Trade_Code = SI.Cust_Code
    ORDER BY SI.SI_NO
END
GO


