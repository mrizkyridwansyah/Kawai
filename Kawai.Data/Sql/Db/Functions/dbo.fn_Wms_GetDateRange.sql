SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create or alter function fn_Wms_GetDateRange
(
	@periodFrom date
)
returns int
as
begin

	declare @range int

	-- Bulan & tahun sekarang
	declare @currentYear int = year(getdate())
	declare @currentMonth int = month(getdate())

	-- Bulan & tahun parameter
	declare @paramYear int = year(@periodFrom)
	declare @paramMonth int = month(@periodFrom)

	if (@paramYear = @currentYear and @paramMonth = @currentMonth)
		set @range = 1       -- bulan ini
	else if (
		@paramYear = year(dateadd(month, -1, getdate()))
		and @paramMonth = month(dateadd(month, -1, getdate()))
	)
		set @range = 0       -- bulan lalu
	else if (
		@paramYear = year(dateadd(month, 1, getdate()))
		and @paramMonth = month(dateadd(month, 1, getdate()))
	)
		set @range = 2       -- bulan depan
	else
		set @range = -1      -- di luar range (opsional, biar aman)
	
	return @range;
end