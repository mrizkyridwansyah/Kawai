SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [sp_Wms_Trolley_Delete]
	@TrolleyCode varchar(25)
as
begin
	if not exists (select 1 from MS_Trolley where TrolleyCode = @TrolleyCode)
	begin
		raiserror('NG Code didn''t Exists',16,1)
		return;
	end

	if   exists (select 1 from StockDetail where RefNo = @TrolleyCode and Qty > 0)
	begin
		raiserror('Trolley already used another process',16,1)
		return;
	end

	delete from  MS_Trolley where TrolleyCode = @TrolleyCode
end
GO
