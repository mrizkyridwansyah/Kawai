SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Warehouse_ValidateImport]
	@DataImport tvp_WarehouseImport READONLY,
	@UserId varchar(25)
as
begin	
	DECLARE @Result TABLE(
		[FactoryCode] [varchar](25),
		[WarehouseCode] [varchar](25),
		[WarehouseName] [varchar](100),
		[AdmGroup] [varchar](15),
		[StockControlCls] [varchar](2),
		[NGCls] [varchar](2),
		[UseEndDate] [date],
		[RowNumber] int,
		[Errors] varchar(max)
	)

	insert into @Result
	select * from @DataImport

	declare @tempValidate1 table 
	(
		[WarehouseCode] [varchar](25),
		[ExcelCount] int
	)

	insert into @tempValidate1
	select WarehouseCode, count(WarehouseCode) from @Result 
	group by WarehouseCode
	having count(WarehouseCode) > 1

	if exists (select 1 from @tempValidate1)
	begin
		update a 
		set 
			Errors += ISNULL(Errors, '') + CASE WHEN Errors IS NULL OR Errors = '' THEN '' ELSE CHAR(13) + CHAR(10) END 
				   + 'Warehouse Code tidak boleh duplikat dalam 1 file import.' 
		from @Result a
		inner join @tempValidate1 b on a.WarehouseCode = b.WarehouseCode
	end

	update a 
	set 
		Errors += ISNULL(Errors, '') + CASE WHEN Errors IS NULL OR Errors = '' THEN '' ELSE CHAR(13) + CHAR(10) END 
			   + 'Warehouse Code sudah terdaftar.' 
	from @Result a
	inner join WareHouse_Master b on a.WarehouseCode = b.WH_Code

	update a 
	set 
		Errors += ISNULL(Errors, '') + CASE WHEN Errors IS NULL OR Errors = '' THEN '' ELSE CHAR(13) + CHAR(10) END 
			   + 'User tidak memiliki hak akses untuk factory ini.' 
	from @Result a
	left join 
	(
		select FactoryCode From SS_UserFactoryPrivilege where UserID = @UserId and AllowAccess = 1
	) b on a.FactoryCode = b.FactoryCode
	where b.FactoryCode is null
	
	select * From @Result
end
GO
