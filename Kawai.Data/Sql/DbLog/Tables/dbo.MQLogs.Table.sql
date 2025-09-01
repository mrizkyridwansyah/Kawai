SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MQLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TimeStamp] [bigint] NULL,
	[TransactionType] [varchar](100) NULL,
	[FormatMessage] [varchar](100) NULL,
	[Method] [varchar](100) NULL,
	[Path] [varchar](200) NULL,
	[UserID] [varchar](50) NULL,
	[FullName] [varchar](200) NULL,
	[ElapsedtimeMs] [bigint] NULL,
 CONSTRAINT [PK_MQLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO