SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_PartMaterialRequestWomin_Save]
	@LineCode varchar(25), 
	@NewRequest tvp_PartMaterialRequestWomin READONLY,
	@UserId varchar(25)
as
begin

	declare @RowCount int = (select count(1) from @NewRequest)
	declare @prefix varchar(10) = 'REQ.' + FORMAT(GETDATE(), 'yyyyMM')
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

	set @RowCount = (select count(distinct bomws.WorkStationCode) from @Request a inner join MS_BOMPerworkstation bomws on a.ItemCode = bomws.ParentItem_Code)
	declare @prefixDetail varchar(14) = 'REQ.DTL.' + FORMAT(GETDATE(), 'yyyyMM')
	declare @lastSequenceDetail int

	exec GenerateNumeratorBatch @Prefix = @prefixDetail, @RowCount = @RowCount, @LastSequence = @lastSequenceDetail OUTPUT

	insert into PartMaterialRequestDetail 
	(
		RequestDetailNo, RequestID, WorkStationCode, AreaCode, SEQ, RackNumber, RefNumber, RequestStatusID, Remarks, RegisterDate, RegisterUser
	)
	select 
		@prefixDetail + RIGHT(REPLICATE('0', 4) + CAST(isnull(@lastSequenceDetail, 0) + ROW_NUMBER() OVER (ORDER BY bomws.WorkStationCode) AS VARCHAR), 4),  
		r.RequestId, bomws.WorkStationCode, '', ROW_NUMBER() over (order by bomws.WorkStationCode), null, null, 0, '', getdate(), @UserId
	From 
	(
		select distinct ParentItem_Code, WorkStationCode From MS_BOMPerworkstation 
	) bomws
	inner join @Request r on bomws.ParentItem_Code = r.ItemCode

	insert into PartMaterialRequestItemDetail 
	(
		RequestDetailID, ItemCode, unit_Cls, ChildRequirement_Qty, Remarks, RegisterDate, RegisterUser
	)
	select 
		pmrd.RequestDetailID, bomws.ChildItem_Code, bomws.Unit_Cls, bomws.Qty * r.RequestSetQty, '', getdate(), @UserId
	From MS_BOMPerworkstation bomws
	inner join @Request r on bomws.ParentItem_Code = r.ItemCode
	inner join PartMaterialRequestDetail pmrd on pmrd.RequestID = r.RequestId and pmrd.WorkStationCode = bomws.WorkStationCode

end
GO
