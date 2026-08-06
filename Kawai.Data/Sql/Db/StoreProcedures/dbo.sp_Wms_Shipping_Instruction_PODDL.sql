 

CREATE procedure [dbo].[sp_Wms_Shipping_Instruction_PODDL]
	@Keyword varchar(max) = '',
	 @SupplierCode varchar(25),
	@TypeDate varchar(25),
	@PeriodFrom date = null,
	@PeriodUntil date = null,
	@ShowOptionAll bit,
	@UserId varchar(25)
as
begin

   select  od.PO_No as PONumber
		, od.Cust_Code
		, sm.SI_NO
	from OrderEntry_Detail od
	left outer join ShippingInstruction_Master sm on od.PO_No = sm.PO_NO
	where (@SupplierCode = 'All' or od.Cust_Code = @SupplierCode)
	and CAST(Delivery_Date AS DATE) between CAST(@PeriodFrom AS DATE) and CAST(@PeriodUntil AS DATE)
	--and od.PO_No = 'KI3-11261'
	group by od.PO_No
		, od.Cust_Code
		, sm.SI_NO
	order by od.PO_No


 
end
GO


