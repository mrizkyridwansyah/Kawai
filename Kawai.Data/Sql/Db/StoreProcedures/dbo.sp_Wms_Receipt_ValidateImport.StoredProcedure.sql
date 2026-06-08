CREATE PROCEDURE [dbo].[sp_Wms_Receipt_ValidateImport]
      @SupplierCode VARCHAR(15)		
    , @DNNumber VARCHAR(50)			
    , @ReceiptDate DATE				
    , @BCType VARCHAR(15)			
    , @BCNumber VARCHAR(50)			
    , @BCDate DATE					
	, @DataImport tvp_ReceiptImport READONLY 
	, @UserId varchar(25)
as
begin	
	DECLARE @ResultHeader TABLE(
	    SupplierCode VARCHAR(15),
        DNNumber VARCHAR(50),
        ReceiptDate DATE,
        BCType VARCHAR(15),
        BCNumber VARCHAR(50),
        BCDate DATE,
		Errors varchar(max)
	)

	DECLARE @ResultDetail TABLE(
	    [PONumber] [varchar](25) NOT NULL,
	    [ItemCode] [varchar](25) NOT NULL,
	    [ReceiptQty] [int] NULL,
		[RowNumber] [int] NULL,
		[Errors] varchar(max)
	)

	insert into @ResultHeader
	select  @SupplierCode , @DNNumber , @ReceiptDate , @BCType , @BCNumber , @BCDate , ''

	insert into @ResultDetail
	select * from @DataImport

	UPDATE a
		SET Errors = CONCAT(
				Errors,
				CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
				'Supplier Code tidak terdaftar.'
			)
		FROM @ResultHeader a
		Left join Trade_Master b on a.SupplierCode = b.Trade_Code
	    where b.Trade_Code is null;

	UPDATE a
	SET Errors = CONCAT(
			Errors,
			CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
			'BC Type tidak terdaftar.'
		)
	FROM @ResultHeader a
	LEFT JOIN BCType_Cls b ON a.BCType = b.BCType_Cls
	WHERE b.BCType_Cls IS NULL;

	UPDATE a
	SET Errors = CONCAT(
			Errors,
			CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
			'DN Number / Surat Jalan Sudah Terdaftar.'
		)
	FROM @ResultHeader a
	LEFT JOIN PartReceiptHeader b ON a.DNNumber = b.DNNumber
	WHERE b.DNNumber IS Not NULL;
 
	select * from @ResultHeader  

	declare @tempValidate1 table 
	(
		[PONumber] [varchar](25),
		[ItemCode] [varchar](25) ,
		[ExcelCount] int
	)

	declare @tempValidate2 table 
	(
		[PONumber] [varchar](25) 
	)

	insert into @tempValidate1
	select [PONumber], [ItemCode],count([ItemCode]) from @ResultDetail 
	group by [PONumber], [ItemCode]
	having count([ItemCode]) > 1

	if exists (select 1 from @tempValidate1)
	begin
		update a 
		SET Errors = CONCAT(
				Errors,
				CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
				'Product Item tidak boleh duplikat dalam 1 PO Number.'
			)
		from @ResultDetail a
		inner join @tempValidate1 b on a.PONumber = b.PONumber and a.ItemCode = b.ItemCode
	end

	update a 
	SET Errors = CONCAT(
			Errors,
			CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
			'Product Item tidak terdaftar dalam Item Master. '
		)
	from @ResultDetail a
	Left join Item_Master b on  a.ItemCode = b.Item_Code
	WHERE b.Item_Code is NULL


	update a 
	SET Errors = CONCAT(
			Errors,
			CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
			'PO Number tidak terdaftar dalam Purchase Order Master. '
		)
	from @ResultDetail a
	Left join PurchaseOrder_Master b on  a.PONumber = b.PO_No
	WHERE b.PO_No is NULL

	update a 
	SET Errors = CONCAT(
			Errors,
			CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
			'Supplier Code tidak terdaftar dalam Purchase Order Master. '
		)
	from @ResultDetail a
	Left join PurchaseOrder_Master b on  a.PONumber = b.PO_No and b.Supplier_Code = @SupplierCode
	WHERE b.PO_No is NULL

	update a 
	SET Errors = CONCAT(
			Errors,
			CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
			'Qty Packing Item ini belum disetting!. '
		)
	from @ResultDetail a
	left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
	left join Item_Master mi on a.ItemCode = mi.Item_Code
	where isnull(b.QtyPacking, mi.Number_Box) <= 0

	Insert into @tempValidate2
	select  RTRIM(a.PO_No) PONumber From PurchaseOrder_Detail a
	inner join Item_Master mi on a.Item_Code = mi.Item_Code
	left join 
	(
		select 
			prd.PONumber, prd.ItemCode, sum(prd.ReceiptQty) ReceiptQty 
		from PartReceiptDetail prd
		inner join @ResultDetail dl on prd.PONumber = dl.PONumber and prd.ItemCode = dl.ItemCode
		group by prd.PONumber, prd.ItemCode
	) rcpSum on a.PO_No = rcpSum.PONumber and a.Item_Code = rcpSum.ItemCode
	inner join @ResultDetail dtl on a.PO_No = dtl.PONumber and a.Item_Code = dtl.ItemCode
	where a.Qty - (isnull(rcpSum.ReceiptQty, 0) + dtl.ReceiptQty) < 0
	 

	if exists (select 1 from @tempValidate2)
	begin
		update a 
		SET Errors = CONCAT(
				Errors,
				CASE WHEN ISNULL(Errors,'') <> '' THEN CHAR(13)+CHAR(10) ELSE '' END,
				'Qty receipt must be lower then purchase qty. '
			)

		from @ResultDetail a
		inner join @tempValidate2 b on a.PONumber = b.PONumber  
	end

	Select * from @ResultDetail  	
end
