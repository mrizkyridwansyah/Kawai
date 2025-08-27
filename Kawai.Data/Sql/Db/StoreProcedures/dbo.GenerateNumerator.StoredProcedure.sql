SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [GenerateNumerator]
    @Prefix VARCHAR(50),
    @LengthSequence INT,
    @Result VARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewSeq INT = ISNULL(
        (SELECT LastSeq FROM LastSequence WHERE Prefix = @Prefix), 0
    ) + 1;

    IF NOT EXISTS (SELECT 1 FROM LastSequence WHERE Prefix = @Prefix)
    BEGIN
        INSERT INTO LastSequence(Prefix, LastSeq) VALUES (@Prefix, @NewSeq);
    END
    ELSE
    BEGIN
        UPDATE LastSequence SET LastSeq = @NewSeq WHERE Prefix = @Prefix;
    END

    SET @Result = @Prefix + RIGHT(REPLICATE('0', @LengthSequence) + CAST(@NewSeq AS VARCHAR), @LengthSequence);
END
GO
