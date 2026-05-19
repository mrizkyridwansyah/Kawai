USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  StoredProcedure [dbo].[sp_Wms_Mobile_PickingByScan_Instruction_DDL]    Script Date: 5/19/2026 9:04:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_Wms_Mobile_PickingByScan_Instruction_DDL]
    @Keyword    NVARCHAR(50) = NULL
AS
BEGIN
    SELECT DISTINCT 
        SI.SI_NO AS InstructionNo,
        Customer = TRIM(SI.Cust_Code) + ' - ' + TM.Trade_Name,
        InstructionDate = CASE WHEN SI.SI_Date IS NULL THEN NULL ELSE SI.SI_Date END
    FROM dbo.ShippingInstruction_Master SI
    --JOIN dbo.ShippingInstruction_Detail DI ON DI.SI_No = SI.SI_NO AND DI.Item_Code = SI.Item_Code AND DI.PO_NO = SI.PO_NO AND DI.PO_SeqNo = SI.PO_SeqNo
    LEFT JOIN Trade_Master TM 
        ON TM.Trade_Code = SI.Cust_Code
    --WHERE SI.SI_NO LIKE '%' + @Keyword + '%'
    ORDER BY SI.SI_NO
END
GO

