CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_Evidence_Capture](
    @InstructionNo VARCHAR(50)
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT TOP 1
        H.Loading_No,
        H.SI_No,
        H.Container_No,
        H.Vehicle_No,
        H.Seal_No,
        H.Loading_Status,
        H.Before_Status,
        H.After_Status,
        H.Start_Date,
        H.Finish_Date,

        E.Evidence_No,
        E.Evidence_Type,
        E.Remark,
        E.Driver_Name,
        E.Transport_Vendor,
        E.Latitude,
        E.Longitude,
        E.Revision_No,
        E.Register_Date
    FROM dbo.LoadingConfirmationScan_Header H
    LEFT JOIN dbo.LoadingConfirmationEvidence E ON H.Loading_No = E.Loading_No AND ISNULL(E.Is_Deleted,0) = 0
    WHERE H.SI_No = @InstructionNo
    ORDER BY E.Register_Date DESC;
END
GO


