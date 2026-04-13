SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE   procedure [sp_Wms_Andon_Filter_DDL]
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = ''
as
begin
	 
	  
    select ClasificationPart_Cls as AreaCode , Description AreaName 
	from ClasificationPart_Cls
	where 1=1
	 and  Description like '%'+ @Keyword +'%'
end
GO
