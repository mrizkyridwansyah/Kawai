SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   view [vw_NGCls]
as
select '01' Code, 'Yes' Description 
union all
select '02' Code, 'No' Description 
GO
