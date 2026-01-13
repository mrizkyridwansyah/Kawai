SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [sp_Wms_NGClaimMaterial_Create]
	@ClaimNo		varchar(50),
	@DNNumber		varchar(50),
	@SupplierCode	varchar(25),
	@DNDate			date,
	@BCNumber		varchar(50),
	@BCType			varchar(15),
	@BCDate			date,
	@VehicleNo		varchar(15),
	@Transport		varchar(15),
	@Details		tvp_NGClaimDetail READONLY,
	@RegisterBy		varchar(25)
as

begin
	 
 

	declare @ClaimDate date = getdate()

	begin transaction ClaimTransaction
	begin try
		insert into MaterialNGClaimHeader 
		(ClaimNo, SupplierCode, SupplierName, ClaimDate, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, RegisterDate, RegisterUser,  Transport,  [Status])
		values 
		(@ClaimNo, @SupplierCode, @SupplierCode, @ClaimDate, @DNNumber, @DNDate, @BCNumber, @BCType, @BCDate, @VehicleNo, getdate(), @RegisterBy,  @Transport, 'NEW')



		declare @newid bigint = (select SCOPE_IDENTITY())
		--select * from Material
		insert into MaterialNGClaimDetail(
		     ClaimID
			,ItemCode
			,PONumber
			,ReceiptNo
			,QtyNG
			,UnitPrice
			,Amount
			,NGCode
			,NGDescription
			,RegisterDate
			,RegisterUser)
		select 
			@newid, 
			a.ItemCode, 
			a.PONumber,
			a.[ReceiptNumber], 
			a.Qty, 
			  ISNULL(b.Price, 0), 
			a.Qty *    ISNULL(b.Price, 0) , 
			a.NGCode, 
			'' ,Getdate(), 
			@RegisterBy
		from @Details a left join Price_Master b on a.ItemCode  = b.Item_Code and b.Trade_Code = @SupplierCode and @ClaimDate between[dbo].[ConvertToDateTimeFromFuckingString](b.Start_Date) and [dbo].[ConvertToDateTimeFromFuckingString](b.End_Date)

		select @newid

		commit transaction ClaimTransaction
	end try
	begin catch
		rollback transaction ClaimTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end
GO
