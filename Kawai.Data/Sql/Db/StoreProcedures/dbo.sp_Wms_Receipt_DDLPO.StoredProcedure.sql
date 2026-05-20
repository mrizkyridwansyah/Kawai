


create   procedure [dbo].[sp_Wms_Receipt_DDLPO]
	@Keyword varchar(max) = '',
	@FactoryCode varchar(25),
	@SupplierCode varchar(25),
	@TypeDate varchar(25),
	@PeriodFrom date = null,
	@PeriodUntil date = null,
	@ReceiptId bigint = NULL,
	@ShowOptionAll bit,
	@UserId varchar(25)
as
begin
	declare @tblPO table (Urutan int, PONumber varchar(100))

	declare @tblWarehouseLine table (WarehouseCode varchar(25))

	IF @FactoryCode <> 'ALL'
	BEGIN
		insert into @tblWarehouseLine
		select WarehouseCode From vw_WarehouseLine where FactoryCode = @FactoryCode
	END
	else 
	begin
		insert into @tblWarehouseLine
		select a.WarehouseCode from vw_WarehouseLine a 
		inner join 
		(
			select * From SS_UserWarehousePrivilege where UserID = @UserId and AllowAccess = 1
		) b on a.WarehouseCode = b.WarehouseCode
	end

	if isnull(@TypeDate, '') = 'PO'
	begin
		insert into @tblPO
		select 2, PO_No from PurchaseOrder_Master x inner join @tblWarehouseLine y on x.WHTo = y.WarehouseCode
		where 1=1 and PO_No like '%' + @Keyword + '%' 
		and PO_Date between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = Supplier_Code then 1 else 0 end 
		and isnull(x.Fix_Cls, '0') = '1'
	end
	else if isnull(@TypeDate, '') = 'DELIVERY'
	begin
		insert into @tblPO
		select 2, PO_No from PurchaseOrder_Master x inner join @tblWarehouseLine y on x.WHTo = y.WarehouseCode
		where 1=1 and PO_No like '%' + @Keyword + '%' 
		and Delivery_Date between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = Supplier_Code then 1 else 0 end 
		and isnull(x.Fix_Cls, '0') = '1'
	end
	else
	begin
		insert into @tblPO
		select 2, PO_No from PurchaseOrder_Master x inner join @tblWarehouseLine y on x.WHTo = y.WarehouseCode
		where 1=1 and PO_No like '%' + @Keyword + '%' 
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = Supplier_Code then 1 else 0 end 
		and isnull(x.Fix_Cls, '0') = '1'
	end

	if (select count(1) from @tblPO) > 0
	begin
		insert into @tblPO
		select 1, 'ALL' PONumber WHERE 1 = @ShowOptionAll
	end

	if @ReceiptId is null
	begin
		select distinct po.* From @tblPO po
		inner join PurchaseOrder_Detail pod on po.PONumber = pod.PO_No
		left join 
		(
			select 
			 	PO_No PONumber, Item_Code ItemCode, sum(Qty) ReceiptQty 
			from Part_Receipt 
			WHERE (RefWMSReceiptId is null or isnull(RefWMSReceiptId, 0) <> isnull(@ReceiptId, 0))
			group by PO_No, Item_Code
		) rcpSum on pod.PO_No = rcpSum.PONumber and pod.Item_Code = rcpSum.ItemCode
		WHERE (pod.Qty - isnull(rcpSum.ReceiptQty, 0)) > 0
		UNION ALL
		select 1, 'ALL' PONumber WHERE 1 = @ShowOptionAll
		order by Urutan
	end
	else
	begin
		declare @statusReceipt varchar(20) = (select StatusReceipt From PartReceiptHeader where Id = @ReceiptId)

		IF ISNULL(@statusReceipt, 'NEW') = 'NEW'
		BEGIN			
			select distinct po.* From @tblPO po
			inner join PurchaseOrder_Detail pod on po.PONumber = pod.PO_No
			left join 
			(
				select 
			 		PO_No PONumber, Item_Code ItemCode, sum(Qty) ReceiptQty 
				from Part_Receipt 
				WHERE (RefWMSReceiptId is null or isnull(RefWMSReceiptId, 0) <> isnull(@ReceiptId, 0))
				group by PO_No, Item_Code
			) rcpSum on pod.PO_No = rcpSum.PONumber and pod.Item_Code = rcpSum.ItemCode
			WHERE (pod.Qty - isnull(rcpSum.ReceiptQty, 0)) > 0
			UNION ALL
			select 1, 'ALL' PONumber WHERE 1 = @ShowOptionAll
			order by Urutan

			RETURN
		END		
		ELSE
		BEGIN
			select distinct po.* From @tblPO po
			inner join PurchaseOrder_Detail pod on po.PONumber = pod.PO_No
			INNER join 
			(
				select 
			 		PO_No PONumber, Item_Code ItemCode, sum(Qty) ReceiptQty 
				from Part_Receipt 
				WHERE isnull(RefWMSReceiptId, 0) = isnull(@ReceiptId, 0)
				group by PO_No, Item_Code
			) rcpSum on pod.PO_No = rcpSum.PONumber and pod.Item_Code = rcpSum.ItemCode
			UNION ALL
			select 1, 'ALL' PONumber WHERE 1 = @ShowOptionAll
			order by Urutan

			RETURN
		END
	end
end
