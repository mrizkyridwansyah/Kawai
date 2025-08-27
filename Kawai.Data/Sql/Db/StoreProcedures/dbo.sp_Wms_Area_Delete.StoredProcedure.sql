SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [sp_Wms_Area_Delete]
	@AreaCode varchar(25)
as
begin
	if not exists (select 1 from MS_Area where Areacode = @AreaCode)
	begin
		raiserror('Area Code didn''t Exists',16,1)
		return;
	end

	delete from MS_Area where Areacode = @AreaCode
end
GO
