SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_StockCalculate_ReceiptBatch]
	@SourceRef varchar(50)
as
begin
    SET NOCOUNT ON;

    DECLARE @ErrorMessage VARCHAR(MAX);

    BEGIN TRY
        BEGIN TRANSACTION StockMutationRectipTransaction;

		declare @tblTemp table (Urutan int, Id bigint)
		insert into @tblTemp
		select ROW_NUMBER() over(order by Id), Id From StockMutation where SourceRef = @SourceRef

		declare @i int = 1
		while @i <= (select count(1) from @tblTemp)
		begin
			declare @Id bigint = (select Id From @tblTemp where Urutan = @i)
			exec sp_Wms_StockCalculate_Receipt @Id

			set @i += 1
		end

		update PartReceiptHeader set HasValid = 1, ValidDate = getdate() where Id = @SourceRef
		update PartReceiptDetail set HasValid = 1, ValidDate = getdate() where ReceiptId = @SourceRef
		update PartReceiptDetailBarcode set HasValid = 1, ValidDate = getdate() where ReceiptId = @SourceRef

        COMMIT TRANSACTION StockMutationRectipTransaction;
	END TRY
	BEGIN CATCH
        ROLLBACK TRANSACTION StockMutationRectipTransaction;
		set @ErrorMessage = (select ERROR_MESSAGE()) 
		update StockMutation set HasCalculate = 1, StatusCalculate = 'FAILED', ErrorMessage = @ErrorMessage, CalculateDate = getdate() where SourceRef = @SourceRef
		raiserror(@ErrorMessage, 16, 1)
		return
	END CATCH
	
end



GO
