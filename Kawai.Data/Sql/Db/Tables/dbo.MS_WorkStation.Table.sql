SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_WorkStation](
	[WorkStationCode] [char](15) NOT NULL,
	[WorkStationName] [varchar](100) NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[WorkStationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
