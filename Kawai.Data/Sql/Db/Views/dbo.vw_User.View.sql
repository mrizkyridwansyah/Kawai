SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [vw_User]
as
select 'WMS' AppID, UserID, FullName From SS_UserSetup
union all
select App_ID, Username, Name from User_Setup
GO
