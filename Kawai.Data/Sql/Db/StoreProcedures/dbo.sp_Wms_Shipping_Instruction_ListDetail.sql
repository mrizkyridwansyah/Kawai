CREATE procedure [dbo].[sp_Wms_Shipping_Instruction_ListDetail]
	--Declare
	@DeliveryFrom Datetime,
    @DeliveryTo Datetime,
    @PONumber Varchar(100),
    @ShippingInstructionNo Varchar(100),
    @Supplier Varchar(100) 
 

 
as
begin
    IF EXISTS (select TOP 1 1 F from ShippingInstruction_Master where SI_NO = @ShippingInstructionNo)
	BEGIN
	Select 
	    1 SIDetailID,
		 Cust_Code as Supplier
		,SI_NO as ShippingInstructionNo 
		,SI_Date as ShippingInstructionDate
		,PO_NO as PONumber
		,PO_SeqNo
		,Item_Code
		,Item_Name
		,Unit_Cls
		,Unit_Desc
		,Qty
		,PO_DelivDate as DeliveryDate
		, Qty_Stock		= ISNULL(sm.TMCurrent,0)
		, Qty_Picking	= 0
		, Serial_No		= CONCAT(TRIM( SerialNo_From),' - ', TRIM( SerialNo_To))
		, SerialNo_From	=  SerialNo_From
		, SerialNo_to	=  SerialNo_To
		 
    from ShippingInstruction_Master a
	LEFT JOIN (select ItemCode , SUM(TMCurrent) TMCurrent from StockHeader where WarehouseCode = 'WH-002-FG' Group by ItemCode ) sm	on a.Item_Code = sm.ItemCode  
	where SI_NO = @ShippingInstructionNo order by PO_SeqNo
	END
	ELSE
	BEGIN
		SELECT
	      1 SIDetailID 
		, Supplier		= od.Cust_Code
		, ShippingInstructionNo			= ''
		, ShippingInstructionDate		= NULL
		, PONumber = od.PO_NO
		, PO_SeqNo		= od.Seq_No
		, od.Item_Code
		, im.Item_Name
		, Unit_Desc		= u.Description
		, Qty	= od.Qty
		, DeliveryDate	= od.Delivery_Date
		, Qty_Stock		= ISNULL(sm.TMCurrent,0)
		, Qty_Picking	= 0
		, Serial_No		= CONCAT(TRIM(od.SerialNoFrom),' - ', TRIM(od.SerialNoto))
		, od.SerialNoFrom
		, od.SerialNoto
	FROM OrderEntry_Detail	od
	INNER JOIN Item_Master  im	on od.Item_Code = im.Item_Code
	INNER JOIN Unit_Cls		u	on od.Unit_Cls	= u.Unit_Cls		
	LEFT JOIN (select ItemCode , SUM(TMCurrent) TMCurrent from StockHeader where WarehouseCode = 'WH-002-FG' Group by ItemCode ) sm	on od.Item_Code = sm.ItemCode  
	WHERE od.PO_No = @PONumber  order by PO_SeqNo
	 

	END
 
end

