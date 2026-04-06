SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CReate   PROCEDURE [sp_Wms_BOMWorkStation_GetQty]
	@Trolley_Cls varchar(25)
as
begin
	select ISNULL(Qty,0) MaxQtySet from Trolley_Cls where Trolley_Cls = @Trolley_Cls
end
GO
