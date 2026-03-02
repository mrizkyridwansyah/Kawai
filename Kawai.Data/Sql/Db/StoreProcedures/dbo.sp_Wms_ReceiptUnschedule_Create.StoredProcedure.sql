SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   procedure [sp_Wms_ReceiptUnschedule_Create]
	@ReceiptNo		varchar(50),
	@DNNumber		varchar(50),
	@FactoryCode	varchar(25),
	@SupplierCode	varchar(25),
	@DNDate			date,
	@BCNumber		varchar(50),
	@BCType			varchar(15),
	@BCDate			date,
	@VehicleNo		varchar(15),
	@Transport		varchar(15),
	@ReferenceNo	varchar(50),
	@Details		tvp_ReceiptUnscheduleDetail READONLY,
	@RegisterBy		varchar(25)
as
begin
	if exists 
	(
		select * From @Details a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		left join Item_Master mi on a.ItemCode = mi.Item_Code
		where isnull(b.QtyPacking, mi.Number_Box) <= 0
	)
	begin
		raiserror('Qty Packing Item ini belum disetting!', 16, 1)
		return
	end

	if not exists (select 1 from SS_UserFactoryPrivilege where UserID = @RegisterBy and isnull(AllowAccess, 0) = 1)
	begin
		raiserror('User tidak memiliki hak akses ke factory ini!', 16, 1)
		return
	end

	declare @ReceiptDate date = getdate()

	begin transaction receiptTransaction
	begin try
		insert into PartReceiptHeader 
		(ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, RegisterDate, RegisterUser, IsManual, Transport, Remarks, SourceMenu, CompanyCode, ReferenceNo)
		values 
		(@ReceiptNo, @ReceiptDate, @SupplierCode, @DNNumber, @DNDate, @BCNumber, @BCType, @BCDate, @VehicleNo, getdate(), @RegisterBy, 1, @Transport, '', 'RECEIPT UNSCHEDULE', @FactoryCode, @ReferenceNo)

		declare @newid bigint = (select SCOPE_IDENTITY())

		insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, Remarks)
		select @newid, @ReceiptDate, NULL, a.ItemCode, isnull(isp.UnitCls, mi.Unit_Cls), 0, CEILING(CAST(a.ReceiptQty AS FLOAT) / isnull(isp.QtyPacking, mi.Number_Box)), a.ReceiptQty, '' 
		from @Details a
		left join ItemSupplierPacking isp on a.ItemCode = isp.ItemCode and isp.SupplierCode = @SupplierCode
		left join Item_Master mi on a.ItemCode = mi.Item_Code

		declare @seqNo int = (isnull((select max(Seq_No) From Part_Receipt with (updlock, holdlock)), 0))

		insert into Part_Receipt 
		(
			Seq_No, Supplier_Code, PO_No, Warehouse_Code, Address, Receipt_Cls, Receipt_Date, Item_Code, Qty, SerialNoFrom, SerialNoTo, 
			Unit_Cls, Currency_Code, Price, Amount, SuratJalan_No, ProductionResult_Cls, DailySeq_No, Remarks, Transport_Cls,
			Last_Update, Last_User, Register_Date, BC_Type, BC40_No, BC40_Date, Receipt_Status, No_Register, RefWMSReceiptId
		)
		select 
			@seqNo + ROW_NUMBER() OVER (ORDER BY dtl.Id), hd.SupplierCode, '' , it.WH_Code, '' [Address], 'R', @ReceiptDate, dtl.ItemCode, dtl.ReceiptQty, null SerialNoFrom, null SerialNoTo,  
			dtl.UnitCls, pm.Currency_Code, pm.Price, pm.Price * dtl.ReceiptQty , hd.DNNumber, 0, null DailySeq_No, '', @Transport,
			getdate(), @RegisterBy, getdate(), hd.BCType, hd.BCNumber, hd.BCDate, null Receipt_Status, hd.ReceiptNo, hd.Id
		From PartReceiptHeader hd
		inner join PartReceiptDetail dtl on hd.Id = dtl.ReceiptId
		left join Item_Master it on dtl.ItemCode = it.Item_Code
		left join Price_Master pm on dtl.ItemCode = pm.Item_Code and hd.SupplierCode = pm.Trade_Code and Price_Cls = '01'
		where hd.Id = @newid

		select @newid

		commit transaction receiptTransaction
	end try
	begin catch
		rollback transaction receiptTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end
GO
