CREATE  PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_Save_Picking]
(
    @ShippingInstructionNo  varchar(100)='SI-KI3-eee',
    @ItemCode  varchar(100)='100T3171',
    @PONumber  varchar(100)='KI3-14968' ,
    @PO_SeqNo int= 1,
	@Details	dbo.[tvp_ShippingInstructionPicking] READONLY,
	@PickingBy	nvarchar(50)
)
AS
BEGIN
	
	BEGIN TRY 

	 
	 Declare @SerialNo Varchar(100), @Picking bit = 0

		 IF NOT EXISTS (  Select Top 1 1 
						   from ShippingInstruction_Detail A  
						   Inner JOIN @Details B ON A.Serial_No = B.SerialNo where A.SI_No = @ShippingInstructionNo 
	                    )

						BEGIN
						RAISERROR('Please Create Shipping Instuction Before Picking Process !!!',16,1)
				        RETURN 
						END

		DECLARE cursor_request CURSOR FOR
		SELECT * FROM @Details

		OPEN cursor_request;
 
		-- loop through a cursor
		FETCH NEXT FROM cursor_request 
			INTO @SerialNo, 
				@Picking 
		WHILE @@FETCH_STATUS = 0
			BEGIN
				
			--VALIADI CURRENT STOCK
			DECLARE @curr_stock numeric(18,5)
					,@msgError nvarchar(max)
			
			SELECT @curr_stock		= ISNULL(SUM(TMCurrent),0)
			FROM StockHeader
			WHERE WarehouseCode	= 'WH-002-FG'
			AND ItemCode			= @ItemCode
			group by ItemCode

			IF(@curr_stock = 0)
			BEGIN
				SELECT @msgError = CONCAT('Cannot picking process, this Currenct Stock ', @ItemCode, ' 0')
				RAISERROR(@msgError,16,1)
				RETURN 
			END

			--LOLOS VALIDASI, UPDATE PICKING			
			UPDATE ShippingInstruction_Detail
			SET IsPicking		= 1
			    ,Picking_By     = Case when ISNULL(IsPicking,'0') ='0' then  @PickingBy else Picking_By end 
				,Picking_Date   =  Case when ISNULL(IsPicking,'0') ='0' then  Getdate() else Picking_Date end   
				,Update_By		=  Case when ISNULL(IsPicking,'0') ='0' then  @PickingBy else Update_By end  
				,Update_Date	= Case when ISNULL(IsPicking,'0') ='0' then  getdate() else Update_Date end   
			WHERE PO_NO		= @PONumber
			AND PO_SeqNo	= @PO_SeqNo
			AND Item_Code	= @ItemCode
			AND Serial_No	= @SerialNo
			

			FETCH NEXT FROM cursor_request 
				INTO  @SerialNo, 
				      @Picking 
			END;
 
		-- close and deallocate cursor
		CLOSE cursor_request;
		DEALLOCATE cursor_request;
	END TRY  
	BEGIN CATCH
		declare @msg varchar(max) = ERROR_MESSAGE();
		raiserror(@msg,16,1 );
		return;
	END CATCH
END
