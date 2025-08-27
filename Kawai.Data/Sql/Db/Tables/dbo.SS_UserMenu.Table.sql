SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SS_UserMenu](
	[MenuID] [varchar](10) NOT NULL,
	[MenuName] [nvarchar](100) NULL,
	[MenuDescription] [nvarchar](max) NULL,
	[MenuGroup] [varchar](50) NULL,
	[GroupIndex] [int] NULL,
	[MenuIndex] [int] NULL,
	[ImageName] [varchar](100) NULL,
	[SubGroup] [varchar](10) NULL,
	[SubGroupIndex] [int] NULL,
	[SPPortin] [varchar](200) NULL,
 CONSTRAINT [PK_SS_UserMenu] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
