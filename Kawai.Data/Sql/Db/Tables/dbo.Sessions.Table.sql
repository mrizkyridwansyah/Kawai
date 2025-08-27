SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Sessions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](50) NOT NULL,
	[Date] [bigint] NOT NULL,
	[ExpiryDate] [bigint] NULL,
	[UserAgent] [nvarchar](255) NULL,
	[RemoteAddr] [nvarchar](30) NULL,
	[DeviceId] [nvarchar](max) NULL,
	[Status] [nvarchar](20) NULL,
	[Token] [nvarchar](100) NULL,
	[FCMToken] [nvarchar](2000) NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
