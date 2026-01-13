SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_PartMaterialRequestBom_GetListHeader]
	@PeriodFrom date			,--= '2025-12-16',
	@PeriodUntil date			,--= '2026-01-07',
	@FactoryCode varchar(25)	,--= '00000',
	@SupplierCode varchar(25)	,--= '999',
	@PONumber varchar(50)		,--= '999',
	@RemainingCls bit			= null
as
begin
	select * from
	(
		select 
			pos.PONumber, pos.PODate, pos.ItemCode, mi.Item_Name ItemName, mi.Unit_Cls UnitCls, mu.Description UnitClsDesc, 
			pos.Qty PlanQty, pmrh.RequestID, pmrh.RequestNo, pmrh.RequestDate, pmrh.RequestSetQty, pmrh.RegisterUser, pmrh.RegisterDate,
			RemainingQty = pos.Qty - isnull((select sum(RequestSetQty) from PartMaterialRequestHeader_PO x where x.PO_NO = pos.PONumber) , 0)
		From 
		(
			select 
				po.PO_No PONumber, po.PO_Date PODate, pod.Item_Code ItemCode, pod.Qty
			From PurchaseOrder_Master po
			inner join PurchaseOrder_Detail pod on po.PO_No = pod.PO_No
			inner join WareHouse_Master wh on po.WHTo = wh.WH_Code
			where 1=1
			and 1 = case when @PONumber = 'ALL' then 1 when po.PO_No = @PONumber then 1 else 0 end
			and wh.Company_Code = @FactoryCode
			and po.Supplier_Code = @SupplierCode
			and po.PO_Date between @PeriodFrom and @PeriodUntil
			and isnull(po.Fix_Cls, '0') = '1'
		) pos 
		inner join Item_Master mi on pos.ItemCode = mi.Item_Code
		inner join Unit_Cls mu on mi.Unit_Cls = mu.Unit_Cls
		left join PartMaterialRequestHeader_PO pmrh 
			on pos.PODate = pmrh.ProductionDate and pos.ItemCode = pmrh.ParentItem_Code and pos.PONumber = pmrh.PO_NO
	) res
	where 1 = case when @RemainingCls is null then 1 
				   when @RemainingCls = 1 and RemainingQty > 0 then 1
				   when @RemainingCls = 0 and RemainingQty <= 0 then 1
				   else 0 end

end
GO
