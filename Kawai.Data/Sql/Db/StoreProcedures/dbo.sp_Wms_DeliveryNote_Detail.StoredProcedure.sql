SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryNote_Detail]
	@DNNumber varchar(20)
as
begin
	IF OBJECT_ID('tempdb..#tblTemp') IS NOT NULL
		DROP TABLE #tblTemp;

	create table #tblTemp 
	(
		DNNumber varchar(100),
		SupplierCode varchar(15),
		DNDate date,
		BCNumber varchar(10),
		BCType varchar(25),
		BCDate date,
		VehicleNo varchar(15),
		IsComplete bit
	)

	insert into #tblTemp
	select '123123', 'S0331', cast(dateadd(day, -7, getdate()) as date), '123', 'BC 2.3', cast(dateadd(day, -7, getdate()) as date), 'B 123 TES', CAST(0 AS BIT)
	UNION all
	select '456456', 'S0012', cast(dateadd(day, -5, getdate()) as date), '456', 'BC 2.5', cast(dateadd(day, -5, getdate()) as date), 'B 456 TES', CAST(0 AS BIT)
	UNION all
	select '789789', 'S0031', cast(dateadd(day, -1, getdate()) as date), '789', 'BC 2.5', cast(dateadd(day, -1, getdate()) as date), 'B 789 TES', CAST(0 AS BIT)
	
	select
		a.DNNumber, a.SupplierCode, b.Trade_Name SupplierName, a.DNDate, a.BCNumber, a.BCType, a.BCDate, a.VehicleNo, a.IsComplete
	FROM #tblTemp a
	left join trade_master b on a.SupplierCode = b.Trade_Code
	WHERE 1=1
	and a.DNNumber = @DNNumber	
end
GO
