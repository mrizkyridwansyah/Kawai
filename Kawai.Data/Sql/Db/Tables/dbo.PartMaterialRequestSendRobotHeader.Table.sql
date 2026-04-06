SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestSendRobotHeader](
	[RequestSendID] [varchar](50) NOT NULL,
	[RequestID] [bigint] NOT NULL,
	[WorkStationCode] [varchar](15) NOT NULL,
	[Seq] [int] NOT NULL,
	[LineCode] [varchar](15) NOT NULL,
	[ProductionDate] [datetime] NOT NULL,
	[ParentItem_Code] [varchar](25) NOT NULL,
	[Trolley_Cls] [varchar](2) NOT NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
