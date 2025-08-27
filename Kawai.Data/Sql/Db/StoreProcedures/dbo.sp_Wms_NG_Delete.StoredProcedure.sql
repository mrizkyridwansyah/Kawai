SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_NG_Delete]
	@NGCode varchar(25)
as
begin
	if not exists (select 1 from MS_NG where NGCode = @NGCode)
	begin
		raiserror('NG Code didn''t Exists',16,1)
		return;
	end

	delete from  MS_NG where NGCode = @NGCode
end
GO
