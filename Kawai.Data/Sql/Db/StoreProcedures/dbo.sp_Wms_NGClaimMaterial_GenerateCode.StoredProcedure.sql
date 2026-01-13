SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_NGClaimMaterial_GenerateCode]
as
begin

	DECLARE	@ClaimNo varchar(50), @prefixClaimNo varchar(12) = 'CL.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixClaimNo, @LengthSequence = 4, @Result = @ClaimNo OUTPUT;			

	SELECT @ClaimNo  
end
GO
