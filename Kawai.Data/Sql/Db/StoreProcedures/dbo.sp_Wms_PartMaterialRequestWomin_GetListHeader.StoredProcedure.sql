SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_PartMaterialRequestWomin_GetListHeader]
	@PeriodFrom date			,--= '2025-01-22',
	@PeriodUntil date			,--= '2025-01-22',
	@FactoryCode varchar(25)	,--= '00000',
	@ProcessCode varchar(25)	,--= '999',
	@LineCode varchar(25)		,--= '009',
	@RemainingCls bit			= null
as
begin
	select * from
	(
		select 
			dp.ProductionId, dp.Schedule_Date ScheduleDate, dp.Item_code ItemCode, mi.Item_Name ItemName, mi.Unit_Cls UnitCls, mu.Description UnitClsDesc, 
			dp.Qty PlanQty, pmrh.RequestID, pmrh.RequestNo, pmrh.RequestDate, pmrh.RequestSetQty, pmrh.RegisterUser, pmrh.RegisterDate, 
			RemainingQty = dp.Qty - isnull(pmrh.RequestSetQty , 0)
		From 
		(
			select 
				x.Seq_No ProductionId, x.Schedule_Date, x.Item_code, x.Qty
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
		inner join Unit_Cls mu on mi.Unit_Cls = mu.Unit_Cls
		left join PartMaterialRequestHeader pmrh 
			on dp.Schedule_Date = pmrh.ProductionDate and dp.Item_code = pmrh.ParentItem_Code and dp.ProductionId = pmrh.ProductionID
	) res
	where 1 = case when @RemainingCls is null then 1 
				   when @RemainingCls = 1 and RemainingQty > 0 then 1
				   when @RemainingCls = 0 and RemainingQty <= 0 then 1
				   else 0 end

end
GO
