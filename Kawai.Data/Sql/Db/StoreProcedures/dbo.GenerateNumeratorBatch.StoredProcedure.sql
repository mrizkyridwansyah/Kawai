
CREATE PROCEDURE [dbo].[GenerateNumeratorBatch]
    @Prefix VARCHAR(50),
    @RowCount INT = 1,
    @LastSequence INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

    -- Validasi proteksi: Pastikan kuota yang direquest minimal 1
    IF @RowCount < 1 SET @RowCount = 1;

    -- Atomic Update: Ambil nilai saat ini sebagai titik awal, lalu tambahkan kuota
    UPDATE LastSequence 
    SET @LastSequence = LastSeq, 
        LastSeq = LastSeq + @RowCount 
    WHERE Prefix = @Prefix;

    -- Jika Prefix belum terdaftar di tabel
    IF @@ROWCOUNT = 0
    BEGIN
        SET @LastSequence = 0; -- Mulai dari 0 agar pemanggil menggunakan range 1 s/d RowCount
        
        BEGIN TRY
            INSERT INTO LastSequence (Prefix, LastSeq) 
            VALUES (@Prefix, @RowCount);
        END TRY
        BEGIN CATCH
            -- Tangani Race Condition: Jika Thread lain keduluan melakukan INSERT
            IF ERROR_NUMBER() IN (2601, 2627)
            BEGIN
                UPDATE LastSequence 
                SET @LastSequence = LastSeq, 
                    LastSeq = LastSeq + @RowCount 
                WHERE Prefix = @Prefix;
            END
            ELSE
            BEGIN
                ;THROW
            END
        END CATCH
    END
END