SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER FUNCTION [ConvertToDateTimeFromFuckingString] (
    @DateString VARCHAR(8)
)
RETURNS DATETIME
AS
BEGIN
    DECLARE @Result DATETIME;

    IF @DateString = '99999999'
    BEGIN
        SET @Result = '9999-12-31';
    END
    ELSE IF LEN(@DateString) = 8 
         AND ISNUMERIC(@DateString) = 1
         AND ISDATE(STUFF(STUFF(@DateString, 5, 0, '-'), 8, 0, '-')) = 1
    BEGIN
        SET @Result = CONVERT(DATETIME, STUFF(STUFF(@DateString, 5, 0, '-'), 8, 0, '-'), 120);
    END
    ELSE
    BEGIN
        SET @Result = NULL;
    END

    RETURN @Result;
END
GO
