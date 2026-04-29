CREATE PROCEDURE sp_Wms_ProductionQualityJudgement_Save
    @NewRequest ProductionQualityJudgementType READONLY,
    @UserId VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE d
    SET d.ResultType = r.ResultType,
        d.LastUpdate = GETDATE(),
        d.LastUser = @UserId
    FROM ProductionResultDetail d
    JOIN ProductionResultHeader h 
        ON h.ProdResultID = d.ProdResultID
    JOIN @NewRequest r 
        ON r.ProdResultID = h.ProdResultID
        AND RTRIM(r.ItemCode) = RTRIM(h.ItemCode)
END