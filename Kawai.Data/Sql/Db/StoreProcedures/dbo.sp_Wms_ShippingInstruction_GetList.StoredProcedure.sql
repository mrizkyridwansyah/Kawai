SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- EXEC  [sp_Wms_ShippingInstruction_GetList] 'KI3-02660'
CREATE     procedure [sp_Wms_ShippingInstruction_GetList]
(
	@PONo nvarchar(35)
)
AS
BEGIN

	--FIND DATA EXISTS 
	DROP TABLE IF EXISTS #ExistsDetail
	SELECT isPicking = 1
		, a.Cust_Code, a.SI_NO, a.SI_Date, a.PO_NO, a.PO_SeqNo
		, a.Item_Code, a.Item_Name, a.Unit_Desc
		, Qty_Shipping	= a.Qty
		, PO_DelivDate
		, Qty_Stock		= ISNULL(b.TM_Current,0)
		, Qty_Picking	= 0
		, Serial_No		= CONCAT(TRIM(a.SerialNo_From),' - ', TRIM(a.SerialNo_To))
		, SerialNoFrom	= a.SerialNo_From
		, SerialNoto	= a.SerialNo_To
	INTO #ExistsDetail
	FROM ShippingInstruction_Master a
	LEFT JOIN Stock_Master b on a.Item_Code = b.Item_Code and b.Warehouse_Code = 'WH-002' 
	WHERE PO_NO = @PONo -- 'KI-11311'	
	--SELECT * FROM #ExistsDetail

	DROP TABLE IF EXISTS #ExistsSerial
	SELECT *
	INTO #ExistsSerial
	FROM ShippingInstruction_Detail
	WHERE  PO_NO = @PONo --'KI-11311'	
	--SELECT * FROM #ExistsSerial

	--UPDATE EXISTS DETAIL
	UPDATE d
	SET d.Qty_Picking = x.Qty_Picking
	FROM #ExistsDetail d
	INNER JOIN (
		SELECT 
			d.PO_NO, 
			d.PO_SeqNo, 
			d.Item_Code, 
			Qty_Picking = COUNT(1)
		FROM #ExistsDetail d
		INNER JOIN #ExistsSerial s 
			ON d.PO_NO = s.PO_NO 
			AND d.PO_SeqNo = s.PO_SeqNo 
			AND d.Item_Code = s.Item_Code
		WHERE ISNULL(s.IsPicking,0) = 1
		GROUP BY 
			d.PO_NO, 
			d.PO_SeqNo, 
			d.Item_Code
	) x
		ON d.PO_NO = x.PO_NO
		AND d.PO_SeqNo = x.PO_SeqNo
		AND d.Item_Code = x.Item_Code

	--FIND DATA NOT EXISTS 		
	DROP TABLE IF EXISTS  #NotExistsDetail
	SELECT
		isPicking		= 0
		, Cust_Code		= od.Cust_Code
		, SI_NO			= ''
		, SI_Date		= NULL
		, od.PO_NO
		, PO_SeqNo		= od.Seq_No
		, od.Item_Code
		, im.Item_Name
		--, od.Unit_Cls
		, Unit_Desc		= u.Description
		, Qty_Shipping	= od.Qty
		, PO_DelivDate	= od.Delivery_Date
		, Qty_Stock		= ISNULL(sm.TM_Current,0)
		, Qty_Picking	= 0
		, Serial_No		= CONCAT(TRIM(od.SerialNoFrom),' - ', TRIM(od.SerialNoto))
		, od.SerialNoFrom
		, od.SerialNoto
	INTO #NotExistsDetail
	FROM OrderEntry_Detail	od
	INNER JOIN Item_Master  im	on od.Item_Code = im.Item_Code
	INNER JOIN Unit_Cls		u	on od.Unit_Cls	= u.Unit_Cls		
	LEFT JOIN Stock_Master	sm	on od.Item_Code = sm.Item_Code and sm.Warehouse_Code = 'WH-002' 
	WHERE od.PO_No = @PONo   --'KI-11311' 
	AND NOT EXISTS (
		SELECT 1 FROM #ExistsDetail ed
		WHERE ed.PO_NO = od.PO_No
		and ed.PO_SeqNo = od.Seq_No
	)
	--SELECT * FROM #NotExistsDetail

	DECLARE @NotExistsSerial TABLE  (
		SI_NO			nvarchar(50)
		,[PO_NO]		nvarchar(50)
		,[PO_SeqNo]		int
		,[Item_Code]	nvarchar(50)
		,[Serial_No]	nvarchar(50)
		,[Address]		nvarchar(255)
		,IsPicking		nvarchar(5)
		,Picking_Date	datetime
		,Picking_By		nvarchar(50)
		,Register_Date	datetime		
		,Register_By	nvarchar(50)
		,Update_Date	datetime		
		,Update_By		nvarchar(50)
	)

	-- declare variables used in cursor
	DECLARE @item_code NVARCHAR(100),		@seq_no int;
	DECLARE @serial_no_from NVARCHAR(50),	@serial_no_to NVARCHAR(50);
 
	-- declare cursor
	DECLARE cursor_serial CURSOR FOR
		SELECT Item_Code
			, PO_SeqNo
			, SerialNoFrom
			, SerialNoto 
		FROM #NotExistsDetail
 
	-- open cursor
	OPEN cursor_serial;
 
	-- loop through a cursor
	FETCH NEXT FROM cursor_serial 
		INTO @item_code, 
			@seq_no,
			@serial_no_from, 
			@serial_no_to;
	WHILE @@FETCH_STATUS = 0
		BEGIN
		
		--PRINT CONCAT('seq_no: ', @seq_no, 'item_code: ', @item_code, ' / serial_no_from: ', @serial_no_from, ' / serial_no_to: ', @serial_no_to);

		INSERT INTO @NotExistsSerial (PO_NO, PO_SeqNo, Item_Code, Serial_No)
		SELECT @PONo
			, @seq_no
			, @item_code
			, Serial_No
		FROM Serial_Detail
		WHERE Serial_No BETWEEN @serial_no_from AND @serial_no_to 
		ORDER BY Serial_No
		
		FETCH NEXT FROM cursor_serial 
			INTO @item_code, 
				@seq_no,
				@serial_no_from, 
				@serial_no_to;
		END;
 
	-- close and deallocate cursor
	CLOSE cursor_serial;
	DEALLOCATE cursor_serial;
	--SELECT * FROM @NotExistsSerial

	--DATA YG MUNCUL
	SELECT * FROM (
		SELECT * FROM #ExistsDetail
		UNION
		SELECT * FROM #NotExistsDetail
	) D ORDER BY isPicking DESC

	SELECT * FROM (
		SELECT * FROM #ExistsSerial
		UNION
		SELECT * FROM @NotExistsSerial
	)S ORDER BY IsPicking DESC
END
GO
