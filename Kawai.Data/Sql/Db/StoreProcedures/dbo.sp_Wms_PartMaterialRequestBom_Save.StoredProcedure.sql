SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_PartMaterialRequestBom_Save]
	@WarehouseCode varchar(25), 
	@NewRequest tvp_PartMaterialRequestBom READONLY,
	@UserId varchar(25)
as
begin

	declare @RowCount int = (select count(1) from @NewRequest)
	declare @prefix varchar(11) = 'REQB.' + FORMAT(GETDATE(), 'yyyyMM')
	declare @lastSequence int
	exec GenerateNumeratorBatch @Prefix = @prefix, @RowCount = @RowCount, @LastSequence = @lastSequence OUTPUT

	DECLARE @Request TABLE
	(
		TempRowId UNIQUEIDENTIFIER,
		RequestId INT NULL,
		PONumber VARCHAR(50),
		PODate date,
		ItemCode VARCHAR(25),
		RequestSetQty NUMERIC(18,9)
	);

	INSERT INTO @Request (TempRowId, PONumber, PODate, ItemCode, RequestSetQty)
	SELECT NEWID(), PONumber, PODate, ItemCode, RequestSetQty FROM @NewRequest;

	DECLARE @InsertedHeader TABLE
	(
		TempRowId UNIQUEIDENTIFIER,
		RequestId INT
	);

	MERGE PartMaterialRequestHeader_PO AS tgt
	USING
	(
		SELECT
			TempRowId,
			PONumber,
			PODate,
			ItemCode,
			RequestSetQty,
			ROW_NUMBER() OVER (ORDER BY PONumber) AS RowNum
		FROM @Request
	) AS src
	ON 1 = 0   -- FORCE INSERT
	WHEN NOT MATCHED THEN
		INSERT
		(
			RequestNo,
			RequestDate,
			PO_NO,
			Warehouse,
			ProductionDate,
			ParentItem_Code,
			RequestSetQty,
			Status,
			Remarks,
			RegisterDate,
			RegisterUser
		)
		VALUES
		(
			@prefix + RIGHT(
				REPLICATE('0', 4)
				+ CAST(isnull(@lastSequence, 0) + src.RowNum AS VARCHAR),
				4
			),
			GETDATE(),
			src.PONumber,
			@WarehouseCode,
			src.PODate,
			src.ItemCode,
			src.RequestSetQty,
			'',
			'',
			GETDATE(),
			@UserId
		)
	OUTPUT
		INSERTED.RequestID,
		src.TempRowId
	INTO @InsertedHeader (RequestId, TempRowId);

	UPDATE r
	SET r.RequestId = i.RequestId
	FROM @Request r
	JOIN @InsertedHeader i
		ON r.TempRowId = i.TempRowId;

	set @RowCount = 
	(
		select count(*)
		from
		(
			select distinct a.PONumber, ma.AreaCode
			from @Request a 
			inner join BOM_Master bom on a.ItemCode = bom.Parent_ItemCode
			inner join Item_Master mi on bom.Item_Code = mi.Item_Code
			inner join MS_Area ma on mi.ClasificationPart_Cls = ma.ItemType
		) x
	)

	declare @prefixDetail varchar(15) = 'REQB.DTL.' + FORMAT(GETDATE(), 'yyyyMM')
	declare @lastSequenceDetail int

	exec GenerateNumeratorBatch @Prefix = @prefixDetail, @RowCount = @RowCount, @LastSequence = @lastSequenceDetail OUTPUT

	insert into PartMaterialRequestDetail_PO 
	(
		RequestDetailNo, RequestID, AreaCode, SEQ, RackNumber, RefNumber, RequestStatusID, Remarks, RegisterDate, RegisterUser
	)
	select 
		@prefixDetail + RIGHT(REPLICATE('0', 4) + CAST(isnull(@lastSequenceDetail, 0) + ROW_NUMBER() OVER (ORDER BY res.AreaCode) AS VARCHAR), 4),  
		res.RequestId, res.AreaCode, ROW_NUMBER() over (order by res.AreaCode), null, null, 0, '', getdate(), @UserId
	from 
	(
		select distinct r.RequestId, ma.AreaCode
		from @Request r
		inner join BOM_Master bom on r.ItemCode = bom.Parent_ItemCode
		inner join Item_Master mi on bom.Item_Code = mi.Item_Code
		inner join MS_Area ma on mi.ClasificationPart_Cls = ma.ItemType
	) res

	insert into PartMaterialRequestItemDetail_PO 
	(
		RequestDetailID, ItemCode, unit_Cls, ChildRequirement_Qty, Remarks, RegisterDate, RegisterUser
	)
	select 
		pmrd.RequestDetailID, bom.Item_Code, bom.Unit_Cls, bom.Qty * r.RequestSetQty, '', getdate(), @UserId
	from @Request r
	inner join BOM_Master bom on r.ItemCode = bom.Parent_ItemCode
	inner join Item_Master mi on bom.Item_Code = mi.Item_Code
	inner join MS_Area ma on mi.ClasificationPart_Cls = ma.ItemType
	inner join PartMaterialRequestDetail_PO pmrd on pmrd.RequestID = r.RequestId

end
GO
