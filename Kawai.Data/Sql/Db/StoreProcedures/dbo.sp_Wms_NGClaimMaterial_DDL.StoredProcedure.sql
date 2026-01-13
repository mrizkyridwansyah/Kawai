SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sp_Wms_NGClaimMaterial_DDL]
	@Keyword varchar(max) = '',
	@Status varchar(max) = '',
    @SupplierCode varchar(25),
	@PeriodFrom date = null,
	@PeriodUntil date = null,
	@UserId varchar(25)
as
begin
	 
	 
	 

	if @PeriodFrom is not null and @PeriodUntil is not null
	begin
		select ClaimId, ClaimNo 
		from MaterialNGClaimHeader x  
		where 1=1 and ClaimNo like '%' + @Keyword + '%' 
		and ClaimDate between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = SupplierCode then 1 else 0 end 
	    and 1 = case when @Status = 'ALL' then 1 when @Status = [Status] then 1 else 0 end 
 

	end
	 
end
GO
