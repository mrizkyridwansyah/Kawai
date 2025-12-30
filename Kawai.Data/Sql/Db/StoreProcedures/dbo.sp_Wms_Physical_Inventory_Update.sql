SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create or ALTER   procedure [dbo].[sp_Wms_Physical_Inventory_Update]
(
	@WarehouseCode	varchar(25),
	@ItemCode		varchar(25),
	@Period			date,	
	@Inventory		decimal(18,2),
	@Reason			nvarchar(max),
	@LastUser		nvarchar(50)
)
as
begin

	--VALIDASI PERIOD
	declare @dateRange int

	select @dateRange = dbo.fn_Wms_GetDateRange(@Period) 
	--select @dateRange = dbo.fn_Wms_GetDateRange('2025-12-01')

	if(@dateRange = -1)
	begin 
		raiserror('Period tidak sesuai!!!',16,1)
		return
	end
	
	PRINT @dateRange
	-------------
	
	SELECT @Inventory	= ISNULL(@Inventory,0)
	SELECT @Reason		= ISNULL(@Reason,'')

	IF(@dateRange = 0)   --LAST MONTH (LM)
	BEGIN 
		UPDATE stock_master WITH (UPDLOCK)
		SET
			lm_inventory = @Inventory,
			tm_premonth  = @Inventory,
			tm_current   = tm_receipt - tm_supply - tm_lossreject + @Inventory,
			nm_premonth  = tm_receipt - tm_supply - tm_lossreject + @Inventory,
			nm_current   = tm_receipt - tm_supply - tm_lossreject 
						   + nm_receipt - nm_supply - nm_lossreject + @Inventory,
			lm_reason    = @Reason,
			Last_Update  = GETDATE(),
			Last_User    = @LastUser
		WHERE
			warehouse_code = @WarehouseCode
			AND item_code  = @ItemCode;
	END

	IF(@dateRange = 1) --THIS MONTH (TM)
	BEGIN 
		UPDATE stock_master WITH (UPDLOCK)
		SET
			tm_inventory = @Inventory,
			tm_reason    = @Reason,
			nm_premonth  = @Inventory,
			nm_current   = @Inventory + nm_receipt - nm_supply - nm_lossreject,
			Last_Update  = GETDATE(),
			Last_User    = @LastUser
		WHERE
			warehouse_code = @WarehouseCode
			AND item_code  = @ItemCode;
	END


	IF(@dateRange = 2) -- NEXT MONTH (NM)
	BEGIN 
		UPDATE stock_master WITH (UPDLOCK)
		SET
			nm_inventory = @Inventory,
			nm_reason    = @Reason,
			Last_Update  = GETDATE(),
			Last_User    = @LastUser
		WHERE
			warehouse_code = @WarehouseCode
			AND item_code  = @ItemCode;
	END
end