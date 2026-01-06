SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GenerateNumeratorBatch]
    @Prefix VARCHAR(50),
	@RowCount INT = 1,
    @LastSequence INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRAN

		SET @LastSequence = (SELECT LastSeq FROM LastSequence WITH (HOLDLOCK, UPDLOCK) WHERE Prefix = @Prefix);
		DECLARE @NewSeq INT = ISNULL(@LastSequence, 0) + @RowCount;

		IF NOT EXISTS (SELECT 1 FROM LastSequence WHERE Prefix = @Prefix)
		BEGIN
			INSERT INTO LastSequence(Prefix, LastSeq) VALUES (@Prefix, @NewSeq);
		END
		ELSE
		BEGIN
			UPDATE LastSequence SET LastSeq = @NewSeq WHERE Prefix = @Prefix;
		END

		COMMIT TRAN
	END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        THROW;
    END CATCH
END
GO
