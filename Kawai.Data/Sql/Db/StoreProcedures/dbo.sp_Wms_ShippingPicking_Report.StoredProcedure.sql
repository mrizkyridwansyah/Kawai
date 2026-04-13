SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC [sp_Wms_ShippingPicking_Report] 1, 5, '', 'ALL','2026-03-01', '2026-03-31'
CREATE   PROCEDURE [sp_Wms_ShippingPicking_Report]
(
	@Page		int = 1,
	@Length		int = 10,
	@Sort		nvarchar(max) = '',
    @Keyword    nvarchar(max) = '',

	@CustCode	nvarchar(50) = 'ALL',
	@DateFrom	datetime,
	@DateTo		datetime,
	@ShippingNo nvarchar(50) = 'ALL'
)
AS
BEGIN
	DECLARE @Offset int = (@Page - 1) * @Length

	DECLARE @TotalRow int = (
		SELECT  TotalRow = COUNT(1) 
		FROM ShippingInstruction_Master m
		INNER JOIN ShippingInstruction_Detail d ON m.SI_NO = d.SI_No AND m.PO_NO = d.PO_NO AND m.PO_SeqNo = d.PO_SeqNo AND M.Item_Code = d.Item_Code
		INNER JOIN Trade_Master t on m.Cust_Code = t.Trade_Code
		LEFT JOIN SS_UserSetup s on d.Picking_By = s.UserID
		WHERE (ISNULL(@CustCode, 'ALL') = 'ALL' OR m.Cust_Code = @CustCode)
		AND CAST(m.SI_Date AS DATE) BETWEEN CAST(@DateFrom as DATE) AND CAST(@DateTo as DATE) 
		AND (ISNULL(@ShippingNo, 'ALL') = 'ALL' OR m.SI_NO = @ShippingNo)
		AND ISNULL(d.IsPicking,0) = 1
		AND (
				@Keyword = ''
				OR m.Cust_Code		LIKE '%' + @Keyword + '%'
				OR t.Trade_Name		LIKE '%' + @Keyword + '%'
				OR m.SI_NO			LIKE '%' + @Keyword + '%'
				OR m.Item_Code		LIKE '%' + @Keyword + '%'
				OR m.Item_Name		LIKE '%' + @Keyword + '%'
				OR d.Serial_No		LIKE '%' + @Keyword + '%'
				OR d.Address		LIKE '%' + @Keyword + '%'
				OR s.FullName		LIKE '%' + @Keyword + '%'
		)
	)

	SELECT TotalRows		= @TotalRow,
			m.Cust_Code
			,t.Trade_Name
			,m.SI_NO
			,m.SI_Date
			,m.Item_Code
			,m.Item_Name
			,d.Serial_No
			,d.Address
			,Picking_Date	= CAST(d.Picking_Date AS DATE)
			,Picking_Time	= CAST(d.Picking_Date AS TIME)
			,d.Picking_By
			,Picking_Name = s.FullName
	FROM ShippingInstruction_Master m
	INNER JOIN ShippingInstruction_Detail d ON m.SI_NO = d.SI_No AND m.PO_NO = d.PO_NO AND m.PO_SeqNo = d.PO_SeqNo AND M.Item_Code = d.Item_Code
	INNER JOIN Trade_Master t on m.Cust_Code = t.Trade_Code
	LEFT JOIN SS_UserSetup s on d.Picking_By = s.UserID
	WHERE (ISNULL(@CustCode, 'ALL') = 'ALL' OR m.Cust_Code = @CustCode)
	AND CAST(m.SI_Date AS DATE) BETWEEN CAST(@DateFrom as DATE) AND CAST(@DateTo as DATE) 
	AND (ISNULL(@ShippingNo, 'ALL') = 'ALL' OR m.SI_NO = @ShippingNo)
	AND ISNULL(d.IsPicking,0) = 1
	AND (
				@Keyword = ''
				OR m.Cust_Code		LIKE '%' + @Keyword + '%'
				OR t.Trade_Name		LIKE '%' + @Keyword + '%'
				OR m.SI_NO			LIKE '%' + @Keyword + '%'
				OR m.Item_Code		LIKE '%' + @Keyword + '%'
				OR m.Item_Name		LIKE '%' + @Keyword + '%'
				OR d.Serial_No		LIKE '%' + @Keyword + '%'
				OR d.Address		LIKE '%' + @Keyword + '%'
				OR s.FullName		LIKE '%' + @Keyword + '%'
		)
	ORDER BY 
		CASE WHEN RIGHT(LOWER(@Sort), 3) = 'asc'  THEN d.Serial_No END ASC,
		CASE WHEN RIGHT(LOWER(@Sort), 4) = 'desc' THEN d.Serial_No END DESC
	OFFSET @Offset ROWS
	FETCH NEXT @Length ROWS ONLY;

END
GO
