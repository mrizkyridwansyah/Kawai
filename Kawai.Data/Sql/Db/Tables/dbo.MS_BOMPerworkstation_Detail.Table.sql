SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_BOMPerworkstation_Detail](
	[DetailID] [bigint] IDENTITY(1,1) NOT NULL,
	[Bomws_ID] [bigint] NOT NULL,
	[ChildItem_Code] [char](25) NOT NULL,
	[Unit_Cls] [char](2) NOT NULL,
	[Qty] [numeric](9, 5) NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
