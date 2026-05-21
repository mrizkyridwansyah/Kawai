



create   procedure [dbo].[sp_Wms_ReceiptUnschedule_Create]
	@ReceiptNo		varchar(50),
	@ReceiptDate	date,
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
	@Details		tvp_ReceiptUnscheduleDetail20260519 READONLY,
	@RegisterBy		varchar(25)
as
begin
	if @ReceiptDate < cast(DAteAdd(day, -1,getdate()) as date)
	begin
		raiserror('Receipt Date tidak boleh back date!', 16, 1)
		return
	end

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

	begin transaction receiptTransaction
	begin try
		DECLARE @registerNox TABLE (RegisterNo VARCHAR(100))
		DECLAre @registerNo varchar(100)

		IF @BCType NOT IN ('BC 2.3', 'BC 2.6.2', 'BC 4.0')
		BEGIN
			INSERT INTO @registerNox
			EXEC sp_GetNoRegister @ReceiptDate, 'R', @RegisterBy

			SELECT top 1 @registerNo = RegisterNo FROM @registerNox
		END

		insert into PartReceiptHeader 
		(ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, RegisterDate, RegisterUser, IsManual, Transport, Remarks, SourceMenu, CompanyCode, ReferenceNo, RegisterNo)
		values 
		(@ReceiptNo, @ReceiptDate, @SupplierCode, @DNNumber, @DNDate, @BCNumber, @BCType, @BCDate, @VehicleNo, getdate(), @RegisterBy, 1, @Transport, '', 'RECEIPT UNSCHEDULE', @FactoryCode, @ReferenceNo, @registerNo)

		declare @newid bigint = (select SCOPE_IDENTITY())

		insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, Remarks, QtyPacking, NoSeri)
		select @newid, @ReceiptDate, NULL, a.ItemCode, isnull(isp.UnitCls, mi.Unit_Cls), 0, CEILING(CAST(a.ReceiptQty AS FLOAT) / isnull(isp.QtyPacking, mi.Number_Box)), a.ReceiptQty, '', isnull(isp.QtyPacking, mi.Number_Box), a.NoSeri
		from @Details a
		left join ItemSupplierPacking isp on a.ItemCode = isp.ItemCode and isp.SupplierCode = @SupplierCode
		left join Item_Master mi on a.ItemCode = mi.Item_Code

		declare @seqNo int = (isnull((select max(Seq_No) From Part_Receipt with (updlock, holdlock)), 0))
		DECLARE @BCTypeVal varchar(100) = (SELECT Description fROM BCType_Cls	WHERE BCType_Cls = @BCType)

		declare @priceTable table (Currency_Code varchar(50), Item_Code varchar(25), Price numeric(18,9), StartDate date, EndDate date)
		insert into @priceTable
		SELECT Currency_Code, Item_Code, Price, dbo.ConvertToDateTimeFromString(Start_Date), dbo.ConvertToDateTimeFromString(End_Date)
		fROM Price_Master where Trade_Code = @SupplierCode and Price_Cls = '01'

		insert into Part_Receipt 
		(
			Seq_No, Supplier_Code, PO_No, Warehouse_Code, Address, Receipt_Cls, Receipt_Date, Item_Code, Qty, SerialNoFrom, SerialNoTo, 
			Unit_Cls, Currency_Code, Price, Amount, SuratJalan_No, ProductionResult_Cls, DailySeq_No, Remarks, Transport_Cls,
			Last_Update, Last_User, Register_Date, BC_Type, BC40_No, BC40_Date, Receipt_Status, No_Register, RefWMSReceiptId, No_Seri
		)
		select 
			@seqNo + ROW_NUMBER() OVER (ORDER BY dtl.Id), hd.SupplierCode, '' , it.WH_Code, '' [Address], 'R', @ReceiptDate, dtl.ItemCode, dtl.ReceiptQty, null SerialNoFrom, null SerialNoTo,  
			dtl.UnitCls, pm.Currency_Code, pm.Price, pm.Price * dtl.ReceiptQty , hd.DNNumber, 0, null DailySeq_No, '', @Transport,
			getdate(), @RegisterBy, getdate(), isnull(@BCTypeVal, hd.BCType), hd.BCNumber, hd.BCDate, null Receipt_Status, @registerNo, hd.Id, dtl.NoSeri
		From PartReceiptHeader hd
		inner join PartReceiptDetail dtl on hd.Id = dtl.ReceiptId
		left join Item_Master it on dtl.ItemCode = it.Item_Code
		left join @priceTable pm on dtl.ItemCode = pm.Item_Code and hd.ReceiptDate between pm.StartDate and pm.EndDate
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
