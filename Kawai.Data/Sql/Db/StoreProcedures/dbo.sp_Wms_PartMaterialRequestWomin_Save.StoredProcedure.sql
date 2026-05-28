CREATE procedure [dbo].[sp_Wms_PartMaterialRequestWomin_Save]
	@LineCode varchar(25), 
	@NewRequest tvp_PartMaterialRequestWomin READONLY,
	@UserId varchar(25)
as
begin
	declare @prodIdSampling bigint = (select top 1 ProductionId from @NewRequest)
	declare @prefixFactory varchar(5)
	select @prefixFactory = fak.PrefixGlobalBarcode From Company_Profile fak
	inner join Manufacture_Line ml on fak.Company_Code = ml.Company_Code
	inner join
	(
		select Factory_code, Line_Code From Daily_Production where Seq_No = @prodIdSampling
	) dp on dp.Factory_code = ml.Manufacture_Code and dp.Line_Code = ml.Line_Code

	declare @RowCount int = (select count(1) from @NewRequest)
	declare @prefix varchar(20) = @prefixFactory + 'REQ.' + FORMAT(GETDATE(), 'yyyyMM')
	declare @lastSequence int
	exec GenerateNumeratorBatch @Prefix = @prefix, @RowCount = @RowCount, @LastSequence = @lastSequence OUTPUT

	DECLARE @Request TABLE
	(
		TempRowId UNIQUEIDENTIFIER,
		RequestId INT NULL,
		ProductionId INT,
		ScheduleDate date,
		ItemCode VARCHAR(25),
		RequestSetQty NUMERIC(18,9)
	);

	INSERT INTO @Request (TempRowId, ProductionId, ScheduleDate, ItemCode, RequestSetQty)
	SELECT NEWID(), ProductionId, ScheduleDate, ItemCode, RequestSetQty FROM @NewRequest;

	DECLARE @InsertedHeader TABLE
	(
		TempRowId UNIQUEIDENTIFIER,
		RequestId INT
	);

	MERGE PartMaterialRequestHeader AS tgt
	USING
	(
		SELECT
			TempRowId,
			ProductionId,
			ScheduleDate,
			ItemCode,
			RequestSetQty,
			ROW_NUMBER() OVER (ORDER BY ProductionId) AS RowNum
		FROM @Request
	) AS src
	ON 1 = 0   -- FORCE INSERT
	WHEN NOT MATCHED THEN
		INSERT
		(
			RequestNo,
			RequestDate,
			ProductionID,
			LineCode,
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
			src.ProductionId,
			@LineCode,
			src.ScheduleDate,
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

	DECLARE @BomTemp table 
	(
		ProductionId bigint, 
		ScheduleDate date, 
		WorkStationCode varchar(100), 
		ParentItemCode varchar(100), 
		ChildItemCode varchar(100), 
		UnitCls varchar(25), 
		SetNumber int, 
		ChildClassification varchar(25), 
		QtyBOM numeric(18, 9), 
		QtySet numeric(18, 9), 
		RequirementQty numeric(18, 9)
	)

	;WITH Numbers AS (
		SELECT TOP (1000)
			ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
		FROM sys.objects
	)

	insert into @BomTemp
	SELECT
		req.ProductionId,
		req.ScheduleDate,
		bomws.WorkStationCode,
		req.ItemCode AS ParentItemCode,
		bomws.ChildItem_Code AS ChildItemCode,
		bomws.Unit_Cls UnitCls,
		n.n SetNumber,
		mi.Grouping_Class_Part_Code AS ClasificationPart_Cls,
		bomws.Qty AS QtyBOM,
		CASE
			WHEN n.n * bomws.MaxCapacity <= TotalQty THEN bomws.MaxCapacity
			ELSE TotalQty - ((n.n - 1) * bomws.MaxCapacity)
		END AS QtySet,

		bomws.Qty *
		CASE
			WHEN n.n * bomws.MaxCapacity <= TotalQty THEN bomws.MaxCapacity
			ELSE TotalQty - ((n.n - 1) * bomws.MaxCapacity)
		END AS RequirementQty
	FROM 
	(
		select 
			hd.Bomws_ID, hd.Line_Code, hd.WorkStationCode, hd.ParentItemCode, hd.MAX_Qty_Set MaxCapacity, hd.Troly_Cls, 
			dt.DetailID, dt.ChildItem_Code, dt.Unit_Cls, dt.Qty 
		from MS_BOMPerworkstation_Header hd
		inner join MS_BOMPerworkstation_Detail dt on hd.Bomws_ID = dt.Bomws_ID
	) bomws
	INNER JOIN @NewRequest req
		ON bomws.ParentItemCode = req.ItemCode and bomws.Line_Code = @LineCode
	INNER JOIN MS_WorkStation ws
		ON bomws.WorkStationCode = ws.WorkStationCode
	LEFT JOIN Item_Master mi on bomws.ChildItem_Code = mi.Item_Code
	CROSS APPLY (
		SELECT req.RequestSetQty AS TotalQty
	) q

	INNER JOIN Numbers n
		ON n.n <= CEILING(q.TotalQty * 1.0 / bomws.MaxCapacity);

	set @RowCount = 
	(
		select sum(TotalDetil) 
		from 
		(
			select a.ProductionId, bomws.WorkStationCode, max(SetNumber) TotalDetil from @Request a inner join @BomTemp bomws on a.ItemCode = bomws.ParentItemCode
			group by a.ProductionId, bomws.WorkStationCode, bomws.ChildClassification
		) x
	)

	declare @prefixDetail varchar(14) = @prefixFactory + 'REQ.DTL.' + FORMAT(GETDATE(), 'yyyyMM')
	declare @lastSequenceDetail int

	exec GenerateNumeratorBatch @Prefix = @prefixDetail, @RowCount = @RowCount, @LastSequence = @lastSequenceDetail OUTPUT

	insert into PartMaterialRequestDetail 
	(
		RequestDetailNo, RequestID, WorkStationCode, AreaCode, SEQ, Trolley_No, RefNumber, RequestStatusID, Remarks, RegisterDate, RegisterUser
	)
	select 
		@prefixDetail + RIGHT(REPLICATE('0', 4) + CAST(isnull(@lastSequenceDetail, 0) + ROW_NUMBER() OVER (ORDER BY bomws.WorkStationCode) AS VARCHAR), 4),  
		r.RequestId, 
		bomws.WorkStationCode, 
		isnull(bomws.ChildClassification, '20'), -- untuk default adalah OTHERS 
		bomws.SetNumber, 
		null, 
		cast(r.RequestId as varchar) + rtrim(bomws.WorkStationCode) + cast(bomws.SetNumber as varchar), 
		0, '', getdate(), @UserId
	From 		
	(
		select distinct ProductionId, WorkStationCode, ChildClassification, SetNumber from @BomTemp
	) bomws
	inner join @Request r on bomws.ProductionId = r.ProductionId

	insert into PartMaterialRequestItemDetail 
	(
		RequestDetailID, ItemCode, unit_Cls, ChildRequirement_Qty, Remarks, RegisterDate, RegisterUser
	)
	select 
		pmrd.RequestDetailID, bomws.ChildItemCode, bomws.UnitCls, bomws.RequirementQty, '', getdate(), @UserId
	From @BomTemp bomws
	inner join 
	(
		select dtl.RequestDetailID, r.ProductionId , dtl.WorkStationCode, dtl.AreaCode, dtl.SEQ URutan From @Request r 
		inner join PartMaterialRequestDetail dtl on dtl.RequestID = r.RequestId	
	) pmrd on bomws.ProductionId = pmrd.ProductionId and pmrd.WorkStationCode = bomws.WorkStationCode and pmrd.URutan = bomws.SetNumber 
	and pmrd.AreaCode = bomws.ChildClassification

end
