SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_StockCalculate_GetPendingStock]
as
begin
	select Id, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, QtyTrans, SublotNo, SourceType, SourceRef, SourceRefNo, RefMutationId, RegisterUser
	From stockmutation where isnull(HasCalculate, 0) = 0
end
GO
