SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Destination_Cls](
	[Destination_Cls] [varchar](2) NOT NULL,
	[Description] [varchar](50) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Destination_Cls] PRIMARY KEY CLUSTERED 
(
	[Destination_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
