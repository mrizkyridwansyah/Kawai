SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ProdMaterialReqParams](
	[ParamKey] [varchar](100) NOT NULL,
	[Factory] [varchar](25) NOT NULL,
	[Process] [varchar](25) NOT NULL,
	[Line] [varchar](25) NOT NULL,
	[Model] [varchar](25) NOT NULL,
	[ScheduleDate] [date] NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
