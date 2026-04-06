SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--EXEC [sp_Wms_ShippingInstruction_DLL] 'ALL','2026-03-01', '2026-03-31'
CREATE   PROCEDURE [sp_Wms_ShippingInstruction_DLL]
(
	@CustCode	nvarchar(50) = 'ALL'
	,@DateFrom	datetime
	,@DateTo	datetime
)
AS
BEGIN
	
	SELECT SI_NO = 'ALL'
	UNION
	SELECT SI_NO
	FROM ShippingInstruction_Master
	WHERE (ISNULL(@CustCode,'ALL') = 'ALL' OR Cust_Code = @CustCode)
	AND CAST(SI_Date AS DATE) BETWEEN CAST(@DateFrom AS DATE) AND CAST(@DateTo AS DATE)
	GROUP BY SI_NO

END
GO
