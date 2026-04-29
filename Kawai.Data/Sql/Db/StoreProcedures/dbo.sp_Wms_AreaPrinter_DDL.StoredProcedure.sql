






CREATE  procedure [dbo].[sp_Wms_AreaPrinter_DDL]
	@Keyword varchar(max) = '' 
as
begin
select IPAddress , [Description] ,  IPAddress +' | '+  [Description]  DDLDescription from (
Select '192.168.0.134' IPAddress , 'Printer_Name' [Description]
) A where   (IPAddress like '%'+ @Keyword +'%' or [Description] like '%'+ @Keyword +'%')
end 


 

