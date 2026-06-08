
CREATE PROCEDURE [dbo].[GenerateNumerator]
    @Prefix VARCHAR(50),
    @LengthSequence INT,
    @Result VARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewSeq INT;

    -- Atomic Update: Eksekusi instan, tambah 1 dan kunci baris secara otomatis
    UPDATE LastSequence 
    SET @NewSeq = LastSeq = LastSeq + 1 
    WHERE Prefix = @Prefix;

    -- Jika Prefix belum terdaftar di tabel
    IF @@ROWCOUNT = 0
    BEGIN
        SET @NewSeq = 1;
        
        BEGIN TRY
            INSERT INTO LastSequence (Prefix, LastSeq) 
            VALUES (@Prefix, @NewSeq);
        END TRY
        BEGIN CATCH
            -- Tangani Race Condition: Jika Thread lain keduluan melakukan INSERT
            IF ERROR_NUMBER() IN (2601, 2627)
            BEGIN
                UPDATE LastSequence 
                SET @NewSeq = LastSeq = LastSeq + 1 
                WHERE Prefix = @Prefix;
            END
            ELSE
            BEGIN
                ;THROW;
            END
        END CATCH
    END

    -- Format penggabungan Prefix dan Angka (Contoh: INV0001)
    SET @Result = @Prefix + RIGHT(REPLICATE('0', @LengthSequence) + CAST(@NewSeq AS VARCHAR), @LengthSequence);
END
