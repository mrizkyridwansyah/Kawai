SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Cls_Parameter](
	[Code] [varchar](10) NOT NULL,
	[Description] [varchar](100) NULL,
	[ParGroup] [varchar](20) NULL,
	[RegisterUser] [varchar](30) NULL,
	[RegisterDate] [datetime] NULL,
	[UpdateUser] [varchar](30) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Cls_Parameter] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
