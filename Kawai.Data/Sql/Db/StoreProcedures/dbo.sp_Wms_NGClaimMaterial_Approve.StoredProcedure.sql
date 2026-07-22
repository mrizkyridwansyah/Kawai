







create   procedure [dbo].[sp_Wms_NGClaimMaterial_Approve]
	@ClaimId				bigint,
	@DNNumber		varchar(50),
	@SupplierCode	varchar(25),
	@DNDate			date,
	@BCNumber		varchar(50),
	@BCType			varchar(15),
	@BCDate			date,
	@VehicleNo		varchar(15),
	@Transport		varchar(15),
	@Details		tvp_NGClaimDetail READONLY,
	@UpdateBy		varchar(25)
as
begin
 

	if not exists (select 1 from MaterialNGClaimHeader where ClaimID = @ClaimId)
	begin
		raiserror('Data Material NG Claim tidak ditemukan!', 16, 1)
		return
	end

	if exists (select 1 from MaterialNGClaimHeader where ClaimID = @ClaimId and [status] <>'NEW')
	begin
		raiserror('Data Material NG Claim sudah tidak bisa diubah!', 16, 1)
		return
	end

	 

	 

	 
	update MaterialNGClaimHeader 
	set 
		[Status] = 'APPROVED', 
	 	Approved_User = @UpdateBy,
        Approved_Date = Getdate()
		
	where ClaimID = @ClaimId

	---- HAPUS DETAIL LAMA
	--DELETE FROM MaterialNGClaimDetail WHERE ClaimID = @ClaimId

	--declare @ClaimDate date = (select ClaimDate From MaterialNGClaimHeader where ClaimID = @ClaimId)

	--insert into MaterialNGClaimDetail(
	--	     ClaimID
	--		,ItemCode
	--		,PONumber
	--		,ReceiptNo
	--		,QtyNG
	--		,UnitPrice
	--		,Amount
	--		,NGCode
	--		,NGDescription
	--		,RegisterDate
	--		,RegisterUser)
	--	select 
	--		@ClaimId, 
	--		a.ItemCode, 
	--		a.PONumber,
	--		a.[ReceiptNumber], 
	--		a.Qty, 
	--		ISNULL(b.Price, 0), 
	--		a.Qty * ISNULL(b.Price, 0) , 
	--		a.NGCode, 
	--		'' ,Getdate(), 
	--		@UpdateBy
	--	from @Details a left join Price_Master b on a.ItemCode  = b.Item_Code and b.Trade_Code = @SupplierCode and @ClaimDate between[dbo].[ConvertToDateTimeFromString](b.Start_Date) and [dbo].[ConvertToDateTimeFromString](b.End_Date)


 

end
