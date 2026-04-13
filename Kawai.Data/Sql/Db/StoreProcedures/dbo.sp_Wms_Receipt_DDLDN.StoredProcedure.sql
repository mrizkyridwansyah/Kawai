SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Receipt_DDLDN]
	@Keyword varchar(max) = '',
	@Status varchar(max) = '',
	@FactoryCode varchar(25),
	@SupplierCode varchar(25),
	@PeriodFrom date = null,
	@PeriodUntil date = null,
	@UserId varchar(25)
as
begin
	declare @tblFactory table (Factory varchar(25))

	IF @FactoryCode <> 'ALL'
	BEGIN
		insert into @tblFactory
		select Company_Code From Company_Profile where Company_Code = @FactoryCode
	END
	else 
	begin
		insert into @tblFactory
		select a.Company_Code from Company_Profile a 
		inner join 
		(
			select * From SS_UserFactoryPrivilege where UserID = @UserId and AllowAccess = 1
		) b on a.Company_Code = b.FactoryCode
	end

	IF ISNULL(@Status, '') = ''
	BEGIN
		SET @Status = 'ALL'
	END

	if @PeriodFrom is not null and @PeriodUntil is not null
	begin
		select Id, DNNumber
		from PartReceiptHeader x inner join @tblFactory y on x.CompanyCode = y.Factory
		where 1=1 and DNNumber like '%' + @Keyword + '%' 
		and ReceiptDate between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = SupplierCode then 1 else 0 end 
		and 1 = case when @Status = 'ALL' then 1 when StatusReceipt = @Status then 1 else 0 end
	end
	else
	begin
		select Id, DNNumber 
		from PartReceiptHeader x inner join @tblFactory y on x.CompanyCode = y.Factory
		where 1=1 and DNNumber like '%' + @Keyword + '%' 
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = SupplierCode then 1 else 0 end 
		and 1 = case when @Status = 'ALL' then 1 when StatusReceipt = @Status then 1 else 0 end
	end
end
GO
