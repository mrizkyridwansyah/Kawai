CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_GetEvidence]
(
    @InstructionNo     VARCHAR(50),
    @EvidenceType      VARCHAR(20), -- BEFORE / AFTER
    @IsLatestOnly      BIT = 1
)
AS
BEGIN

    SET NOCOUNT ON;

    -------------------------------------------------------
    -- VALIDASI
    -------------------------------------------------------
    IF ISNULL(@EvidenceType,'') NOT IN ('BEFORE','AFTER')
    BEGIN
        RAISERROR('Evidence type tidak valid!',16,1)
        RETURN
    END

    DECLARE
        @LoadingNo         VARCHAR(50),
        @LoadingStatus     VARCHAR(20),
        @BeforeStatus      BIT,
        @AfterStatus       BIT

    -------------------------------------------------------
    -- GET HEADER
    -------------------------------------------------------
    SELECT TOP 1
        @LoadingNo     = Loading_No,
        @LoadingStatus = Loading_Status,
        @BeforeStatus  = Before_Status,
        @AfterStatus   = After_Status
    FROM dbo.LoadingConfirmationScan_Header
    WHERE SI_No = @InstructionNo

    -------------------------------------------------------
    -- VALIDASI
    -------------------------------------------------------
    IF @LoadingNo IS NULL
    BEGIN
        RAISERROR('Loading data tidak ditemukan!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- HEADER EVIDENCE
    -------------------------------------------------------
    ;WITH CTE_EVIDENCE AS (
        SELECT
            ROW_NUMBER() OVER (PARTITION BY le.Evidence_Type ORDER BY ISNULL(le.Revision_No,0) DESC, le.Register_Date DESC) AS RowNum,
            le.Evidence_No,
            le.Loading_No,
            lsh.SI_No,
            le.Evidence_Type,
            le.Container_No,
            le.Vehicle_No,
            le.Seal_No,
            le.Remark,
            le.Driver_Name,
            le.Transport_Vendor,
            le.Latitude,
            le.Longitude,
            le.Evidence_Date,
            le.Register_Date,
            le.Register_By,
            le.Is_Deleted,
            ISNULL(le.Revision_No,0) AS Revision_No,
            lsh.Loading_Status,
            lsh.Before_Status,
            lsh.After_Status,

            -------------------------------------------------------
            -- UI FLAGS
            -------------------------------------------------------
            CanEdit =
                CAST(
                    CASE
                        WHEN lsh.Loading_Status = 'CLOSED'
                            THEN 0
                        WHEN le.Evidence_Type = 'BEFORE' AND lsh.Loading_Status <> 'CLOSED'
                            THEN 1
                        WHEN le.Evidence_Type = 'AFTER' AND lsh.Loading_Status = 'LOADED_COMPLETE'
                            THEN 1
                        ELSE 0
                    END
                AS BIT),
            CanUploadAttachment =
                CAST(
                    CASE
                        WHEN lsh.Loading_Status = 'CLOSED'
                            THEN 0
                        ELSE 1
                    END
                AS BIT),
            CanDeleteAttachment =
                CAST(
                    CASE
                        WHEN lsh.Loading_Status = 'CLOSED'
                            THEN 0
                        ELSE 1
                    END
                AS BIT),
            CanSubmit =
                CAST(
                    CASE
                        WHEN lsh.Loading_Status = 'CLOSED'
                            THEN 0
                        WHEN le.Evidence_Type = 'BEFORE' AND lsh.Before_Status = 1 AND lsh.Loading_Status <> 'CLOSED'
                            THEN 1
                        WHEN le.Evidence_Type = 'AFTER' AND lsh.Loading_Status = 'LOADED_COMPLETE'
                            THEN 1
                        ELSE 0
                    END
                AS BIT),
            IsClosed =
                CAST(
                    CASE
                        WHEN lsh.Loading_Status = 'CLOSED'
                            THEN 1
                        ELSE 0
                    END
                AS BIT)
        FROM dbo.LoadingConfirmationEvidence le
        INNER JOIN dbo.LoadingConfirmationScan_Header lsh ON lsh.Loading_No = le.Loading_No
        WHERE lsh.SI_No = @InstructionNo AND le.Evidence_Type = @EvidenceType AND ISNULL(le.Is_Deleted,0) = 0
    )

    SELECT
        Evidence_No,
        Loading_No,
        SI_No,
        Evidence_Type,
        Container_No,
        Vehicle_No,
        Seal_No,
        Remark,
        Driver_Name,
        Transport_Vendor,
        Latitude,
        Longitude,
        Evidence_Date,
        Register_Date,
        Register_By,
        Revision_No,
        Loading_Status,
        Before_Status,
        After_Status,
        CanEdit,
        CanUploadAttachment,
        CanDeleteAttachment,
        CanSubmit,
        IsClosed
    FROM CTE_EVIDENCE
    WHERE (@IsLatestOnly = 0 OR (@IsLatestOnly = 1 AND RowNum = 1))
    ORDER BY
        Revision_No DESC,
        Register_Date DESC

    -------------------------------------------------------
    -- ATTACHMENT
    -------------------------------------------------------
    SELECT
        le.Evidence_No,
        lea.SeqNo,
        lea.File_Name,
        lea.File_Path,
        lea.File_Extension,
        lea.Register_Date,
        lea.Register_By
    FROM dbo.LoadingConfirmationEvidence le
    INNER JOIN dbo.LoadingConfirmationEvidence_Attachment lea ON lea.Evidence_No = le.Evidence_No
    INNER JOIN dbo.LoadingConfirmationScan_Header lsh ON lsh.Loading_No = le.Loading_No
    WHERE
            lsh.SI_No = @InstructionNo
        AND le.Evidence_Type = @EvidenceType
        AND ISNULL(le.Is_Deleted,0) = 0
        AND ISNULL(lea.IsDeleted,0) = 0

        AND
        (
            @IsLatestOnly = 0
            OR
            le.Evidence_No =
            (
                SELECT TOP 1 x.Evidence_No
                FROM dbo.LoadingConfirmationEvidence x
                WHERE
                        x.Loading_No = le.Loading_No
                    AND x.Evidence_Type = le.Evidence_Type
                    AND ISNULL(x.Is_Deleted,0) = 0
                ORDER BY
                    ISNULL(x.Revision_No,0) DESC,
                    x.Register_Date DESC
            )
        )

    ORDER BY
        le.Evidence_No,
        lea.SeqNo

    -------------------------------------------------------
    -- REVISION HISTORY
    -------------------------------------------------------
    SELECT
        rh.History_ID,
        rh.Evidence_No,
        rh.Field_Name,
        rh.Old_Value,
        rh.New_Value,
        rh.Revision_Reason,
        rh.Change_Date,
        rh.Change_By
    FROM dbo.LoadingConfirmationEvidence_RevisionHistory rh
    INNER JOIN dbo.LoadingConfirmationEvidence le ON le.Evidence_No = rh.Evidence_No
    INNER JOIN dbo.LoadingConfirmationScan_Header lsh ON lsh.Loading_No = le.Loading_No
    WHERE
            lsh.SI_No = @InstructionNo
        AND le.Evidence_Type = @EvidenceType

    ORDER BY
        rh.Change_Date DESC,
        rh.History_ID DESC

END
GO


