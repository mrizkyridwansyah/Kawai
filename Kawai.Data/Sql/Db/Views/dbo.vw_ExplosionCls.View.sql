SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   view [vw_ExplosionCls]
as
select '01' Code, 'All' Description
union all
select '02' Code, '1 Level' Description
GO
