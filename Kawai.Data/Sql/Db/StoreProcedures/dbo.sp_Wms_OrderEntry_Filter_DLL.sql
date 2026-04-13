SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec sp_Wms_OrderEntry_Filter_DLL 'All','2022-03-01','2022-03-31'
CREATE OR ALTER     procedure [dbo].[sp_Wms_OrderEntry_Filter_DLL]
(
	@CustCode nvarchar(50)
	,@DateFrom datetime
	,@DateTo datetime
)
as
begin

	select  od.PO_No
		, od.Cust_Code
		, sm.SI_NO
	from OrderEntry_Detail od
	left outer join ShippingInstruction_Master sm on od.PO_No = sm.PO_NO
	where (@CustCode = 'All' or od.Cust_Code = @CustCode)
	and CAST(Delivery_Date AS DATE) between CAST(@DateFrom AS DATE) and CAST(@DateTo AS DATE)
	--and od.PO_No = 'KI3-11261'
	group by od.PO_No
		, od.Cust_Code
		, sm.SI_NO
	order by od.PO_No

end