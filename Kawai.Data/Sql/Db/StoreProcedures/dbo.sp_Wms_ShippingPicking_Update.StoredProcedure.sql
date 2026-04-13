SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     PROCEDURE [sp_Wms_ShippingPicking_Update]
(
	@LastUser	nvarchar(50),
	@Request	dbo.[tvp_PickingRequest] READONLY
)
AS
BEGIN
	
	BEGIN TRY 

		DECLARE @PONo		nvarchar(50),
				@POSeqNo	int,
				@ItemCode	nvarchar(50),				
				@SerialNo nvarchar(50)

		DECLARE cursor_request CURSOR FOR
			SELECT * FROM @Request

		OPEN cursor_request;
 
		-- loop through a cursor
		FETCH NEXT FROM cursor_request 
			INTO @PONo, 
				@POSeqNo, 
				@ItemCode,
				@SerialNo
		WHILE @@FETCH_STATUS = 0
			BEGIN
				
			--VALIADI CURRENT STOCK
			DECLARE @curr_stock numeric(18,5)
					,@msgError nvarchar(max)
			
			SELECT @curr_stock		= ISNULL(TM_Current,0)
			FROM Stock_Master
			WHERE Warehouse_Code	= 'WH-002'
			AND Item_Code			= @ItemCode

			IF(@curr_stock = 0)
			BEGIN
				SELECT @msgError = CONCAT('Tidak bisa melakukan picking, karen Currenct Stock ', @ItemCode, ' 0')
				RAISERROR(@msgError,16,1)
				RETURN 
			END

			--LOLOS VALIDASI, UPDATE PICKING			
			UPDATE ShippingInstruction_Detail
			SET IsPicking		= 1
				,Update_By		= @LastUser
				,Update_Date	= getdate()
			WHERE PO_NO		= @PONo
			AND PO_SeqNo	= @POSeqNo
			AND Item_Code	= @ItemCode
			AND Serial_No	= @SerialNo

			FETCH NEXT FROM cursor_request 
				INTO @PONo, 
					@POSeqNo,
					@ItemCode,
					@SerialNo
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
