SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AMRRequestHistory](
	[RequestNo] [varchar](50) NOT NULL,
	[FromData] [varchar](25) NULL,
	[ToData] [varchar](25) NULL,
	[Action] [varchar](100) NULL,
	[SourceAction] [varchar](200) NULL,
	[StatusAMR] [varchar](max) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

