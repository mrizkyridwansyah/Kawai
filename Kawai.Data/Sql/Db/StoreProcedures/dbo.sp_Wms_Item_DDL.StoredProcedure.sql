SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Item_DDL]
	@Keyword varchar(max) = ''
as
begin
	select
		RTRIM(Item_Code) ItemCode, Item_Name ItemName
	From Item_Master
	where 1=1
	and Item_Name like '%'+ @Keyword +'%'
end

GO
