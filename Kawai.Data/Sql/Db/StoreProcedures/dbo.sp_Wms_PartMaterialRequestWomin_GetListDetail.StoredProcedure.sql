CREATE   procedure [dbo].[sp_Wms_PartMaterialRequestWomin_GetListDetail]
	@LineCode varchar(25),
	@NewRequest tvp_PartMaterialRequestWomin READONLY
as
begin

	declare @tblScan table (RequestDetailId bigint, ItemCode varchar(25), TotalScan numeric(18,9))

	if exists (select * From @NewRequest where RequestId is not null)
	begin
		insert into @tblScan
		select pmrd.RequestDetailID, pmrids.ItemCode, sum(pmrids.Qty) TotalScan From PartMaterialRequestItemDetailScan pmrids
		inner join PartMaterialRequestItemDetail pmrid on pmrids.IDSeq = pmrid.IDSeq
		inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
		inner join @NewRequest req on pmrd.RequestID = req.RequestId
		group by pmrd.RequestDetailID, pmrids.ItemCode
	end

	declare @LineName varchar(100) = (select Line_Name From Manufacture_Line where Line_Code = @LineCode)

	;WITH Numbers AS (
		SELECT TOP (1000)
			ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
		FROM sys.objects
	)
	SELECT
		req.RequestId,
		req.ProductionId,
		req.ScheduleDate,
		@LineCode AS LineCode,
		@LineName AS LineName,
		bomws.WorkStationCode,
		ws.WorkStationName,

		req.ItemCode AS ParentItemCode,
		mi.Item_Name AS ParentItemName,
		bomws.ChildItem_Code AS ChildItemCode,
		mic.Item_Name AS ChildItemName,
		isnull(mic.ClasificationPart_Cls, '20') AS ChildClassificationPart,
		isnull(cp.Description, 'OTHERS') AS ChildClassificationPartDesc,

		bomws.Qty AS QtyBOM,
		n.n SetNumber,
		-- QtySet per row
		CASE
			WHEN n.n * bomws.MaxCapacity <= TotalQty THEN bomws.MaxCapacity
			ELSE TotalQty - ((n.n - 1) * bomws.MaxCapacity)
		END AS QtySet,

		bomws.Qty *
		CASE
			WHEN n.n * bomws.MaxCapacity <= TotalQty THEN bomws.MaxCapacity
			ELSE TotalQty - ((n.n - 1) * bomws.MaxCapacity)
		END AS RequirementQty,
		pmrd.RefNumber PickingNo,
		reqCls.StatusDescription [Status],
		scan.TotalScan

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
	LEFT JOIN Item_Master mi
		ON req.ItemCode = mi.Item_Code
	LEFT JOIN Item_Master mic
		ON bomws.ChildItem_Code = mic.Item_Code
	LEFT JOIN ClasificationPart_Cls cp
		ON mic.ClasificationPart_Cls = cp.ClasificationPart_Cls
	LEFT JOIN PartMaterialRequestHeader pmrh
		ON pmrh.RequestID = req.RequestId

	CROSS APPLY (
		SELECT
			ISNULL(pmrh.RequestSetQty, req.RequestSetQty) AS TotalQty
	) q

	INNER JOIN Numbers n
		ON n.n <= CEILING(q.TotalQty * 1.0 / bomws.MaxCapacity)
	LEFT JOIN PartMaterialRequestDetail pmrd
		ON pmrd.RequestID = pmrh.RequestID and pmrd.SEQ = n.n and pmrd.WorkStationCode = bomws.WorkStationCode and pmrd.AreaCode = isnull(mic.ClasificationPart_Cls, '20')
	LEFT JOIN RequestStatusCls reqCls on pmrd.RequestStatusID = reqCls.RequestStatusID
	LEFT JOIN @tblScan scan on pmrd.RequestDetailID = scan.RequestDetailId and bomws.ChildItem_Code = scan.ItemCode

	order by n.n, bomws.WorkStationCode

end