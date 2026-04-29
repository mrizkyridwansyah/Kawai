 CREATE procedure [dbo].[sp_Wms_ProductionResultManualInput_GetListHeader]
 --Declare
	@PeriodFrom date			= '2026-04-01 00:00:00.000',
	@PeriodUntil date			= '2026-04-24 00:00:00.000',
	@FactoryCode varchar(25)	= '00000',
	@ProcessCode varchar(25)	= 'DP',
	@LineCode varchar(25)		= 'DP-01',
	@CompleteCls bit			= 1
as

 
 
begin
	select * from
	(
		select 
			dp.ProductionId, dp.Schedule_Date ScheduleDate, dp.Item_code ItemCode, mi.Item_Name ItemName, mi.Unit_Cls UnitCls, mu.Description UnitClsDesc, 
			dp.Qty PlanQty, ProdResultID , pmrh.BarcodeQty, pmrh.BarcodeNo,   pmrh.LotNo,  pmrh.BarcodeQty as ResultQty , us.FullName RegisterUser, pmrh.RegisterDate, 
			RemainingQty = dp.Qty - isnull(pmrh.Qty , 0)
		From 
		(
			select 
				x.Seq_No ProductionId, x.Schedule_Date, x.Item_code, x.Qty , Complete_Cls
			From Daily_Production x
			inner join Manufacture_Line y on x.Factory_code = y.Manufacture_Code and x.Line_Code = y.Line_Code
			where 1=1
			and x.Factory_code = @ProcessCode
			and x.Line_Code = @LineCode
			and x.Schedule_Date between @PeriodFrom and @PeriodUntil
			and y.Company_Code = @FactoryCode
			--group by x.Schedule_Date, x.Item_code
		) dp 
		inner join Item_Master mi on dp.Item_code = mi.Item_Code
		Left join Unit_Cls mu on mi.Unit_Cls = mu.Unit_Cls
		left join ( select ph.ProdResultID, ProductionID , ProductionDate , BarcodeNo ,    itemcode,LotNo , pd.Qty as BarcodeQty ,  ResultType , TotalGoodQty as Qty , pd.RegisterDate , pd.RegisterUser
					from ProductionResultHeader ph 
					left join  ProductionResultDetail pd on ph.ProdResultID = pd.ProdResultID) pmrh 
			on dp.Schedule_Date = pmrh.ProductionDate and dp.Item_code = pmrh.ItemCode and dp.ProductionId = pmrh.ProductionID
		left join SS_UserSetup us on pmrh.RegisterUser  = us.UserID
	) res
 
end
