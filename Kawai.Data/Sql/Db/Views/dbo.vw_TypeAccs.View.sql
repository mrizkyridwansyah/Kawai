SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER VIEW [vw_TypeAccs]
as

select '01' Code, 'Set' Description 
union all
select '02' Code, 'FG' Description 
GO
