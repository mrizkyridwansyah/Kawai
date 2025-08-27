SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER FUNCTION [GetTimeAgo] (@InputDate DATETIME)
RETURNS NVARCHAR(100)
AS
BEGIN
    DECLARE @Now DATETIME = GETDATE();
    DECLARE @Result NVARCHAR(100);

    -- Hitung total detik dan menit selisih waktu
    DECLARE @TotalSeconds INT = DATEDIFF(SECOND, @InputDate, @Now);
    DECLARE @TotalMinutes INT = DATEDIFF(MINUTE, @InputDate, @Now);
    DECLARE @TotalHours INT = DATEDIFF(HOUR, @InputDate, @Now);
    DECLARE @TotalDays INT = DATEDIFF(DAY, @InputDate, @Now);

    IF @TotalSeconds < 60
        SET @Result = 'Just Now';
    ELSE IF @TotalMinutes < 60
        SET @Result = CAST(@TotalMinutes AS VARCHAR(10)) + ' minutes ago';
    ELSE IF @TotalHours < 24
    BEGIN
        DECLARE @Hours INT = @TotalMinutes / 60;
        DECLARE @Minutes INT = @TotalMinutes % 60;

        SET @Result = 
            CAST(@Hours AS VARCHAR(10)) + ' hours ' +
            CAST(@Minutes AS VARCHAR(10)) + ' minutes ago';
    END
    ELSE
        SET @Result = CAST(@TotalDays AS VARCHAR(10)) + ' days ago';

    RETURN @Result;
END
GO
