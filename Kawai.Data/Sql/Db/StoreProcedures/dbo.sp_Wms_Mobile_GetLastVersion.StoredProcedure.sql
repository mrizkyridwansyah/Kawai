SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Mobile_GetLastVersion]
as
SELECT [Version] FROM vw_MobileLastVersion
GO
