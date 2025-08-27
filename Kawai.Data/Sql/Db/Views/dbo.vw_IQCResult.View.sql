SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER VIEW [vw_IQCResult]
as
select 'PASS' Code, 'Pass' Description
union all
select 'NG' Code, 'NG' Description
GO
