SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ErrorLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [bigint] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Level] [nvarchar](255) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[InnerStackTrace] [nvarchar](max) NULL,
	[UserId] [nvarchar](255) NULL,
	[FullName] [nvarchar](max) NULL,
	[UserAgent] [nvarchar](max) NULL,
	[RemoteAddr] [nvarchar](255) NULL,
	[StatusCode] [int] NOT NULL,
	[Method] [nvarchar](10) NULL,
	[RequestPath] [nvarchar](2000) NULL,
	[RequestBody] [nvarchar](max) NULL,
	[ElapsedMilliseconds] [bigint] NULL,
 CONSTRAINT [PK_ErrorLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
