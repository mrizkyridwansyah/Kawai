SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PackingItem_Master](
	[Item_code] [char](35) NOT NULL,
	[PackingStyle_Cls] [char](2) NOT NULL,
	[GrossWeight] [numeric](10, 3) NULL,
	[NetWeight] [numeric](10, 3) NULL,
	[Length] [numeric](9, 2) NULL,
	[Width] [numeric](9, 2) NULL,
	[Thickness] [numeric](9, 2) NULL,
	[Number_Entering] [numeric](7, 2) NULL,
 CONSTRAINT [PK_PackingItem_Master] PRIMARY KEY CLUSTERED 
(
	[Item_code] ASC,
	[PackingStyle_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
