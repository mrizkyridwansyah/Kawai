CREATE PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_Create]
	@ShippingInstructionNo		varchar(50),
	@ShippingInstructionDate	Date,
	@PONumber		varchar(50),
	@Supplier       Varchar(50),
	@Details		tvp_ShippingInstructionDetails READONLY,
	@RegisterBy		varchar(25)
as

begin
 

	 
	begin transaction ShippingInstructionTransaction
	begin try
		insert into ShippingInstruction_Master 
		(Cust_Code,SI_NO,SI_Date,PO_NO,PO_SeqNo,Item_Code,Item_Name,Unit_Cls,Unit_Desc,Qty,PO_DelivDate,SerialNo_From,SerialNo_To,Regsister_Date,Register_By)
        SELECT       od.Cust_Code
					, @ShippingInstructionNo
					, @ShippingInstructionDate
					, od.PO_No
					, od.Seq_No
					, od.Item_Code
					, im.Item_Name
					, od.Unit_Cls
					, Unit_Desc			= u.Description
					, od.Qty
					, od.Delivery_Date
					, SerialNoFrom
					, SerialNoto
					, getdate()
					, @RegisterBy
				from OrderEntry_Detail	od
				inner join Item_Master  im	on od.Item_Code = im.Item_Code
				inner join Unit_Cls		u	on od.Unit_Cls	= u.Unit_Cls
				inner join @Details dd      on dd.Item_Code = od.Item_Code 	AND od.Seq_No = dd.PO_SeqNo
				 
			   INSERT INTO ShippingInstruction_Detail
				(	
					[SI_No]
				   ,[PO_NO]
				   ,[PO_SeqNo]
				   ,[Item_Code]
				   ,[Serial_No]
				   ,[Register_Date]
				   ,[Register_By]
			   )
				SELECT @ShippingInstructionNo
					, @PONumber
					, BB.PO_SeqNo
					, BB.Item_Code
					, AA.Serial_No
					, Register_Date	= getdate()
					, Register_By	= @RegisterBy
				FROM Serial_Detail AA  
				INNER JOIN @Details BB ON BB.Item_Code = AA.Item_Code and BB.PO_SeqNo = AA.PO_SeqNo
				WHERE AA.Serial_No BETWEEN BB.SerialNo_From AND BB.SerialNo_To 
				ORDER BY Serial_No

				if NOT EXISTS(SELECT TOP 1 1 FROM Serial_Detail AA  INNER JOIN @Details BB ON BB.Item_Code = AA.Item_Code and BB.PO_SeqNo = AA.PO_SeqNo WHERE AA.Serial_No BETWEEN BB.SerialNo_From AND BB.SerialNo_To)
				begin
					INSERT INTO ShippingInstruction_Detail
					(	
						[SI_No]
					   ,[PO_NO]
					   ,[PO_SeqNo]
					   ,[Item_Code]
					   ,[Serial_No]
					   ,[Register_Date]
					   ,[Register_By]
				   )
					SELECT @ShippingInstructionNo
						, od.PO_No
						, od.Seq_No
						, od.Item_Code
						, ''
						, getdate()
						, @RegisterBy
					from OrderEntry_Detail	od
					inner join Item_Master  im	on od.Item_Code = im.Item_Code
					inner join Unit_Cls		u	on od.Unit_Cls	= u.Unit_Cls
					INNER JOIN @Details BB ON  BB.PO_SeqNo = od.Seq_No
					where PO_No = @PONumber   --'KI-11311' 
			    END 
	 

		commit transaction ShippingInstructionTransaction
	end try
	begin catch
		rollback transaction ShippingInstructionTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end

