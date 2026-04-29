CREATE PROCEDURE [dbo].[sp_Wms_ProductionQualityJudgement_Capture]
    @ProdResultID BIGINT
AS
BEGIN
    -- Header
    SELECT * 
    FROM ProductionResultHeader
    WHERE ProdResultID = @ProdResultID

  
END