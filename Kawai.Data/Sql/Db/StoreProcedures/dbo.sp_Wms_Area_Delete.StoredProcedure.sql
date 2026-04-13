SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





create   procedure [sp_Wms_Area_Delete]
	@AreaCode varchar(25)
as
begin
	if not exists (select 1 from MS_Area where Areacode = @AreaCode)
	begin
		raiserror('Area Code didn''t Exists',16,1)
		return;
	end

	if exists (select 1 from MS_Address where AreaCode = @AreaCode)
	begin
		raiserror('Data Area already used as reference data',16,1)
		return;
	end

	if exists (select 1 from StockDetail where AreaCode = @AreaCode)
	begin
		raiserror('Data Area already used as reference data',16,1)
		return;
	end

	delete from MS_Area where Areacode = @AreaCode
end
GO
