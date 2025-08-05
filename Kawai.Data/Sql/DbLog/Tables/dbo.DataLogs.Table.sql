SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [DataLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [bigint] NOT NULL,
	[UserId] [nvarchar](100) NULL,
	[FullName] [nvarchar](255) NULL,
	[UserAgent] [nvarchar](max) NULL,
	[RemoteAddr] [nvarchar](255) NULL,
	[Method] [nvarchar](10) NULL,
	[RequestPath] [nvarchar](2000) NULL,
	[Action] [nvarchar](10) NULL,
	[Activity] [nvarchar](100) NULL,
	[DocumentType] [nvarchar](50) NULL,
	[EntityId] [nvarchar](100) NOT NULL,
	[ReferenceId] [nvarchar](100) NULL,
	[Data] [nvarchar](max) NULL,
	[ElapsedMilliseconds] [bigint] NOT NULL,
 CONSTRAINT [PK_DataLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
