CREATE PROCEDURE [dbo].[sp_Wms_PartMaterialRequestBom_Save]
    @WarehouseCode varchar(25), 
    @NewRequest tvp_PartMaterialRequestBom READONLY,
    @UserId varchar(25)
AS
BEGIN
    SET NOCOUNT ON;

    -- ====================================================================
    -- FASE 1: PRE-CALCULATION & PRE-FETCH (DILUAR TRANSAKSI)
    -- Operasi ini murni kalkulasi di memory dan baca/tulis cepat ke tabel Sequence.
    -- Gembok dilepas instan, tidak memblokir user lain.
    -- ====================================================================

    DECLARE @poNumberSampling varchar(100) = (select top 1 PONumber from @NewRequest)
	DECLARE @supplier varchar(25) = (select Supplier_Code From PurchaseOrder_Master where PO_No = @poNumberSampling)

	if not exists (select 1 from Trade_Master where Trade_Code = @supplier and isnull(Subcon_WH_Code, '') <> '')
	begin
		raiserror('Silahkan setting warehouse subcon terlebih dahulu di Trade Master!', 16, 1)
		return
	end


    DECLARE @prefixFactory varchar(5)
    
    SELECT @prefixFactory = fak.PrefixGlobalBarcode FROM Company_Profile fak
    INNER JOIN WareHouse_Master mw ON fak.Company_Code = mw.Company_Code
    INNER JOIN (
        SELECT WHTo FROM PurchaseOrder_Master WHERE PO_No = @poNumberSampling
    ) dp ON dp.WHTo = mw.WH_Code

    -- 1. Pindahkan data parameter ke variabel tabel dan buatkan TempRowId
    DECLARE @Request TABLE (
        TempRowId UNIQUEIDENTIFIER,
        RequestId INT NULL,
        PONumber VARCHAR(50),
        PODate date,
        ItemCode VARCHAR(25),
        RequestSetQty NUMERIC(18,9)
    );

    INSERT INTO @Request (TempRowId, PONumber, PODate, ItemCode, RequestSetQty)
    SELECT NEWID(), PONumber, PODate, ItemCode, RequestSetQty FROM @NewRequest;

    -- 2. Dapatkan kuota dan nomor urut untuk HEADER
    DECLARE @HeaderRowCount int = (SELECT COUNT(1) FROM @NewRequest)
    DECLARE @prefixHeader varchar(21) = @prefixFactory + 'REQB.' + FORMAT(GETDATE(), 'yyyyMM')
    DECLARE @lastSequenceHeader int
    
    EXEC GenerateNumeratorBatch @Prefix = @prefixHeader, @RowCount = @HeaderRowCount, @LastSequence = @lastSequenceHeader OUTPUT

    -- 3. Hitung jumlah kuota untuk DETAIL menggunakan TempRowId (tidak perlu nunggu RequestId)
    DECLARE @DetailRowCount int = (
        SELECT COUNT(*)
        FROM (
            SELECT DISTINCT a.TempRowId, ISNULL(mi.Grouping_Class_Part_Code, 'OT') ClasificationCode
            FROM @Request a 
            INNER JOIN BOM_Master bom ON a.ItemCode = bom.Parent_ItemCode
            INNER JOIN Item_Master mi ON bom.Item_Code = mi.Item_Code
        ) x
    )

    -- 4. Dapatkan kuota dan nomor urut untuk DETAIL
    DECLARE @prefixDetail varchar(25) = @prefixFactory + 'REQB.DTL.' + FORMAT(GETDATE(), 'yyyyMM')
    DECLARE @lastSequenceDetail int

    EXEC GenerateNumeratorBatch @Prefix = @prefixDetail, @RowCount = @DetailRowCount, @LastSequence = @lastSequenceDetail OUTPUT

    -- ====================================================================
    -- FASE 2: TRANSAKSI FISIK (DI DALAM TRANSAKSI)
    -- Semua nomor sudah disiapkan di Fase 1. Transaksi ini akan sangat cepat.
    -- Kalau ada error di tahap Detail, Header otomatis batal (Rollback).
    -- ====================================================================

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @InsertedHeader TABLE (
            TempRowId UNIQUEIDENTIFIER,
            RequestId INT
        );

        -- 5. INSERT Header (Gunakan @lastSequenceHeader)
        MERGE PartMaterialRequestHeader_PO AS tgt
        USING (
            SELECT
                TempRowId, PONumber, PODate, ItemCode, RequestSetQty,
                ROW_NUMBER() OVER (ORDER BY PONumber) AS RowNum
            FROM @Request
        ) AS src
        ON 1 = 0   -- FORCE INSERT
        WHEN NOT MATCHED THEN
            INSERT (
                RequestNo, RequestDate, PO_NO, Warehouse, ProductionDate,
                ParentItem_Code, RequestSetQty, Status, Remarks, RegisterDate, RegisterUser
            )
            VALUES (
                @prefixHeader + RIGHT(REPLICATE('0', 4) + CAST(ISNULL(@lastSequenceHeader, 0) + src.RowNum AS VARCHAR), 4),
                GETDATE(), src.PONumber, @WarehouseCode, src.PODate,
                src.ItemCode, src.RequestSetQty, '', '', GETDATE(), @UserId
            )
        OUTPUT
            INSERTED.RequestID, src.TempRowId
        INTO @InsertedHeader (RequestId, TempRowId);

        -- 6. Update RequestId kembali ke Temp Table
        UPDATE r
        SET r.RequestId = i.RequestId
        FROM @Request r
        JOIN @InsertedHeader i ON r.TempRowId = i.TempRowId;

        -- 7. INSERT Detail (Gunakan @lastSequenceDetail)
        INSERT INTO PartMaterialRequestDetail_PO (
            RequestDetailNo, RequestID, AreaCode, SEQ, RackNumber, RefNumber, RequestStatusID, Remarks, RegisterDate, RegisterUser
        )
        SELECT 
            @prefixDetail + RIGHT(REPLICATE('0', 4) + CAST(ISNULL(@lastSequenceDetail, 0) + ROW_NUMBER() OVER (ORDER BY res.ClasificationCode) AS VARCHAR), 4),  
            res.RequestId, res.ClasificationCode, ROW_NUMBER() over (order by res.ClasificationCode), null, 
            cast(res.RequestId as varchar) + rtrim(cls.Grouping_Class_Part_Code) + cast(ROW_NUMBER() over (order by res.ClasificationCode) as varchar), 
            0, '', getdate(), @UserId
        FROM (
            SELECT DISTINCT r.RequestId, ISNULL(mi.Grouping_Class_Part_Code, 'OT') ClasificationCode
            FROM @Request r
            INNER JOIN BOM_Master bom ON r.ItemCode = bom.Parent_ItemCode
            INNER JOIN Item_Master mi ON bom.Item_Code = mi.Item_Code
        ) res
        INNER JOIN Grouping_Class_Part cls ON res.ClasificationCode = cls.Grouping_Class_Part_Code

        -- 8. INSERT Item Detail
        INSERT INTO PartMaterialRequestItemDetail_PO (
            RequestDetailID, ItemCode, unit_Cls, ChildRequirement_Qty, Remarks, RegisterDate, RegisterUser
        )
        SELECT 
            pmrd.RequestDetailID, bom.Item_Code, bom.Unit_Cls, bom.Qty * r.RequestSetQty, '', getdate(), @UserId
        FROM @Request r
        INNER JOIN BOM_Master bom ON r.ItemCode = bom.Parent_ItemCode
        INNER JOIN Item_Master mi ON bom.Item_Code = mi.Item_Code
        INNER JOIN PartMaterialRequestDetail_PO pmrd ON pmrd.RequestID = r.RequestId AND pmrd.AreaCode = ISNULL(mi.Grouping_Class_Part_Code, 'OT')

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
		declare @msgErr varchar(max) = (select ERROR_MESSAGE())

        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

		raiserror(@msgErr, 16, 1)
        RETURN;
    END CATCH
END
