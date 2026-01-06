SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   view [vw_MakeOrBuyCls]
as
select '01' Code, 'Make' Description
union all
select '02' Code, 'Buy' Description
GO
