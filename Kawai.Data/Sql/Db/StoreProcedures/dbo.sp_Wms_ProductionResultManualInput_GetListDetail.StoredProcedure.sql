CREATE   procedure [dbo].[sp_Wms_ProductionResultManualInput_GetListDetail]
--declare
	@LineCode varchar(25) ='DP-01',
    @NewRequest tvp_ProductionResultManualInput READONLY
as
begin

--declare @NewRequest Table ([ProdResultID] [bigint] NULL,
--	[ProductionId] [bigint] NULL,
--	[ScheduleDate] [date] NULL,
--	[ItemCode] [varchar](25) NULL,
--	[ResultQty] [numeric](18, 9) NULL)

--	insert into @NewRequest
--	select NULL, 1020401, '2026-04-17', '100R3070' , 70

	declare @tblScan table (RequestDetailId bigint, ItemCode varchar(25), TotalScan numeric(18,9))
	declare @tblScanRequest table (  ItemCode varchar(25), TotalScan numeric(18,9))

	--if exists (select * From @NewRequest where ProdResultID is not null)
	--begin
	--	insert into @tblScan
	--	select pmrd.RequestDetailID, pmrids.ItemCode, sum(pmrids.Qty) TotalScan From PartMaterialRequestItemDetailScan pmrids
	--	inner join PartMaterialRequestItemDetail pmrid on pmrids.IDSeq = pmrid.IDSeq
	--	inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
	--	inner join @NewRequest req on pmrd.RequestID = req.RequestId
	--	group by pmrd.RequestDetailID, pmrids.ItemCode
	--end

	declare @LineName varchar(100) = (select Line_Name From Manufacture_Line where Line_Code = @LineCode)



	   insert into @tblScanRequest
	    select  pmrids.ItemCode, sum(pmrids.Qty) TotalScan 
		From PartMaterialRequestItemDetailScan pmrids
		inner join PartMaterialRequestItemDetail pmrid on pmrids.IDSeq = pmrid.IDSeq
		inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
		inner join PartMaterialRequestHeader pmrh on pmrh.RequestID = pmrd.RequestID
		inner join @NewRequest req on pmrh.ProductionID = req.ProductionId
		group by pmrids.ItemCode

select * from (
	Select 
	    req.[ProdResultID],
		req.ProductionId,
		req.ScheduleDate,
		@LineCode AS LineCode,
		@LineName AS LineName,
 
		req.ItemCode AS ParentItemCode,
		mi.Item_Name AS ParentItemName,
		bomws.ChildItem_Code AS MaterialItemCode,
		mic.Item_Name AS MaterialItemName,
		TotalQty as ResultQty,
		bomws.Qty AS BOMQty ,
		TotalQty * bomws.Qty AS RequirementQty,
		ISNULL(dd.TotalScan,0) as ScanQty,
		(TotalQty * bomws.Qty) - ISNULL(dd.TotalScan,0) as RemainingQty ,
		 case when (TotalQty * bomws.Qty) - ISNULL(dd.TotalScan,0)  <= 0 then 1 else 0 end  [Status]

	FROM 
	(
	    select 
		hd.Line_Code,  hd.ParentItemCode,  
		dt.ChildItem_Code, dt.Unit_Cls, SUM(dt.Qty) Qty
		from MS_BOMPerworkstation_Header hd
		inner join MS_BOMPerworkstation_Detail dt on hd.Bomws_ID = dt.Bomws_ID
		where Line_Code = @LineCode  
		Group by  hd.Line_Code,  hd.ParentItemCode, dt.ChildItem_Code, dt.Unit_Cls
		) bomws INNER JOIN @NewRequest req
		ON bomws.ParentItemCode = req.ItemCode
		LEFT JOIN Item_Master mi
		ON req.ItemCode = mi.Item_Code
		LEFT JOIN Item_Master mic
		ON bomws.ChildItem_Code = mic.Item_Code
		Left JOIN @tblScanRequest dd on dd.ItemCode = bomws.ChildItem_Code
		CROSS APPLY (
		SELECT 
			ISNULL(  req.ResultQty,0) AS  TotalQty
	      ) q
		  ) A order by Status ASC 
end