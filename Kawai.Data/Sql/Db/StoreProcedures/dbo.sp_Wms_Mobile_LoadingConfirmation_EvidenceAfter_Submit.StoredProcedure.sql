CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_EvidenceAfter_Submit]
(
    @InstructionNo         VARCHAR(50),
    @ContainerNo           VARCHAR(50),
    @VehicleNo             VARCHAR(50),
    @SealNo                VARCHAR(50),
    @Remark                VARCHAR(MAX) = NULL,
    @DriverName            VARCHAR(100) = NULL,
    @TransportVendor       VARCHAR(100) = NULL,
    @Latitude              VARCHAR(50) = NULL,
    @Longitude             VARCHAR(50) = NULL,
    @RevisionReason        VARCHAR(500) = NULL,
    @UserID                VARCHAR(50),

    @Files                 dbo.tvp_LoadingConfirmationEvidenceFile READONLY,
    @ExistingFiles         dbo.tvp_LoadingConfirmationExistingFile READONLY
)
AS
BEGIN

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @LoadingNo             VARCHAR(50),
        @EvidenceNo            VARCHAR(50),
        @OldEvidenceNo         VARCHAR(50),
        @LoadingStatus         VARCHAR(30),
        @DateNow               DATETIME = GETDATE(),
        @RevisionNo            INT = 0,
        @NextRevisionNo        INT = 1,
        @IsChanged             BIT = 0,
        @OldContainerNo        VARCHAR(50),
        @OldVehicleNo          VARCHAR(50),
        @OldSealNo             VARCHAR(50),
        @OldRemark             VARCHAR(MAX),
        @OldDriverName         VARCHAR(100),
        @OldTransportVendor    VARCHAR(100),
        @OldLatitude           VARCHAR(50),
        @OldLongitude          VARCHAR(50)

    -------------------------------------------------------
    -- VALIDASI FILE
    -------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM @Files) AND NOT EXISTS (SELECT 1 FROM @ExistingFiles)
    BEGIN
        RAISERROR('Evidence After wajib upload file!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI HEADER
    -------------------------------------------------------
    IF NOT EXISTS(
        SELECT 1
        FROM dbo.LoadingConfirmationScan_Header
        WHERE
            SI_No = @InstructionNo
            AND ISNULL(Before_Status,0) = 1
            AND ISNULL(AllScanned_Status,0) = 1
    )
    BEGIN
        RAISERROR('Scan data belum complete!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI CLOSED
    -------------------------------------------------------
    IF NOT EXISTS(
        SELECT 1
        FROM dbo.LoadingConfirmationScan_Header
        WHERE
            SI_No = @InstructionNo
            AND ISNULL(Loading_Status, 'OPEN') = 'OPEN'
    )
    BEGIN
        RAISERROR('Submit gagal. Loading Nomor Instruction (%s) sudah Closed!', 16, 1, @InstructionNo)
        RETURN
    END

    BEGIN TRY

        BEGIN TRAN

        -------------------------------------------------------
        -- GET HEADER
        -------------------------------------------------------
        SELECT TOP 1
            @LoadingNo     = Loading_No,
            @LoadingStatus = Loading_Status
        FROM dbo.LoadingConfirmationScan_Header
        WHERE
            SI_No = @InstructionNo
            AND ISNULL(AllScanned_Status,0) = 1

        -------------------------------------------------------
        -- VALIDASI CLOSED
        -------------------------------------------------------
        IF ISNULL(@LoadingStatus,'OPEN') = 'CLOSED'
        BEGIN
            RAISERROR('Loading sudah CLOSED!',16,1)
            ROLLBACK TRAN
            RETURN
        END

        -------------------------------------------------------
        -- GET LAST EVIDENCE AFTER
        -------------------------------------------------------
        SELECT TOP 1
            @OldEvidenceNo = Evidence_No,
            @RevisionNo = ISNULL(Revision_No,0)
        FROM dbo.LoadingConfirmationEvidence
        WHERE
            Loading_No = @LoadingNo
            AND Evidence_Type = 'AFTER'
            AND ISNULL(Is_Deleted,0) = 0
        ORDER BY Register_Date DESC

        SET @NextRevisionNo = ISNULL(@RevisionNo,0) + 1

        -------------------------------------------------------
        -- LOAD OLD VALUE
        -------------------------------------------------------
        SELECT TOP 1
            @OldContainerNo = Container_No,
            @OldVehicleNo = Vehicle_No,
            @OldSealNo = Seal_No,
            @OldRemark = Remark,
            @OldDriverName = Driver_Name,
            @OldTransportVendor = Transport_Vendor,
            @OldLatitude = Latitude,
            @OldLongitude = Longitude
        FROM dbo.LoadingConfirmationEvidence
        WHERE Evidence_No = @OldEvidenceNo

        -------------------------------------------------------
        -- CHECK HEADER CHANGES
        -------------------------------------------------------
        IF EXISTS (
            SELECT 1
            FROM dbo.LoadingConfirmationEvidence
            WHERE
                Evidence_No = @OldEvidenceNo AND
                (
                    ISNULL(Container_No,'') <> ISNULL(@ContainerNo,'')
                    OR ISNULL(Vehicle_No,'') <> ISNULL(@VehicleNo,'')
                    OR ISNULL(Seal_No,'') <> ISNULL(@SealNo,'')
                    OR ISNULL(Remark,'') <> ISNULL(@Remark,'')
                    OR ISNULL(Driver_Name,'') <> ISNULL(@DriverName,'')
                    OR ISNULL(Transport_Vendor,'') <> ISNULL(@TransportVendor,'')
                    OR ISNULL(Latitude,'') <> ISNULL(@Latitude,'')
                    OR ISNULL(Longitude,'') <> ISNULL(@Longitude,'')
                )
        )
        BEGIN
            SET @IsChanged = 1
        END

        -------------------------------------------------------
        -- CHECK FILE COUNT
        -------------------------------------------------------
        DECLARE @OldFileCount INT
        DECLARE @NewFileCount INT

        SELECT @OldFileCount = COUNT(*)
        FROM dbo.LoadingConfirmationEvidence_Attachment
        WHERE
            Evidence_No = @OldEvidenceNo
            AND ISNULL(IsDeleted,0) = 0

        SELECT
            @NewFileCount =
                (SELECT COUNT(*) FROM @Files)
                +
                (SELECT COUNT(*) FROM @ExistingFiles)

        IF ISNULL(@OldFileCount,0) <> ISNULL(@NewFileCount,0)
        BEGIN
            SET @IsChanged = 1
        END

        -------------------------------------------------------
        -- CHECK FILE NAME
        -------------------------------------------------------
        IF EXISTS (
            SELECT File_Name
            FROM dbo.LoadingConfirmationEvidence_Attachment
            WHERE
                Evidence_No = @OldEvidenceNo
                AND ISNULL(IsDeleted,0) = 0

            EXCEPT

            SELECT FileName FROM @Files
            UNION
            SELECT FileName FROM @ExistingFiles
        )
        BEGIN
            SET @IsChanged = 1
        END

        -------------------------------------------------------
        -- NO CHANGE
        -------------------------------------------------------
        IF @OldEvidenceNo IS NOT NULL AND ISNULL(@IsChanged,0) = 0
        BEGIN
            RAISERROR('Tidak ada perubahan data evidence.',16,1)
            ROLLBACK TRAN
            RETURN
        END

        -------------------------------------------------------
        -- INSERT REVISION HISTORY
        -------------------------------------------------------
        INSERT INTO dbo.LoadingConfirmationEvidence_RevisionHistory
        (
            Evidence_No,
            Field_Name,
            Old_Value,
            New_Value,
            Change_Type,
            Revision_No,
            Revision_Reason,
            Change_Date,
            Change_By
        )
        SELECT
            @OldEvidenceNo,
            V.Field_Name,
            V.Old_Value,
            V.New_Value,
            'HEADER_CHANGE',
            @NextRevisionNo,
            @RevisionReason,
            @DateNow,
            @UserID
        FROM
        (
            VALUES
                ('Container_No', @OldContainerNo, @ContainerNo),
                ('Vehicle_No', @OldVehicleNo, @VehicleNo),
                ('Seal_No', @OldSealNo, @SealNo),
                ('Remark', @OldRemark, @Remark),
                ('Driver_Name', @OldDriverName, @DriverName),
                ('Transport_Vendor', @OldTransportVendor, @TransportVendor),
                ('Latitude', @OldLatitude, @Latitude),
                ('Longitude', @OldLongitude, @Longitude)
        ) V(Field_Name, Old_Value, New_Value)
        WHERE ISNULL(V.Old_Value,'') <> ISNULL(V.New_Value,'')

        -------------------------------------------------------
        -- GENERATE EVIDENCE NO
        -------------------------------------------------------
        IF @OldEvidenceNo IS NULL
        BEGIN

            DECLARE @PrefixEvidence VARCHAR(20)

            SET @PrefixEvidence = 'EVA.' + FORMAT(@DateNow,'yyyyMMdd') + '.'

            EXEC dbo.GenerateNumerator
                @Prefix = @PrefixEvidence,
                @LengthSequence = 4,
                @Result = @EvidenceNo OUTPUT

            SET @RevisionNo = 0

        END
        ELSE
        BEGIN
            SET @EvidenceNo = @OldEvidenceNo
            SET @RevisionNo = ISNULL(@RevisionNo,0) + 1
        END

        -------------------------------------------------------
        -- INSERT / UPDATE EVIDENCE
        -------------------------------------------------------
        IF @RevisionNo = 0
        BEGIN

            INSERT INTO dbo.LoadingConfirmationEvidence
            (
                Evidence_No,
                Loading_No,
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
                Is_Deleted,
                Revision_No
            )
            VALUES
            (
                @EvidenceNo,
                @LoadingNo,
                'AFTER',
                @ContainerNo,
                @VehicleNo,
                @SealNo,
                @Remark,
                @DriverName,
                @TransportVendor,
                @Latitude,
                @Longitude,
                @DateNow,
                @DateNow,
                @UserID,
                0,
                @RevisionNo
            )

        END
        ELSE
        BEGIN

            UPDATE dbo.LoadingConfirmationEvidence
            SET
                Container_No = @ContainerNo,
                Vehicle_No = @VehicleNo,
                Seal_No = @SealNo,
                Remark = @Remark,
                Driver_Name = @DriverName,
                Transport_Vendor = @TransportVendor,
                Latitude = @Latitude,
                Longitude = @Longitude,
                Revision_No = @RevisionNo,
                Update_Date = @DateNow,
                Update_By = @UserID
            WHERE Evidence_No = @EvidenceNo

        END

        -------------------------------------------------------
        -- FILE ADDED HISTORY
        -------------------------------------------------------
        INSERT INTO dbo.LoadingConfirmationEvidence_RevisionHistory
        (
            Evidence_No,
            Field_Name,
            Old_Value,
            New_Value,
            Change_Type,
            Revision_No,
            Revision_Reason,
            Change_Date,
            Change_By
        )
        SELECT
            @EvidenceNo,
            'Attachment',
            NULL,
            F.FileName,
            'FILE_ADDED',
            @NextRevisionNo,
            @RevisionReason,
            @DateNow,
            @UserID
        FROM @Files F
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.LoadingConfirmationEvidence_Attachment A
            WHERE
                A.Evidence_No = @EvidenceNo
                AND A.File_Name = F.FileName
        )

        -------------------------------------------------------
        -- FILE REMOVED HISTORY
        -------------------------------------------------------
        INSERT INTO dbo.LoadingConfirmationEvidence_RevisionHistory
        (
            Evidence_No,
            Field_Name,
            Old_Value,
            New_Value,
            Change_Type,
            Revision_No,
            Revision_Reason,
            Change_Date,
            Change_By
        )
        SELECT
            @EvidenceNo,
            'Attachment',
            A.File_Name,
            NULL,
            'FILE_REMOVED',
            @NextRevisionNo,
            @RevisionReason,
            @DateNow,
            @UserID
        FROM dbo.LoadingConfirmationEvidence_Attachment A
        WHERE
            A.Evidence_No = @EvidenceNo
            AND ISNULL(A.IsDeleted,0) = 0
            AND NOT EXISTS
            (
                SELECT 1
                FROM @ExistingFiles F
                WHERE F.FileName = A.File_Name
            )

        -------------------------------------------------------
        -- REFRESH ATTACHMENT
        -------------------------------------------------------
        DELETE FROM dbo.LoadingConfirmationEvidence_Attachment
        WHERE Evidence_No = @EvidenceNo

        -------------------------------------------------------
        -- INSERT EXISTING FILE
        -------------------------------------------------------
        INSERT INTO dbo.LoadingConfirmationEvidence_Attachment
        (
            Evidence_No,
            SeqNo,
            File_Name,
            File_Path,
            File_Extension,
            Register_Date,
            Register_By,
            IsDeleted
        )
        SELECT
            @EvidenceNo,
            ROW_NUMBER() OVER(ORDER BY FileName),
            FileName,
            FilePath,
            RIGHT(FileName, CHARINDEX('.', REVERSE(FileName)) - 1),
            @DateNow,
            @UserID,
            0
        FROM @ExistingFiles

        UNION ALL

        -------------------------------------------------------
        -- INSERT NEW FILE
        -------------------------------------------------------
        SELECT
            @EvidenceNo,
            ROW_NUMBER() OVER(ORDER BY FileName) + ISNULL((SELECT COUNT(*) FROM @ExistingFiles),0),
            FileName,
            FilePath,
            RIGHT(FileName, CHARINDEX('.', REVERSE(FileName)) - 1),
            @DateNow,
            @UserID,
            0
        FROM @Files

        -------------------------------------------------------
        -- CLOSE LOADING
        -------------------------------------------------------
        UPDATE dbo.LoadingConfirmationScan_Header
        SET
            Container_No = @ContainerNo,
            Vehicle_No = @VehicleNo,
            Seal_No = @SealNo,
            After_Status = 1,
            Loading_Status = 'CLOSED',
            Finish_Date = @DateNow,
            Update_Date = @DateNow,
            Update_By = @UserID
        WHERE Loading_No = @LoadingNo

        COMMIT TRAN

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRAN

        DECLARE @Msg VARCHAR(MAX)

        SET @Msg = ERROR_MESSAGE()

        RAISERROR(@Msg,16,1)

    END CATCH

END
GO


