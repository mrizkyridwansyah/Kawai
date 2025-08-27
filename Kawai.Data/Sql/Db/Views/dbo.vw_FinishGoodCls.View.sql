SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER VIEW [vw_FinishGoodCls]
as
select '01' Code, 'Finish Goods' Description
union all
select '02' Code, 'Parts/WIP/Material' Description
GO
