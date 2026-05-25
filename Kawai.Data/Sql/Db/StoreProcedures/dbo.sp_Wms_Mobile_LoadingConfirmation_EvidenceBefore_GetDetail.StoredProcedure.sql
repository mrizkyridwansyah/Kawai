CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_EvidenceBefore_GetDetail](
    @InstructionNo VARCHAR(50)
)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE
        @LoadingNo VARCHAR(50),
        @EvidenceNo VARCHAR(50)

    -------------------------------------------------------
    -- GET LOADING NO
    -------------------------------------------------------
    SELECT TOP 1
        @LoadingNo = Loading_No
    FROM dbo.LoadingConfirmationScan_Header
    WHERE SI_No = @InstructionNo

    -------------------------------------------------------
    -- VALIDATION
    -------------------------------------------------------
    IF @LoadingNo IS NULL
    BEGIN
        -------------------------------------------------------
        -- HEADER EMPTY
        -------------------------------------------------------
        SELECT
            CAST(0 AS BIT) AS HasData,
            NULL AS EvidenceNo,
            NULL AS LoadingNo,
            NULL AS EvidenceType,
            NULL AS ContainerNo,
            NULL AS VehicleNo,
            NULL AS SealNo,
            NULL AS Remark,
            NULL AS DriverName,
            NULL AS TransportVendor,
            NULL AS Latitude,
            NULL AS Longitude,
            0 AS RevisionNo,
            NULL AS EvidenceDate,
            NULL AS RegisterDate,
            NULL AS RegisterBy

        -------------------------------------------------------
        -- ATTACHMENT EMPTY
        -------------------------------------------------------
        SELECT
            CAST(NULL AS INT) AS SeqNo,
            CAST(NULL AS VARCHAR(200)) AS FileName,
            CAST(NULL AS VARCHAR(500)) AS FilePath,
            CAST(NULL AS VARCHAR(20)) AS FileExtension,
            CAST(NULL AS DATETIME) AS RegisterDate,
            CAST(NULL AS VARCHAR(50)) AS RegisterBy
        WHERE 1 = 0

        RETURN
    END

    -------------------------------------------------------
    -- GET ACTIVE EVIDENCE BEFORE
    -------------------------------------------------------
    SELECT TOP 1
        @EvidenceNo = Evidence_No
    FROM dbo.LoadingConfirmationEvidence
    WHERE
        Loading_No = @LoadingNo
        AND Evidence_Type = 'BEFORE'
        AND ISNULL(Is_Deleted,0) = 0

    -------------------------------------------------------
    -- NO DATA
    -------------------------------------------------------
    IF @EvidenceNo IS NULL
    BEGIN
        -------------------------------------------------------
        -- HEADER EMPTY
        -------------------------------------------------------
        SELECT
            CAST(0 AS BIT) AS HasData,
            NULL AS EvidenceNo,
            NULL AS LoadingNo,
            NULL AS EvidenceType,
            NULL AS ContainerNo,
            NULL AS VehicleNo,
            NULL AS SealNo,
            NULL AS Remark,
            NULL AS DriverName,
            NULL AS TransportVendor,
            NULL AS Latitude,
            NULL AS Longitude,
            0 AS RevisionNo,
            NULL AS EvidenceDate,
            NULL AS RegisterDate,
            NULL AS RegisterBy

        -------------------------------------------------------
        -- ATTACHMENT EMPTY
        -------------------------------------------------------
        SELECT
            CAST(NULL AS INT) AS SeqNo,
            CAST(NULL AS VARCHAR(200)) AS FileName,
            CAST(NULL AS VARCHAR(500)) AS FilePath,
            CAST(NULL AS VARCHAR(20)) AS FileExtension,
            CAST(NULL AS DATETIME) AS RegisterDate,
            CAST(NULL AS VARCHAR(50)) AS RegisterBy
        WHERE 1 = 0

        RETURN
    END

    -------------------------------------------------------
    -- HEADER
    -------------------------------------------------------
    SELECT
        CAST(1 AS BIT) AS HasData,
        Evidence_No AS EvidenceNo,
        Loading_No AS LoadingNo,
        Evidence_Type AS EvidenceType,
        Container_No AS ContainerNo,
        Vehicle_No AS VehicleNo,
        Seal_No AS SealNo,
        Remark,
        Driver_Name AS DriverName,
        Transport_Vendor AS TransportVendor,
        Latitude,
        Longitude,
        Revision_No AS RevisionNo,
        Evidence_Date AS EvidenceDate,
        Register_Date AS RegisterDate,
        Register_By AS RegisterBy
    FROM dbo.LoadingConfirmationEvidence
    WHERE Evidence_No = @EvidenceNo

    -------------------------------------------------------
    -- ATTACHMENT
    -------------------------------------------------------
    SELECT
        SeqNo,
        File_Name AS FileName,
        File_Path AS FilePath,
        File_Extension AS FileExtension,
        Register_Date AS RegisterDate,
        Register_By AS RegisterBy

    FROM dbo.LoadingConfirmationEvidence_Attachment
    WHERE
        Evidence_No = @EvidenceNo
        AND ISNULL(IsDeleted,0) = 0

    ORDER BY SeqNo

END
GO


