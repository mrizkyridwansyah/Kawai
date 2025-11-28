SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WorkStationLineSetting](
	[LineCode] [char](15) NOT NULL,
	[WorkStationCode] [char](15) NULL,
	[Barcode] [varchar](35) NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
ALTER TABLE [WorkStationLineSetting]  WITH CHECK ADD FOREIGN KEY([WorkStationCode])
REFERENCES [MS_WorkStation] ([WorkStationCode])
GO
