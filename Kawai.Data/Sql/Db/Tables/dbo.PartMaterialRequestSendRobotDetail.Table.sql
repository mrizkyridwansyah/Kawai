SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestSendRobotDetail](
	[RequestSendDetailID] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestSendID] [varchar](50) NOT NULL,
	[Stop_Point] [varchar](25) NULL,
	[Pickup_Seq] [int] NULL,
	[Status] [bit] NOT NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
