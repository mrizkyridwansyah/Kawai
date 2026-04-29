create FUNCTION [dbo].[ConvertToDateTimeFromString] (
    @DateString VARCHAR(8)
)
RETURNS DATETIME
AS
BEGIN
    DECLARE @Result DATETIME
    DECLARE @Formatted VARCHAR(10)

    IF @DateString = '99999999'
    BEGIN
        RETURN '9999-12-31'
    END

    IF LEN(@DateString) = 8 
       AND @DateString NOT LIKE '%[^0-9]%'
    BEGIN
        -- yyyy d d M M  →  yyyy-MM-dd
        SET @Formatted =
            SUBSTRING(@DateString,1,4) + '-' +
            SUBSTRING(@DateString,7,2) + '-' +
            SUBSTRING(@DateString,5,2)

        IF ISDATE(@Formatted) = 1
            SET @Result = CONVERT(DATETIME, @Formatted, 120)
        ELSE
            SET @Result = NULL
    END
    ELSE
    BEGIN
        SET @Result = NULL
    END

    RETURN @Result
END