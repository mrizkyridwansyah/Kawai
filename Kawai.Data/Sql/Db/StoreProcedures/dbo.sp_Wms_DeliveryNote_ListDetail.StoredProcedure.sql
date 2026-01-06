SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_DeliveryNote_ListDetail] 
	@DNNumber varchar(20)
as
begin
	IF OBJECT_ID('tempdb..#tblTemp') IS NOT NULL
		DROP TABLE #tblTemp;

	create table #tblTemp 
	(
		DNNumber varchar(100),
		PONumber varchar(25),
		ItemCode varchar(25),
		UnitClsCode varchar(2),
		Qty numeric(18, 9),
		TotalPacking int
	)

	insert into #tblTemp
	select '123123', 'PO-123', '930-050319-000', '01', 1000, 10
	UNION all
	select '456456', 'PO-456', '3000006580', '01', 2000, 10
	UNION all
	select '789789', 'PO-789', '3000013228', '01', 50, 1
	union all
	select '234234', 'PO-234', '930-050319-000', '01', 1000, 10
	union all
	select '234234', 'PO-234A', '3000013228', '01', 100, 1
	UNION all
	select '345345', 'PO-345', '3000006580', '01', 2000, 10
	UNION all
	select '567567', 'PO-567', '3000013228', '01', 50, 1
	union all
	select '678678', 'PO-678', '930-050319-000', '01', 1000, 10
	UNION all
	select '321321', 'PO-321', '3000006580', '01', 2000, 10
	UNION all
	select '432432', 'PO-432', '3000013228', '01', 50, 1
	union all
	select '543543', 'PO-543', '930-050319-000', '01', 1000, 10
	UNION all
	select '654654', 'PO-654', '3000006580', '01', 2000, 10
	UNION all
	select '765765', 'PO-765', '3000013228', '01', 50, 1
	
	select
		a.DNNumber, a.PONumber, a.ItemCode, b.Item_Name ItemName, a.UnitClsCode, c.Description UnitClsName, a.Qty, a.TotalPacking
	FROM #tblTemp a
	left join item_master b on a.ItemCode = b.item_code
	left join Unit_Cls c on a.UnitClsCode = c.Unit_Cls
	WHERE 1=1
	and a.DNNumber = @DNNumber	
end
GO
