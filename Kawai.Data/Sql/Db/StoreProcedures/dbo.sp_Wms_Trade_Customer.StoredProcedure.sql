SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec sp_Wms_Trade_Customer
CREATE     procedure [sp_Wms_Trade_Customer]
as
begin
	select Trade_Code = 'All'
		, Trade_Name = 'All'
	union all
	select Trade_Code, Trade_Name
	from Trade_Master
	where Trade_Cls in (2,3)
end

GO
