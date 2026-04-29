CREATE Proc [dbo].[sp_Wms_BOMWorkStation_CopyData]
--Declare
@FromLine   Varchar(100)='011',
@ToLine Varchar(100)='004',
@ItemCode Varchar(100)='10072070',
@UserID Varchar(100) = 'admin'
as

   
   IF Not Exists (select * from WorkStationLineSetting where LineCode = @ToLine)
   Begin
        raiserror('Cannot process copy bom workstation, please set workstation Line Setting for To Line Copy  ',16,1)
		return;
   end
   if @FromLine = @ToLine
	begin
		raiserror('From Copy and To Copy cannot be same line',16,1)
		return;
	end

   if not exists (select top 1 1 from MS_BOMPerworkstation_Header where Line_Code = @FromLine and ParentItemCode = @ItemCode)
	begin
		raiserror('From Copy Data Bom per Workstation didn''t Exists',16,1)
		return;
	end

	if exists (select top 1 1 from MS_BOMPerworkstation_Header where Line_Code = @ToLine and ParentItemCode = @ItemCode)
	begin
		raiserror('To Copy Data Bom per Workstation already Exists please setting manual!',16,1)
		return;
	end


	
      DECLARE @Map TABLE
        (
            OldId BIGINT,
            NewId BIGINT
        )

        -------------------------------------------------
        -- COPY HEADER (MERGE METHOD)
        -------------------------------------------------
        MERGE MS_BOMPerworkstation_Header AS target
        USING (
            SELECT *
            FROM MS_BOMPerworkstation_Header
            WHERE Line_Code = @FromLine
            AND ParentItemCode = @ItemCode
        ) AS src
        ON 1 = 0  -- force insert only

        WHEN NOT MATCHED THEN
        INSERT
        (
            Line_Code,
            ParentItemCode,
            WorkStationCode,
            MAX_Qty_Set,
            Troly_Cls,
            RegisterDate,
            RegisterUser
        )
        VALUES
        (
            @ToLine,
            src.ParentItemCode,
            src.WorkStationCode,
            src.MAX_Qty_Set,
            src.Troly_Cls,
            GETDATE(),
            @UserID
        )

        OUTPUT
            src.Bomws_ID,
            inserted.Bomws_ID
        INTO @Map(OldId, NewId);


        -------------------------------------------------
        -- VALIDASI
        -------------------------------------------------
        IF NOT EXISTS (SELECT 1 FROM @Map)
        BEGIN
            RAISERROR('Header tidak ditemukan',16,1)
            ROLLBACK
            RETURN
        END

        -------------------------------------------------
        -- COPY DETAIL
        -------------------------------------------------
        INSERT INTO MS_BOMPerworkstation_Detail
        (
            Bomws_ID,
            ChildItem_Code,
            Unit_Cls,
            Qty,
            RegisterDate,
            RegisterUser,
            LastUpdate
        )
        SELECT
            m.NewId,
            d.ChildItem_Code,
            d.Unit_Cls,
            d.Qty,
            GETDATE(),
            @UserID,
            GETDATE()
        FROM MS_BOMPerworkstation_Detail d
        JOIN @Map m ON d.Bomws_ID = m.OldId