SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_InventoryClosing_GetLastClosing] 
 
AS
BEGIN
	IF EXISTS(SELECT 1 FROM Inventory_Control)
	BEGIN
		declare @nextperiod date = (SELECT dateadd(month, 1, max(cast(cast(Inventory_Year as varchar) + '-' + right('0' + cast(Inventory_Month as varchar), 2)+'-01' as date))) FROM Inventory_Control)

		SELECT FORMAT(@nextperiod,'yyyy-MM') [Period]
	END
	ELSE
	BEGIN
		SELECT 
		CAST(YEAR(GETDATE()) AS varchar) + '-' + RIGHT('0' + CAST(MONTH(GETDATE()) AS varchar),2) [Period]
	END
END
GO
