SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [sp_Wms_ProdMaterialRequirement_Calculate]
	@ParamKey varchar(100)  ,--= '00000|ALL|ALL|ALL|20260326',
	@Factory varchar(25)	,--= '00000', 
	@Process varchar(25)	,--= 'ALL', 
	@Line varchar(25)		,--= 'ALL', 
	@Model varchar(25)		,--= 'ALL',
	@ScheduleDateTo date	,--= getdate(),
	@UserId varchar(25)		 --= 'admin'
AS
BEGIN
	IF @ScheduleDateTo < CAST(GETDATE() AS DATE)
	BEGIN
		RAISERROR('Schedule Up To must be greater than today!',16,1)
		RETURN
	END

	DECLARE @minProdDate date = (select top 1 DATEADD(MONTH, -1, DATEFROMPARTS(Inventory_Year, Inventory_Month, 1)) from Inventory_Control order by Inventory_Year desc, Inventory_Month desc)

	if not exists (select 1 From ProdMaterialReqParams where ParamKey = @ParamKey)
	begin
		insert into ProdMaterialReqParams (ParamKey, Factory, Process, Line, Model, ScheduleDate, RegisterDate, RegisterUser)
		values (@ParamKey, @Factory, @Process, @Line, @Model, @ScheduleDateTo, getdate(), @UserId)
	end
	else 
	begin
		update ProdMaterialReqParams set LastUpdate = getdate(), LastUser = @UserId where ParamKey = @ParamKey
		delete from ProdMaterialReqHeader where ParamKey = @ParamKey
		delete from ProdMaterialReqDetail where ParamKey = @ParamKey
	end

	DECLARE @tblSupply TABLE (ProductionId bigint, ChildItemCode varchar(25), WorkstationCode varchar(25), TotalScan numeric(18,9))
	insert into @tblSupply
	select pmrh.ProductionID, scan.ItemCode, pmrd.WorkStationCode, sum(qty) TotalScan From PartMaterialRequestHeader pmrh 
	inner join PartMaterialRequestDetail pmrd on pmrh.RequestID = pmrd.RequestID
	inner join PartMaterialRequestItemDetail pmrid on pmrd.RequestDetailID = pmrid.RequestDetailID
	inner join PartMaterialRequestItemDetailScan scan on pmrid.IDSeq = scan.IDSeq
	group by pmrh.ProductionID, scan.ItemCode, pmrd.WorkStationCode

	declare @tblDetails table 
	(
		ProductionId bigint, ScheduleDate date, Process varchar(25), Line varchar(25), ParentItem varchar(25), ChildItem varchar(25), UnitCls varchar(25),
		ReqBomQty numeric(18,9), TotalScan numeric(18,9), FinalReqBomQty numeric(18,9)
	)
	INSERT INTO @tblDetails
	SELECT 
		ProductionId, ScheduleDate, ProcessCode, LineCode, ParentItemCode, ChildItemCode, UnitCls,
		isnull(sum(ReqBomQty), 0) ReqBomQty, isnull(SUM(TotalScan), 0) TotalScan, isnull(sum(FinalReqBomQty), 0) FinalReqBomQty
	FROM
	(
		SELECT 
			dp.Seq_No ProductionId, dp.Schedule_Date ScheduleDate,
			dp.Factory_code ProcessCode,
			dp.Line_Code LineCode,
			dp.Item_code ParentItemCode,
			bomdtl.ChildItem_Code ChildItemCode, mi.Unit_Cls UnitCls, 
			bomdtl.Qty BomQty, dp.Qty PlanQty,
			bomdtl.Qty * dp.Qty ReqBomQty, isnull(supply.TotalScan,0) TotalScan, (bomdtl.Qty * dp.Qty) - isnull(supply.TotalScan,0) FinalReqBomQty
		fROM Daily_Production dp
		inner join Item_Master pmi on dp.Item_code = pmi.Item_Code
		inner join MS_BOMPerworkstation_Header bomh on dp.Line_Code = bomh.Line_Code and dp.Item_code = bomh.ParentItemCode
		inner join MS_BOMPerworkstation_Detail bomdtl on bomh.Bomws_ID = bomdtl.Bomws_ID
		inner join Item_Master mi on bomdtl.ChildItem_Code = mi.Item_Code
		left join @tblSupply supply on dp.Seq_No = supply.ProductionId and bomdtl.ChildItem_Code = supply.ChildItemCode and bomh.WorkStationCode = supply.WorkstationCode
		WHERE 1=1
		and 1 = case when @Process = 'ALL' THEN 1 WHEN dp.Factory_code = @Process THEN 1 ELSE 0 END
		and 1 = case when @Line = 'ALL' THEN 1 WHEN dp.Line_Code = @Line THEN 1 ELSE 0 END
		and 1 = case when @Model = 'ALL' THEN 1 WHEN pmi.Model_Cls = @Model THEN 1 ELSE 0 END
		and Schedule_Date between @minProdDate and @ScheduleDateTo
	) res
	GROUP BY res.ProductionId, res.ScheduleDate, res.ProcessCode, res.LineCode, res.ParentItemCode, res.ChildItemCode, res.UnitCls

	insert into ProdMaterialReqHeader(ParamKey, ChildItemCode, UnitCls, TotalReqQty, CurrentStock, Shortage)
	select @ParamKey, ChildItem, UnitCls, isnull(sum(FinalReqBomQty), 0), isnull(stok.CurrentQty, 0),  isnull(stok.CurrentQty, 0) - isnull(sum(FinalReqBomQty), 0)
	from @tblDetails res
	LEFT JOIN 
	(
		SELECT ItemCode, SUM(Qty) CurrentQty FROM StockDetail WHERE ISNULL(Picking_No, '') = ''
		GROUP BY ItemCode
	) stok on res.ChildItem = stok.ItemCode
	GROUP BY res.ChildItem, res.UnitCls, stok.CurrentQty

	insert into ProdMaterialReqDetail(ParamKey, ChildItemCode, ProductionId, Process, Line, ScheduleDate, ParentItemCode, ReqQty, ScanQty, FinalReqQty)
	select @ParamKey, ChildItem, ProductionId, Process, Line, ScheduleDate, ParentItem, ReqBomQty, TotalScan, FinalReqBomQty 
	From @tblDetails
END
GO
