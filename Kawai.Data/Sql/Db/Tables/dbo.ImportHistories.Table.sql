SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ImportHistories](
	[Id] [nvarchar](100) NOT NULL,
	[Key] [nvarchar](100) NULL,
	[Template] [nvarchar](100) NULL,
	[Status] [nvarchar](20) NULL,
	[FileName] [nvarchar](255) NULL,
	[ContentType] [nvarchar](255) NULL,
	[ProcessDuration] [bigint] NOT NULL,
	[SizeFile] [bigint] NOT NULL,
	[RowsCount] [int] NOT NULL,
	[ValidRowsCount] [int] NOT NULL,
	[InvalidRowsCount] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[UserId] [varchar](25) NOT NULL
) ON [PRIMARY]
GO
