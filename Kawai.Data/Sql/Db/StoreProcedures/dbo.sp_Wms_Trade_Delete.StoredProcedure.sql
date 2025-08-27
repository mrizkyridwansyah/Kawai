SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [sp_Wms_Trade_Delete]
	@Trade_Code varchar(25)
as
begin
	if not exists (select 1 from Trade_Master where Trade_Code = @Trade_Code)
	begin
		raiserror('Trade Code didn''t Exists',16,1)
		return;
	end

	delete from  Trade_Master where Trade_Code = @Trade_Code
end
GO
