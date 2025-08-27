SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_HS_DDL]
	@Keyword varchar(max)
as
select HS_Code [HSCode] From HS_Master where HS_Code like '%'+ @Keyword +'%'
GO
