SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_ShippingInstruction_Insert]
(
	@LastUser nvarchar(35)
	,@Request	dbo.[tvp_ShippingRequest] READONLY
)
AS
BEGIN
	
	BEGIN TRY 

		DECLARE @PONo		nvarchar(50),
				@POSeqNo	int,
				@ItemCode	nvarchar(50),
				@SerialNoFrom nvarchar(50),
				@SerialNoTo	nvarchar(50),
				@SIDate		datetime

		DECLARE cursor_request CURSOR FOR
			SELECT * FROM @Request

		OPEN cursor_request;
 
		-- loop through a cursor
		FETCH NEXT FROM cursor_request 
			INTO @PONo, 
				@POSeqNo, 
				@ItemCode,
				@SerialNoFrom,
				@SerialNoTo,
				@SIDate
		WHILE @@FETCH_STATUS = 0
			BEGIN
				
				DECLARE @SI_NO			nvarchar(100)	= ''
				SELECT @SI_NO = 'SI-'+ @PONo

				INSERT INTO [dbo].[ShippingInstruction_Master]
				(
					[Cust_Code]
				   ,[SI_NO]
				   ,[SI_Date]
				   ,[PO_NO]
				   ,[PO_SeqNo]
				   ,[Item_Code]
				   ,[Item_Name]
				   ,[Unit_Cls]
				   ,[Unit_Desc]
				   ,[Qty]
				   ,[PO_DelivDate]
				   ,[SerialNo_From]
				   ,[SerialNo_To] 
				   ,[Regsister_Date]
				   ,[Register_By]
				)
				SELECT od.Cust_Code
					, @SI_NO
					, @SIDate
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
					, @LastUser
				from OrderEntry_Detail	od
				inner join Item_Master  im	on od.Item_Code = im.Item_Code
				inner join Unit_Cls		u	on od.Unit_Cls	= u.Unit_Cls
				where PO_No = @PONo   --'KI-11311' 
				AND od.Seq_No = @POSeqNo

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
				SELECT @SI_NO
					, @PONo
					, @POSeqNo
					, @ItemCode
					, Serial_No
					, Register_Date	= getdate()
					, Register_By	= @LastUser
				FROM Serial_Detail
				WHERE Serial_No BETWEEN @SerialNoFrom AND @SerialNoTo 
				ORDER BY Serial_No

			FETCH NEXT FROM cursor_request 
				INTO @PONo, 
					@POSeqNo,
					@ItemCode,
					@SerialNoFrom,
					@SerialNoTo,
					@SIDate
			END;
 
		-- close and deallocate cursor
		CLOSE cursor_request;
		DEALLOCATE cursor_request;
	END TRY  
	BEGIN CATCH
		SELECT
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_STATE() AS ErrorState,
			ERROR_SEVERITY() AS ErrorSeverity,
			ERROR_PROCEDURE() AS ErrorProcedure,
			ERROR_LINE() AS ErrorLine,
			ERROR_MESSAGE() AS ErrorMessage;
	END CATCH
	


END
GO
