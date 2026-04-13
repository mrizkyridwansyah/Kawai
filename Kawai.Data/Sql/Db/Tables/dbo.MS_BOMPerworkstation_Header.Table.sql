SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_BOMPerworkstation_Header](
	[Bomws_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Line_Code] [char](25) NOT NULL,
	[ParentItemCode] [char](25) NOT NULL,
	[WorkStationCode] [char](15) NOT NULL,
	[MAX_Qty_Set] [int] NOT NULL,
	[Troly_Cls] [varchar](2) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
