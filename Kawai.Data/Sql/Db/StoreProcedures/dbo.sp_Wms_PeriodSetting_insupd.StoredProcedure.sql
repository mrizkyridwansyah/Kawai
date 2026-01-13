SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_PeriodSetting_insupd]
    @Year        INT,
    @Details     tvp_PeriodSettingDetail READONLY,
    @UpdateBy    VARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentYear INT;
    SELECT @CurrentYear = YEAR(GETDATE());

    IF @Year < @CurrentYear
    BEGIN
        RAISERROR('Period Year has passed!',16,1);
        RETURN;
    END

    /* =====================================================
       VALIDATION AREA
       ===================================================== */

    -- 1. Validasi StartPeriod vs EndPeriod Tahun Sebelumnya
    IF EXISTS (
        SELECT 1
        FROM @Details d
        CROSS APPLY (
            SELECT MAX(m.EndPeriod) AS MaxEndDatePreviousYear
            FROM MS_PeriodSetting m
            WHERE m.[Year] = d.[Year] - 1
        ) prev
        WHERE
            prev.MaxEndDatePreviousYear IS NOT NULL
            AND d.StartPeriod IS NOT NULL
            AND d.StartPeriod < prev.MaxEndDatePreviousYear
    )
    BEGIN
        RAISERROR(
            'Start Date must be greater than End Date of Previous Period Year!',
            16, 1
        );
        RETURN;
    END

    -- 2. Validasi Finish SO vs Start SO
    IF EXISTS (
        SELECT 1
        FROM @Details
        WHERE
            StartSO IS NOT NULL
            AND FinishSO IS NOT NULL
            AND FinishSO < StartSO
    )
    BEGIN
        RAISERROR(
            'Finish SO must be greater than Start SO!',
            16, 1
        );
        RETURN;
    END

    /* =====================================================
       INSERT (DATA BELUM ADA)
       ===================================================== */
    INSERT INTO MS_PeriodSetting
    (
        Period,
        [Year],
        [Month],
        StartPeriod,
        EndPeriod,
        StartSO,
        FinishSO,
        RegisterDate,
        RegisterUser
    )
    SELECT
        d.Period,
        d.[Year],
        d.[Month],
        d.StartPeriod,
        d.EndPeriod,
        d.StartSO,
        d.FinishSO,
        GETDATE(),
        @UpdateBy
    FROM @Details d
    LEFT JOIN MS_PeriodSetting m
        ON m.Period = d.Period
    WHERE m.Period IS NULL;

    /* =====================================================
       UPDATE (DATA SUDAH ADA)
       ===================================================== */
    UPDATE m
    SET
        StartPeriod =
            CASE
                WHEN d.StartPeriod IS NOT NULL
                     AND ISNULL(m.StartPeriod,'1900-01-01') <> d.StartPeriod
                THEN d.StartPeriod
                ELSE m.StartPeriod
            END,

        EndPeriod =
            CASE
                WHEN d.EndPeriod IS NOT NULL
                     AND ISNULL(m.EndPeriod,'1900-01-01') <> d.EndPeriod
                THEN d.EndPeriod
                ELSE m.EndPeriod
            END,

        StartSO =
            CASE
                WHEN d.StartSO IS NOT NULL
                     AND ISNULL(m.StartSO,'1900-01-01') <> d.StartSO
                THEN d.StartSO
                ELSE m.StartSO
            END,

        FinishSO =
            CASE
                WHEN d.FinishSO IS NOT NULL
                     AND ISNULL(m.FinishSO,'1900-01-01') <> d.FinishSO
                THEN d.FinishSO
                ELSE m.FinishSO
            END,

        LastUpdate = GETDATE(),
        LastUser   = @UpdateBy
    FROM MS_PeriodSetting m
    INNER JOIN @Details d
        ON m.Period = d.Period
    WHERE
        (d.StartPeriod IS NOT NULL AND ISNULL(m.StartPeriod,'1900-01-01') <> d.StartPeriod)
        OR (d.EndPeriod   IS NOT NULL AND ISNULL(m.EndPeriod,'1900-01-01')   <> d.EndPeriod)
        OR (d.StartSO     IS NOT NULL AND ISNULL(m.StartSO,'1900-01-01')     <> d.StartSO)
        OR (d.FinishSO    IS NOT NULL AND ISNULL(m.FinishSO,'1900-01-01')    <> d.FinishSO);

END
GO
